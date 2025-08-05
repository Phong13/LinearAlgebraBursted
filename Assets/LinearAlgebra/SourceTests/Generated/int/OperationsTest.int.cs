using System;
using LinearAlgebra;
using NUnit.Framework;
using Unity.Burst;
using Unity.Collections;

using Unity.Jobs;

public class intOperationsTest {

    [BurstCompile]
    public struct BasicVecOpTestJob : IJob
    {
        public void SortOfAssertAreEqual(int a, int b)
        {
            if (a != b) UnityEngine.Debug.LogError("Failed");
        }

        public void Execute()
        {
            var arena = new Arena(Allocator.Persistent);

            int vecLen = 16;

            int s = 1;
            intN a = arena.intVec((int) vecLen, (int) 10);


            SortOfAssertAreEqual(vecLen, (int) a.N); 

            intN b = arena.intVec((int) vecLen, (int) 10);

            SortOfAssertAreEqual(a[(int) vecLen/2], b[ (int) vecLen/2]);
            
            SortOfAssertAreEqual((int) 2, (int) arena.AllocationsCount);

            intN result = default;

            result = a + s;

            result = s + a;

            result = a - s;
            result = s - a;

            SortOfAssertAreEqual(4, (int) arena.TempAllocationsCount);

            result = ~a;

            arena.ClearTemp();

            result = a * s;
            result = s * a;

            result = a / s;
            result = a % s;
            result = s / a;
            result = s % a;

            result = a | s;
            result = s | a;

            result = a & s;
            result = s & a;

            result = a ^ s;
            result = s ^ a;

            result = result << 5;
            result = result >> 5;

            result = a + b;
            result = a - b;
            result = a * b;
            result = a / b;
            result = a % b;

            result = a | b;
            result = a & b;
            result = a ^ b;

            //SortOfAssertAreEqual(11, arena.TempAllocationsCount);

            arena.Dispose();
        }
    }

    [Test]
    public void BasicVecOperationsSimple()
    {
        new BasicVecOpTestJob().Run();
    }

    [BurstCompile]
    public struct BasicMatOpTestJob : IJob
    {
        public void Execute()
        {
            var arena = new Arena(Allocator.Persistent);

            int rows = 8;
            int cols = 8;

            int elements = rows * cols;

            int s = 1;
            intMxN a = arena.intMat(rows, cols, 10);

            intMxN b = arena.intMat(rows, cols, 10);

            intMxN result = default;

            result = a + s;

            result = s + a;

            result = a - s;
            result = s - a;

            result = a * s;
            result = s * a;

            result = a / s;
            result = a % s;
            result = s / a;
            result = s % a;

            result = a + b;
            result = a - b;
            result = a * b;
            result = a / b;
            result = a % b;

            arena.Dispose();
        }
    }

    [Test]
    public void BasicMatOperationsSimple()
    {
        new BasicMatOpTestJob().Run();
    }
    
    [BurstCompile]
    public struct BasicPreciseOPTestJob : IJob
    {
        public enum TestType
        {
            AddVec,
            SubVec,
            MulVec,
            DivVec,
            ModVec,
            SignFlipVec,

            AddMat,
            SubMat,
            MulMat,
            DivMat,
            ModMat,
            SignFlipMat,
        }

        public TestType Type;

        public void Execute()
        {
            switch(Type)
            {
                case TestType.AddVec:
                    AddVec();
                break;

                case TestType.SubVec:
                    SubVec();
                    break;

                case TestType.MulVec:
                    MulVec();
                    break;

                case TestType.DivVec:
                    DivVec();
                    break;

                case TestType.ModVec:
                    ModVec();
                    break;
                case TestType.SignFlipVec:
                    SignFlipVec();
                    break;
                // Matrix operations
                case TestType.AddMat:
                    AddMat();
                    break;
                case TestType.SubMat:
                    SubMat();
                    break;
                case TestType.MulMat:
                    MulMat();
                    break;
                case TestType.DivMat:
                    DivMat();
                    break;
                case TestType.ModMat:
                    ModMat();
                    break;
                case TestType.SignFlipMat:
                    SignFlipVec();
                    break;

            }
        }

        public void SortOfAssertAreEqual(int a, int b)
        {
            if (a != b) UnityEngine.Debug.LogError("Failed");
        }

