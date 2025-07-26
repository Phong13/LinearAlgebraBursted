using LinearAlgebra;
using NUnit.Framework;
using Unity.Burst;
using Unity.Collections;

using Unity.Jobs;
using Unity.Mathematics;

public class fProxyInitTest
{
    [BurstCompile]
    public struct InitVecTestJob : IJob
    {
        public void Execute()
        {
            var arena = new Arena(Allocator.Persistent);

            Assert.AreEqual(0, arena.AllocationsCount);

            int vecLen = 7;

            fProxyN vec = arena.fProxyVec(vecLen);

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

            fProxyMxN vec = arena.fProxyMat(lenRows, lenColumns);

            Assert.AreEqual(lenRows * lenColumns, vec.Length);
            Assert.AreEqual(1, arena.AllocationsCount);

            {
                float3 v0 = new float3(1, 2, 3);
                fProxyN vv0 = arena.fProxyVec(v0);
                Assert.IsTrue(v0[0] == vv0[0] && v0[1] == vv0[1] && v0[2] == vv0[2]);
                Assert.IsTrue(arena.DB_isPersistant(vv0));
            }
            {
                float3 v0 = new float3(1, 2, 3);
                fProxyN vvv0 = arena.tempfProxyVec(v0);
                Assert.IsTrue(v0[0] == vvv0[0] && v0[1] == vvv0[1] && v0[2] == vvv0[2]);
                Assert.IsTrue(arena.DB_isTemp(vvv0));
            }

            {
                float4 v0 = new float4(1, 2, 3, 4);
                fProxyN vv0 = arena.fProxyVec(v0);
                Assert.IsTrue(v0[0] == vv0[0] && v0[1] == vv0[1] && v0[2] == vv0[2] && v0[3] == vv0[3]);
                Assert.IsTrue(arena.DB_isPersistant(vv0));

                fProxyN vvv0 = arena.tempfProxyVec(v0);
                Assert.IsTrue(v0[0] == vvv0[0] && v0[1] == vvv0[1] && v0[2] == vvv0[2] && v0[3] == vv0[3]);
                Assert.IsTrue(arena.DB_isTemp(vvv0));
            }

            {
                float3x3 m0 = float3x3.EulerXYZ(math.radians(15), math.radians(25), math.radians(35));
                fProxyMxN mm0 = arena.fProxyMat(m0);
                Assert.IsTrue(m0.c0.x == mm0[0, 0] && m0.c1.x == mm0[0, 1] && m0.c2.x == mm0[0, 2]);
                Assert.IsTrue(m0.c0.y == mm0[1, 0] && m0.c1.y == mm0[1, 1] && m0.c2.y == mm0[1, 2]);
                Assert.IsTrue(m0.c0.z == mm0[2, 0] && m0.c1.z == mm0[2, 1] && m0.c2.z == mm0[2, 2]);
                Assert.IsTrue(arena.DB_isPersistant(mm0));
            }

            {
                float4x4 m0 = float4x4.EulerXYZ(math.radians(15), math.radians(25), math.radians(35));
                m0.c3 = new float4(5, 6, 7, 8);
                m0.c0.w = 3; m0.c1.w = 4; m0.c2.w = 5; m0.c3.w = 9;
                fProxyMxN mm0 = arena.fProxyMat(m0);

                UnityEngine.Debug.Log($"m0 \n {m0}");
                UnityEngine.Debug.Log($"mm0 \n {mm0}");

                Assert.IsTrue(m0.c0.x == mm0[0, 0] && m0.c1.x == mm0[0, 1] && m0.c2.x == mm0[0, 2] && m0.c3.x == mm0[0, 3]);
                Assert.IsTrue(m0.c0.y == mm0[1, 0] && m0.c1.y == mm0[1, 1] && m0.c2.y == mm0[1, 2] && m0.c3.y == mm0[1, 3]);
                Assert.IsTrue(m0.c0.z == mm0[2, 0] && m0.c1.z == mm0[2, 1] && m0.c2.z == mm0[2, 2] && m0.c3.z == mm0[2, 3]);
                Assert.IsTrue(m0.c0.w == mm0[3, 0] && m0.c1.w == mm0[3, 1] && m0.c2.w == mm0[3, 2] && m0.c3.w == mm0[3, 3]);
                Assert.IsTrue(arena.DB_isPersistant(mm0));
            }

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
