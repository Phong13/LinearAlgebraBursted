using LinearAlgebra;
using NUnit.Framework;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

// Coverage tests for float methods that are not exercised elsewhere
// (or are exercised only on a single code path). Companion to the
// existing OperationsTest / DotOperationTests / InitTest suites.
public class floatInplaceCoverageTests
{
    // setAll: post-construction in-place fill. The constructor form
    // floatVec(N, s) is covered elsewhere; this hits the extension
    // method directly.
    [BurstCompile]
    public struct SetAllJob : IJob
    {
        public void Execute()
        {
            var arena = new Arena(Allocator.Persistent);

            int n = 7;
            floatN vec = arena.floatVec(n);
            vec.setAll(3.5f);
            for (int i = 0; i < n; i++)
                Assert.AreEqual((float)3.5f, vec[i]);

            floatMxN mat = arena.floatMat(3, 4);
            mat.setAll(-2f);
            for (int i = 0; i < mat.Length; i++)
                Assert.AreEqual((float)(-2f), mat[i]);

            arena.Dispose();
        }
    }

    [Test] public void SetAll() { new SetAllJob().Run(); }

    // copyInpl: element-wise copy, no allocation.
    [BurstCompile]
    public struct CopyInplJob : IJob
    {
        public void Execute()
        {
            var arena = new Arena(Allocator.Persistent);

            int n = 8;
            floatN src = arena.floatVec(n);
            floatN dst = arena.floatVec(n);
            for (int i = 0; i < n; i++)
            {
                src[i] = (float)(i + 1);
                dst[i] = 0;
            }

            dst.copyInpl(src);

            for (int i = 0; i < n; i++)
                Assert.AreEqual(src[i], dst[i]);

            arena.Dispose();
        }
    }

    [Test] public void CopyInpl() { new CopyInplJob().Run(); }

    // EqualsByValue negative + length-mismatch cases. Positive case is
    // already exercised heavily in OperationsTest / DotOperationTests.
    [BurstCompile]
    public struct EqualsByValueNegativeJob : IJob
    {
        public void Execute()
        {
            var arena = new Arena(Allocator.Persistent);

            int n = 5;
            floatN a = arena.floatVec(n);
            floatN c = arena.floatVec(n);
            floatN d = arena.floatVec(n + 1); // length-mismatched, zeroed
            for (int i = 0; i < n; i++)
            {
                a[i] = (float)(i + 0.25f);
                c[i] = (float)(i + 0.25f);
            }
            c[2] = (float)999f; // diverge in one element

            Assert.IsFalse(a.EqualsByValue(c));
            Assert.IsFalse(a.EqualsByValue(d));

            arena.Dispose();
        }
    }

    [Test] public void EqualsByValueNegative() { new EqualsByValueNegativeJob().Run(); }

    // AlmostEqualsByValue: not tested anywhere else.
    [BurstCompile]
    public struct AlmostEqualsByValueJob : IJob
    {
        public void Execute()
        {
            var arena = new Arena(Allocator.Persistent);

            int n = 5;
            floatN a = arena.floatVec(n);
            floatN b = arena.floatVec(n);
            floatN c = arena.floatVec(n);
            floatN d = arena.floatVec(n + 1);
            for (int i = 0; i < n; i++)
            {
                a[i] = (float)1.0f;
                b[i] = (float)1.0001f;   // within 1e-3
                c[i] = (float)1.01f;     // outside 1e-3
            }

            Assert.IsTrue (a.AlmostEqualsByValue(b, (float)1e-3f));
            Assert.IsFalse(a.AlmostEqualsByValue(c, (float)1e-3f));
            Assert.IsFalse(a.AlmostEqualsByValue(d, (float)1e-3f));

            arena.Dispose();
        }
    }

    [Test] public void AlmostEqualsByValue() { new AlmostEqualsByValueJob().Run(); }

    // dotInpl(matrix target, A, B, transposeA: true) — the transposeA
    // path was previously rejected by an over-strict dim guard
    // (a.N_Cols == b.N_Cols). After the guard fix this 3x2 / 3x4 case
    // is accepted and computed correctly.
    [BurstCompile]
    public struct DotInplMatrixTransAJob : IJob
    {
        public void Execute()
        {
            var arena = new Arena(Allocator.Persistent);

            // A=3x2, B=3x4, A^T*B = 2x4. Hand-computed reference values.
            floatMxN A = arena.floatMat(3, 2);
            A[0,0]=1; A[0,1]=2;
            A[1,0]=3; A[1,1]=4;
            A[2,0]=5; A[2,1]=6;

            floatMxN B = arena.floatMat(3, 4);
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 4; c++)
                    B[r, c] = (float)(r * 4 + c + 1);

            floatMxN C = arena.floatMat(2, 4);
            // garbage init, dotInpl must zero-fill before MAC
            for (int i = 0; i < C.Length; i++) C[i] = (float)999f;

            C.dotInpl(A, B, transposeA: true);

            // row 0 of A^T = column 0 of A = [1, 3, 5]
            // row 1 of A^T = column 1 of A = [2, 4, 6]
            // C[0,0]=1*1+3*5+5*9 =61   C[0,1]=1*2+3*6+5*10=70
            // C[0,2]=1*3+3*7+5*11=79   C[0,3]=1*4+3*8+5*12=88
            // C[1,0]=2*1+4*5+6*9 =76   C[1,1]=2*2+4*6+6*10=88
            // C[1,2]=2*3+4*7+6*11=100  C[1,3]=2*4+4*8+6*12=112
            Assert.AreEqual((float)61f,  C[0, 0]);
            Assert.AreEqual((float)70f,  C[0, 1]);
            Assert.AreEqual((float)79f,  C[0, 2]);
            Assert.AreEqual((float)88f,  C[0, 3]);
            Assert.AreEqual((float)76f,  C[1, 0]);
            Assert.AreEqual((float)88f,  C[1, 1]);
            Assert.AreEqual((float)100f, C[1, 2]);
            Assert.AreEqual((float)112f, C[1, 3]);

