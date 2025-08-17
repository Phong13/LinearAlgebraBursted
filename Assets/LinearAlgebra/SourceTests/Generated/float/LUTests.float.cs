using System;

using LinearAlgebra;
using LinearAlgebra.Stats;

using NUnit.Framework;
using Unity.Burst;
using Unity.Collections;

using Unity.Jobs;
using UnityEngine;
using UnityEngine.TestTools;

public class floatLUTests
{

    [BurstCompile(FloatPrecision = FloatPrecision.High, FloatMode = FloatMode.Default)] 
    public struct TestJob : IJob
    {
        public enum TestType
        {
            LUDecompIdentity,
            LUDecompPredefined,
            LUDecompRandomDiagonal,
            LUDecompRandom,
            LUDecompRandomLarge,
            LUDecompHilbert,
            LUDecompPermutation,
            LUDecompZero,
            LUSolveSystem,
            LUSolveSystemInplace
        }

        public TestType Type;


        public void Execute()
        {
            switch(Type)
            {
                case TestType.LUDecompIdentity:
                    LUDecompIdentity();
                break;
                case TestType.LUDecompPredefined:
                    LUDecompPredefined();
                break;
                case TestType.LUDecompRandomDiagonal:
                    LUDecompRandomDiagonal();
                break;
                case TestType.LUDecompRandom:
                    LUDecompRandom();
                break;
                case TestType.LUDecompRandomLarge:
                    LUDecompRandomLarge();
                    break;
                case TestType.LUDecompHilbert:
                    LUDecompHilbert();
                break;
                case TestType.LUDecompPermutation:
                    LUDecompPermutation();
                break;
                case TestType.LUDecompZero:
                    LUDecompZero();
                break;
                case TestType.LUSolveSystem:
                    SolveSystem();
                break;
                case TestType.LUSolveSystemInplace:
                    SolveSystemInplace();
                    break;

            }
        }

        private floatMxN GetRandomMatrix(ref Arena arena, int dim, float min, float max, uint seed) {

            var mat = arena.floatRandomMatrix(dim, dim, min, max, seed);

            return mat;
        }

        public void LUDecompIdentity()
        {
            var arena = new Arena(Allocator.Persistent);

            int dim = 8;

            var U = arena.floatIdentityMatrix(dim, true);
            var L = arena.floatIdentityMatrix(dim, true);

            var A = U.CopyPersistent();

            LU.luDecompositionNoPivot(ref U, ref L);

            AssertLU(in A, in L, in U, false);

            arena.Dispose();
        }
        public void LUDecompRandomDiagonal()
        {
            var arena = new Arena(Allocator.Persistent);

            int dim = 8;

            var U = arena.floatRandomDiagonalMatrix(dim, 1f, 3f);
            var L = arena.floatIdentityMatrix(dim, true);

            var A = U.CopyPersistent();

            LU.luDecompositionNoPivot(ref U, ref L);


            AssertLU(in A, in U, in L, false);

            arena.Dispose();
        }

        public void LUDecompPredefined() {

            var arena = new Arena(Allocator.Persistent);

            var dim = 5;

            var U = arena.floatMat(dim);
            var L = arena.floatIdentityMatrix(dim, true);
            
            var pivot = new Pivot(dim, Allocator.Temp);

            U[0] = -2f;
            U[1] = 1f;
            U[2] = -2f;
            U[3] = 3f;
            U[4] = 1f;

            U[5] = 1f;
            U[6] = -2f;
            U[7] = 3f;
            U[8] = -5f;
            U[9] = 4f;

            U[10] = 4f;
            U[11] = 3f;
            U[12] = -1f;
            U[13] = 2f;
            U[14] = -3f;

            U[15] = 1f;
            U[16] = 1f;
            U[17] = -1f;
            U[18] = -11f;
            U[19] = 11f;

            U[20] = -1f;
            U[21] = -9f;
            U[22] = -1f;
            U[23] = 7f;
            U[24] = 1f;

            Print.Log(L);
            Print.Log(U);

            //LU.luDecompositionNoPivot(ref U, ref L);
            LU.luDecompositionInplace(ref U, ref pivot);

            pivot.ApplyInverseRow(ref U); 
            //Print.Log(L);
            Print.Log(U);


            pivot.Dispose();

            arena.Dispose();
        }

        public void LUDecompRandom()
        {
            var arena = new Arena(Allocator.Persistent);

            int dim = 18;

            var U = arena.floatRandomMatrix(dim, dim, 1f, 10f, 314221);
            var L = arena.floatIdentityMatrix(dim, true);
            
            // add to diagonals of U
            for(int d = 0; d < dim; d++)
                U[d, d] += 5f;
            
            var A = U.CopyPersistent();

            var pivot = new Pivot(dim, Allocator.Temp);

            //LU.luDecompositionNoPivot(ref U, ref L);
            LU.luDecomposition(ref U, ref L, ref pivot);

            pivot.ApplyInverseRow(ref A);

            Print.Log(U);
            Print.Log(L);

            pivot.Dispose();

            AssertLU(in A, in L, in U, true, 1E-05f);

            arena.Dispose();
        }

