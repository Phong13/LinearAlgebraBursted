using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using System;

namespace LinearAlgebra
{
    // A m x n matrix
    // m = rows
    // n = cols
    public partial struct fProxyMxN : IDisposable, IUnsafefProxyArray, IMatrix<fProxy> {
        
        public int M_Rows;
        public int N_Cols;

        public UnsafeList<fProxy> Data { get; private set; }

        [NativeDisableUnsafePtrRestriction]
        private unsafe Arena* _arenaPtr;

        public readonly int Length;

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
        }

        public unsafe fProxyMxN Copy()
        {

            return _arenaPtr->fProxyMat(in this);
        }

        public unsafe fProxyMxN TempCopy()
        {
            return _arenaPtr->tempfProxyMat(in this);
        }

        public void Dispose() {
#if LINALG_DEBUG
            for (int i = 0; i < Length; i++) this[i] = float.NaN;
#endif
            Data.Dispose();
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