        public void SignFlipVec()
        {
            var arena = new Arena(Allocator.Persistent);

            int vecLen = 16;

            intN a = arena.intVec(vecLen, 10);

            a = -a;

            for (int i = 0; i < vecLen; i++)
                SortOfAssertAreEqual(-(int)10f, a[i]);

            arena.Dispose();
        }

        public void AddVec()
        {
            var arena = new Arena(Allocator.Persistent);

            int vecLen = 16;

            intN a = arena.intVec(vecLen, 10);

            for (int i = 0; i < vecLen; i++)
                SortOfAssertAreEqual((int)10d, a[i]);

            a += 1;
            
            for (int i = 0; i < vecLen; i++)
                SortOfAssertAreEqual((int)11d, a[i]);

            intN r = arena.intVec(vecLen, 5);

            a += r;

            for (int i = 0; i < vecLen; i++)
                SortOfAssertAreEqual((int)16, a[i]);

            arena.Dispose();
        }

        public void SubVec()
        {
            var arena = new Arena(Allocator.Persistent);

            int vecLen = 16;

            intN a = arena.intVec(vecLen, 10);

            a -= 1;
            
            for (int i = 0; i < vecLen; i++)
                SortOfAssertAreEqual((int)9f, a[i]);

            intN r = arena.intVec(vecLen, 5);

            a -= r;

            for (int i = 0; i < vecLen; i++)
                SortOfAssertAreEqual((int)4d, a[i]);

            a = arena.intVec(vecLen, 10);
            
            a = 1 - a;

            for (int i = 0; i < vecLen; i++)
                SortOfAssertAreEqual(-(int)9d, a[i]);

            arena.Dispose();
        }

        public void MulVec()
        {
            var arena = new Arena(Allocator.Persistent);

            int vecLen = 16;

            intN a = arena.intVec(vecLen, 1);

            a *= 1;

            for (int i = 0; i < vecLen; i++)
                SortOfAssertAreEqual((int)1d, a[i]);

            a *= 2;

            for (int i = 0; i < vecLen; i++)
                SortOfAssertAreEqual((int)2d, a[i]);
                        
            a = arena.intIndexZeroVector(vecLen);

            a *= 2;
            
            for (int i = 0; i < vecLen; i++)
                SortOfAssertAreEqual((int)(2d*i), a[i]);

            a = arena.intIndexZeroVector(vecLen);
            intN b = arena.intIndexZeroVector(vecLen);

            var c = a * b;

            for (int i = 0; i < vecLen; i++)
                SortOfAssertAreEqual((int)(i * i), c[i]);

            arena.Dispose();
        }

        public void DivVec()
        {
            var arena = new Arena(Allocator.Persistent);

            int vecLen = 16;

            intN a = arena.intVec(vecLen, 2);

            a /= 2;

            for (int i = 0; i < vecLen; i++)
                SortOfAssertAreEqual((int)1, a[i]);

            a /= 1;

            for (int i = 0; i < vecLen; i++)
                SortOfAssertAreEqual((int)1, a[i]);

            a = arena.intIndexZeroVector(vecLen);

            a /= 2;

            for (int i = 0; i < vecLen; i++)
                SortOfAssertAreEqual((int)(0.5 * i), a[i]);

            a = arena.intIndexZeroVector(vecLen);
            intN b = arena.intIndexZeroVector(vecLen);

            // add 1 so no division by zero
            a += 1;
            b += 1;

            var c0 = a / b;
            var c1 = b / a;

            for (int i = 0; i < vecLen; i++)
            {
                SortOfAssertAreEqual((int)1, c0[i]);
                SortOfAssertAreEqual((int)1, c1[i]);
            }

            a = arena.intVec(vecLen, 2);

            a = 2 / a;

            for (int i = 0; i < vecLen; i++)
                SortOfAssertAreEqual((int)1, a[i]);   

            arena.Dispose();
        }