        public void LUDecompRandomLarge()
        {
            /*
            var arena = new Arena(Allocator.Persistent);

            int dim = 512;

            var R = arena.floatMat(dim);
            var U = arena.floatRandomMatrix(dim * 2, dim, -5f, 5f, 9612221);

            var A = U.Copy();

            OrthoOP.LUDecomposition(ref U, ref R);

            AssertLU(in A, in U, in R, 1E-03f);

            arena.Dispose();*/
        }

        public void LUDecompHilbert()
        {
            /*
            var arena = new Arena(Allocator.Persistent);

            int dim = 20;

            var U = arena.floatHilbertMatrix(dim);
            var R = arena.floatMat(dim);

            var A = U.Copy();

            OrthoOP.LUDecomposition(ref U, ref R);

            //Print.Log(A);
            //Print.Log(U);
            //Print.Log(R);

            AssertLU(in A, in U, in R);

            arena.Dispose();*/
        }

        public void LUDecompPermutation() {
            /*
            var arena = new Arena(Allocator.Persistent);

            int tests = 32;
            int dim = 16;
            var rand = new Unity.Mathematics.Random(24011);

            for (int i = 0; i < tests; i++) {

                int p0 = rand.NextInt(0, dim);
                int p1 = rand.NextInt(0, dim);

                while(p0 == p1) {
                    p1 = rand.NextInt(0, dim);
                }

                var U = arena.floatPermutationMatrix(dim, p0, p1);

                p0 = rand.NextInt(0, dim);
                p1 = rand.NextInt(0, dim);

                while (p0 == p1) {
                    p1 = rand.NextInt(0, dim);
                }

                U = floatOP.dot(arena.floatPermutationMatrix(dim, p0, p1), U);

                var R = arena.floatMat(dim);

                var A = U.Copy();

                OrthoOP.LUDecomposition(ref U, ref R);

                //Print.Log(A);
                //Print.Log(U);
                //Print.Log(R);

                AssertLU(in A, in U, in R);
            }
            arena.Dispose();*/
        }

        public void LUDecompZero() {

            /*
            var arena = new Arena(Allocator.Persistent);

            int dim = 8;

            var U = arena.floatMat(dim, dim);
            var R = arena.floatMat(dim);

            var A = U.Copy();

            OrthoOP.LUDecomposition(ref U, ref R);

            //Print.Log(A);
            //Print.Log(U);
            //Print.Log(R);

            AssertLU(in A, in U, in R);

            arena.Dispose();*/
        }

        
        public void SortOfAssert(bool check)
        {
            if (!check) Debug.LogError("failed");
        }

        public void SolveSystem() {

            var arena = new Arena(Allocator.Persistent);

            int dim = 512; 

            var A = arena.floatRandomMatrix(dim, dim, -10f, 10f, 314221);

            if (!(arena.AllocationsCount == 1 && arena.TempAllocationsCount == 0)) UnityEngine.Debug.LogError("FAILED");

            for (int d = 0; d < dim; d++) {
                A[d, d] *= 2f;
                if (Unity.Mathematics.math.abs(A[d, d]) < 0.01f)
                    A[d, d] *= 10f;
            }

            var x_Known = arena.floatRandomVector(dim, 1f, 10f, 901);

            if (!(arena.AllocationsCount == 2 && arena.TempAllocationsCount == 0)) Debug.LogError("failed");

            var b = floatOP.dot(A, x_Known);

            var U = A.CopyPersistent();
            var L = arena.floatIdentityMatrix(dim, true);

            SortOfAssert(arena.AllocationsCount == 4 && arena.TempAllocationsCount == 1);
            SortOfAssert(arena.DB_isPersistant(U) && arena.DB_isPersistant(L) && arena.DB_isTemp(b));

            var pivot = new Pivot(dim, Allocator.Temp);
             
            //LU.luDecompositionNoPivot(ref U, ref L);
            LU.luDecomposition(ref U, ref L, ref pivot);

            Assert.IsTrue(arena.AllocationsCount == 4 && arena.TempAllocationsCount == 1);

            var x_Solved = b.CopyPersistent();

            SortOfAssert(arena.AllocationsCount == 5 && arena.TempAllocationsCount == 1);

            LU.LUSolve(ref L, ref U, in pivot, ref x_Solved);

            SortOfAssert(arena.AllocationsCount == 5 && arena.TempAllocationsCount == 1);

            var zeroError = Analysis.MaxZeroError(x_Known - x_Solved);

            Debug.Log($"Error of max(abs(x_Known - x_Solved)): {zeroError}");


            SortOfAssert(zeroError < 3E-03f);

            pivot.Dispose();

            arena.Dispose();
        }

