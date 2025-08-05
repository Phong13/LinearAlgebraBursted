using System;
using LinearAlgebra;
using NUnit.Framework;
using Unity.Burst;
using Unity.Collections;

using Unity.Jobs;

public class floatOperationsTest {

    [BurstCompile]
    public struct BasicVecOpTestJob : IJob
    {
        public unsafe void Execute()
        {
            var arena = new Arena(Allocator.Persistent);

            int vecLen = 16;

            float s = 1f;
            floatN a = arena.floatVec(vecLen, 10f);

            Assert.AreEqual(vecLen, a.N); 

            floatN b = arena.floatVec(vecLen, 10f);

            Assert.AreEqual(a[vecLen/2], b[vecLen/2]);
            
            Assert.AreEqual(2, arena.AllocationsCount);

            floatN result = default;

            result = a + s;
            Assert.IsTrue(arena.DB_isTemp(result));

            result = s + a;

            result = a - s;
            result = s - a;

            Assert.IsTrue(arena.DB_isTemp(result));

            Assert.AreEqual(4, arena.TempAllocationsCount);

            arena.ClearTemp();

            Assert.IsTrue(arena.DB_isPersistant(a));
            Assert.IsTrue(arena.DB_isPersistant(b));

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

            Assert.AreEqual(11, arena.TempAllocationsCount);
            arena.Clear();

            // inpl operations should not move the vector or matrix that it is being called on.
            floatN c = arena.floatVec(vecLen, 0f);
            floatN d = c; // both c and d point at the same buffer.

            a = arena.floatVec(vecLen, 10f);
            b = arena.floatVec(vecLen, 10f);

            Assert.IsTrue(arena.DB_isPersistant(in c));
            Assert.IsTrue(c.Data.Ptr == d.Data.Ptr);
            c.addInpl(a);
            Assert.IsTrue(c.Data.Ptr == d.Data.Ptr); // verify that c still points to the same buffer
            c.subInpl(b);
            Assert.IsTrue(c.Data.Ptr == d.Data.Ptr); // verify that c still points ot the same buffer.
            c.signFlipInpl();
            Assert.IsTrue(c.Data.Ptr == d.Data.Ptr); // verify that c still points ot the same buffer.
            
            c.modInpl(3);
            Assert.IsTrue(c.Data.Ptr == d.Data.Ptr); // verify that c still points ot the same buffer.
            c.divInpl(5);
            Assert.IsTrue(c.Data.Ptr == d.Data.Ptr); // verify that c still points ot the same buffer.
            c.compMulInpl(a);
            Assert.IsTrue(c.Data.Ptr == d.Data.Ptr); // verify that c still points ot the same buffer.

            floatN nonZero = arena.tempfloatVec(c.N, 4);
            
            c.compDivInpl(nonZero);

            

            Assert.IsTrue(c.Data.Ptr == d.Data.Ptr); // verify that c still points ot the same buffer.

            Assert.IsTrue(arena.DB_isPersistant(in c)); // verify c has not been converted
            arena.Dispose();
            arena.Dispose();
            arena.Dispose();
        }
    }

    [Test]
    public void VecAddInPlace()
    {
        var arena = new Arena(Allocator.Persistent);
        floatN a = arena.floatVec(3);
        a[0] = 1;
        a[1] = 2;
        a[2] = 3;
         
        floatN b = arena.floatVec(3);
        b[0] = 5;
        b[1] = 6;
        b[2] = 7;

        floatN c = arena.floatVec(3, 7);
        c.addInpl(a, b);

        Assert.IsTrue(arena.AllocationsCount == 3 && arena.TempAllocationsCount == 0);

        floatN cc = arena.floatVec(3, 7);
        cc[0] = 6;
        cc[1] = 8;
        cc[2] = 10;

        Assert.IsTrue(c.EqualsByValue(cc));
        Assert.IsTrue(arena.DB_isPersistant(c));

        arena.Dispose();
    }

