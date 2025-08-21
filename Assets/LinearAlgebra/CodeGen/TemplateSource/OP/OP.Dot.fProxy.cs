#define UNITY_BURST_EXPERIMENTAL_LOOP_INTRINSICS 

using System.Runtime.CompilerServices;
using System;
using Unity.Mathematics;

namespace LinearAlgebra
{
    /// <summary>           
    /// Inpl = inplace
    /// </summary>
    public static partial class fProxyOP {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fProxy dot(fProxyN a, fProxyN b)
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
        public static fProxy dot(fProxyN a, fProxyN b, int start, int end = -1) {
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
        public static fProxyMxN outerDot(fProxyN a, fProxyN b)
        {
            Arena.CheckValid(a);
            Arena.CheckValid(b);
            fProxyMxN result = a.tempfProxyMat(a.N, b.N, true);

            unsafe
            {
                UnsafeOP.vecOuterDot(a.Data.Ptr, b.Data.Ptr, result.Data.Ptr, a.N, b.N);
            }

            return result;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fProxyN dot(this in fProxyMxN A, fProxyN x)
        {
            return dot(A, x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fProxyN dot(fProxyMxN A, fProxyN x)
        {
            Arena.CheckValid(A);
            Arena.CheckValid(x);
            Assume.SameDim(A.N_Cols, x.N);

            fProxyN result = x.tempfProxyVec(A.M_Rows);

            unsafe {
                
                UnsafeOP.matVecDot(A.Data.Ptr, x.Data.Ptr, result.Data.Ptr, A.M_Rows, A.N_Cols);
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fProxyN dot(fProxyN y, fProxyMxN A)
        {
            Arena.CheckValid(y);
            Arena.CheckValid(A);
            Assume.SameDim(A.M_Rows, y.N);

            fProxyN result = y.tempfProxyVec(A.N_Cols);

            unsafe {
                UnsafeOP.vecMatDot(y.Data.Ptr, A.Data.Ptr, result.Data.Ptr, A.M_Rows, A.N_Cols);
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fProxyMxN dot(this in fProxyMxN a, fProxyMxN b, bool transposeA = false)
        {
            return dot(a, b, transposeA);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fProxyMxN dot(fProxyMxN a, fProxyMxN b, bool transposeA = false)
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
            fProxyMxN c = a.tempfProxyMat(m, k);

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
        public static void dotInpl(this fProxyMxN target, fProxyMxN a, fProxyMxN b, bool transposeA = false)
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
        public static void dotInpl(this fProxyN target, fProxyMxN A, fProxyN x, bool transposeA = false)
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
        public static fProxyMxN trans(fProxyMxN A)
        {
            Arena.CheckValid(A);
            var T = A.tempfProxyMat(A.N_Cols, A.M_Rows, true);

            unsafe
            {
                UnsafeOP.matTrans(A.Data.Ptr, T.Data.Ptr, A.M_Rows, A.N_Cols);
            }

            return T;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fProxy l2Norm(this in fProxyN v)
        {
            Arena.CheckValid(v);
            return math.sqrt(v.sumSqr());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fProxy sumSqr(this in fProxyN v)
        {
            Arena.CheckValid(v);
            unsafe
            {
                fProxy sumSqr = 0;
                for (int i = 0; i < v.N; i++)
                {
                    sumSqr += v[i] * v[i];
                }

                return sumSqr;
            }
        }
    }
}