        public void SolveSystemInplace() {

            var arena = new Arena(Allocator.Persistent);

            int dim = 512; 

            var A = arena.floatRandomMatrix(dim, dim, -10f, 10f, 314221);

            SortOfAssert(arena.AllocationsCount == 1 && arena.TempAllocationsCount == 0);

            for (int d = 0; d < dim; d++) {
                A[d, d] *= 2f;
                if (Unity.Mathematics.math.abs(A[d, d]) < 0.01f)
                    A[d, d] *= 10f;
            }

            var x_Known = arena.floatRandomVector(dim, 1f, 10f, 901);
            
            SortOfAssert(arena.AllocationsCount == 2 && arena.TempAllocationsCount == 0);

            var b = floatOP.dot(A, x_Known);

            var LUmat = A.CopyPersistent();

            SortOfAssert(arena.AllocationsCount == 3 && arena.TempAllocationsCount == 1);
            SortOfAssert(arena.DB_isPersistant(LUmat) && arena.DB_isTemp(b));


            var pivot = new Pivot(dim, Allocator.Temp);

            LU.luDecompositionInplace(ref LUmat, ref pivot);

            SortOfAssert(arena.AllocationsCount == 3 && arena.TempAllocationsCount == 1);

            var x_Solved = b.CopyPersistent();

            SortOfAssert(arena.AllocationsCount == 4 && arena.TempAllocationsCount == 1);

            LU.LUSolve(ref LUmat, ref pivot, ref x_Solved);

            SortOfAssert(arena.AllocationsCount == 4 && arena.TempAllocationsCount == 1);

            if (Analysis.IsAnyNan(in x_Solved))
                throw new System.Exception("TestJob: NaN detected");

            var zeroError = Analysis.MaxZeroError(x_Known - x_Solved);

            Debug.Log($"Error of max(abs(x_Known - x_Solved)): {zeroError}");

            SortOfAssert(zeroError < 3E-03f);

            pivot.Dispose();

            arena.Dispose();
        }

        private void AssertLU(in floatMxN A, in floatMxN L, in floatMxN U, bool pivoted) => AssertLU(in A, in L, in U, pivoted, 1E-6f);
        private void AssertLU(in floatMxN A, in floatMxN L, in floatMxN U, bool pivoted, float precision)
        {
            floatMxN shouldBeZero = A - floatOP.dot(L, U);

            var zeroError = Analysis.MaxZeroError(shouldBeZero);

            if (Analysis.IsAnyNan(in shouldBeZero))
                throw new System.Exception("TestJob: NaN detected");

            Debug.Log($"Error of max(abs(A - LU)): {zeroError}");

            SortOfAssert(Analysis.IsZero(in shouldBeZero, precision));
            SortOfAssert(Analysis.IsLowerTriangular(L, precision));
            SortOfAssert(Analysis.IsUpperTriangular(U, precision));

            if(pivoted)
            unsafe {
                var maxAbs = LinearAlgebra.UnsafeOP.maxAbs(L.Data.Ptr, L.Length);

                if(maxAbs > 1f)
                    throw new System.Exception("TestJob: L has values greater than 1f");
            }
        }

    }

    [BurstCompile]
    public struct PrecisionReconstructTestJob : IJob {

        public enum TestType {
            Random,
            RandomDiagonal
        }

        public TestType Type;

        public void Execute() {

            /*var arena = new Arena(Allocator.Persistent);

            int tests = 64;
            float errorSum = 0;

            for (uint i = 0; i < tests; i++) {

                int dim = 32;

                floatMxN A; 
                
                if(Type == TestType.RandomDiagonal)
                    A = arena.floatRandomDiagonalMatrix(dim, 1f, 3f, 21410 + i*i + i*7);
                else
                    A = arena.floatRandomMatrix(dim*2, dim, -25f, +25f, 21410 + i*i + i*7);
                
                var U = A.Copy();
                var R = arena.floatMat(dim);

                OrthoOP.LUDecomposition(ref U, ref R);

                //Print.Log(U);
                //Print.Log(R);

                errorSum += ErrorCheckLU(in A, in U, in R);

                arena.Clear();
            }

            float avgError = errorSum / tests;

            Debug.Log($"Average error of max(abs(A - LU)): {avgError}");

            arena.Dispose();*/
        }

        private float ErrorCheckLU(in floatMxN A, in floatMxN Q, in floatMxN R) {
            
            floatMxN shouldBeZero = A - floatOP.dot(Q, R);

            if(Analysis.IsAnyNan(in shouldBeZero))
                throw new System.Exception("PrecisionReconstructTestJob: NaN detected");

            //Print.Log(shouldBeZero); 

            float zeroError = Analysis.MaxZeroError(shouldBeZero);

            return zeroError;
        }
    }


    public static Array GetEnums() {
        return Enum.GetValues(typeof(TestJob.TestType));
    }

    [TestCaseSource("GetEnums")]
    public void LUDecompTests(TestJob.TestType type)
    {
        new TestJob() { Type = type }.Run();
    }

    [Test]
    public void LUDecompErrorBenchRandom() {
        new PrecisionReconstructTestJob() { Type = PrecisionReconstructTestJob.TestType.Random }.Run();
    }

    [Test]
    public void LUDecompErrorBenchDiagonal() {
        new PrecisionReconstructTestJob() { Type = PrecisionReconstructTestJob.TestType.RandomDiagonal }.Run();
    }

}
