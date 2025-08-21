// <copyright file="Svd.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2015 Math.NET
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

using Unity.Mathematics;

namespace LinearAlgebra.MathNet.Numerics
{
    /// <summary>
    /// <para>A class which encapsulates the functionality of the singular value decomposition (SVD).</para>
    /// <para>Suppose M is an m-by-n matrix whose entries are real numbers.
    /// Then there exists a factorization of the form M = UΣVT where:
    /// - U is an m-by-m unitary matrix;
    /// - Σ is m-by-n diagonal matrix with nonnegative real numbers on the diagonal;
    /// - VT denotes transpose of V, an n-by-n unitary matrix;
    /// Such a factorization is called a singular-value decomposition of M. A common convention is to order the diagonal
    /// entries Σ(i,i) in descending order. In this case, the diagonal matrix Σ is uniquely determined
    /// by M (though the matrices U and V are not). The diagonal entries of Σ are known as the singular values of M.</para>
    /// </summary>
    /// <remarks>
    /// The computation of the singular value decomposition is done at construction time.
    /// </remarks>
    /// <typeparam name="T">Supported data types are double, single, <see cref="Complex"/>, and <see cref="Complex32"/>.</typeparam>
    public class Svdfloat
    {
        public floatN S;
        public floatMxN U;
        public floatMxN VT;
        public floatMxN W;

        /// <summary>Indicating whether U and VT matrices have been computed during SVD factorization.</summary>
        protected readonly bool VectorsComputed;

        public Svdfloat(bool vecotrsComputed)
        {
            VectorsComputed = vecotrsComputed;
        }

        protected Svdfloat(floatN s, floatMxN u, floatMxN vt, floatMxN w, bool vectorsComputed)
        {
            S = s;
            U = u;
            VT = vt;
            W = w;
            VectorsComputed = vectorsComputed;
        }


        public void ComputeW(ref Arena arena, bool isTemp)
        {
            var rows = U.RowCount;
            var columns = VT.ColumnCount;
            floatMxN result;
            if (isTemp)
            {
                result = arena.tempfloatMat(rows, columns, 0f);
            }
            else
            {
                result = arena.floatMat(rows, columns, 0f);
            }

            for (var i = 0; i < S.N; i++)
            {
                result[i, i] = S[i];
            }

            W = result;
        }

        /*
        /// <summary>
        /// Gets the effective numerical matrix rank.
        /// </summary>
        /// <value>The number of non-negligible singular values.</value>
        public override int Rank
        {
            get
            {
                double tolerance = Precision.EpsilonOf(S.Maximum()) * Math.Max(U.RowCount, VT.RowCount);
                return S.Count(t => math.abs(t) > tolerance);
            }
        }
        */
        

        /// <summary>
        /// Gets the two norm of the <see cref="Matrix{T}"/>.
        /// </summary>
        /// <returns>The 2-norm of the <see cref="Matrix{T}"/>.</returns>
        public double L2Norm => math.abs(S[0]);

        /// <summary>
        /// Gets the condition number <b>max(S) / min(S)</b>
        /// </summary>
        /// <returns>The condition number.</returns>
        public double ConditionNumber
        {
            get
            {
                var tmp = math.min(U.RowCount, VT.ColumnCount) - 1;
                return math.abs(S[0]) / math.abs(S[tmp]);
            }
        }

        /// <summary>
        /// Gets the absolute value of the determinant of the square matrix for which the SVD was computed.
        /// </summary>
        public double Determinant
        {
            get
            {
                if (U.RowCount != VT.ColumnCount)
                {
                    throw new System.ArgumentException("Matrix must be square.");
                }

                var det = 1.0;
                for (int i = 0; i < S.Length; i++)
                {
                    var value = S[i];
                    det *= value;

                    if (Precisionfloat.AlmostEqual((float) math.abs(value), (float) 0.0))
                    {
                        return 0;
                    }
                }

                return math.abs(det);
            }
        }
    }
}
