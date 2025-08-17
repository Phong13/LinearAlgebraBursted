#define UNITY_BURST_EXPERIMENTAL_LOOP_INTRINSICS 

using System.Runtime.CompilerServices;
using System;
using Unity.Mathematics;

namespace LinearAlgebra
{
    /// <summary>           
    /// Inpl = inplace
    /// </summary>
    public static partial class doubleOP {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double dot(doubleN a, doubleN b)
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
        public static double dot(doubleN a, doubleN b, int start, int end = -1) {
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
        public static doubleMxN outerDot(doubleN a, doubleN b)
        {
            Arena.CheckValid(a);
            Arena.CheckValid(b);
            doubleMxN result = a.tempdoubleMat(a.N, b.N, true);

            unsafe
            {
                UnsafeOP.vecOuterDot(a.Data.Ptr, b.Data.Ptr, result.Data.Ptr, a.N, b.N);
            }

            return result;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static doubleN dot(this in doubleMxN A, doubleN x)
        {
            return dot(A, x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static doubleN dot(doubleMxN A, doubleN x)
        {
            Arena.CheckValid(A);
            Arena.CheckValid(x);
            Assume.SameDim(A.N_Cols, x.N);

            doubleN result = x.tempdoubleVec(A.M_Rows);

            unsafe {
                
                UnsafeOP.matVecDot(A.Data.Ptr, x.Data.Ptr, result.Data.Ptr, A.M_Rows, A.N_Cols);
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static doubleN dot(doubleN y, doubleMxN A)
        {
            Arena.CheckValid(y);
            Arena.CheckValid(A);
            Assume.SameDim(A.M_Rows, y.N);

            doubleN result = y.tempdoubleVec(A.N_Cols);

            unsafe {
                UnsafeOP.vecMatDot(y.Data.Ptr, A.Data.Ptr, result.Data.Ptr, A.M_Rows, A.N_Cols);
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static doubleMxN dot(this in doubleMxN a, doubleMxN b, bool transposeA = false)
        {
            return dot(a, b, transposeA);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static doubleMxN dot(doubleMxN a, doubleMxN b, bool transposeA = false)
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
            doubleMxN c = a.tempdoubleMat(m, k);

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
        public static void dotInpl(this doubleMxN target, doubleMxN a, doubleMxN b, bool transposeA = false)
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
        public static void dotInpl(this doubleN target, doubleMxN A, doubleN x, bool transposeA = false)
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
        public static doubleMxN trans(doubleMxN A)
        {
            Arena.CheckValid(A);
            var T = A.tempdoubleMat(A.N_Cols, A.M_Rows, true);

            unsafe
            {
                UnsafeOP.matTrans(A.Data.Ptr, T.Data.Ptr, A.M_Rows, A.N_Cols);
            }

            return T;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double l2Norm(this in doubleN v)
        {
            Arena.CheckValid(v);
            return math.sqrt(v.sumSqr());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double sumSqr(this in doubleN v)
        {
            Arena.CheckValid(v);
            unsafe
            {
                double sumSqr = 0;
                for (int i = 0; i < v.N; i++)
                {
                    sumSqr += v[i] * v[i];
                }

                return sumSqr;
            }
        }
    }
}
