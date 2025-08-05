using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using System;

namespace LinearAlgebra
{
    // A m x n matrix
    // m = rows
    // n = cols
    public partial struct fProxyMxN : IDisposable, IUnsafefProxyArray, IMatrix<fProxy>, IEquatable<fProxyMxN> {
        
        public int M_Rows;
        public int N_Cols;

        public UnsafeList<fProxy> Data { get; private set; }

        [NativeDisableUnsafePtrRestriction]
        internal unsafe Arena* _arenaPtr;

        public readonly int Length;

        /// <summary>
        /// Why is this a container of length 1 and not a variable? Because fProxN's are structs. When the Arena
        /// returns freshly allocated fProxyN it is returning a copy of it's internal fProxyN that points to the same
        /// buffers. We want all copies of this fProxyN to see the same changes. To accomplish this the flags needs
        /// to be a pointer.
        /// </summary>
        public UnsafeList<Arena.ArrayFlags> flags { get; private set; }

        public bool IsSquare => M_Rows == N_Cols;

        int IMatrix<fProxy>.M_Rows => M_Rows;

        int IMatrix<fProxy>.N_Cols => N_Cols;

        public unsafe fProxyMxN(int M_rows, int N_cols, Allocator allocator, bool uninit = false)
        {
            _arenaPtr = null;
            M_Rows = M_rows;
            N_Cols = N_cols;
            Length = M_Rows * N_Cols;
            var data = new UnsafeList<fProxy>(Length, allocator, uninit ? NativeArrayOptions.UninitializedMemory : NativeArrayOptions.ClearMemory);
            data.Resize(Length, NativeArrayOptions.UninitializedMemory);
            Data = data;
            flags = new UnsafeList<Arena.ArrayFlags>(1, allocator);
            flags.Resize(1);
            flags.Ptr[0] = Arena.ArrayFlags.None;
        }
        /// <summary>
        /// Creates a new matrix of dimension N
        /// </summary>
        /// <param name="N_cols"></param>
        /// <param name="allocator"></param>
        public unsafe fProxyMxN(int M_rows, int N_cols, in Arena arena, bool uninit = false)
        {
            fixed (Arena* arenaPtr = &arena)
                _arenaPtr = arenaPtr;

            M_Rows = M_rows;
            N_Cols = N_cols;
            Length = M_Rows * N_Cols;
            var data = new UnsafeList<fProxy>(Length, _arenaPtr->Allocator, uninit? NativeArrayOptions.UninitializedMemory : NativeArrayOptions.ClearMemory );
            data.Resize(Length, NativeArrayOptions.UninitializedMemory);
            Data = data;
            flags = new UnsafeList<Arena.ArrayFlags>(1, _arenaPtr->Allocator);
            flags.Resize(1);
            flags.Ptr[0] = Arena.ArrayFlags.None;
        }

        /// <summary>
        /// Creates a copy of vector with new allocation
        /// </summary>
        /// <param name="orig"></param>
        public unsafe fProxyMxN(in fProxyMxN orig, Allocator allocator = Allocator.Invalid)
        {
            if (allocator == Allocator.Invalid)
                allocator = orig._arenaPtr->Allocator;

            _arenaPtr = orig._arenaPtr;
            M_Rows = orig.M_Rows;
            N_Cols = orig.N_Cols;
            Length = orig.Length;
            var data = new UnsafeList<fProxy>(Length, allocator, NativeArrayOptions.UninitializedMemory);
            data.Resize(Length, NativeArrayOptions.UninitializedMemory);
            data.CopyFrom(orig.Data);
            Data = data;
            flags = new UnsafeList<Arena.ArrayFlags>(1, allocator);
            flags.Resize(1);
            flags.Ptr[0] = Arena.ArrayFlags.None;
        }

        /// <summary>
        /// Allocates a copy in the arena's persistent buffer
        /// </summary>
        public unsafe fProxyMxN CopyPersistent()
        {
            return _arenaPtr->fProxyMat(in this);
        }

        /// <summary>
        /// Allocates a copy in the arena's temp buffer
        /// </summary>
        public unsafe fProxyMxN TempCopy()
        {
            return _arenaPtr->tempfProxyMat(in this);
        }

        public unsafe bool Equals(fProxyMxN other)
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

        public unsafe bool IsDisposed()
        {
            return (flags.Ptr[0] & Arena.ArrayFlags.isDisposed) != 0 || !Data.IsCreated;
        }

        public void Dispose() {
            Dispose(true);
        }

        private unsafe void Dispose(bool disposing)
        {
            if (!IsDisposed())
            {
                if (disposing)
                {
                    // Dispose manged resources here
                }

                // Dispose unmanged resources here
                if (Length > 0 && float.IsNaN((float) Data[0])) UnityEngine.Debug.LogError("Vector data was NaN. Might be double freeing.");
#if LINALG_DEBUG
                for (int i = 0; i < Length; i++) this[i] = float.NaN;
#endif
                Data.Dispose();
                flags.Ptr[0] = Arena.ArrayFlags.isDisposed; // unset other flags.
                flags.Dispose();
            }
        }

        void IMatrix<fProxy>.CopyTo(IMatrix<fProxy> destination) {
            throw new NotImplementedException();
        }

        void IMatrix<fProxy>.CopyFrom(IMatrix<fProxy> source) {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            // Get the dimensions of the matrix.
            int rows = M_Rows;
            int cols = N_Cols;

            // Determine the maximum width needed for each column.
            int[] colWidths = new int[cols];
            for (int j = 0; j < cols; j++)
            {
                for (int i = 0; i < rows; i++)
                {
                    // Format each number with two decimal places.
                    string cellStr = this[i, j].ToString();
                    if (cellStr.Length > colWidths[j])
                    {
                        colWidths[j] = cellStr.Length;
                    }
                }
            }

            // Use a StringBuilder to accumulate the formatted matrix string.
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < rows; i++)
            {
                sb.Append("[ ");
                for (int j = 0; j < cols; j++)
                {
                    // Format the cell with the determined width.
                    string cellStr = this[i, j].ToString().PadLeft(colWidths[j]);
                    sb.Append(cellStr);

                    // Append a separator if not the last column.
                    if (j < cols - 1)
                    {
                        sb.Append("  ");
                    }
                }
                sb.Append(" ]");

                // Add a newline for each row except the last one.
                if (i < rows - 1)
                {
                    sb.AppendLine();
                }
            }
            return sb.ToString();
        }
    }
}