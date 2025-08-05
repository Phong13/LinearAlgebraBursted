using LinearAlgebra;
using NUnit.Framework;
using Unity.Burst;
using Unity.Collections;

using Unity.Jobs;
using Unity.Mathematics;



public class floatInitTest
{
    [BurstCompile]
    public struct InitVecTestJob : IJob
    {
        public void Execute()
        {
            var arena = new Arena(Allocator.Persistent);

            Assert.AreEqual(0, arena.AllocationsCount);

            int vecLen = 7;

            floatN vec = arena.floatVec(vecLen);

            Assert.AreEqual(vecLen, vec.N);
            Assert.AreEqual(1, arena.AllocationsCount);
            
            arena.Clear();

            Assert.AreEqual(0, arena.AllocationsCount);
        }
    }

    [Test]
    public void InitTestVecPass()
    {
        new InitVecTestJob().Run();
    }

    [BurstCompile]
    public struct InitMatrixTestJob : IJob
    {
        public void Execute()
        {
            var arena = new Arena(Allocator.Persistent);

            int lenRows = 7;
            int lenColumns = 7;

            floatMxN vec = arena.floatMat(lenRows, lenColumns);

            Assert.AreEqual(lenRows * lenColumns, vec.Length);
            Assert.AreEqual(1, arena.AllocationsCount);

            {
                float3 v0 = new float3(1, 2, 3);
                floatN vv0 = arena.floatVec(v0);
                Assert.IsTrue(v0[0] == vv0[0] && v0[1] == vv0[1] && v0[2] == vv0[2]);
                Assert.IsTrue(arena.DB_isPersistant(vv0));
            }
            {
                float3 v0 = new float3(1, 2, 3);
                floatN vvv0 = arena.tempfloatVec(v0);
                Assert.IsTrue(v0[0] == vvv0[0] && v0[1] == vvv0[1] && v0[2] == vvv0[2]);
                Assert.IsTrue(arena.DB_isTemp(vvv0));
            }

            {
                float4 v0 = new float4(1, 2, 3, 4);
                floatN vv0 = arena.floatVec(v0);
                Assert.IsTrue(v0[0] == vv0[0] && v0[1] == vv0[1] && v0[2] == vv0[2] && v0[3] == vv0[3]);
                Assert.IsTrue(arena.DB_isPersistant(vv0));

                floatN vvv0 = arena.tempfloatVec(v0);
                Assert.IsTrue(v0[0] == vvv0[0] && v0[1] == vvv0[1] && v0[2] == vvv0[2] && v0[3] == vv0[3]);
                Assert.IsTrue(arena.DB_isTemp(vvv0));
            }

            {
                float3x3 m0;
                m0.c0 = new float3(1, 2, 3);
                m0.c1 = new float3(4, 5, 6);
                m0.c2 = new float3(7, 8, 9);
                floatMxN mm0 = arena.floatMat(m0);
                Assert.IsTrue(m0.c0.x == mm0[0, 0] && m0.c1.x == mm0[0, 1] && m0.c2.x == mm0[0, 2]);
                Assert.IsTrue(m0.c0.y == mm0[1, 0] && m0.c1.y == mm0[1, 1] && m0.c2.y == mm0[1, 2]);
                Assert.IsTrue(m0.c0.z == mm0[2, 0] && m0.c1.z == mm0[2, 1] && m0.c2.z == mm0[2, 2]);
                Assert.IsTrue(arena.DB_isPersistant(mm0));
            }

            {
                float4x4 m0;
                m0.c0 = new float4(1, 2, 3,4);
                m0.c1 = new float4(4, 5, 6,5);
                m0.c2 = new float4(7, 8, 9,6);
                m0.c3 = new float4(5, 6, 7, 8);
                m0.c0.w = 3; m0.c1.w = 4; m0.c2.w = 5; m0.c3.w = 9;
                floatMxN mm0 = arena.floatMat(m0);

                UnityEngine.Debug.Log($"m0 \n {m0}");
                UnityEngine.Debug.Log($"mm0 \n {mm0}");

                Assert.IsTrue(m0.c0.x == mm0[0, 0] && m0.c1.x == mm0[0, 1] && m0.c2.x == mm0[0, 2] && m0.c3.x == mm0[0, 3]);
                Assert.IsTrue(m0.c0.y == mm0[1, 0] && m0.c1.y == mm0[1, 1] && m0.c2.y == mm0[1, 2] && m0.c3.y == mm0[1, 3]);
                Assert.IsTrue(m0.c0.z == mm0[2, 0] && m0.c1.z == mm0[2, 1] && m0.c2.z == mm0[2, 2] && m0.c3.z == mm0[2, 3]);
                Assert.IsTrue(m0.c0.w == mm0[3, 0] && m0.c1.w == mm0[3, 1] && m0.c2.w == mm0[3, 2] && m0.c3.w == mm0[3, 3]);
                Assert.IsTrue(arena.DB_isPersistant(mm0));
            }

            arena.ClearTemp();

            /*
            {
                floatN v0 = arena.floatVec(new float[] { 1,2,3,4,5,6,7});
                Assert.IsTrue(arena.DB_isPersistant(v0));
                float3 f3 = v0.GetSubvecAsFloat3(2);
                Assert.IsTrue(f3.x == 3 && f3.y == 4 && f3.z == 5);
            }

            {
                floatN v0 = arena.tempfloatVec(new float[] { 1, 2, 3, 4, 5, 6, 7 });
                Assert.IsTrue(arena.TempAllocationsCount == 1 && arena.DB_isTemp(v0)); 
                float4 f3 = v0.GetSubvecAsFloat4(2);
                Assert.IsTrue(f3.x == 3 && f3.y == 4 && f3.z == 5 && f3.w == 6);
            }

            int ac = arena.AllocationsCount;
            {
                floatN v0 = arena.tempfloatVec(new float[] { 1, 2, 3, 4, 5, 6, 7 });
                Assert.IsTrue(arena.TempAllocationsCount == 2 && arena.DB_isTemp(v0));
                
                floatN f3 = v0.GetSubvec(2,3, false);
                ac += 1;
                Assert.IsTrue(arena.AllocationsCount == ac && arena.DB_isPersistant(f3));
                Assert.IsTrue(f3[0] == 3 && f3[1] == 4 && f3[2] == 5);
                floatN f4 = v0.GetSubvec(2, 3, true);
                Assert.IsTrue(arena.TempAllocationsCount == 3 && arena.DB_isTemp(f4));
                Assert.IsTrue(f3[0] == 3 && f3[1] == 4 && f3[2] == 5);
            }
            */
            /*
            {
                float4x4 ff = new f4x4(new float4(1, 2, 3, 4), new float4(5, 6, 7, 8), new float4(9, 10, 11, 12), new float4(13, 14, 15, 16));

                floatMxN mm0 = arena.Convert(in ff, false);
                ac += 1;
                Assert.IsTrue(arena.AllocationsCount == ac && arena.DB_isPersistant(mm0));
                floatN c = mm0.Col(2);
                Assert.IsTrue(arena.TempAllocationsCount == 4 && arena.DB_isTemp(c));
                Assert.IsTrue(c[0] == mm0[0, 2] && c[1] == mm0[1, 2] && c[2] == mm0[2, 2] && c[3] == mm0[3, 2]);

                float3 c3 = mm0.GetColAsFloat3(2);
                Assert.IsTrue(c3[0] == mm0[0, 2] && c3[1] == mm0[1, 2] && c3[2] == mm0[2, 2]);

                float4 c4 = mm0.GetColAsFloat4(2);
                Assert.IsTrue(c4[0] == mm0[0, 2] && c4[1] == mm0[1, 2] && c4[2] == mm0[2, 2] && c4[3] == mm0[3,2]);

                float3 c5 = new float3(66, 55, 44);
                mm0.SetCol(c5, 1, 1);
                Assert.IsTrue(66 == mm0[1, 1] && 55 == mm0[2, 1] && 44 == mm0[3, 1]);

                float4 c6 = new float4(66, 55, 44, 33);
                mm0.SetCol(c6, 2);
                Assert.IsTrue(66 == mm0[0, 2] && 55 == mm0[1, 2] && 44 == mm0[2, 2] && 33 == mm0[3,2]);

                c[0] = 111; c[1] = 222; c[2] = 333; c[3] = 444;
                mm0.SetCol(c, 3);
                Assert.IsTrue(111 == mm0[0, 3] && 222 == mm0[1, 3] && 333 == mm0[2, 3] && 444 == mm0[3, 3]);
            }

            {
                floatMxN mm0 = arena.floatMat(new float[,] { {1,2,3,4 },
                                                              {5,6,7,8 },
                                                              {9,10,11,12 },
                                                              {13,14,15,16 }});
                ac += 1;
                float3x3 m = mm0.GetSubMatrixFloat3x3(1, 1);
                Assert.IsTrue(arena.TempAllocationsCount == 4);
                floatMxN mm = arena.floatMat(m);
                ac += 1;

                var m22 = mm0.GetSubMatrix(2, 2, 1, 2, true);
                Assert.IsTrue(arena.TempAllocationsCount == 5 && arena.DB_isTemp(m22));
                Assert.IsTrue(m22.M_Rows == 2 && m22.N_Cols == 2);

                Assert.IsTrue(m22[0, 0] == 10 && m22[0, 1] == 11 && m22[1, 0] == 14 && m22[1, 1] == 15);

                mm0.SetSubMatrix(m22, 0, 0);
                Assert.IsTrue(mm0[0, 0] == 10 && mm0[0, 1] == 11 && mm0[1, 0] == 14 && mm0[1, 1] == 15);

                float3x3 f33;
                f33.c0 = new float3(1, 2, 3);
                f33.c1 = new float3(4, 5, 6);
                f33.c2 = new float3(7, 8, 9);
                mm0.SetSubMatrix(f33, 1, 1);
                

            }
            */

            arena.Dispose();
            Assert.AreEqual(0, arena.AllocationsCount);
            Assert.AreEqual(0, arena.TempAllocationsCount);
        }
    }

    [Test]
    public void InitMatrixVecPass()
    {
        new InitMatrixTestJob().Run();
    }
    
}
