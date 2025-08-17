using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using System;
using System.Runtime.InteropServices;

namespace LinearAlgebra
{
    [StructLayout(LayoutKind.Sequential)]
    public partial struct doubleN : IDisposable, IUnsafedoubleArray, IEquatable<doubleN> {

        [NativeDisableUnsafePtrRestriction]
        internal unsafe Arena* _arenaPtr;

        public int N => Data.Length;

        public int Length => Data.Length;

        public int Count => Data.Length; // Compatibility with Math.Net


        public UnsafeList<double> Data { get; private set; }

        /// <summary>
        /// Why is this a container of length 1 and not a variable? Because fProxN's are structs. When the Arena
        /// returns freshly allocated doubleN it is returning a copy of it's internal doubleN that points to the same
        /// buffers. We want all copies of this doubleN to see the same changes. To accomplish this the flags needs
        /// to be a pointer.
        /// </summary>
        public UnsafeList<Arena.ArrayFlags> flags { get; private set; }

        /// <summary>
        /// Creates a new vector of dimension N
        /// </summary>
        /// <param name="n"></param>
        /// <param name="allocator"></param>
        public unsafe doubleN(int n, in Arena arena, bool uninit = false) { 

            fixed (Arena* arenaPtr = &arena)
                _arenaPtr = arenaPtr;

            var allocator = arena.Allocator;
            //var allocator1 = _arenaPtr->Allocator;
            //UnityEngine.Debug.Log($"Vector: {allocator}");
            //UnityEngine.Debug.Log($"Vector: {allocator1}");

            var data = new UnsafeList<double>(n, allocator, uninit? NativeArrayOptions.UninitializedMemory : NativeArrayOptions.ClearMemory);
            data.Resize(n, NativeArrayOptions.UninitializedMemory);

            Data = data;

            flags = new UnsafeList<Arena.ArrayFlags>(1, allocator);
            flags.Resize(1);
            flags.Ptr[0] = Arena.ArrayFlags.None;
        }

        /// <summary>
        /// Creates a copy of vector with new allocation
        /// </summary>
        /// <param name="orig"></param>
        public unsafe doubleN(in doubleN orig, Allocator allocator = Allocator.Invalid) {

            _arenaPtr = orig._arenaPtr;

            if(allocator == Allocator.Invalid)
                allocator = _arenaPtr->Allocator;

            //var allocator1 = _arenaPtr->Allocator;
            //UnityEngine.Debug.Log($"Vector: {allocator}");
            //UnityEngine.Debug.Log($"Vector: {allocator1}");

            var data = new UnsafeList<double>(orig.N, allocator, NativeArrayOptions.UninitializedMemory);
            data.Resize(orig.N, NativeArrayOptions.UninitializedMemory);
            data.CopyFrom(orig.Data);

            Data = data;

            flags = new UnsafeList<Arena.ArrayFlags>(1, allocator);
            flags.Resize(1);
            flags.Ptr[0] = Arena.ArrayFlags.None;

            doubleN nn;
        }

        /// <summary>
        /// Creates a copy of vector with new allocation
        /// </summary>
        /// <param name="orig"></param>
        public unsafe doubleN(int n, Allocator allocator = Allocator.Invalid, bool uninit = false)
        {
            _arenaPtr = null;

            if (allocator == Allocator.Invalid)
                allocator = _arenaPtr->Allocator;

            var data = new UnsafeList<double>(n, allocator, NativeArrayOptions.UninitializedMemory);
            data.Resize(n, uninit ? NativeArrayOptions.UninitializedMemory : NativeArrayOptions.ClearMemory);
            
            Data = data;

            flags = new UnsafeList<Arena.ArrayFlags>(1, allocator);
            flags.Resize(1);
            flags.Ptr[0] = Arena.ArrayFlags.None;
        }

        public unsafe doubleN CopyPersistent()
        {
            return _arenaPtr->doubleVec(in this);
        }

        public unsafe doubleN TempCopy()
        {
            return _arenaPtr->tempdoubleVec(in this);
        }

        public void CopyTo(in doubleN vec)
        {
            if (this.N != vec.N)
                throw new Exception("CopyTo: dimensions do not match!");

            vec.Data.CopyFrom(Data);
        }

        public void CopyFrom(in doubleN vec) {

            if (this.N != vec.N)
                throw new Exception("CopyFrom: dimensions do not match!");

            Data.CopyFrom(vec.Data);
        }

        public unsafe bool IsDisposed()
        {
            return (flags.Ptr[0] & Arena.ArrayFlags.isDisposed) != 0;
        }

        public unsafe bool Equals(doubleN other)
        {
            if (Data.Ptr == other.Data.Ptr) return true;
            return false;
        }

        public unsafe override int GetHashCode()
        {
            long dataPtrHash = 0;
            if (Data.IsCreated)
            {
                dataPtrHash = (long)Data.Ptr;
            }

            long flagsPtrHash = 0;
            if (flags.IsCreated)
            {
                flagsPtrHash = (long)flags.Ptr;
            }

            // Use HashCode.Combine to mix the hash codes of the pointers.
            return HashCode.Combine((long)_arenaPtr, dataPtrHash, flagsPtrHash);
        }

        public unsafe bool CheckValid()
        {
            if (_arenaPtr == null) return false;
            if (IsDisposed()) return false;
            if (!Data.IsCreated) return false;
            return true;
        }

        public unsafe bool CheckValid(bool expectPersistent)
        {
            if (_arenaPtr == null) return false;
            if (IsDisposed()) return false;
            if (!Data.IsCreated) return false;
            if (expectPersistent && (flags.Ptr[0] & Arena.ArrayFlags.isPersistent) == 0) return false; // must be persistent
            else if ((flags.Ptr[0] & Arena.ArrayFlags.isTemp) == 0) return false; // must be temp
            return true;
        }

        public void Dispose() 
        {
            Dispose(true);
        }

        private unsafe void Dispose(bool disposing)
        {
            bool disposed = IsDisposed();
            if (!disposed)
            {
                if (disposing)
                {
                    // Dispose manged resources here
                }

                // Dispose unmanged resources here
                if (N > 0 && float.IsNaN((float) Data[0])) UnityEngine.Debug.LogError("Vector data was NaN. Might be double freeing.");
#if LINALG_DEBUG
                for (int i = 0; i < N; i++) this[i] = float.NaN;
#endif
                Data.Dispose();
                flags.Ptr[0] = Arena.ArrayFlags.isDisposed; // unset other flags.
                flags.Dispose();
            }
        }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < N; i++)
            {
                sb.Append(", ");
                sb.Append(this[i]);
            }

            return sb.ToString();
        }
    }
}