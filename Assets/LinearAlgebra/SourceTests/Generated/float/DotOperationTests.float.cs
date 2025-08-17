using System.Collections;
using System.Collections.Generic;

using LinearAlgebra;
using NUnit.Framework;
using Unity.Burst;
using Unity.Collections;

using Unity.Jobs;

using UnityEngine;
using UnityEngine.TestTools;

public class floatDotOperationTests
{
    [BurstCompile]
    public struct DotOperationTestsJob : IJob
    {
        public enum TestType
        {
            VecVec,
            MatVec,
            VecMat,
            MatMat,
            VecMatNonSquare,
            MatVecNonSquare,
            MatMatNonSquare,
            OuterDot,
            MatMatInpl,
            MatVecInpl,
        }

        public TestType Type;

        public void Execute()
        {

            switch(Type)
            {
                case TestType.VecVec:
                    VecVecDot();
                    break;
                case TestType.MatVec:
                    MatVecDot();
                break;
                case TestType.VecMat:
                    VecMatDot();
                break;
                case TestType.MatMat:
                    MatMatDot();
                break;
                case TestType.VecMatNonSquare:
                    VecMatDotNonSquare();
                break;
                case TestType.MatVecNonSquare:
                    MatVecDotNonSquare();
                break;
                case TestType.MatMatNonSquare:
                    MatMatDotNonSquare();
                break;
                case TestType.OuterDot:
                    OuterDot();
                break;
                case TestType.MatMatInpl:
                    MatMatDotInpl();
                break;
                case TestType.MatVecInpl:
                    MatVecDotInpl();
                    break;
            }
        }

        public void VecVecDot()
        {
            var arena = new Arena(Allocator.Persistent);

            int vecLen = 32;

            floatN x = arena.floatVec(vecLen, 1f);
            floatN y = arena.floatVec(vecLen, 1f);

            float b = floatOP.dot(x, y);

            Assert.AreEqual((float)vecLen, b);
            
            x = arena.floatVec(vecLen);
            y = arena.floatVec(vecLen);

            for(int i = 0; i < vecLen; i++)
            {
                x[i] = (i+0f) % 2f;
                y[i] = (i+1f) % 2f;
            }

            b = floatOP.dot(x, y);

            Assert.AreEqual((float)0f, b);

            arena.Dispose();
        }

        public unsafe void MatVecDot()
        {
            var arena = new Arena(Allocator.Persistent);

            int inVecLen = 20;
            int outVecLen = 5;

            floatN x = arena.floatVec(inVecLen, 1f);
            floatMxN A = arena.floatRandomMatrix(outVecLen, inVecLen, -0.01f, 0.01f);

            floatN xx = x;
            floatMxN AA = A;

            floatN b = floatOP.dot(A, x);

            Assert.AreEqual(outVecLen, b.N);

            Assert.IsTrue(arena.AllocationsCount == 2);
            Assert.IsTrue(arena.TempAllocationsCount == 1);
            Assert.IsTrue(arena.DB_isTemp(b));
            Assert.IsTrue(xx.Data.Ptr == x.Data.Ptr);
            Assert.IsTrue(AA.Data.Ptr == A.Data.Ptr);

            arena.Dispose();
        }

        public void VecMatDot()
        {
            var arena = new Arena(Allocator.Persistent);

            int vecLen = 20;

            floatN x = arena.floatRandomUnitVector(vecLen);
            floatMxN A = arena.floatIdentityMatrix(vecLen, true);

            Assert.IsTrue(arena.AllocationsCount == 2 && arena.TempAllocationsCount == 0);
            floatN b = floatOP.dot(x, A);

            Assert.IsTrue(arena.AllocationsCount == 2 && arena.TempAllocationsCount == 1);
            Assert.IsTrue(arena.DB_isTemp(b));
            Assert.AreEqual(vecLen, b.N);
            
            for (int i = 0; i < vecLen; i++)
                Assert.AreEqual(x[i], b[i]);

            x = arena.floatIndexZeroVector(vecLen, true);

            Assert.IsTrue(arena.AllocationsCount == 3 && arena.TempAllocationsCount == 1);
            Assert.IsTrue(arena.DB_isTemp(b));

            b = floatOP.dot(x, A);

            Assert.IsTrue(arena.AllocationsCount == 3 && arena.TempAllocationsCount == 2);
            Assert.IsTrue(arena.DB_isTemp(b));

            for (int i = 0; i < vecLen; i++)
                Assert.AreEqual((float)i, b[i]);

            arena.Dispose();
        }

