// <copyright file="EvdTests.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2016 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using LinearAlgebra;
using LinearAlgebra.MathNet.Numerics;
using System.Collections.Generic;
using NUnit.Framework;

namespace LinearAlgebra.MathNet.Numerics.Tests.LinearAlgebraTests.Double.Factorization
{
    using Complex = System.Numerics.Complex;

    
    /// <summary>
    /// Eigenvalues factorization tests for a dense matrix.
    /// </summary>
    [TestFixture, Category("LAFactorization")]
    public class EvdTestsdouble
    {
        internal static class AssertHelpers
        {
            public static void AlmostEqual(Complex expected, Complex actual)
            {
                if (expected.IsNaN() && actual.IsNaN() || expected.IsInfinity() && expected.IsInfinity())
                {
                    return;
                }

                if (!ComplexExtensions.AlmostEqual(expected.Real, actual.Real))
                {
                    Assert.Fail("Real components are not equal. Expected:{0}; Actual:{1}", expected.Real, actual.Real);
                }

                if (!ComplexExtensions.AlmostEqual(expected.Imaginary, actual.Imaginary))
                {
                    Assert.Fail("Imaginary components are not equal. Expected:{0}; Actual:{1}", expected.Imaginary, actual.Imaginary);
                }
            }

            /*
            public static void AlmostEqual(Complex32 expected, Complex32 actual)
            {
                if (expected.IsNaN() && actual.IsNaN() || expected.IsInfinity() && expected.IsInfinity())
                {
                    return;
                }

                if (!expected.Real.AlmostEqual(actual.Real))
                {
                    Assert.Fail("Real components are not equal. Expected:{0}; Actual:{1}", expected.Real, actual.Real);
                }

                if (!expected.Imaginary.AlmostEqual(actual.Imaginary))
                {
                    Assert.Fail("Imaginary components are not equal. Expected:{0}; Actual:{1}", expected.Imaginary, actual.Imaginary);
                }
            }
           */

