using System.Collections;
using System.Collections.Generic;

using LinearAlgebra;
using NUnit.Framework;
using Unity.Burst;
using Unity.Collections;

using Unity.Jobs;

using UnityEngine;
using UnityEngine.TestTools;

public class iProxyDotOperationTests
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
            }
        }

        public void SortOfAssertAreEqual(iProxy a, iProxy b)
        {
            if (a != b) UnityEngine.Debug.LogError("Failed");
        }

        public void VecVecDot()
        {
            var arena = new Arena(Allocator.Persistent);

            int vecLen = 32;

            iProxyN x = arena.iProxyVec(vecLen, 1);
            iProxyN y = arena.iProxyVec(vecLen, 1);

            iProxy b = iProxyOP.dot(x, y);

            SortOfAssertAreEqual((iProxy)vecLen, b);
            
            x = arena.iProxyVec(vecLen);
            y = arena.iProxyVec(vecLen);

            for(int i = 0; i < vecLen; i++)
            {
                x[i] = (iProxy) ((i+0) % 2);
                y[i] = (iProxy) ((i+1) % 2);
            }

            b = iProxyOP.dot(x, y);

            SortOfAssertAreEqual((iProxy)0f, b);

            arena.Dispose();
        }

        public void MatVecDot()
        {
            var arena = new Arena(Allocator.Persistent);

            int inVecLen = 20;
            int outVecLen = 5;

            iProxyN x = arena.iProxyVec(inVecLen, 1);
            iProxyMxN A = arena.iProxyRandomMatrix(outVecLen, inVecLen, -100, +100);

            iProxyN b = iProxyOP.dot(A, x);

            SortOfAssertAreEqual((iProxy) outVecLen, (iProxy) b.N);

            arena.Dispose();
        }

        public void VecMatDot()
        {
            var arena = new Arena(Allocator.Persistent);

            int vecLen = 20;

            iProxyN x = arena.iProxyIndexOneVector(vecLen);
            iProxyMxN A = arena.iProxyIdentityMatrix(vecLen);

            iProxyN b = iProxyOP.dot(x, A);

            SortOfAssertAreEqual((iProxy) vecLen, (iProxy) b.N);
            
            for (int i = 0; i < vecLen; i++)
                SortOfAssertAreEqual(x[i], b[i]);

            x = arena.iProxyIndexZeroVector(vecLen);

            b = iProxyOP.dot(x, A);

            for (int i = 0; i < vecLen; i++)
                SortOfAssertAreEqual((iProxy)i, b[i]);

            arena.Dispose();
        }

        public void MatMatDot()
        {
            var arena = new Arena(Allocator.Persistent);

            int matLen = 16;

            iProxyMxN A = arena.iProxyIdentityMatrix(matLen);
            iProxyMxN B = arena.iProxyIdentityMatrix(matLen);

            iProxyMxN C = iProxyOP.dot(A, B);

            for (int i = 0; i < matLen; i++)
            for (int j = 0; j < matLen; j++)
            {
                if (i == j)
                    SortOfAssertAreEqual((iProxy)1f, C[i, j]);
                else
                    SortOfAssertAreEqual((iProxy)0f, C[i, j]);
            }

            iProxyMxN R = arena.iProxyRandomMatrix(matLen, matLen);

            C = iProxyOP.dot(A, R);

            for (int i = 0; i < matLen; i++)
            for (int j = 0; j < matLen; j++)
            {
                SortOfAssertAreEqual(R[i, j], C[i, j]);
            }

            C = arena.iProxyIdentityMatrix(matLen);

            iProxyMxN D = iProxyOP.dot(C, C);

            for (int i = 0; i < matLen; i++)
            for (int j = 0; j < matLen; j++)
            {
                if (i == j)
                    SortOfAssertAreEqual((iProxy)1f, C[i, j]);
                else
                    SortOfAssertAreEqual((iProxy)0f, C[i, j]);
            }

            arena.Dispose();
        }

        public void MatVecDotNonSquare()
        {
            var arena = new Arena(Allocator.Persistent);

            int inVecLen = 64;
            int outVecLen = 16;

            iProxyN x = arena.iProxyVec(inVecLen, 1);
            iProxyMxN A = arena.iProxyRandomMatrix(outVecLen, inVecLen, -100, +100);

            iProxyN b = iProxyOP.dot(A, x);

            SortOfAssertAreEqual((iProxy) outVecLen, (iProxy) b.N);

            arena.Dispose();
        }

        public void VecMatDotNonSquare()
        {
            var arena = new Arena(Allocator.Persistent);

            int inVecLen = 64;
            int outVecLen = 16;

            iProxyN x = arena.iProxyVec(inVecLen, 1);
            iProxyMxN A = arena.iProxyRandomMatrix(inVecLen, outVecLen, -100, +100);

            iProxyN b = iProxyOP.dot(x, A);
            
            SortOfAssertAreEqual((iProxy) outVecLen, (iProxy) b.N);

            arena.Dispose();
        }

        public void MatMatDotNonSquare()
        {

        }

        public void OuterDot()
        {
            var arena = new Arena(Allocator.Persistent);

            int vecM = 16;
            int vecN = 32;

            iProxyN x = arena.iProxyVec(vecM, 1);
            iProxyN y = arena.iProxyVec(vecN, 1);

            iProxyMxN A = iProxyOP.outerDot(x, y);

            SortOfAssertAreEqual((iProxy)vecM, (iProxy)A.M_Rows);
            SortOfAssertAreEqual((iProxy)vecN, (iProxy)A.N_Cols);

            iProxyMxN B = iProxyOP.outerDot(y, x);

            for (int i = 0; A.Length < i; i++)
                SortOfAssertAreEqual((iProxy)1, A[i]);

            SortOfAssertAreEqual((iProxy)vecM, (iProxy)B.N_Cols);
            SortOfAssertAreEqual((iProxy)vecN, (iProxy)B.M_Rows);

            for (int i = 0; B.Length < i; i++)
                SortOfAssertAreEqual((iProxy)1, B[i]);

            x = arena.iProxyLinVector(vecM, 0, 20);
            y = arena.iProxyLinVector(vecN, 0, 20);

            iProxyMxN C = iProxyOP.outerDot(x, y);

            for (int i = 0; i < vecM; i++)
                for (int j = 0; j < vecN; j++)
                    SortOfAssertAreEqual((iProxy)(x[i] * y[j]), (iProxy)C[i, j]);

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
}
