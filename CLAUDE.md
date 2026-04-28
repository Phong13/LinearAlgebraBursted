# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

A Burst-compiled linear algebra library for Unity, designed as an extension of `Unity.Mathematics`. Provides vectors / matrices (`floatN`, `floatMxN`, `doubleN`, `intMxN`, `boolN`, …) for `float`, `double`, `int`, `short`, `long`, and `bool`, plus operations (component-wise, dot, norms, QR, LU, solvers, stats, etc.). The library is in `Assets/LinearAlgebra/Source/` — copy that folder into another project to consume it.

Unity version: **2022.3.8f1**. Required packages (see `Packages/manifest.json`): `com.unity.collections`, `com.unity.burst` (transitive), `com.unity.test-framework`, and `com.annulusgames.unity-codegen` (the code generator that drives this project).

## The codegen pipeline (read this before editing source)

Almost all of `Assets/LinearAlgebra/Source/Generated/` is **generated**. **Never hand-edit files under `Assets/LinearAlgebra/Source/Generated/` or `Assets/LinearAlgebra/SourceTests/Generated/`** — they are overwritten on every regen.

The real source of truth lives under:

- `Assets/LinearAlgebra/CodeGen/TemplateSource/` — runtime templates → emitted to `Assets/LinearAlgebra/Source/Generated/`
- `Assets/LinearAlgebra/CodeGen/TemplateSourceTests/` — test templates → emitted to `Assets/LinearAlgebra/SourceTests/Generated/`

### How template expansion works

Templates are valid C# (they compile against the proxy structs in `TemplateSource/proxyStructs.cs`). The generator (`CodeGen/TemplateConverter.cs`, driven by `TemplateSourceGenerator.cs` / `TemplateSourceTestsGenerator.cs`) walks template files and substitutes type proxies:

| Proxy        | Expands to                  |
|--------------|-----------------------------|
| `fProxy`     | `float`, `double`           |
| `iProxy`     | `int`, `short`, `long`      |
| `FProxy`     | `Float`, `Double` (PascalCase) |
| `IProxy`     | `Int`, `Short`, `Long`      |

Type lists live in `CodeGen/GenUtils.cs` (`floatTypes`, `intTypes`, `boolTypes`, etc.).

**Multiplication rules** (see `TemplateConverter.Execute`):

- A file with `fProxy` in its **filename or contents** is emitted once per float type.
- A file with `iProxy` in its **filename or contents** is emitted once per int type.
- All other files are emitted as-is ("singular files"). A file may also opt in via the `//singularFile//` marker.
- `proxyStructs.cs`, `proxyShims.cs`, and `markers.cs` are **always skipped** (see `TemplateConverter.IgnoreFile`).

### In-file expansion markers

Inside a template, blocks can repeat per type. From `GenUtils.cs`:

```csharp
//+copyReplace        ...  //-copyReplace        // expand block per type (fProxy → float,double; iProxy → int,short,long)
//+copyReplaceAll     ...  //-copyReplaceAll     // expand for ALL types (float+int+bool)
//+copyReplaceFill[X] ...  //-copyReplaceFill    // same as copyReplace but joins emitted copies with literal X
//+deleteThis         ...  //-deleteThis         // strip block from generated output (used to make the template itself compile)
```

`Arena.cs` (singular file) is a good example: it uses `copyReplace` to fan out per-type lists/loops while staying compilable as a template.

### Regenerating

In Unity: **Tools → UnityCodeGen → Generate** (or **Force Generate**). There is a "Auto-generate on Compile" toggle in the same menu. There is **no CLI build / generate step** — code generation only runs inside the Unity Editor. After editing a template, regenerate before running tests.

## Architecture

### Memory model: `Arena`

`Arena` (`TemplateSource/Arena/Arena.cs`, plus per-type partials `Arena.fProxy.cs`, `Arena.iProxy.cs`, `Arena.bool.cs`) owns all vector/matrix allocations. It holds two `UnsafeList<T>` per type pair:

- `<type>Vectors` / `<type>Matrices` — long-lived allocations (`Clear()` / `Dispose()` to free)
- `temp<type>Vectors` / `temp<type>Matrices` — temporaries from operator overloads (e.g. `a + b`); free with `ClearTemp()`

Vector / matrix structs (`fProxyN`, `fProxyMxN`, …) hold a raw `Arena*` (`NativeDisableUnsafePtrRestriction`) so operator overloads can allocate temps back into the originating arena. This means **arenas must outlive any vec/mat created from them**, and you cannot freely move an arena by value while vecs/mats reference it.

User-facing flow (from README):