        public unsafe void MatVecDotInpl()
        {
            var arena = new Arena(Allocator.Persistent);
            floatMxN A = arena.floatMat(2, 3);
            A[0, 0] = 1; A[0, 1] = 2; A[0, 2] = 3;
            A[1, 0] = 4; A[1, 1] = 5; A[1, 2] = 6;

            floatN x = arena.floatVec(3);
            x[0] = 1;
            x[1] = 2;
            x[2] = 3;

            floatN b = arena.floatVec(2);

            b.dotInpl(A, x);

            Assert.IsTrue(arena.AllocationsCount == 3 && arena.TempAllocationsCount == 0);
            Assert.IsTrue(arena.DB_isPersistant(b));

            floatN expectedB = arena.floatVec(2);
            expectedB[0] = 14;
            expectedB[1] = 32;

            Assert.IsTrue(b.EqualsByValue(expectedB));
  
        }

        public unsafe void MatMatDotInpl()
        {
            var arena = new Arena(Allocator.Persistent);
            floatMxN A = arena.floatMat(2, 3);
            A[0, 0] = 1; A[0, 1] = 2; A[0, 2] = 3;
            A[1, 0] = 4; A[1, 1] = 5; A[1, 2] = 6;

            floatMxN B = floatOP.trans(A);

            Debug.Log($"A \n{A}");
            Debug.Log($"BB\n{B}");

            floatMxN C = arena.floatMat(2, 2, 7);
            Assert.IsTrue(arena.AllocationsCount == 2 && arena.TempAllocationsCount == 1);

            C.dotInpl(A, B);

            // no new allocations;
            Assert.IsTrue(arena.AllocationsCount == 2 && arena.TempAllocationsCount == 1);
            Assert.IsTrue(arena.DB_isPersistant(C));

            floatMxN CC = arena.floatMat(2,2);
            CC[0, 0] = 14; CC[0, 1] = 32;
            CC[1, 0] = 32; CC[1, 1] = 77;

            Debug.Log($"C \n{C}");
            Debug.Log($"CC\n{CC}");

            Assert.IsTrue(C.EqualsByValue(CC));
        }

        public unsafe void MatMatDot()
        {
            var arena = new Arena(Allocator.Persistent);

            int matLen = 16;

            floatMxN A = arena.floatIdentityMatrix(matLen, true);
            floatMxN B = arena.floatIdentityMatrix(matLen, true);

            floatMxN AA = A;
            floatMxN BB = B;

            floatMxN C = floatOP.dot(A, B);

            Assert.IsTrue(arena.AllocationsCount == 2);
            Assert.IsTrue(arena.TempAllocationsCount == 1);
            Assert.IsTrue(arena.DB_isTemp(in C));
            Assert.IsTrue(BB.Data.Ptr == B.Data.Ptr);
            Assert.IsTrue(AA.Data.Ptr == A.Data.Ptr);

            for (int i = 0; i < matLen; i++)
            for (int j = 0; j < matLen; j++)
            {
                if (i == j)
                    Assert.AreEqual((float)1f, C[i, j]);
                else
                    Assert.AreEqual((float)0f, C[i, j]);
            }

            floatMxN R = arena.floatRandomMatrix(matLen, matLen);
            
            C = floatOP.dot(A, R);

            for (int i = 0; i < matLen; i++)
            for (int j = 0; j < matLen; j++)
            {
                Assert.AreEqual(R[i, j], C[i, j]);
            }

            C = arena.floatIdentityMatrix(matLen, true);

            floatMxN D = floatOP.dot(C, C);

            for (int i = 0; i < matLen; i++)
            for (int j = 0; j < matLen; j++)
            {
                if (i == j)
                    Assert.AreEqual((float)1f, C[i, j]);
                else
                    Assert.AreEqual((float)0f, C[i, j]);
            }

            arena.Dispose();
        }

