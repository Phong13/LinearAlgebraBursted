# Using LinearAlgebraBursted (Generated/)

License: MIT, see `../LICENSE` (one folder up, in `Source/`). The whole
`Source/` tree — including this `Generated/` folder, the asmdef, and any
static singular files — ships under that one license.

This folder contains generated code from `CodeGen/TemplateSource/`. Do not
edit files here — they are overwritten on every codegen run. Edit the
templates under `Assets/LinearAlgebra/CodeGen/TemplateSource/` instead.

This file is a usage guide for callers of the generated API. For the
codegen pipeline itself, see the project root `CLAUDE.md`.

## Mental model

Every `fProxyN` / `fProxyMxN` (where `fProxy` ∈ {`float`, `double`}) and
every `iProxyN` / `iProxyMxN` (where `iProxy` ∈ {`int`, `short`, `long`})
holds a raw pointer back to the `Arena` it came from. The arena owns the
underlying memory and tracks two lists per type:

- **persistent** — long-lived buffers (model state, weights, reusable
  workspace).
- **temp** — short-lived scratchpads produced by operator overloads
  (`a + b`, `mat * scalar`, etc.) or by explicit `arena.tempfloatVec(...)`
  / `arena.tempfloatMat(...)` calls.

Operator overloads always allocate into the temp list. The arena lives
across the call so the temp can outlive the operator expression.

## Best-practice lifecycle

```csharp
public sealed class MyComputation : IDisposable
{
    Arena _arena;
    floatN _state;
    floatMxN _weights;
    floatN _scratch;        // reused workspace, not a temp

    public MyComputation()
    {
        _arena = new Arena(Allocator.Persistent);

        // Allocate every persistent buffer up front in Init.
        _state   = _arena.floatVec(64);
        _weights = _arena.floatMat(64, 64);
        _scratch = _arena.floatVec(64);

        // Lock in the expected count. Any later persistent allocation
        // is a bug; CheckPersistentAllocationCount() will surface it.
        _arena.SetExpectedPersistentAllocationCount();
    }

    public void Step(floatN input)
    {
        // Use temp freely for intermediate results from operator overloads.
        floatN h = floatOP.dot(_weights, input) + _state;   // temps
        floatOP.compMulInpl(_scratch, h);                   // no alloc

        // ... finish a coherent unit of work ...

        // Drain temps when the multi-step result is consumed/copied.
        _arena.ClearTemp();

#if LINALG_DEBUG
        // Cheap to leave on in dev builds; verifies no Inpl method
        // sneaked an allocation into the persistent list.
        _arena.CheckPersistentAllocationCount();
#endif
    }

    public void Dispose()
    {
        _arena.Dispose();   // disposes both persistent and temp lists
    }
}
```

Key points:

1. **Allocate persistent buffers exactly once in your Init/ctor**, then
   call `arena.SetExpectedPersistentAllocationCount()`.
2. **Call `arena.ClearTemp()`** at every natural boundary (end of frame,
   end of solver step, after copying a result out of a temp). Operator
   overloads accumulate temps until you do this.
3. **The arena should live in an `IDisposable`** with a deterministic
   lifecycle. Never let an arena go out of scope while any
   `fProxyN`/`fProxyMxN` referencing it is still in use — those structs
   carry a raw `Arena*` and dereferencing it after dispose is a crash
   waiting to happen.
4. **Use `*Inpl` methods on hot paths.** Anything ending in `Inpl`
   (`addInpl`, `dotInpl`, `compMulInpl`, …) writes through an existing
   buffer and never allocates. Operator overloads are convenient but
   leak temps until `ClearTemp()`.

## Common failure modes

### "Vector data was NaN. Might be double freeing."

Logged by `fProxyN.Dispose(bool)` / `fProxyMxN.Dispose(bool)` when the
buffer about to be freed contains NaN at index 0. Most common cause:

- The buffer was created with `uninit: true` and never written to before
  the arena was disposed. Initialize with the default `uninit: false`
  (zero-fill) when in doubt, or `setAll(0f)` after construction.
- The buffer was already disposed once and you're disposing again.

The check is best-effort — it cannot detect every double-free, only ones
where the freed memory happens to look like NaN (which uninitialized
memory often does).

### `CheckPersistentAllocationCount()` returns false

Something in your inner loop is allocating into the persistent list when
it shouldn't. Usual culprits:

- Calling `arena.floatVec(...)` instead of `arena.tempfloatVec(...)` for
  an intermediate value.
- Calling `mat.CopyPersistent()` where `mat.TempCopy()` was meant.
- A handwritten "Inpl" method that internally calls a non-Inpl op.

### `TempAllocationsCount` grows unboundedly

You have a hot loop that uses operator overloads (`a + b`, `mat * 2f`)
without ever calling `arena.ClearTemp()`. Each operator emits one temp.
Add `arena.ClearTemp()` at the bottom of the loop iteration, or rewrite
hot inner loops with `*Inpl` methods.

### NullReferenceException / access-violation crash inside burst

A vec/mat outlived its arena. `fProxyN._arenaPtr` is a raw `Arena*`. If
the arena was disposed first, every method that goes through `_arenaPtr`
(`TempCopy`, `CopyPersistent`, `Col`, etc.) crashes. Ensure your
`Dispose()` order disposes consumers before the arena.

### `Assert.IsTrue(arena.AllocationsCount == ...)` flakes

`AllocationsCount` is the *persistent* count. `TempAllocationsCount` is
separate. If a test asserts on `AllocationsCount` after running operator
overloads, the temps don't show up — but if the test ran an Inpl method
that mistakenly allocates, persistent count grows. Use both counts in
tests, and prefer `CheckPersistentAllocationCount()` after a baseline
`SetExpectedPersistentAllocationCount()` for tighter contracts.

### Burst rejects `Assert.IsTrue(bool, string, params object[])`

Burst can compile only the no-message overloads of NUnit asserts. Drop
the message argument inside `[BurstCompile] IJob.Execute()` bodies; use
plain `Assert.IsTrue(b)` / `Assert.AreEqual(a, b)`.

## When to use which API

| Need | Call |
|---|---|
| Long-lived buffer (model state, reusable workspace) | `arena.floatVec(N)` / `arena.floatMat(M, N)` |
| One-shot intermediate from a multi-step calc | `arena.tempfloatVec(...)` / `arena.tempfloatMat(...)` |
| Result of `a + b`, `mat * 2f`, etc. | (operator overloads — auto-temp) |
| Mutate an existing buffer | `vec.addInpl(scalar)`, `vec.copyInpl(other)`, `mat.dotInpl(A, B)` |
| Copy into the persistent pool | `vec.CopyPersistent()` |
| Copy into the temp pool | `vec.TempCopy()` |
| Reset just the scratchpad | `arena.ClearTemp()` |
| Reset everything (persistent + temp) | `arena.Clear()` |
| Tear down the arena | `arena.Dispose()` |