            arena.Dispose();
        }
    }

    [Test] public void DotInplMatrixTransposeA() { new DotInplMatrixTransAJob().Run(); }

    // dotInpl(vec target, A, x, transposeA: true) — the transposeA
    // arg was previously silently ignored (only the non-transpose path
    // ran). After the fix, y = A^T * x is actually computed.
    [BurstCompile]
    public struct DotInplVectorTransAJob : IJob
    {
        public void Execute()
        {
            var arena = new Arena(Allocator.Persistent);

            // A is 3x2; A^T is 2x3. x has length 3 (= A.M_Rows).
            // y = A^T * x has length 2 (= A.N_Cols).
            floatMxN A = arena.floatMat(3, 2);
            A[0,0]=1; A[0,1]=2;
            A[1,0]=3; A[1,1]=4;
            A[2,0]=5; A[2,1]=6;

            floatN x = arena.floatVec(3);
            x[0] = 1; x[1] = 1; x[2] = 1;

            floatN y = arena.floatVec(2);
            y[0] = (float)999f; y[1] = (float)999f;

            y.dotInpl(A, x, transposeA: true);

            // y[0] = 1+3+5 = 9; y[1] = 2+4+6 = 12
            Assert.AreEqual((float)9f,  y[0]);
            Assert.AreEqual((float)12f, y[1]);

            arena.Dispose();
        }
    }

    [Test] public void DotInplVectorTransposeA() { new DotInplVectorTransAJob().Run(); }

    // SetCol: writes a vector into one column of a matrix in place.
    // (Col is exercised in InitTest; SetCol is not.)
    [BurstCompile]
    public struct SetColJob : IJob
    {
        public void Execute()
        {
            var arena = new Arena(Allocator.Persistent);

            floatMxN M = arena.floatMat(3, 3);
            M[0,0]=1; M[0,1]=2; M[0,2]=3;
            M[1,0]=4; M[1,1]=5; M[1,2]=6;
            M[2,0]=7; M[2,1]=8; M[2,2]=9;

            floatN newCol = arena.floatVec(3);
            newCol[0] = (float)10f; newCol[1] = (float)20f; newCol[2] = (float)30f;

            M.SetCol(newCol, colIdx: 0, rowStartIdx: 0);

            // column 0 was overwritten...
            Assert.AreEqual((float)10f, M[0, 0]);
            Assert.AreEqual((float)20f, M[1, 0]);
            Assert.AreEqual((float)30f, M[2, 0]);
            // ...but other columns are untouched.
            Assert.AreEqual((float)2f, M[0, 1]);
            Assert.AreEqual((float)8f, M[2, 1]);

            arena.Dispose();
        }
    }

    [Test] public void SetCol() { new SetColJob().Run(); }

    // Inpl no-allocation contract.
    // Snapshot persistent + temp allocation counts, exercise every *Inpl
    // method, then assert nothing allocated. Catches future regressions
    // where someone sneaks an alloc into an Inpl method.
    [BurstCompile]
    public struct InplNoAllocContractJob : IJob
    {
        public void Execute()
        {
            var arena = new Arena(Allocator.Persistent);

            int n = 8;
            floatN a = arena.floatVec(n);
            floatN b = arena.floatVec(n);
            floatN c = arena.floatVec(n);
            floatMxN M1 = arena.floatMat(4, 4);
            floatMxN M2 = arena.floatMat(4, 4);
            floatMxN Mout = arena.floatMat(4, 4);

            for (int i = 0; i < n; i++)
            {
                a[i] = (float)(i + 1);
                b[i] = (float)(i * 2 + 1);
                c[i] = 0;
            }
            for (int i = 0; i < M1.Length; i++)
            {
                M1[i] = (float)(i + 1);
                M2[i] = (float)(i * 0.5f);
            }

            // Pre-allocate temps used below BEFORE locking the baseline.
            // Anything allocated after must come from a misbehaving Inpl.
            floatN x = arena.floatVec(4);
            floatN y = arena.floatVec(4);
            for (int i = 0; i < 4; i++) x[i] = (float)(i + 1);

            arena.SetExpectedPersistentAllocationCount();
            int baselineTemps = arena.TempAllocationsCount;

            // Vector scalar-arg Inpls
            a.setAll(1f);
            a.addInpl((float)0.5f);
            a.mulInpl((float)2f);
            a.divInpl((float)2f);
            a.subInpl((float)0.5f);
            a.modInpl((float)10f);
            a.signFlipInpl();

            // Vector vector-arg Inpls
            a.copyInpl(b);
            a.addInpl(b);
            a.subInpl(b);
            c.addInpl(a, b);
            c.subInpl(a, b);
            a.compMulInpl(b);
            a.compDivInpl(b);

            // Matrix Inpls
            M1.setAll(1f);
            M1.addInpl((float)0.25f);
            M1.signFlipInpl();
            Mout.dotInpl(M1, M2);                        // C = A * B
            Mout.dotInpl(M1, M2, transposeA: true);      // C = A^T * B

            // Vector dotInpl, both branches
            y.dotInpl(M1, x);
            y.dotInpl(M1, x, transposeA: true);

            // If either fails, an Inpl method allocated.
            Assert.IsTrue(arena.CheckPersistentAllocationCount());
            Assert.AreEqual(baselineTemps, arena.TempAllocationsCount);

            arena.Dispose();
        }
    }

    [Test] public void InplNoAllocContract() { new InplNoAllocContractJob().Run(); }
}
