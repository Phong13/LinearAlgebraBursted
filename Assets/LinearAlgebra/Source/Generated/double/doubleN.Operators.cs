using System;
using System.Runtime.CompilerServices;

namespace LinearAlgebra
{

    public partial struct doubleN {

        #region SCALAR OPERATIONS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static doubleN operator +(in doubleN a) => a;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static doubleN operator -(in doubleN a) {

            doubleN vec = a.CopyTemp();
            doubleOP.signFlipInpl(vec);

            return vec;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static doubleN operator +(in doubleN a, double s) {

            doubleN vec = a.CopyTemp();
            doubleOP.addInpl(vec, s);

            return vec; 
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static doubleN operator +(double s, in doubleN a) => a + s;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static doubleN operator -(in doubleN a, double s) {
            
            doubleN vec = a.CopyTemp();
            doubleOP.addInpl(vec, -s);
            
            return vec;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static doubleN operator -(double s, in doubleN a)
        {
            doubleN vec = a.CopyTemp();
            vec.rsubInpl(s);
            return vec;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static doubleN operator *(in doubleN a, double s) {
            
            doubleN vec = a.CopyTemp();

            doubleOP.mulInpl(vec, s);

            return vec;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static doubleN operator *(double s, in doubleN a) => a * s;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static doubleN operator /(in doubleN a, double s)
        {
            doubleN vec = a.CopyTemp();

            if (s == 0f)
                throw new DivideByZeroException();

            doubleOP.divInpl(vec, s);

            return vec;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static doubleN operator /(double s, doubleN a)
        {
            doubleN vec = a.CopyTemp();

            doubleOP.divInpl(s, vec);

            return vec;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static doubleN operator %(in doubleN a, double s)
        {
            doubleN vec = a.CopyTemp();

            if (s == 0f)
                throw new DivideByZeroException();

            doubleOP.modInpl(vec, s);

            return vec;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static doubleN operator %(double s, doubleN a)
        {
            doubleN vec = a.CopyTemp();

            doubleOP.modInpl(s, vec);

            return vec;
        }
        #endregion

        #region COMPONENT-WISE OPERATIONS

        /// <summary>
        /// Component-wise addition
        /// Vectors have to be same dimensions
        /// </summary>
        /// <returns>Same dimension vector</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static doubleN operator +(in doubleN a, in doubleN b) {

            Assume.SameDim(in a, in b);

            doubleN vec = a.CopyTemp();

            doubleOP.addInpl(vec, b);

            return vec;
        }

        /// <summary>
        /// Component-wise subtraction
        /// Vectors have to be same dimensions
        /// </summary>
        /// <returns>Same dimension vector</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static doubleN operator -(in doubleN a, in doubleN b) {

            Assume.SameDim(in a, in b);

            doubleN vec = a.CopyTemp();
            doubleOP.subInpl(vec, b);
            
            return vec;
        }

        /// <summary>
        /// Component-wise multiplication
        /// Vectors have to be same dimensions
        /// </summary>
        /// <returns>Same dimension vector</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static doubleN operator *(in doubleN a, in doubleN b) {

            Assume.SameDim(in a, in b);

            doubleN vec = a.CopyTemp();

            doubleOP.compMulInpl(b, vec);

            return vec;
        }

        /// <summary>
        /// Component-wise division
        /// Vectors have to be same dimensions
        /// Dividend / divisor
        /// </summary>
        /// <returns>Same dimension vector</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static doubleN operator /(in doubleN dividend, in doubleN divisor)
        {
            Assume.SameDim(in dividend, in divisor);

            doubleN newDividendVec = dividend.CopyTemp();
            doubleOP.compDivInpl(newDividendVec, divisor);

            return newDividendVec;
        }

        /// <summary>
        /// Component-wise modulo
        /// Vectors have to be same dimensions
        /// Dividend / divisor
        /// </summary>
        /// <returns>Same dimension vector</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static doubleN operator %(in doubleN dividend, in doubleN divisor)
        {
            Assume.SameDim(in dividend, in divisor);

            doubleN newDividendVec = dividend.CopyTemp();
            doubleOP.compModDiv(newDividendVec, divisor);

            return newDividendVec;
        }

        #endregion

    }
}