#define UNITY_BURST_EXPERIMENTAL_LOOP_INTRINSICS 

using System.Runtime.CompilerServices;
using System;

namespace LinearAlgebra
{
    /// <summary>           
    /// Inpl = inplace
    /// </summary>
    public static partial class floatOP {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float dot(floatN a, floatN b)
        {
            Arena.CheckValid(a);
            Arena.CheckValid(b);
            if (a.N != b.N)
                throw new ArgumentException("dot: Vector must have same dimension");

            unsafe {
                return UnsafeOP.vecDot(a.Data.Ptr, b.Data.Ptr, a.Data.Length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float dot(floatN a, floatN b, int start, int end = -1) {
            Arena.CheckValid(a);
            Arena.CheckValid(b);
            if (a.N != b.N)
                throw new ArgumentException("dot: Vector must have same dimension");

            if(end == -1)
                end = a.N;

            unsafe {
                return UnsafeOP.vecDotRange(a.Data.Ptr, b.Data.Ptr, start, end);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static floatMxN outerDot(floatN a, floatN b)
        {
            Arena.CheckValid(a);
            Arena.CheckValid(b);
            floatMxN result = a.tempfloatMat(a.N, b.N, true);

            unsafe
            {
                UnsafeOP.vecOuterDot(a.Data.Ptr, b.Data.Ptr, result.Data.Ptr, a.N, b.N);
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static floatN dot(floatMxN A, floatN x)
        {
            Arena.CheckValid(A);
            Arena.CheckValid(x);
            Assume.SameDim(A.N_Cols, x.N);

            floatN result = x.tempfloatVec(A.M_Rows);

            unsafe {
                
                UnsafeOP.matVecDot(A.Data.Ptr, x.Data.Ptr, result.Data.Ptr, A.M_Rows, A.N_Cols);
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static floatN dot(floatN y, floatMxN A)
        {
            Arena.CheckValid(y);
            Arena.CheckValid(A);
            Assume.SameDim(A.M_Rows, y.N);

            floatN result = y.tempfloatVec(A.N_Cols);

            unsafe {
                UnsafeOP.vecMatDot(y.Data.Ptr, A.Data.Ptr, result.Data.Ptr, A.M_Rows, A.N_Cols);
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static floatMxN dot(floatMxN a, floatMxN b, bool transposeA = false)
        {
            Arena.CheckValid(a);
            Arena.CheckValid(b);
            if (transposeA)
                Assume.SameDim(a.N_Cols, b.N_Cols);
            else
                Assume.SameDim(a.N_Cols, b.M_Rows);

            int m, n, k;

            if (transposeA)
            {
                m = a.N_Cols; n = a.M_Rows ; k = b.N_Cols;
            }
            else {
                m = a.M_Rows; n = a.N_Cols; k = b.N_Cols;
            }
            floatMxN c = a.tempfloatMat(m, k);

            unsafe
            {
                if(transposeA)
                    UnsafeOP.matMatDotTransA(a.Data.Ptr, b.Data.Ptr, c.Data.Ptr, m, n, k);
                else
                    UnsafeOP.matMatDot(a.Data.Ptr, b.Data.Ptr, c.Data.Ptr, m, n, k);
            }

            return c;
        }

        /// <summary>
        /// No allocations, stores result in this matrix
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void dotCompInpl(this floatMxN target, floatMxN a, floatMxN b, bool transposeA = false)
        {
            Arena.CheckValid(a);
            Arena.CheckValid(b);
            Arena.CheckValid(target);
            if (transposeA)
                Assume.SameDim(a.N_Cols, b.N_Cols);
            else
                Assume.SameDim(a.N_Cols, b.M_Rows);

            int m, n, k;

            if (transposeA)
            {
                m = a.N_Cols; n = a.M_Rows; k = b.N_Cols;
            }
            else
            {
                m = a.M_Rows; n = a.N_Cols; k = b.N_Cols;
            }

            Assume.SameDim(target.M_Rows, m);
            Assume.SameDim(target.N_Cols, k);

            unsafe
            {
                for (int i = 0; i < target.Length; i++) target[i] = 0;
                if (transposeA)
                    UnsafeOP.matMatDotTransA(a.Data.Ptr, b.Data.Ptr, target.Data.Ptr, m, n, k);
                else
                    UnsafeOP.matMatDot(a.Data.Ptr, b.Data.Ptr, target.Data.Ptr, m, n, k);
            }
        }

        /// <summary>
        /// No allocations, stores result in this matrix
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void dotCompInpl(this floatN target, floatMxN A, floatN x, bool transposeA = false)
        {
            Arena.CheckValid(target);
            Arena.CheckValid(A);
            Arena.CheckValid(x);
            Assume.SameDim(A.N_Cols, x.N);
            unsafe
            {
                for (int i = 0; i < target.N; i++) target[i] = 0;
                UnsafeOP.matVecDot(A.Data.Ptr, x.Data.Ptr, target.Data.Ptr, A.M_Rows, A.N_Cols);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static floatMxN trans(floatMxN A)
        {
            Arena.CheckValid(A);
            var T = A.tempfloatMat(A.N_Cols, A.M_Rows, true);

            unsafe
            {
                UnsafeOP.matTrans(A.Data.Ptr, T.Data.Ptr, A.M_Rows, A.N_Cols);
            }

            return T;
        }
    }
}
