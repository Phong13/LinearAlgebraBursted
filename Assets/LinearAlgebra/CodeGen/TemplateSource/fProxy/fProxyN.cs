using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using System;
using System.Runtime.InteropServices;

namespace LinearAlgebra
{
    [StructLayout(LayoutKind.Sequential)]
    public partial struct fProxyN : IDisposable, IUnsafefProxyArray {

        [NativeDisableUnsafePtrRestriction]
        internal unsafe Arena* _arenaPtr;

        public int N => Data.Length;
        
        public UnsafeList<fProxy> Data { get; private set; }

        /// <summary>
        /// Why is this a container of length 1 and not a variable? Because fProxN's are structs. When the Arena
        /// returns freshly allocated fProxyN it is returning a copy of it's internal fProxyN that points to the same
        /// buffers. We want all copies of this fProxyN to see the same changes. To accomplish this the flags needs
        /// to be a pointer.
        /// </summary>
        public UnsafeList<Arena.ArrayFlags> fflags { get; private set; }

        /// <summary>
        /// Creates a new vector of dimension N
        /// </summary>
        /// <param name="n"></param>
        /// <param name="allocator"></param>
        public unsafe fProxyN(int n, in Arena arena, bool uninit = false) { 

            fixed (Arena* arenaPtr = &arena)
                _arenaPtr = arenaPtr;

            var allocator = arena.Allocator;
            //var allocator1 = _arenaPtr->Allocator;
            //UnityEngine.Debug.Log($"Vector: {allocator}");
            //UnityEngine.Debug.Log($"Vector: {allocator1}");

            var data = new UnsafeList<fProxy>(n, allocator, uninit? NativeArrayOptions.UninitializedMemory : NativeArrayOptions.ClearMemory);
            data.Resize(n, NativeArrayOptions.UninitializedMemory);

            Data = data;

            fflags = new UnsafeList<Arena.ArrayFlags>(1, allocator);
            fflags.Resize(1);
            fflags.Ptr[0] = Arena.ArrayFlags.None;
        }

        /// <summary>
        /// Creates a copy of vector with new allocation
        /// </summary>
        /// <param name="orig"></param>
        public unsafe fProxyN(in fProxyN orig, Allocator allocator = Allocator.Invalid) {

            _arenaPtr = orig._arenaPtr;

            if(allocator == Allocator.Invalid)
                allocator = _arenaPtr->Allocator;

            //var allocator1 = _arenaPtr->Allocator;
            //UnityEngine.Debug.Log($"Vector: {allocator}");
            //UnityEngine.Debug.Log($"Vector: {allocator1}");

            var data = new UnsafeList<fProxy>(orig.N, allocator, NativeArrayOptions.UninitializedMemory);
            data.Resize(orig.N, NativeArrayOptions.UninitializedMemory);
            data.CopyFrom(orig.Data);

            Data = data;

            fflags = new UnsafeList<Arena.ArrayFlags>(1, allocator);
            fflags.Resize(1);
            fflags.Ptr[0] = Arena.ArrayFlags.None;
        }

        /// <summary>
        /// Creates a copy of vector with new allocation
        /// </summary>
        /// <param name="orig"></param>
        public unsafe fProxyN(int n, Allocator allocator = Allocator.Invalid, bool uninit = false)
        {
            _arenaPtr = null;

            if (allocator == Allocator.Invalid)
                allocator = _arenaPtr->Allocator;

            var data = new UnsafeList<fProxy>(n, allocator, NativeArrayOptions.UninitializedMemory);
            data.Resize(n, uninit ? NativeArrayOptions.UninitializedMemory : NativeArrayOptions.ClearMemory);
            
            Data = data;

            fflags = new UnsafeList<Arena.ArrayFlags>(1, allocator);
            fflags.Resize(1);
            fflags.Ptr[0] = Arena.ArrayFlags.None;
        }

        public unsafe fProxyN CopyPersistent()
        {
            return _arenaPtr->fProxyVec(in this);
        }

        public unsafe fProxyN TempCopy()
        {
            return _arenaPtr->tempfProxyVec(in this);
        }

        public void CopyTo(in fProxyN vec)
        {
            if (this.N != vec.N)
                throw new Exception("CopyTo: dimensions do not match!");

            vec.Data.CopyFrom(Data);
        }

        public void CopyFrom(in fProxyN vec) {

            if (this.N != vec.N)
                throw new Exception("CopyFrom: dimensions do not match!");

            Data.CopyFrom(vec.Data);
        }

        public unsafe bool IsDisposed()
        {
            return (fflags.Ptr[0] & Arena.ArrayFlags.isDisposed) != 0;
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
                fflags.Ptr[0] = Arena.ArrayFlags.isDisposed; // unset other flags.
                fflags.Dispose();
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