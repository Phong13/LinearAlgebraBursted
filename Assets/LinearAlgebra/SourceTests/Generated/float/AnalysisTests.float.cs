using LinearAlgebra;
using NUnit.Framework;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
public class floatAnalysisTests
{
    [BurstCompile]
    public struct AnalysisTestJob : IJob
    {
        public enum TestType
        {
            IsIdentity,
            IsIdentityEpsilon,
            IsSymmetric,
            IsSymmetricEpsilon,
            IsDiagonal,
            IsDiagonalEpsilon,
            IsUpperTriangular,
            IsUpperTriangularEpsilon,
            IsLowerTriangular,
            IsLowerTriangularEpsilon,
            IsOrthogonal,
        }

        public TestType Type;

        public void Execute()
        {
            switch(Type)
            {
                case TestType.IsIdentity:
                    IsIdentity();
                    break;
                case TestType.IsIdentityEpsilon:
                    IsIdentityEpsilon();
                    break;
                case TestType.IsSymmetric:
                    IsSymmetric();
                    break;
                case TestType.IsSymmetricEpsilon:
                    IsSymmetricEpsilon();
                    break;
                case TestType.IsDiagonal:
                    IsDiagonal();
                    break;
                case TestType.IsDiagonalEpsilon:
                    IsDiagonalEpsilon();
                    break;
                case TestType.IsUpperTriangular:
                    IsUpperTriangular();
                    break;
                case TestType.IsUpperTriangularEpsilon:
                    IsUpperTriangularEpsilon();
                    break;
                case TestType.IsLowerTriangular:
                    IsLowerTriangular();
                    break;
                case TestType.IsLowerTriangularEpsilon:
                    IsLowerTriangularEpsilon();
                    break;
                case TestType.IsOrthogonal:
                    IsOrthogonal();
                    break;
            }
        }

        void IsIdentity()
        {
            var arena = new Arena(Allocator.Persistent);

            int dim = 16;
            
            floatMxN A = arena.floatIdentityMatrix(dim, true);

            Assert.IsTrue(Analysis.IsIdentity(A));

            arena.Dispose();
        }

        void IsIdentityEpsilon()
        {
            var arena = new Arena(Allocator.Persistent);

            int dim = 16;
            
            floatMxN A = arena.floatIdentityMatrix(dim, true);

            Assert.IsTrue(Analysis.IsIdentity(A, 0.0001f));

            A += arena.floatRandomMatrix(dim, dim, -0.001f, 0.001f);

            Assert.IsTrue(Analysis.IsIdentity(A, 0.002f));

            arena.Dispose();
        }

        void IsSymmetric()
        {
            var arena = new Arena(Allocator.Persistent);

            int dim = 8;
            
            floatMxN A = arena.floatIdentityMatrix(dim, true);

            Assert.IsTrue(Analysis.IsSymmetric(A));

            A = arena.floatRandomMatrix(dim, dim * 2);

            floatMxN C = floatOP.dot(A, A, true);

            Assert.IsTrue(Analysis.IsSymmetric(C));

            arena.Dispose();
        }

        void IsSymmetricEpsilon()
        {
            var arena = new Arena(Allocator.Persistent);

            int dim = 16;
            
            floatMxN A = arena.floatIdentityMatrix(dim, true);

            Assert.IsTrue(Analysis.IsSymmetric(A, 0.000001f));

            A += arena.floatRandomMatrix(dim, dim, -0.001f, 0.001f);

            Assert.IsTrue(Analysis.IsSymmetric(A, 0.002f));

            floatMxN C = floatOP.dot(A, A, true);

            C += arena.floatRandomMatrix(dim, dim, -0.001f, 0.001f);

            Assert.IsTrue(Analysis.IsSymmetric(C, 0.002f));

            arena.Dispose();
        }
        
        void IsDiagonal()
        {
            var arena = new Arena(Allocator.Persistent);

            int dim = 16;
            
            floatMxN A = arena.floatIdentityMatrix(dim, true);

            Assert.IsTrue(Analysis.IsDiagonal(A));

            A += arena.floatRandomMatrix(dim, dim, -0.001f, 0.001f);

            Assert.IsFalse(Analysis.IsDiagonal(A));

            arena.Dispose();
        }