            public static void AlmostEqual(double expected, double actual, int decimalPlaces)
            {
                if (double.IsNaN(expected) && double.IsNaN(actual))
                {
                    return;
                }

                if (!Precisiondouble.AlmostEqual( expected, actual, decimalPlaces))
                {
                    Assert.Fail("Not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected, actual);
                }
            }

            /*
            public static void AlmostEqual(float expected, float actual, int decimalPlaces)
            {
                if (float.IsNaN(expected) && float.IsNaN(actual))
                {
                    return;
                }

                if (!Precisiondouble.AlmostEqual(expected, actual, decimalPlaces))
                {
                    Assert.Fail("Not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected, actual);
                }
            }
            */

            public static void AlmostEqual(Complex expected, Complex actual, int decimalPlaces)
            {
                if (!ComplexExtensions.AlmostEqual(expected.Real , actual.Real, decimalPlaces))
                {
                    Assert.Fail("Real components are not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected.Real, actual.Real);
                }

                if (!ComplexExtensions.AlmostEqual(expected.Imaginary , actual.Imaginary, decimalPlaces))
                {
                    Assert.Fail("Imaginary components are not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected.Imaginary, actual.Imaginary);
                }
            }
           /*
            public static void AlmostEqual(Complex32 expected, Complex32 actual, int decimalPlaces)
            {
                if (!expected.Real.AlmostEqual(actual.Real, decimalPlaces))
                {
                    Assert.Fail("Real components are not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected.Real, actual.Real);
                }

                if (!expected.Imaginary.AlmostEqual(actual.Imaginary, decimalPlaces))
                {
                    Assert.Fail("Imaginary components are not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected.Imaginary, actual.Imaginary);
                }
            }
           */

            public static void AlmostEqualRelative(double expected, double actual, int decimalPlaces)
            {
                if (double.IsNaN(expected) && double.IsNaN(actual))
                {
                    return;
                }

                if (!Precisiondouble.AlmostEqualRelative(expected, actual, decimalPlaces))
                {
                    Assert.Fail("Not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected, actual);
                }
            }

            /*
            public static void AlmostEqualRelative(float expected, float actual, int decimalPlaces)
            {
                if (float.IsNaN(expected) && float.IsNaN(actual))
                {
                    return;
                }

                if (!Precisiondouble.AlmostEqualRelative(expected, actual, decimalPlaces))
                {
                    Assert.Fail("Not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected, actual);
                }
            }
            */

            public static void AlmostEqualRelative(Complex expected, Complex actual, int decimalPlaces)
            {
                if (!ComplexExtensions.AlmostEqualRelative(expected.Real , actual.Real, decimalPlaces))
                {
                    Assert.Fail("Real components are not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected.Real, actual.Real);
                }

                if (!ComplexExtensions.AlmostEqualRelative(expected.Imaginary , actual.Imaginary, decimalPlaces))
                {
                    Assert.Fail("Imaginary components are not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected.Imaginary, actual.Imaginary);
                }
            }

            /*
            public static void AlmostEqualRelative(Complex32 expected, Complex32 actual, int decimalPlaces)
            {
                if (!expected.Real.AlmostEqualRelative(actual.Real, decimalPlaces))
                {
                    Assert.Fail("Real components are not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected.Real, actual.Real);
                }

                if (!expected.Imaginary.AlmostEqualRelative(actual.Imaginary, decimalPlaces))
                {
                    Assert.Fail("Imaginary components are not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected.Imaginary, actual.Imaginary);
                }
            }
           */
           /*
            public static void AlmostEqual(IList<double> expected, IList<double> actual, int decimalPlaces)
            {
                for (var i = 0; i < expected.Count; i++)
                {
                    if (!actual[i].AlmostEqual(expected[i], decimalPlaces))
                    {
                        Assert.Fail("Not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected[i], actual[i]);
                    }
                }
            }

            public static void AlmostEqual(IList<float> expected, IList<float> actual, int decimalPlaces)
            {
                for (var i = 0; i < expected.Count; i++)
                {
                    if (!actual[i].AlmostEqual(expected[i], decimalPlaces))
                    {
                        Assert.Fail("Not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected[i], actual[i]);
                    }
                }
            }

            public static void AlmostEqual(IList<Complex> expected, IList<Complex> actual, int decimalPlaces)
            {
                for (var i = 0; i < expected.Count; i++)
                {
                    if (!actual[i].AlmostEqual(expected[i], decimalPlaces))
                    {
                        Assert.Fail("Not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected[i], actual[i]);
                    }
                }
            }

            public static void AlmostEqual(IList<Complex32> expected, IList<Complex32> actual, int decimalPlaces)
            {
                for (var i = 0; i < expected.Count; i++)
                {
                    if (!actual[i].AlmostEqual(expected[i], decimalPlaces))
                    {
                        Assert.Fail("Not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected[i], actual[i]);
                    }
                }
            }

            public static void AlmostEqualRelative(IList<double> expected, IList<double> actual, int decimalPlaces)
            {
                for (var i = 0; i < expected.Count; i++)
                {
                    if (!actual[i].AlmostEqualRelative(expected[i], decimalPlaces))
                    {
                        Assert.Fail("Not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected[i], actual[i]);
                    }
                }
            }

            public static void AlmostEqualRelative(IList<float> expected, IList<float> actual, int decimalPlaces)
            {
                for (var i = 0; i < expected.Count; i++)
                {
                    if (!actual[i].AlmostEqualRelative(expected[i], decimalPlaces))
                    {
                        Assert.Fail("Not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected[i], actual[i]);
                    }
                }
            }

            public static void AlmostEqualRelative(IList<Complex> expected, IList<Complex> actual, int decimalPlaces)
            {
                for (var i = 0; i < expected.Count; i++)
                {
                    if (!actual[i].AlmostEqualRelative(expected[i], decimalPlaces))
                    {
                        Assert.Fail("Not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected[i], actual[i]);
                    }
                }
            }

            public static void AlmostEqualRelative(IList<Complex32> expected, IList<Complex32> actual, int decimalPlaces)
            {
                for (var i = 0; i < expected.Count; i++)
                {
                    if (!actual[i].AlmostEqualRelative(expected[i], decimalPlaces))
                    {
                        Assert.Fail("Not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected[i], actual[i]);
                    }
                }
            }
            */

            public static void AlmostEqual(doubleMxN expected, doubleMxN actual, int decimalPlaces)
            {
                if (expected.ColumnCount != actual.ColumnCount || expected.RowCount != actual.RowCount)
                {
                    Assert.Fail("Matrix dimensions mismatch. Expected: {0}; Actual: {1}", expected, actual);
                }

                for (var i = 0; i < expected.RowCount; i++)
                {
                    for (var j = 0; j < expected.ColumnCount; j++)
                    {
                        if (!actual[i, j].AlmostEqual(expected[i, j], decimalPlaces))
                        {
                            Assert.Fail("Not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected[i, j], actual[i, j]);
                        }
                    }
                }
            }

            /*
            public static void AlmostEqual(floatMxN expected, floatMxN actual, int decimalPlaces)
            {
                if (expected.ColumnCount != actual.ColumnCount || expected.RowCount != actual.RowCount)
                {
                    Assert.Fail("Matrix dimensions mismatch. Expected: {0}; Actual: {1}", expected, actual);
                }

                for (var i = 0; i < expected.RowCount; i++)
                {
                    for (var j = 0; j < expected.ColumnCount; j++)
                    {
                        if (!actual[i, j).AlmostEqual(expected[i, j), decimalPlaces))
                        {
                            Assert.Fail("Not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected[i, j), actual[i, j));
                        }
                    }
                }
            }
            */

            public static void AlmostEqualRelative(doubleMxN expected, doubleMxN actual, int decimalPlaces)
            {
                if (expected.ColumnCount != actual.ColumnCount || expected.RowCount != actual.RowCount)
                {
                    Assert.Fail("Matrix dimensions mismatch. Expected: {0}; Actual: {1}", expected, actual);
                }

                for (var i = 0; i < expected.RowCount; i++)
                {
                    for (var j = 0; j < expected.ColumnCount; j++)
                    {
                        if (!actual[i, j].AlmostEqualRelative(expected[i, j], decimalPlaces))
                        {
                            Assert.Fail("Not equal within {0} relative places. Expected:{1}; Actual:{2}", decimalPlaces, expected[i, j], actual[i, j]);
                        }
                    }
                }
            }

            /*
            public static void AlmostEqualRelative(floatMxN expected, floatMxN actual, int decimalPlaces)
            {
                if (expected.ColumnCount != actual.ColumnCount || expected.RowCount != actual.RowCount)
                {
                    Assert.Fail("Matrix dimensions mismatch. Expected: {0}; Actual: {1}", expected, actual);
                }

                for (var i = 0; i < expected.RowCount; i++)
                {
                    for (var j = 0; j < expected.ColumnCount; j++)
                    {
                        if (!actual[i, j).AlmostEqualRelative(expected[i, j), decimalPlaces))
                        {
                            Assert.Fail("Not equal within {0} relative places. Expected:{1}; Actual:{2}", decimalPlaces, expected[i, j), actual[i, j));
                        }
                    }
                }
            }
            */

        }

        List<Complex> GetComplexEigenValues(Evddouble evd)
        {
            System.Collections.Generic.List<Complex> l = new System.Collections.Generic.List<Complex>();
            for (int i = 0; i < evd.EigenValuesReal.Count; i++)
            {
                l.Add(new Complex(evd.EigenValuesReal[i], evd.EigenValuesImaginary[i]));
            }
            return l;
        }

        [Test]
        public void CanFactorizeIdentityMatrix([Values(1, 10, 100)] int order)
        {
            Arena arena = new Arena(Unity.Collections.Allocator.Persistent);
            var matrix = arena.doubleIdentityMatrix(order, true); // doubleMxN.Build.DenseIdentity(order);
            var factorEvd = NumericsOPdouble.Evd(ref arena, matrix, Symmetricity.Unknown);
            var eigenValues = GetComplexEigenValues(factorEvd);
            var eigenVectors = factorEvd.EigenVectors;
            var d = factorEvd.D;

            Assert.AreEqual(matrix.RowCount, eigenVectors.RowCount);
            Assert.AreEqual(matrix.RowCount, eigenVectors.ColumnCount);
            Assert.AreEqual(matrix.ColumnCount, d.RowCount);
            Assert.AreEqual(matrix.ColumnCount, d.ColumnCount);

            for (var i = 0; i < eigenValues.Count; i++)
            {
                Assert.AreEqual(Complex.One, eigenValues[i]);
            }
            arena.Dispose();
        }


        [Test]
        public void CanFactorizeRandomSquareMatrix([Values(1, 2, 5, 10, 50, 100)] int order)
        {
            Arena arena = new Arena(Unity.Collections.Allocator.Persistent);
            var A = arena.doubleRandomMatrix(order, order, 1, true);
            var factorEvd = NumericsOPdouble.Evd(ref arena, A, Symmetricity.Unknown);
            var V = factorEvd.EigenVectors;
            var λ = factorEvd.D;

            UnityEngine.Debug.Log("Rand matrix: " + A);

            Assert.AreEqual(order, V.RowCount);
            Assert.AreEqual(order, V.ColumnCount);
            Assert.AreEqual(order, λ.RowCount);
            Assert.AreEqual(order, λ.ColumnCount);

            // Verify A*V = λ*V
            var Av = doubleOP.dot(A, V);
            var Lv = doubleOP.dot(V, λ);

            UnityEngine.Debug.Log($"AV {Av}");
            UnityEngine.Debug.Log($"LV {Lv}");
            AssertHelpers.AlmostEqual(Av, Lv, 10);
            AssertHelpers.AlmostEqualRelative(Av, Lv, 7);
            arena.Dispose();
        }

        /*
        [Test]
        public void CanFactorizeRandomSymmetricMatrix([Values(1, 2, 5, 10, 50, 100)] int order)
        {
            var A = doubleMxN.Build.RandomPositiveDefinite(order, 1);
            MatrixHelpers.ForceSymmetric(A);
            var factorEvd = A.Evd();
            var V = factorEvd.EigenVectors;
            var λ = factorEvd.D;

            Assert.AreEqual(order, V.RowCount);
            Assert.AreEqual(order, V.ColumnCount);
            Assert.AreEqual(order, λ.RowCount);
            Assert.AreEqual(order, λ.ColumnCount);

            // Verify A = V*λ*VT
            var matrix = V * λ * V.Transpose();
            AssertHelpers.AlmostEqual(matrix, A, 10);
            AssertHelpers.AlmostEqualRelative(matrix, A, 9);
        }

        [Test]
        public void CanCheckRankSquare([Values(10, 50, 100)] int order)
        {
            var A = doubleMxN.Build.Random(order, order, 1);
            Assert.AreEqual(A.Evd().Rank, order);
        }

        [Test]
        public void CanCheckRankOfSquareSingular([Values(10, 50, 100)] int order)
        {
            var A = doubleMxN.Build.Dense(order, order);
            A[0, 0] = 1;
            A[order - 1, order - 1] = 1;
            for (var i = 1; i < order - 1; i++)
            {
                A[i, i - 1] = 1;
                A[i, i + 1] = 1;
                A[i - 1, i] = 1;
                A[i + 1, i] = 1;
            }

            var factorEvd = A.Evd();

            Assert.AreEqual(factorEvd.Determinant, 0);
            Assert.AreEqual(factorEvd.Rank, order - 1);
        }

        [Test]
        public void IdentityDeterminantIsOne([Values(1, 10, 100)] int order)
        {
            var matrixI = doubleMxN.Build.DenseIdentity(order);
            var factorEvd = matrixI.Evd();
            Assert.AreEqual(1.0, factorEvd.Determinant);
        }

        /// <summary>
        /// Can solve a system of linear equations for a random vector and symmetric matrix (Ax=b).
        /// </summary>
        /// <param name="order">Matrix order.</param>
        [Test]
        public void CanSolveForRandomVectorAndSymmetricMatrix([Values(1, 2, 5, 10, 50, 100)] int order)
        {
            var A = doubleMxN.Build.RandomPositiveDefinite(order, 1);
            MatrixHelpers.ForceSymmetric(A);
            var ACopy = A.Clone();
            var evd = A.Evd();

            var b = Vector<double>.Build.Random(order, 2);
            var bCopy = b.Clone();

            var x = evd.Solve(b);

            var bReconstruct = A * x;

            // Check the reconstruction.
            AssertHelpers.AlmostEqual(b, bReconstruct, 8);

            // Make sure A/B didn't change.
            AssertHelpers.AlmostEqual(ACopy, A, 14);
            AssertHelpers.AlmostEqual(bCopy, b, 14);
        }

        //private
        /// <summary>
        /// Can solve a system of linear equations for a random matrix and symmetric matrix (AX=B).
        /// </summary>
        /// <param name="order">Matrix order.</param>
        [Test]
        public void CanSolveForRandomMatrixAndSymmetricMatrix([Values(1, 2, 5, 10, 50, 100)] int order)
        {
            var A = doubleMxN.Build.RandomPositiveDefinite(order, 1);
            MatrixHelpers.ForceSymmetric(A);
            var ACopy = A.Clone();
            var evd = A.Evd();

            var B = doubleMxN.Build.Random(order, order, 2);
            var BCopy = B.Clone();

            var X = evd.Solve(B);

            // The solution X row dimension is equal to the column dimension of A
            Assert.AreEqual(A.ColumnCount, X.RowCount);

            // The solution X has the same number of columns as B
            Assert.AreEqual(B.ColumnCount, X.ColumnCount);

            var BReconstruct = A * X;

            // Check the reconstruction.
            AssertHelpers.AlmostEqual(B, BReconstruct, 8);

            // Make sure A/B didn't change.
            AssertHelpers.AlmostEqual(ACopy, A, 14);
            AssertHelpers.AlmostEqual(BCopy, B, 14);
        }

        /// <summary>
        /// Can solve a system of linear equations for a random vector and symmetric matrix (Ax=b) into a result vector.
        /// </summary>
        /// <param name="order">Matrix order.</param>
        [Test]
        public void CanSolveForRandomVectorAndSymmetricMatrixWhenResultVectorGiven([Values(1, 2, 5, 10, 50, 100)] int order)
        {
            var A = doubleMxN.Build.RandomPositiveDefinite(order, 1);
            MatrixHelpers.ForceSymmetric(A);
            var ACopy = A.Clone();
            var evd = A.Evd();

            var b = Vector<double>.Build.Random(order, 2);
            var bCopy = b.Clone();

            var x = new DenseVector(order);
            evd.Solve(b, x);

            var bReconstruct = A * x;

            // Check the reconstruction.
            AssertHelpers.AlmostEqual(b, bReconstruct, 8);

            // Make sure A/B didn't change.
            AssertHelpers.AlmostEqual(ACopy, A, 14);
            AssertHelpers.AlmostEqual(bCopy, b, 14);
        }

        /// <summary>
        /// Can solve a system of linear equations for a random matrix and symmetric matrix (AX=B) into result matrix.
        /// </summary>
        /// <param name="order">Matrix order.</param>
        [Test]
        public void CanSolveForRandomMatrixAndSymmetricMatrixWhenResultMatrixGiven([Values(1, 2, 5, 10, 50, 100)] int order)
        {
            var A = doubleMxN.Build.RandomPositiveDefinite(order, 1);
            MatrixHelpers.ForceSymmetric(A);
            var ACopy = A.Clone();
            var evd = A.Evd();

            var B = doubleMxN.Build.Random(order, order, 2);
            var BCopy = B.Clone();

            var X = doubleMxN.Build.Dense(order, order);
            evd.Solve(B, X);

            // The solution X row dimension is equal to the column dimension of A
            Assert.AreEqual(A.ColumnCount, X.RowCount);

            // The solution X has the same number of columns as B
            Assert.AreEqual(B.ColumnCount, X.ColumnCount);

            var BReconstruct = A * X;

            // Check the reconstruction.
            AssertHelpers.AlmostEqual(B, BReconstruct, 8);

            // Make sure A/B didn't change.
            AssertHelpers.AlmostEqual(ACopy, A, 14);
            AssertHelpers.AlmostEqual(BCopy, B, 14);
        }
        */
    }
}