        public void ModVec()
        {
            var arena = new Arena(Allocator.Persistent);

            int vecLen = 16;

            intN a = arena.intVec(vecLen, 10);

            for (int i = 0; i < vecLen; i++)
                SortOfAssertAreEqual((int)10, a[i]);

            a %= 2;

            for (int i = 0; i < vecLen; i++)
                SortOfAssertAreEqual((int)0, a[i]);

            a = arena.intIndexZeroVector(vecLen);

            a %= 2;

            for (int i = 0; i < vecLen; i++)
                SortOfAssertAreEqual((int)(i % (int)2), a[i]);

            a = arena.intIndexZeroVector(vecLen);
            intN b = arena.intIndexZeroVector(vecLen);

            // add 1 so no division by zero
            a += 1;
            b += 1;

            var c0 = a % b;
            var c1 = b % a;

            for (int i = 0; i < vecLen; i++)
            {
                SortOfAssertAreEqual((int)0, c0[i]);
                SortOfAssertAreEqual((int)0, c1[i]);
            }

            arena.Dispose();
        }

        public void SignFlipMat()
        {
            var arena = new Arena(Allocator.Persistent);

            int vecLen = 16;
            int totalElements = vecLen * vecLen;
            intMxN a = arena.intMat(vecLen, vecLen, 10);

            a = -a;

            for (int i = 0; i < totalElements; i++)
                SortOfAssertAreEqual(-(int)10f, a[i]);

            arena.Dispose();
        }

        public void AddMat()
        {
            var arena = new Arena(Allocator.Persistent);

            int rows = 8;
            int cols = 8;
            int totalElements = rows * cols;

            intMxN a = arena.intMat(rows, cols, 10);

            // Element-wise addition with scalar
            a += 1;

            for (int i = 0; i < totalElements; i++)
                SortOfAssertAreEqual((int)11f, a[i]);

            arena.Dispose();
        }

        public void SubMat()
        {
            var arena = new Arena(Allocator.Persistent);

            int rows = 8;
            int cols = 8;
            int totalElements = rows * cols;

            intMxN a = arena.intMat(rows, cols, 10);

            // Element-wise subtraction with scalar
            a -= 5;

            for (int i = 0; i < totalElements; i++)
                SortOfAssertAreEqual((int)5f, a[i]);

            arena.Dispose();
        }

        public void MulMat()
        {
            var arena = new Arena(Allocator.Persistent);

            int rows = 8;
            int cols = 8;
            int totalElements = rows * cols;

            intMxN a = arena.intMat(rows, cols, 2);

            // Element-wise multiplication with scalar
            a *= 3;

            for (int i = 0; i < totalElements; i++)
                SortOfAssertAreEqual((int)6f, a[i]);

            a = 3 * a;

            for (int i = 0; i < totalElements; i++)
                SortOfAssertAreEqual((int)18f, a[i]);

            arena.Dispose();
        }

        public void DivMat()
        {
            var arena = new Arena(Allocator.Persistent);

            int rows = 8;
            int cols = 8;
            int totalElements = rows * cols;

            intMxN a = arena.intMat(rows, cols, 10);

            // Element-wise division with scalar
            a /= 2;

            for (int i = 0; i < totalElements; i++)
                SortOfAssertAreEqual((int)5, a[i]);

            a = 5 / a;

            for (int i = 0; i < totalElements; i++)
                SortOfAssertAreEqual((int)1, a[i]);

            arena.Dispose();
        }

        public void ModMat()
        {
            var arena = new Arena(Allocator.Persistent);

            int rows = 8;
            int cols = 8;
            int totalElements = rows * cols;

            intMxN a = arena.intMat(rows, cols, 10);

            // Element-wise modulo with scalar
            a %= 3;

            for (int i = 0; i < totalElements; i++)
                SortOfAssertAreEqual((int)1f, a[i]);

            a = arena.intMat(rows, cols, 4);

            a = 4 % a;

            for (int i = 0; i < totalElements; i++)
                SortOfAssertAreEqual((int)0f, a[i]);

            a = arena.intMat(rows, cols, 3);
            intMxN b = arena.intMat(rows, cols, 2);

            a = a % b;

            for (int i = 0; i < totalElements; i++)
                SortOfAssertAreEqual((int)1f, a[i]);

            arena.Dispose();
        }
    }

    public static Array GetEnums()
    {
        return Enum.GetValues(typeof(BasicPreciseOPTestJob.TestType));
    }

    [TestCaseSource("GetEnums")]
    public void TestCases(BasicPreciseOPTestJob.TestType type)
    {
        new BasicPreciseOPTestJob() { Type = type }.Run();
    }

}
