using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using System;
using System.Runtime.InteropServices;

namespace LinearAlgebra
{
    [StructLayout(LayoutKind.Sequential)]
    public partial struct doubleN : IDisposable, IUnsafedoubleArray {

        [NativeDisableUnsafePtrRestriction]
        internal unsafe Arena* _arenaPtr;

        public int N => Data.Length;
        
        public UnsafeList<double> Data { get; private set; }

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
        }

        public unsafe doubleN Copy()
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

        public void Dispose() {
#if LINALG_DEBUG
            for (int i = 0; i < N; i++) this[i] = float.NaN;
#endif
            Data.Dispose();
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