        void IsDiagonalEpsilon()
        {
            var arena = new Arena(Allocator.Persistent);

            int dim = 16;
            
            floatMxN A = arena.floatIdentityMatrix(dim, true);

            Assert.IsTrue(Analysis.IsDiagonal(A, 0.000001f));

            A += arena.floatRandomMatrix(dim, dim, -0.001f, 0.001f);

            Assert.IsTrue(Analysis.IsDiagonal(A, 0.002f));

            A = arena.floatRandomDiagonalMatrix(dim, -1f, -1f);

            Assert.IsTrue(Analysis.IsDiagonal(A, 0.000001f));

            arena.Dispose();
        }

        void IsUpperTriangular()
        {
            var arena = new Arena(Allocator.Persistent);

            int dim = 16;
            
            floatMxN A = arena.floatIdentityMatrix(dim, true);

            Assert.IsTrue(Analysis.IsUpperTriangular(A));
            
            A += arena.floatRandomMatrix(dim, dim, -0.001f, 0.001f);

            Assert.IsFalse(Analysis.IsUpperTriangular(A));

            A = arena.floatIdentityMatrix(dim, true);

            for (int c = 1; c < dim; c++)
            for (int r = 0; r < c; r++)
                A[r, c] = 5f;

            Assert.IsTrue(Analysis.IsUpperTriangular(A));

            arena.Dispose();
        }

        void IsUpperTriangularEpsilon()
        {
            var arena = new Arena(Allocator.Persistent);

            int dim = 16;
            
            floatMxN A = arena.floatIdentityMatrix(dim, true);

            Assert.IsTrue(Analysis.IsUpperTriangular(A, 0.000001f));

            A += arena.floatRandomMatrix(dim, dim, -0.001f, 0.001f);

            Assert.IsTrue(Analysis.IsUpperTriangular(A, 0.002f));

            A = arena.floatIdentityMatrix(dim, true);

            for(int c = 1; c < dim; c++)
            for(int r = 0; r < c; r++)
                A[r, c] = 5f;

            A += arena.floatRandomMatrix(dim, dim, -0.001f, 0.001f);

            Assert.IsTrue(Analysis.IsUpperTriangular(A, 0.002f));
                        
            arena.Dispose();   
        }

        void IsLowerTriangular()
        {
            var arena = new Arena(Allocator.Persistent);

            int dim = 16;

            floatMxN A = arena.floatIdentityMatrix(dim, true);

            Assert.IsTrue(Analysis.IsLowerTriangular(A));

            A += arena.floatRandomMatrix(dim, dim, -0.001f, 0.001f);
            Assert.IsFalse(Analysis.IsLowerTriangular(A));

            // Reset A to the identity matrix
            A = arena.floatIdentityMatrix(dim, true);

            // Fill elements above the diagonal with a non-zero value and check if it's still lower triangular (it shouldn't be)
            for (int r = 1; r < dim; r++)
                for (int c = 0; c < r; c++)
                    A[r, c] = 5f;

            // The matrix is now lower triangular
            Assert.IsTrue(Analysis.IsLowerTriangular(A));

            arena.Dispose();
        }

        void IsLowerTriangularEpsilon()
        {
            var arena = new Arena(Allocator.Persistent);

            int dim = 16;

            floatMxN A = arena.floatIdentityMatrix(dim, true);

            // Test if an identity matrix is lower triangular within the epsilon tolerance
            Assert.IsTrue(Analysis.IsLowerTriangular(A, 0.000001f));

            // Add small random values and test if it's still lower triangular within a higher tolerance
            A += arena.floatRandomMatrix(dim, dim, -0.001f, 0.001f);
            Assert.IsTrue(Analysis.IsLowerTriangular(A, 0.002f));

            // Reset A to the identity matrix
            A = arena.floatIdentityMatrix(dim, true);

            // Fill elements above the diagonal with a non-zero value
            for (int r = 1; r < dim; r++)
                for (int c = 0; c < r; c++)
                    A[r, c] = 5f;

            // Add small random values again
            A += arena.floatRandomMatrix(dim, dim, -0.001f, 0.001f);

            // Test if the modified matrix is still lower triangular within the higher epsilon tolerance
            Assert.IsTrue(Analysis.IsLowerTriangular(A, 0.002f));

            arena.Dispose();
        }

