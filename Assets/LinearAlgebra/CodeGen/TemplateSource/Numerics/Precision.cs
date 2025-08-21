// <copyright file="Precision.cs" company="Math.NET">
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
using System;
using System.Numerics;
using System.Runtime;
using System.Runtime.InteropServices;

namespace LinearAlgebra.MathNet.Numerics
{
    public static class Precision
    {
        /// <summary>
        /// The number of binary digits used to represent the binary number for a double precision floating
        /// point value. i.e. there are this many digits used to represent the
        /// actual number, where in a number as: 0.134556 * 10^5 the digits are 0.134556 and the exponent is 5.
        /// </summary>
        const int DoubleWidth = 53;

        /// <summary>
        /// The number of binary digits used to represent the binary number for a single precision floating
        /// point value. i.e. there are this many digits used to represent the
        /// actual number, where in a number as: 0.134556 * 10^5 the digits are 0.134556 and the exponent is 5.
        /// </summary>
        const int SingleWidth = 24;

        /// <summary>
        /// Standard epsilon, the maximum relative precision of IEEE 754 double-precision floating numbers (64 bit).
        /// According to the definition of Prof. Demmel and used in LAPACK and Scilab.
        /// </summary>
        public static readonly double DoublePrecision = math.pow(2, -DoubleWidth);

        /// <summary>
        /// Standard epsilon, the maximum relative precision of IEEE 754 double-precision floating numbers (64 bit).
        /// According to the definition of Prof. Higham and used in the ISO C standard and MATLAB.
        /// </summary>
        public static readonly double PositiveDoublePrecision = 2 * DoublePrecision;

        /// <summary>
        /// Standard epsilon, the maximum relative precision of IEEE 754 single-precision floating numbers (32 bit).
        /// According to the definition of Prof. Demmel and used in LAPACK and Scilab.
        /// </summary>
        public static readonly double SinglePrecision = math.pow(2, -SingleWidth);

        /// <summary>
        /// Standard epsilon, the maximum relative precision of IEEE 754 single-precision floating numbers (32 bit).
        /// According to the definition of Prof. Higham and used in the ISO C standard and MATLAB.
        /// </summary>
        public static readonly double PositiveSinglePrecision = 2 * SinglePrecision;

        /// <summary>
        /// Actual double precision machine epsilon, the smallest number that can be subtracted from 1, yielding a results different than 1.
        /// This is also known as unit roundoff error. According to the definition of Prof. Demmel.
        /// On a standard machine this is equivalent to `DoublePrecision`.
        /// </summary>
        public static readonly double MachineEpsilon = MeasureMachineEpsilon();

        /// <summary>
        /// Actual double precision machine epsilon, the smallest number that can be added to 1, yielding a results different than 1.
        /// This is also known as unit roundoff error. According to the definition of Prof. Higham.
        /// On a standard machine this is equivalent to `PositiveDoublePrecision`.
        /// </summary>
        public static readonly double PositiveMachineEpsilon = MeasurePositiveMachineEpsilon();

        /// <summary>
        /// The number of significant decimal places of double-precision floating numbers (64 bit).
        /// </summary>
        public static readonly int DoubleDecimalPlaces = (int)math.floor(math.abs(math.log10(DoublePrecision)));

        /// <summary>
        /// The number of significant decimal places of single-precision floating numbers (32 bit).
        /// </summary>
        public static readonly int SingleDecimalPlaces = (int)math.floor(math.abs(math.log10(SinglePrecision)));

        /// <summary>
        /// Value representing 10 * 2^(-53) = 1.11022302462516E-15
        /// </summary>
        public static readonly double DefaultDoubleAccuracy = DoublePrecision * 10;

        /// <summary>
        /// Value representing 10 * 2^(-24) = 5.96046447753906E-07
        /// </summary>
        public static readonly float DefaultSingleAccuracy = (float)(SinglePrecision * 10);

        public static readonly double DefaultAccuracydouble = DoublePrecision * 10;

        public static readonly float DefaultAccuracyfloat = (float)(SinglePrecision * 10);

        public static readonly float DefaultAccuracyfProxy = DefaultAccuracyfloat;

        public static readonly float PrecisionfProxy = (float) SinglePrecision;

        public static readonly float Precisionfloat = (float)SinglePrecision;

        public static readonly double Precisiondouble = DoublePrecision;

        /// <summary>
        /// Calculates the actual (negative) double precision machine epsilon - the smallest number that can be subtracted from 1, yielding a results different than 1.
        /// This is also known as unit roundoff error. According to the definition of Prof. Demmel.
        /// </summary>
        /// <returns>Positive Machine epsilon</returns>
        static double MeasureMachineEpsilon()
        {
            double eps = 1.0d;

            while ((1.0d - (eps / 2.0d)) < 1.0d)
                eps /= 2.0d;

            return eps;
        }

        /// <summary>
        /// Calculates the actual positive double precision machine epsilon - the smallest number that can be added to 1, yielding a results different than 1.
        /// This is also known as unit roundoff error. According to the definition of Prof. Higham.
        /// </summary>
        /// <returns>Machine epsilon</returns>
        static double MeasurePositiveMachineEpsilon()
        {
            double eps = 1.0d;

            while ((1.0d + (eps / 2.0d)) > 1.0d)
                eps /= 2.0d;

            return eps;
        }
    }
}
