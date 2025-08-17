using LinearAlgebra;
using NUnit.Framework;
using System;
using System.Diagnostics;

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

public class doublePivotTests
{
    [BurstCompile]
    public struct TestsJob : IJob
    {
        public enum TestType
        {
            PivotSimpleTest,
            RowPivotIdentityMatTest,
            ColumnPivotIdentityMatTest,
            RowPivotLargeIdentityMatTest,
            ColumnPivotLargeIdentityMatTest,
            RowPivotPermutationMatTest,
            ColumnPivotPermutationMatTest,
            RowPivotVecTest,
        }

        public TestType Type;

        public void Execute()
        {
            Arena arena = new Arena(Allocator.Temp);
            try 
            {
                switch (Type) 
                {
                    case TestType.PivotSimpleTest:
                        Test(ref arena);
                        break;
                    case TestType.RowPivotIdentityMatTest:
                        RowIdentityMatTest(ref arena);
                        break;
                    case TestType.ColumnPivotIdentityMatTest:
                        ColumnIdentityMatTest(ref arena);
                        break; 
                    case TestType.ColumnPivotLargeIdentityMatTest:
                        ColumnLargeIdentityMatTest(ref arena);
                        break;
                    case TestType.RowPivotLargeIdentityMatTest:
                        RowLargeIdentityMatTest(ref arena);
                        break;
                    case TestType.RowPivotPermutationMatTest:
                        RowPermutationMatTest(ref arena);
                        break;
                    case TestType.ColumnPivotPermutationMatTest:
                        ColumnPermutationMatTest(ref arena);
                        break;
                    case TestType.RowPivotVecTest:
                        PivotVecTest(ref arena);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            finally
            {
                arena.Dispose();
            }
        }

        void Test(ref Arena arena)
        {
            Pivot pivot = new Pivot(4, Allocator.Temp);

            Assert.AreEqual(0, pivot[0]);
            Assert.AreEqual(1, pivot[1]);

            pivot.Swap(0, 1);

            Assert.AreEqual(1, pivot[0]);
            Assert.AreEqual(0, pivot[1]);

            pivot.Swap(0, 1);

            Assert.AreEqual(0, pivot[0]);
            Assert.AreEqual(1, pivot[1]);

            pivot.Dispose();
        }

        void RowIdentityMatTest(ref Arena arena) {

            Pivot pivot = new Pivot(4, Allocator.Temp);

            pivot.Swap(0, 1);
            pivot.Swap(2, 3);

            var identity = arena.doubleIdentityMatrix(4, true);

            pivot.ApplyRow(ref identity);

            Assert.IsFalse(Analysis.IsIdentity(identity));

            pivot.ApplyInverseRow(ref identity);

            Assert.IsTrue(Analysis.IsIdentity(identity));

            pivot.Reset();

            pivot.ApplyRow(ref identity);

            Assert.IsTrue(Analysis.IsIdentity(identity));

            pivot.Dispose();
        }

        void RowLargeIdentityMatTest(ref Arena arena) {

            int dim = 256;

            Pivot pivot = new Pivot(dim, Allocator.Temp);

            Unity.Mathematics.Random rand = new Unity.Mathematics.Random(1232);

            for (int i = 0; i < dim; i++) {
                pivot.Swap(rand.NextInt(0, dim), rand.NextInt(0, dim));
            }

            var identity = arena.doubleIdentityMatrix(dim, true);

            Assert.IsTrue(Analysis.IsIdentity(identity));

            pivot.ApplyRow(ref identity);
            pivot.ApplyRow(ref identity);

            Assert.IsFalse(Analysis.IsIdentity(identity));

            pivot.ApplyInverseRow(ref identity);
            pivot.ApplyInverseRow(ref identity);

            Assert.IsTrue(Analysis.IsIdentity(identity));

            pivot.Reset();

            pivot.ApplyRow(ref identity);

            Assert.IsTrue(Analysis.IsIdentity(identity));

            pivot.Dispose();
        }

        void ColumnIdentityMatTest(ref Arena arena) {

            Pivot pivot = new Pivot(4, Allocator.Temp);

            pivot.Swap(0, 1);
            pivot.Swap(2, 3);

            var identity = arena.doubleIdentityMatrix(4, true);

            pivot.ApplyColumn(ref identity);

            Assert.IsFalse(Analysis.IsIdentity(identity));

            pivot.ApplyInverseColumn(ref identity);

            Assert.IsTrue(Analysis.IsIdentity(identity));

            pivot.Reset();

            pivot.ApplyColumn(ref identity);

            Assert.IsTrue(Analysis.IsIdentity(identity));

            pivot.Dispose();
        }

        void ColumnLargeIdentityMatTest(ref Arena arena) {

            int dim = 256;

            Pivot pivot = new Pivot(dim, Allocator.Temp);

            Unity.Mathematics.Random rand = new Unity.Mathematics.Random(1232);

            for (int i = 0; i < dim; i++) {
                pivot.Swap(rand.NextInt(0, dim), rand.NextInt(0, dim));
            }

            var identity = arena.doubleIdentityMatrix(dim, true);

            Assert.IsTrue(Analysis.IsIdentity(identity));

            pivot.ApplyColumn(ref identity);
            pivot.ApplyColumn(ref identity);

            Assert.IsFalse(Analysis.IsIdentity(identity));

            pivot.ApplyInverseColumn(ref identity);
            pivot.ApplyInverseColumn(ref identity);

            Assert.IsTrue(Analysis.IsIdentity(identity));

            pivot.Reset();

            pivot.ApplyColumn(ref identity);

            Assert.IsTrue(Analysis.IsIdentity(identity));

            pivot.Dispose();
        }

        void RowPermutationMatTest(ref Arena arena) {

            var permutationMatrix = arena.doublePermutationMatrix(8, 2, 3, true);

            permutationMatrix = doubleOP.dot(permutationMatrix, arena.doublePermutationMatrix(8, 3, 6, true));
            permutationMatrix = doubleOP.dot(permutationMatrix, arena.doublePermutationMatrix(8, 6, 7, true));
            permutationMatrix = doubleOP.dot(permutationMatrix, arena.doublePermutationMatrix(8, 1, 4, true));

            Pivot pivot = new Pivot(8, Allocator.Temp);

            pivot.Swap(2, 3);
            pivot.Swap(3, 6);
            pivot.Swap(6, 7);
            pivot.Swap(1, 4);

            // applying inverse pivot operation to permutation matrix should form identity matrix
            pivot.ApplyInverseRow(ref permutationMatrix);

            Assert.IsTrue(Analysis.IsIdentity(permutationMatrix));

            pivot.Dispose();
        }

        void ColumnPermutationMatTest(ref Arena arena) {

            var permutationMatrix = arena.doublePermutationMatrix(8, 2, 3, true);

            permutationMatrix = doubleOP.dot(permutationMatrix, arena.doublePermutationMatrix(8, 3, 6, true));
            permutationMatrix = doubleOP.dot(permutationMatrix, arena.doublePermutationMatrix(8, 6, 7, true));
            permutationMatrix = doubleOP.dot(permutationMatrix, arena.doublePermutationMatrix(8, 1, 4, true));

            permutationMatrix = doubleOP.trans(permutationMatrix);

            Pivot pivot = new Pivot(8, Allocator.Temp);

            pivot.Swap(2, 3);
            pivot.Swap(3, 6);
            pivot.Swap(6, 7);
            pivot.Swap(1, 4);

            // applying inverse pivot operation to permutation matrix should form identity matrix
            pivot.ApplyInverseColumn(ref permutationMatrix);

            Assert.IsTrue(Analysis.IsIdentity(permutationMatrix));
              
            pivot.Dispose();
        }

        void PivotVecTest(ref Arena arena) {
            
            Pivot pivot = new Pivot(4, Allocator.Temp);

            pivot.Swap(1, 2);

            // [1, 0, 0, 0]
            var vec = arena.doubleBasisVector(4, 0, true);

            Print.Log(vec);

            var vecCopy = vec.CopyPersistent();

            Assert.IsTrue(BoolAnalysis.IsAllEqualTo(vec == vecCopy, true));

            // [1, 0, 0, 0] -> [0, 0, 0, 1]
            pivot.ApplyVec(ref vec);

            Assert.IsTrue(BoolAnalysis.IsAllEqualTo(vec == vecCopy, true));

            pivot.Swap(0, 3);

            pivot.ApplyVec(ref vec);

            Assert.AreEqual((double)0f, vec[0]);
            Assert.AreEqual((double)0f, vec[1]);
            Assert.AreEqual((double)0f, vec[2]);
            Assert.AreEqual((double)1f, vec[3]);

            pivot.ApplyInverseVec(ref vec);

            Assert.IsTrue(BoolAnalysis.IsAllEqualTo(vec == vecCopy, true));

            pivot.Dispose();
        }
    }

    public static Array GetEnums()
    {
        return Enum.GetValues(typeof(TestsJob.TestType));
    }

    [TestCaseSource("GetEnums")]
    public void Tests(TestsJob.TestType testType)
    {
        new TestsJob() { Type = testType }.Run();
    }
}
