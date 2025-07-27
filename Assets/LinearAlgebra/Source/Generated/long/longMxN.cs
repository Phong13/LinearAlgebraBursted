using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using System;

namespace LinearAlgebra
{

    // A m x n matrix
    // m = rows
    // n = cols
    public partial struct longMxN : IDisposable, IUnsafelongArray {
        
        public int M_Rows;
        public int N_Cols;

        public UnsafeList<long> Data { get; private set; }

        [NativeDisableUnsafePtrRestriction]
        private unsafe Arena* _arenaPtr;

        public readonly int Length;

        public bool IsSquare => M_Rows == N_Cols;

        public unsafe longMxN(int M_rows, int N_cols, Allocator allocator, bool uninit = false)
        {
            _arenaPtr = null;

            M_Rows = M_rows;
            N_Cols = N_cols;
            Length = M_Rows * N_Cols;
            var data = new UnsafeList<long>(Length, allocator, uninit ? NativeArrayOptions.UninitializedMemory : NativeArrayOptions.ClearMemory);
            data.Resize(Length, NativeArrayOptions.UninitializedMemory);
            Data = data;
        }
        /// <summary>
        /// Creates a new matrix of dimension N
        /// </summary>
        /// <param name="N_cols"></param>
        /// <param name="allocator"></param>
        public unsafe longMxN(int M_rows, int N_cols, in Arena arena, bool uninit = false)
        {
            fixed (Arena* arenaPtr = &arena)
                _arenaPtr = arenaPtr;

            M_Rows = M_rows;
            N_Cols = N_cols;
            Length = M_Rows * N_Cols;
            var data = new UnsafeList<long>(Length, _arenaPtr->Allocator, uninit? NativeArrayOptions.UninitializedMemory : NativeArrayOptions.ClearMemory );
            data.Resize(Length, NativeArrayOptions.UninitializedMemory);
            Data = data;
        }

        /// <summary>
        /// Creates a copy of vector with new allocation
        /// </summary>
        /// <param name="orig"></param>
        public unsafe longMxN(in longMxN orig, Allocator allocator = Allocator.Invalid)
        {
            if (allocator == Allocator.Invalid)
                allocator = orig._arenaPtr->Allocator;

            _arenaPtr = orig._arenaPtr;
            M_Rows = orig.M_Rows;
            N_Cols = orig.N_Cols;
            Length = orig.Length;
            var data = new UnsafeList<long>(Length, allocator, NativeArrayOptions.UninitializedMemory);
            data.Resize(Length, NativeArrayOptions.UninitializedMemory);
            data.CopyFrom(orig.Data);
            Data = data;
        }

        public unsafe longMxN Copy()
        {

            return _arenaPtr->longMat(in this);
        }

        public unsafe longMxN TempCopy()
        {
            return _arenaPtr->templongMat(in this);
        }

        public void Dispose() {

            Data.Dispose();
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