    [Test]
    public void VecSubInPlace()
    {
        var arena = new Arena(Allocator.Persistent);
        floatN a = arena.floatVec(3);
        a[0] = 1;
        a[1] = 2;
        a[2] = 3;

        floatN b = arena.floatVec(3);
        b[0] = 5;
        b[1] = 6;
        b[2] = 7;

        floatN c = arena.floatVec(3, 7);
        c.subInpl(a, b);

        Assert.IsTrue(arena.AllocationsCount == 3 && arena.TempAllocationsCount == 0);

        floatN cc = arena.floatVec(3, 7);
        cc[0] = -4;
        cc[1] = -4;
        cc[2] = -4;

        Assert.IsTrue(c.EqualsByValue(cc));
        Assert.IsTrue(arena.DB_isPersistant(c));

        arena.Dispose();
    }

    [Test]
    public void MatAddInPlace()
    {
        var arena = new Arena(Allocator.Persistent);
        floatMxN A = arena.floatMat(2, 3);
        A[0, 0] = 1; A[0, 1] = 2; A[0, 2] = 3;
        A[1, 0] = 4; A[1, 1] = 5; A[1, 2] = 6;

        floatMxN B = arena.floatMat(2, 3);
        B[0, 0] = 2; B[0, 1] = 3; B[0, 2] = 4;
        B[1, 0] = 5; B[1, 1] = 6; B[1, 2] = 7;

        floatMxN C = arena.floatMat(2, 3, 7);
        C.addInpl(A, B);

        Assert.IsTrue(arena.AllocationsCount == 3 && arena.TempAllocationsCount == 0);

        floatMxN CC = arena.floatMat(2, 3);
        CC[0, 0] = 3; CC[0, 1] = 5; CC[0, 2] = 7;
        CC[1, 0] = 9; CC[1, 1] = 11; CC[1, 2] = 13;

        Assert.IsTrue(C.EqualsByValue(CC));
        Assert.IsTrue(arena.DB_isPersistant(C));

        arena.Dispose();
    }

    [Test]
    public void MatSubInPlace()
    {
        var arena = new Arena(Allocator.Persistent);
        floatMxN A = arena.floatMat(2, 3);
        A[0, 0] = 1; A[0, 1] = 2; A[0, 2] = 3;
        A[1, 0] = 4; A[1, 1] = 5; A[1, 2] = 6;

        floatMxN B = arena.floatMat(2, 3);
        B[0, 0] = 2; B[0, 1] = 3; B[0, 2] = 4;
        B[1, 0] = 5; B[1, 1] = 6; B[1, 2] = 7;

        floatMxN C = arena.floatMat(2, 3, 7);
        C.subInpl(A, B);

        Assert.IsTrue(arena.AllocationsCount == 3 && arena.TempAllocationsCount == 0);

        floatMxN CC = arena.floatMat(2, 3);
        CC[0, 0] = -1; CC[0, 1] = -1; CC[0, 2] = -1;
        CC[1, 0] = -1; CC[1, 1] = -1; CC[1, 2] = -1;

        Assert.IsTrue(C.EqualsByValue(CC));
        Assert.IsTrue(arena.DB_isPersistant(C));

        arena.Dispose();
    }

    [Test]
    public void BasicVecOperationsSimple()
    {
        new BasicVecOpTestJob().Run();
    }

    [BurstCompile]
    public struct BasicMatOpTestJob : IJob
    {
        public void SortOfAssert(bool check)
        {
            if (!check) UnityEngine.Debug.LogError("failed");
        }