        public unsafe void MatVecDotNonSquare()
        {
            var arena = new Arena(Allocator.Persistent);

            int inVecLen = 64;
            int outVecLen = 16;

            floatN x = arena.floatVec(inVecLen, 1f);
            floatMxN A = arena.floatRandomMatrix(outVecLen, inVecLen, -0.01f, 0.01f);

            floatN xx = x;
            floatMxN AA = A;

            floatN b = floatOP.dot(A, x);

            Assert.AreEqual(outVecLen, b.N);

            Assert.IsTrue(arena.AllocationsCount == 2);
            Assert.IsTrue(arena.TempAllocationsCount == 1);
            Assert.IsTrue(arena.DB_isTemp(b));
            Assert.IsTrue(xx.Data.Ptr == x.Data.Ptr);
            Assert.IsTrue(AA.Data.Ptr == A.Data.Ptr);

            arena.Dispose();
        }

        public void VecMatDotNonSquare()
        {
            var arena = new Arena(Allocator.Persistent);

            int inVecLen = 64;
            int outVecLen = 16;

            floatN x = arena.floatVec(inVecLen, 1f);
            floatMxN A = arena.floatRandomMatrix(inVecLen, outVecLen, -0.01f, 0.01f);

            floatN b = floatOP.dot(x, A);
            
            Assert.AreEqual(outVecLen, b.N);

            arena.Dispose();
        }

        public void MatMatDotNonSquare()
        {

        }

        public void OuterDot()
        {
            var arena = new Arena(Allocator.Persistent);

            int vecM = 32;
            int vecN = 64;

            floatN x = arena.floatVec(vecM, 1f);
            floatN y = arena.floatVec(vecN, 1f);

            floatMxN A = floatOP.outerDot(x, y);

            Assert.AreEqual(vecM, A.M_Rows);
            Assert.AreEqual(vecN, A.N_Cols);

            floatMxN B = floatOP.outerDot(y, x);

            for (int i = 0; A.Length < i; i++)
                Assert.AreEqual((float)1f, A[i]);

            Assert.AreEqual(vecM, B.N_Cols);
            Assert.AreEqual(vecN, B.M_Rows);

            for (int i = 0; B.Length < i; i++)
                Assert.AreEqual((float)1f, B[i]);

            x = arena.floatLinVector(vecM, 0f, 2f, true);
            y = arena.floatLinVector(vecN, 0f, 2f, true);

            floatMxN C = floatOP.outerDot(x, y);

            for (int i = 0; i < vecM; i++)
                for (int j = 0; j < vecN; j++)
                    Assert.AreEqual(x[i] * y[j], C[i, j]);

            arena.Dispose();
        }
    }

    [Test]
    public void VecVecDotDet()
    {
        new DotOperationTestsJob() { Type = DotOperationTestsJob.TestType.VecVec }.Run();
    }

    [Test]
    public void MatrixVectorDotTest()
    {
        new DotOperationTestsJob() { Type = DotOperationTestsJob.TestType.MatVec }.Run();
    }

    [Test]
    public void VectorMatrixDotTest()
    {
        new DotOperationTestsJob() { Type = DotOperationTestsJob.TestType.VecMat }.Run();
    }

    [Test]
    public void MatrixMatrixDotTest()
    {
        new DotOperationTestsJob() { Type = DotOperationTestsJob.TestType.MatMat }.Run();
    }

    [Test]
    public void MatrixVectorDotNonSquareTest()
    {
        new DotOperationTestsJob() { Type = DotOperationTestsJob.TestType.MatVecNonSquare }.Run();
    }

    [Test]
    public void VectorMatrixDotNonSquareTest()
    {
        new DotOperationTestsJob() { Type = DotOperationTestsJob.TestType.VecMatNonSquare }.Run();
    }

    [Test]
    public void MatrixMatrixDotNonSquareTest()
    {
        new DotOperationTestsJob() { Type = DotOperationTestsJob.TestType.MatMatNonSquare }.Run();
    }

    [Test]
    public void OuterDotTest()
    {
        new DotOperationTestsJob() { Type = DotOperationTestsJob.TestType.OuterDot }.Run();
    }

    [Test]
    public void MatMatInplTest()
    {
        new DotOperationTestsJob() { Type = DotOperationTestsJob.TestType.MatMatInpl }.Run();
    }

    [Test]
    public void MatVecInplTest()
    {
        new DotOperationTestsJob() { Type = DotOperationTestsJob.TestType.MatVecInpl }.Run();
    }
}