        void IsOrthogonal()
        {
            var arena = new Arena(Allocator.Persistent);

            int dim = 16;

            floatMxN A = arena.floatIdentityMatrix(dim, true);

            Assert.IsTrue(Analysis.IsOrthogonal(A, 0.00001f));

            A = floatOP.dot(arena.floatPermutationMatrix(dim, 5, 13, true), A);

            Assert.IsTrue(Analysis.IsOrthogonal(A, 0.00001f));

            A = floatOP.dot(arena.floatRotationMatrix(dim, 3, 15, math.PI/4f, true), A);

            Assert.IsTrue(Analysis.IsOrthogonal(A, 0.00001f));

            floatN reflect = arena.floatRandomVector(dim, -1f, 1f);

            A = floatOP.dot(arena.floatHouseholderMatrix(dim, reflect, true), A);

            Assert.IsTrue(Analysis.IsOrthogonal(A, 0.00001f));

            reflect = arena.floatRandomVector(dim, -1f, 1f, 50301);
            A = floatOP.dot(arena.floatHouseholderMatrix(dim, reflect, true), A);

            Assert.IsTrue(Analysis.IsOrthogonal(A, 0.00001f));

            // self multiply
            A = floatOP.dot(A, A);

            Assert.IsTrue(Analysis.IsOrthogonal(A, 0.00001f));

            // testing inverse
            A = floatOP.dot(A, A, true);

            Assert.IsTrue(Analysis.IsIdentity(A, 0.00001f));

            arena.Dispose();
        }

    }

    [Test]
    public void IsIdentityTest()
    {
        new AnalysisTestJob() { Type = AnalysisTestJob.TestType.IsIdentity }.Run();
    }

    [Test]
    public void IsIdentityEpsilonTest()
    {
        new AnalysisTestJob() { Type = AnalysisTestJob.TestType.IsIdentityEpsilon }.Run();
    }

    [Test]
    public void IsSymmetricTest()
    {
        new AnalysisTestJob() { Type = AnalysisTestJob.TestType.IsSymmetric }.Run();
    }

    [Test]
    public void IsSymmetricEpsilonTest()
    {
        new AnalysisTestJob() { Type = AnalysisTestJob.TestType.IsSymmetricEpsilon }.Run();
    }

    [Test]
    public void IsDiagonalTest()
    {
        new AnalysisTestJob() { Type = AnalysisTestJob.TestType.IsDiagonal }.Run();
    }

    [Test]
    public void IsDiagonalEpsilonTest()
    {
        new AnalysisTestJob() { Type = AnalysisTestJob.TestType.IsDiagonalEpsilon }.Run();
    }

    [Test]
    public void IsUpperTriangularTest()
    {
        new AnalysisTestJob() { Type = AnalysisTestJob.TestType.IsUpperTriangular }.Run();
    }

    [Test]
    public void IsUpperTriangularEpsilonTest()
    {
        new AnalysisTestJob() { Type = AnalysisTestJob.TestType.IsUpperTriangularEpsilon }.Run();
    }

    [Test]
    public void IsLowerTriangularTest()
    {
        new AnalysisTestJob() { Type = AnalysisTestJob.TestType.IsLowerTriangular }.Run();
    }

    [Test]
    public void IsLowerTriangularEpsilonTest()
    {
        new AnalysisTestJob() { Type = AnalysisTestJob.TestType.IsLowerTriangularEpsilon }.Run();
    }

    [Test]
    public void IsOrthogonalTest()
    {
        new AnalysisTestJob() { Type = AnalysisTestJob.TestType.IsOrthogonal }.Run();
    }


}
