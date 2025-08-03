#define UNITY_BURST_EXPERIMENTAL_LOOP_INTRINSICS 

using System.Runtime.CompilerServices;

using Unity.Burst;
using Unity.Mathematics;

//+deleteThis
using LinearAlgebra.mathProxies;
//-deleteThis


namespace LinearAlgebra
{

    // can add chaining here for inplace methods

    /// <summary>           
    /// Inpl = inplace
    /// </summary>
    public static partial class fProxyOP {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void addInpl<T>(T place, fProxy s) where T : unmanaged, IUnsafefProxyArray {

            unsafe {
                UnsafeOP.scalAdd(place.Data.Ptr, place.Data.Length, s);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void mulInpl<T>(T place, fProxy s) where T : unmanaged, IUnsafefProxyArray
        {
            unsafe {
                UnsafeOP.scalMul(place.Data.Ptr, place.Data.Length, s);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void divInpl<T>(this T place, fProxy s) where T : unmanaged, IUnsafefProxyArray
        {
            unsafe
            {
                UnsafeOP.scalDiv(place.Data.Ptr, place.Data.Length, s);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void divInpl<T>(fProxy s, T place) where T : unmanaged, IUnsafefProxyArray
        {
            unsafe
            {
                UnsafeOP.scalDiv(s, place.Data.Ptr, place.Data.Length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void addInpl<T>(this T place, T from) where T : unmanaged, IUnsafefProxyArray
        {
            unsafe {
                UnsafeOP.compAdd(place.Data.Ptr, from.Data.Ptr, from.Data.Length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void addInpl<T>(this T place, T lhs, T rhs) where T : unmanaged, IUnsafefProxyArray
        {
            unsafe
            {
                UnsafeOP.compAdd(place.Data.Ptr, lhs.Data.Ptr, rhs.Data.Ptr, rhs.Data.Length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void subInpl<T>(this T place, T fromB) where T : unmanaged, IUnsafefProxyArray
        {
            unsafe {
                UnsafeOP.compSub(place.Data.Ptr, fromB.Data.Ptr, fromB.Data.Length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void subInpl<T>(this T place, T lhs, T rhs) where T : unmanaged, IUnsafefProxyArray
        {
            unsafe
            {
                UnsafeOP.compSub(place.Data.Ptr, lhs.Data.Ptr, rhs.Data.Ptr, rhs.Data.Length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void modInpl<T>(this T place, fProxy s) where T : unmanaged, IUnsafefProxyArray
        {
            unsafe
            {
                UnsafeOP.scalMod(place.Data.Ptr, place.Data.Length, s);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void modInpl<T>(fProxy s, T place) where T : unmanaged, IUnsafefProxyArray
        {
            unsafe
            {
                UnsafeOP.scalMod(s, place.Data.Ptr, place.Data.Length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void compMulInpl<T>(this T from, T to) where T : unmanaged, IUnsafefProxyArray
        {
            unsafe {
                UnsafeOP.compMul(from.Data.Ptr, to.Data.Ptr, from.Data.Length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void compDivInpl<T>(this T targetDividend, T fromDivisor) where T : unmanaged, IUnsafefProxyArray
        {
            unsafe {
                UnsafeOP.compDiv(targetDividend.Data.Ptr, fromDivisor.Data.Ptr, targetDividend.Data.Length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void compModDiv<T>(this T targetDividend, T fromDivisor) where T : unmanaged, IUnsafefProxyArray
        {
            unsafe {
                UnsafeOP.compMod(targetDividend.Data.Ptr, fromDivisor.Data.Ptr, targetDividend.Data.Length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void subInpl<T>(this T v, fProxy s) where T : unmanaged, IUnsafefProxyArray
        {
            addInpl(v, -s);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void subInpl<T>(fProxy s, T v) where T : unmanaged, IUnsafefProxyArray
        {
            unsafe {                 
                UnsafeOP.scalSub(s, v.Data.Ptr, v.Data.Length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void signFlipInpl<T>(this T a) where T : unmanaged, IUnsafefProxyArray
        {
            unsafe { 
                UnsafeOP.signFlip(a.Data.Ptr, a.Data.Ptr, a.Data.Length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void setAll<T>(this T a, float s) where T : unmanaged, IUnsafefProxyArray
        {
            unsafe
            {
                mathUnsafefProxy.setAll(a.Data.Ptr, a.Data.Length, s);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsByValue<T>(this T a, T rhs) where T : unmanaged, IUnsafefProxyArray
        {
            unsafe
            {
                if (a.Data.Length != rhs.Data.Length) return false;
                for (int i = 0; i < a.Data.Length; i++)
                {
                    if (a.Data[i] != rhs.Data[i]) return false;
                }
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AlmostEqualsByValue<T>(this T a, T rhs, fProxy eps) where T : unmanaged, IUnsafefProxyArray
        {
            unsafe
            {
                if (a.Data.Length != rhs.Data.Length) return false;
                for (int i = 0; i < a.Data.Length; i++)
                {
                    if (math.abs(a.Data[i] - rhs.Data[i]) > eps) return false;
                }
            }

            return true;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fProxy3 GetSubvecAsFloat3(this fProxyN a, int index = 0)
        {
            fProxy3 v;
            unsafe
            {
                v.x =  a.Data[index];
                v.y =  a.Data[index + 1];
                v.z =  a.Data[index + 2];
            }

            return v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fProxy4 GetSubvecAsFloat4(this fProxyN a, int index = 0)
        {
            fProxy4 v;
            unsafe
            {
                v.x =  a.Data[index];
                v.y =  a.Data[index + 1];
                v.z =  a.Data[index + 2];
                v.w =  a.Data[index + 3];
            }

            return v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fProxyN GetSubvec(this fProxyN a, int index, int len, bool isTemp)
        {
            
            unsafe
            {
                fProxyN v;
                if (isTemp)
                {
                    v = a._arenaPtr->tempfProxyVec(len);
                } else
                {
                    v = a._arenaPtr->fProxyVec(len);
                }

                for (int i = 0; i < len; i++)
                {
                    v[i] = a[index + i];
                }

                return v;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetSubvec(this fProxyN a, fProxyN from, int idx, int num)
        {
            unsafe
            {
                for (int i = 0; i < num; i++)
                {
                    a[idx + i] = from[i];
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetSubvec(this fProxyN a, float3 from, int idx)
        {
            unsafe
            {
                for (int i = 0; i < 3; i++)
                {
                    a[idx + i] = from[i];
                }
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fProxy3 GetColAsFloat3(this fProxyMxN a, int col, int rowIdx = 0)
        {
            fProxy3 c;
            unsafe
            {
                c.x =  a[rowIdx, col];
                c.y =  a[rowIdx + 1, col];
                c.z =  a[rowIdx + 2, col];
            }

            return c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fProxy4 GetColAsFloat4(this fProxyMxN a, int col, int rowIdx = 0)
        {
            fProxy4 c;
            unsafe
            {
                c.x =  a[rowIdx, col];
                c.y =  a[rowIdx + 1, col];
                c.z =  a[rowIdx + 2, col];
                c.w =  a[rowIdx + 3, col];
            }

            return c;
        }

        /// <summary>
        /// Allocated as a tempVec
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fProxyN Col(this fProxyMxN a, int col, int rowStartIdx = 0, bool isTemp = true)
        {
            unsafe
            {
                int len = a.M_Rows - rowStartIdx;
                fProxyN c;
                if (isTemp)
                {
                    c = a._arenaPtr->tempfProxyVec(len);
                } else
                {
                    c = a._arenaPtr->fProxyVec(len);
                }

                for (int i = 0; i < len; i++)
                {
                    c[i] = a[rowStartIdx + i, col];
                }

                return c;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetCol(this fProxyMxN a, fProxy3 c, int colidx, int rowIdx = 0)
        {
            unsafe
            {
                a[rowIdx, colidx] = c.x;
                a[rowIdx + 1, colidx] = c.y;
                a[rowIdx + 2, colidx] = c.z;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetCol(this fProxyMxN a, fProxy4 c, int colIdx, int rowIdx = 0)
        {
            unsafe
            {
                a[rowIdx, colIdx] = c.x;
                a[rowIdx + 1, colIdx] = c.y;
                a[rowIdx + 2, colIdx] = c.z;
                a[rowIdx + 3, colIdx] = c.w;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetCol(this fProxyMxN a, fProxyN c, int colIdx, int rowStartIdx = 0)
        {
            unsafe
            {
                for (int i = 0; i < c.N; i++)
                {
                    a[rowStartIdx + i, colIdx] = c[i];
                }
            }
        }

        public static fProxyMxN GetSubMatrix(this fProxyMxN a, int rowIdx, int numRows, int colIdx, int numCols, bool isTemp)
        {
            unsafe
            {
                fProxyMxN m;
                if (isTemp)
                {
                    m = a._arenaPtr->tempfProxyMat(numRows, numCols);
                } else
                {
                    m = a._arenaPtr->fProxyMat(numRows, numCols);
                }

                for (int i = 0; i < numRows; i++)
                {
                    for (int j = 0; j < numCols; j++)
                    {
                        m[i, j] = a[rowIdx + i, colIdx + j];
                    }
                }

                return m;
            }
        }

        public static fProxy3x3 GetSubMatrixFloat3x3(this fProxyMxN a, int rowIdx, int colIdx)
        {
            unsafe
            {
                fProxy3x3 m;

                m.c0.x =  a[rowIdx    , colIdx]; m.c1.x =  a[rowIdx    , colIdx + 1]; m.c2.x =  a[rowIdx    , colIdx + 2];
                m.c0.y =  a[rowIdx + 1, colIdx]; m.c1.y =  a[rowIdx + 1, colIdx + 1]; m.c2.y =  a[rowIdx + 1, colIdx + 2];
                m.c0.z =  a[rowIdx + 2, colIdx]; m.c1.z =  a[rowIdx + 2, colIdx + 1]; m.c2.z =  a[rowIdx + 2, colIdx + 2];

                return m;
            }
        }

        /// <summary>
        /// Copies part of matrix from to part of target. No allocations
        /// </summary>
        public static void CopySubMatrix(this fProxyMxN target, int targRowIdx, int targColIdx, fProxyMxN from, int srcRowIdx, int numRows, int srcColIdx, int numCols)
        {
            unsafe
            {
                for (int i = 0; i < numRows; i++)
                {
                    for (int j = 0; j < numCols; j++)
                    {
                        target[targRowIdx + i, targColIdx + j] = from[srcRowIdx + i, srcColIdx + j];
                    }
                }
            }
        }

        public static void SetSubMatrix(this fProxyMxN target, fProxyMxN from, int targRowIdx, int targColIdx)
        {
            unsafe
            {
                for (int i = 0; i < from.M_Rows; i++)
                {
                    for (int j = 0; j < from.N_Cols; j++)
                    {
                         target[targRowIdx + i, targColIdx + j] = from[i, j];
                    }
                }
            }
        }

        public static void SetSubMatrix(this fProxyMxN target, int targRowIdx, int targColIdx, fProxyMxN from, int srcRow, int numRows, int srcCol, int numCols)
        {
            unsafe
            {
                for (int i = 0; i < numRows; i++)
                {
                    for (int j = 0; j < numCols; j++)
                    {
                        target[targRowIdx + i, targColIdx + j] = from[srcRow + i, srcCol + j];
                    }
                }
            }
        }



        public static void SetSubMatrix(this fProxyMxN a, fProxy3x3 from, int rowIdx, int colIdx)
        {
            unsafe
            {
                 a[rowIdx    , colIdx] = from.c0.x; a[rowIdx    , colIdx + 1] = from.c1.x; a[rowIdx    , colIdx + 2] = from.c2.x;
                 a[rowIdx + 1, colIdx] = from.c0.y; a[rowIdx + 1, colIdx + 1] = from.c1.y; a[rowIdx + 1, colIdx + 2] = from.c2.y;
                 a[rowIdx + 2, colIdx] = from.c0.z; a[rowIdx + 2, colIdx + 1] = from.c1.z; a[rowIdx + 2, colIdx + 2] = from.c2.z;
            }
        }
    }
}
