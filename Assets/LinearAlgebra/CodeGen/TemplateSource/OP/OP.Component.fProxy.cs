#define UNITY_BURST_EXPERIMENTAL_LOOP_INTRINSICS 

using System.Runtime.CompilerServices;

using Unity.Burst;
using Unity.Mathematics;

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
        public static float3 GetSubvecAsFloat3(this fProxyN a, int index = 0)
        {
            float3 v;
            unsafe
            {
                v.x = (float) a.Data[index];
                v.y = (float) a.Data[index + 1];
                v.z = (float) a.Data[index + 2];
            }

            return v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 GetSubvecAsFloat4(this fProxyN a, int index = 0)
        {
            float4 v;
            unsafe
            {
                v.x = (float) a.Data[index];
                v.y = (float) a.Data[index + 1];
                v.z = (float) a.Data[index + 2];
                v.w = (float) a.Data[index + 3];
            }

            return v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fProxyN GetSubvec(this fProxyN a, int index, int len)
        {
            
            unsafe
            {
                fProxyN v = a._arenaPtr->fProxyVec(len);
                for (int i = 0; i < len; i++)
                {
                    v[i] = a[index + i];
                }

                return v;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fProxyN GetSubvecTemp(this fProxyN a, int index, int len)
        {

            unsafe
            {
                fProxyN v = a._arenaPtr->tempfProxyVec(len);
                for (int i = 0; i < len; i++)
                {
                    v[i] = a[index + i];
                }

                return v;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 GetColAsFloat3(this fProxyMxN a, int col, int rowIdx = 0)
        {
            float3 c;
            unsafe
            {
                c.x = (float) a[rowIdx, col];
                c.y = (float) a[rowIdx + 1, col];
                c.z = (float) a[rowIdx + 2, col];
            }

            return c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 GetColAsFloat4(this fProxyMxN a, int col, int rowIdx = 0)
        {
            float4 c;
            unsafe
            {
                c.x = (float) a[rowIdx, col];
                c.y = (float) a[rowIdx + 1, col];
                c.z = (float) a[rowIdx + 2, col];
                c.w = (float) a[rowIdx + 3, col];
            }

            return c;
        }

        /// <summary>
        /// Allocated as a tempVec
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fProxyN ColTemp(this fProxyMxN a, int col, int rowStartIdx = 0)
        {
            unsafe
            {
                int len = a.M_Rows - rowStartIdx;
                fProxyN c = a._arenaPtr->tempfProxyVec(len);
                for (int i = 0; i < len; i++)
                {
                    c[i] = a[rowStartIdx + i, col];
                }

                return c;
            }
        }
    }
}