        public unsafe void Execute()
        {
            var arena = new Arena(Allocator.Persistent);

            int rows = 8;
            int cols = 8;

            int elements = rows * cols;

            float s = 1f;
            floatMxN a = arena.floatMat(rows, cols, 10f);

            floatMxN b = arena.floatMat(rows, cols, 10f);

            floatMxN aa = a;
            floatMxN bb = b;


            floatMxN result = default;

            SortOfAssert(arena.TempAllocationsCount == 0);
            SortOfAssert(arena.AllocationsCount == 2);
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
            SortOfAssert(arena.TempAllocationsCount == 15);
            SortOfAssert(arena.AllocationsCount == 2);

            // Check that the buffers a and b were not moved by any of these operations
            SortOfAssert(aa.Data.Ptr == a.Data.Ptr);
            SortOfAssert(bb.Data.Ptr == b.Data.Ptr);

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

        public void SignFlipVec()
        {
            var arena = new Arena(Allocator.Persistent);

            int vecLen = 16;

            floatN a = arena.floatVec(vecLen, 10f);

            a = -a;

            for (int i = 0; i < vecLen; i++)
                Assert.AreEqual(-(float)10f, a[i]);

            arena.Dispose();
        }

        public void AddVec()
        {
            var arena = new Arena(Allocator.Persistent);

            int vecLen = 16;

            floatN a = arena.floatVec(vecLen, 10f);

            for (int i = 0; i < vecLen; i++)
                Assert.AreEqual((float)10d, a[i]);

            a += 1f;
            
            for (int i = 0; i < vecLen; i++)
                Assert.AreEqual((float)11d, a[i]);

            floatN r = arena.floatVec(vecLen, 5f);

            a += r;

            for (int i = 0; i < vecLen; i++)
                Assert.AreEqual((float)16, a[i]);

            arena.Dispose();
        }

        public void SubVec()
        {
            var arena = new Arena(Allocator.Persistent);

            int vecLen = 16;

            floatN a = arena.floatVec(vecLen, 10f);

            a -= 1f;
            
            for (int i = 0; i < vecLen; i++)
                Assert.AreEqual((float)9f, a[i]);

            floatN r = arena.floatVec(vecLen, 5f);

            a -= r;

            for (int i = 0; i < vecLen; i++)
                Assert.AreEqual((float)4d, a[i]);

            a = arena.floatVec(vecLen, 10f);
            
            a = 1f - a;

            for (int i = 0; i < vecLen; i++)
                Assert.AreEqual(-(float)9d, a[i]);

            arena.Dispose();
        }

        public void MulVec()
        {
            var arena = new Arena(Allocator.Persistent);

            int vecLen = 16;

            floatN a = arena.floatVec(vecLen, 1f);

            a *= 1f;

            for (int i = 0; i < vecLen; i++)
                Assert.AreEqual((float)1d, a[i]);

            a *= 2f;

            for (int i = 0; i < vecLen; i++)
                Assert.AreEqual((float)2d, a[i]);
                        
            a = arena.floatIndexZeroVector(vecLen);

            a *= 2f;
            
            for (int i = 0; i < vecLen; i++)
                Assert.AreEqual((float)(2d*i), a[i]);

            a = arena.floatIndexZeroVector(vecLen);
            floatN b = arena.floatIndexZeroVector(vecLen);

            var c = a * b;

            for (int i = 0; i < vecLen; i++)
                Assert.AreEqual((float)(i * i), c[i]);

            arena.Dispose();
        }

        public void DivVec()
        {
            var arena = new Arena(Allocator.Persistent);

            int vecLen = 16;

            floatN a = arena.floatVec(vecLen, 1f);

            a /= 1f;

            for (int i = 0; i < vecLen; i++)
                Assert.AreEqual((float)1f, a[i]);

            a /= 2f;

            for (int i = 0; i < vecLen; i++)
                Assert.AreEqual((float)0.5f, a[i]);

            a = arena.floatIndexZeroVector(vecLen);

            a /= 2f;

            for (int i = 0; i < vecLen; i++)
                Assert.AreEqual((float)0.5f * i, a[i]);

            a = arena.floatIndexZeroVector(vecLen);
            floatN b = arena.floatIndexZeroVector(vecLen);

            // add 1 so no division by zero
            a += 1f;
            b += 1f;

            var c0 = a / b;
            var c1 = b / a;

            for (int i = 0; i < vecLen; i++)
            {
                Assert.AreEqual((float)1f, c0[i]);
                Assert.AreEqual((float)1f, c1[i]);
            }

            a = arena.floatVec(vecLen, 2f);

            a = 2f / a;

            for (int i = 0; i < vecLen; i++)
                Assert.AreEqual((float)1f, a[i]);   

            arena.Dispose();
        }

        public void ModVec()
        {
            var arena = new Arena(Allocator.Persistent);

            int vecLen = 16;

            floatN a = arena.floatVec(vecLen, 10f);

            for (int i = 0; i < vecLen; i++)
                Assert.AreEqual((float)10f, a[i]);

            a %= 2f;

            for (int i = 0; i < vecLen; i++)
                Assert.AreEqual((float)0f, a[i]);

            a = arena.floatIndexZeroVector(vecLen);

            a %= 2f;

            for (int i = 0; i < vecLen; i++)
                Assert.AreEqual((float)(i % (float)2d), a[i]);

            a = arena.floatIndexZeroVector(vecLen);
            floatN b = arena.floatIndexZeroVector(vecLen);

            // add 1 so no division by zero
            a += 1f;
            b += 1f;

            var c0 = a % b;
            var c1 = b % a;

            for (int i = 0; i < vecLen; i++)
            {
                Assert.AreEqual((float)0f, c0[i]);
                Assert.AreEqual((float)0f, c1[i]);
            }

            arena.Dispose();
        }

        public void SignFlipMat()
        {
            var arena = new Arena(Allocator.Persistent);

            int vecLen = 16;
            int totalElements = vecLen * vecLen;
            floatMxN a = arena.floatMat(vecLen, vecLen, 10f);

            a = -a;

            for (int i = 0; i < totalElements; i++)
                Assert.AreEqual(-(float)10f, a[i]);

            arena.Dispose();
        }

        public void AddMat()
        {
            var arena = new Arena(Allocator.Persistent);

            int rows = 8;
            int cols = 8;
            int totalElements = rows * cols;

            floatMxN a = arena.floatMat(rows, cols, 10f);

            // Element-wise addition with scalar
            a += 1f;

            for (int i = 0; i < totalElements; i++)
                Assert.AreEqual((float)11f, a[i]);

            arena.Dispose();
        }

        public void SubMat()
        {
            var arena = new Arena(Allocator.Persistent);

            int rows = 8;
            int cols = 8;
            int totalElements = rows * cols;

            floatMxN a = arena.floatMat(rows, cols, 10f);

            // Element-wise subtraction with scalar
            a -= 5f;

            for (int i = 0; i < totalElements; i++)
                Assert.AreEqual((float)5f, a[i]);

            arena.Dispose();
        }

        public void MulMat()
        {
            var arena = new Arena(Allocator.Persistent);

            int rows = 8;
            int cols = 8;
            int totalElements = rows * cols;

            floatMxN a = arena.floatMat(rows, cols, 2f);

            // Element-wise multiplication with scalar
            a *= 3f;

            for (int i = 0; i < totalElements; i++)
                Assert.AreEqual((float)6f, a[i]);

            a = 3f * a;

            for (int i = 0; i < totalElements; i++)
                Assert.AreEqual((float)18f, a[i]);

            var b = arena.floatMat(rows, cols, 0.5f);

            a = a * b;

            for (int i = 0; i < totalElements; i++)
                Assert.AreEqual((float)9f, a[i]);

            arena.Dispose();
        }

        public void DivMat()
        {
            var arena = new Arena(Allocator.Persistent);

            int rows = 8;
            int cols = 8;
            int totalElements = rows * cols;

            floatMxN a = arena.floatMat(rows, cols, 10f);

            // Element-wise division with scalar
            a /= 2f;

            for (int i = 0; i < totalElements; i++)
                Assert.AreEqual((float)5f, a[i]);

            a = 5f / a;

            for (int i = 0; i < totalElements; i++)
                Assert.AreEqual((float)1f, a[i]);

            floatMxN b = arena.floatMat(rows, cols, 0.5f);

            var c0 = a / b;
            var c1 = b / a;

            for (int i = 0; i < totalElements; i++)
            {
                Assert.AreEqual((float)2f, c0[i]);
                Assert.AreEqual((float)0.5f, c1[i]);
            }

            arena.Dispose();
        }

        public void ModMat()
        {
            var arena = new Arena(Allocator.Persistent);

            int rows = 8;
            int cols = 8;
            int totalElements = rows * cols;

            floatMxN a = arena.floatMat(rows, cols, 10f);

            // Element-wise modulo with scalar
            a %= 3f;

            for (int i = 0; i < totalElements; i++)
                Assert.AreEqual((float)1f, a[i]);

            a = arena.floatMat(rows, cols, 4f);

            a = 4f % a;

            for (int i = 0; i < totalElements; i++)
                Assert.AreEqual((float)0f, a[i]);

            a = arena.floatMat(rows, cols, 3f);
            floatMxN b = arena.floatMat(rows, cols, 2f);

            a = a % b;

            for (int i = 0; i < totalElements; i++)
                Assert.AreEqual((float)1f, a[i]);

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