```csharp
var arena = new Arena(Allocator.Persistent);
floatN a = arena.floatVec(128, 1f);
floatMxN M = arena.floatRandomMatrix(16, 16);
floatN sum = a + a;        // allocates a temp inside arena
arena.ClearTemp();          // release temps
arena.Dispose();            // release everything
```

### Operations layout

Operations are static classes named `<type>OP`, `<type>NormsOP`, `OrthoOP`, `LU`, `Solvers`, `<type>StatsOP`, `Pivot`, etc. Most ship in two flavors:

- **Safe / arena-aware** (e.g. `floatOP.dot(A, B)`) — allocates the result into the arena.
- **`Inpl` variants** (e.g. `floatOP.addInpl(C, 1f)`) — write into an existing target, allocate nothing.
- **`Unsafe<...>OP`** — pointer-based kernels that the safe wrappers call. These are what gets `[BurstCompile]`-friendly inner loops.

Operator overloads on the structs (`fProxyN.Operators.cs`, `fProxyMxN.Operators.cs`, comparators, indexing, shortcuts) are thin wrappers that call into the OP classes and push results into `temp<...>` arena lists.

### Assembly layout

Three asmdefs:

- `Assets/LinearAlgebra/Source/BurstLinearAlgebra.asmdef` — runtime library (`allowUnsafeCode: true`)
- `Assets/LinearAlgebra/SourceTests/BurstLinearAlgebra.Tests.asmdef` — Editor-only NUnit tests, gated on `UNITY_INCLUDE_TESTS`
- `Assets/LinearAlgebra/CodeGen/BurstLinearAlgebra.CodeGen-firstpass.asmdef` — Editor-only generator code

The CodeGen asmdef is named `-firstpass` so it compiles before the runtime asmdef, letting it run as a generator on each compile.

## Tests

Tests are written as NUnit `[Test]` methods that wrap `[BurstCompile]` `IJob` structs and call `.Run()`. See `TemplateSourceTests/fProxy/InitTest.fProxy.cs` for the canonical pattern. Each `fProxy` test file expands into `float` and `double` test files under `SourceTests/Generated/<type>/`.

Run them via Unity's **Test Runner** window (Window → General → Test Runner → EditMode), or via Unity batch mode from a shell. To run a single test, filter by class or method name in the Test Runner UI / via the `-testFilter` flag below.

### Batch-mode CLI (Unity must be closed)

Project Unity version: see `ProjectSettings/ProjectVersion.txt` (currently `6000.4.3f1`). Adjust the editor path to whatever version that file pins.

**Compile check** — non-zero exit code means compile errors; grep the log for `error CS`:

```bash
"/c/Program Files/Unity/Hub/Editor/6000.4.3f1/Editor/Unity.exe" \
  -batchmode -nographics \
  -projectPath "D:/Workspace/Unity/LinearAlgebraBursted" \
  -logFile "D:/Workspace/Unity/LinearAlgebraBursted/compile_log.txt" \
  -quit
```

**Run EditMode tests** (this project's tests are EditMode-only — see the `BurstLinearAlgebra.Tests.asmdef`):

```bash
"/c/Program Files/Unity/Hub/Editor/6000.4.3f1/Editor/Unity.exe" \
  -batchmode -nographics \
  -projectPath "D:/Workspace/Unity/LinearAlgebraBursted" \
  -runTests -testPlatform EditMode \
  -testResults "D:/Workspace/Unity/LinearAlgebraBursted/TestResults.xml" \
  -logFile "D:/Workspace/Unity/LinearAlgebraBursted/test_log.txt"
```

Add `-testFilter "ClassName"` (or `"ClassName.MethodName"`) to scope to one test. Read `TestResults.xml` (NUnit format) for pass/fail.

When adding tests:

- Edit the **template** under `TemplateSourceTests/fProxy/` or `TemplateSourceTests/iProxy/`, never the generated copy.
- Use proxy types in the template (`fProxyN`, `arena.fProxyVec(...)`); they expand per concrete type.

## Common pitfalls

- **Editing generated files**: changes will be wiped the next time the generator runs. Always edit templates.
- **Forgetting to regenerate**: after editing a template, run *Tools → UnityCodeGen → Generate* (or enable Auto-generate) before compiling tests.
- **Arena lifetime**: vecs/mats hold a raw `Arena*`. Disposing or moving the arena while live vecs/mats exist will dereference invalid memory.
- **Temp leak**: every operator overload (`+`, `*`, `>`, `!`, …) allocates a temp into the arena. Long-running loops should call `arena.ClearTemp()` periodically.
- **Float vs int templates**: a file named `*.fProxy.cs` only fans out to float types; use `*.iProxy.cs` for int types, or use the `copyReplaceAll` block marker to cover all types from a single template.
