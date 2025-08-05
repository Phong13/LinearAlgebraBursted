using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using System.Runtime.InteropServices;
using System;

//singularFile//
namespace LinearAlgebra
{
    // Allocation helper
    [StructLayout(LayoutKind.Sequential)]
    public partial struct Arena : System.IDisposable {

        [Flags]
        public enum ArrayFlags : byte
        {
            None = 0,
            isDisposed = 1 << 0, // 0b00000001
            isPersistent = 1 << 1, // 0b00000010
            isTemp = 1 << 2, // 0b00000100
        }

        public int AllocationsCount => 
            //+copyReplaceFill[+]
            fProxyVectors.Length + fProxyMatrices.Length
            //-copyReplaceFill
            +
            //+copyReplaceFill[+]
            iProxyVectors.Length + iProxyMatrices.Length
            //-copyReplaceFill
        ;

        public int TempAllocationsCount => 
            //+copyReplaceFill[+]
            tempfProxyVectors.Length + tempfProxyMatrices.Length
            //-copyReplaceFill
            +
            //+copyReplaceFill[+]
            tempiProxyVectors.Length + tempiProxyMatrices.Length
            //-copyReplaceFill
        ;

        public int AllAllocationsCount => AllocationsCount + TempAllocationsCount;

        public Allocator Allocator;

        bool isDisposed;

        public bool Initialized { get; private set; }

        private UnsafeList<boolN> BoolVectors;
        private UnsafeList<boolMxN> BoolMatrices;
        private UnsafeList<boolN> TempBoolVectors;
        private UnsafeList<boolMxN> TempBoolMatrices;
        
        public Arena(Allocator allocator) {

            Initialized = true;
            Allocator = allocator;
            isDisposed = false;

            //+copyReplace
            fProxyVectors = new UnsafeList<fProxyN>(8, Allocator);
            fProxyMatrices = new UnsafeList<fProxyMxN>(8, Allocator);
            tempfProxyVectors = new UnsafeList<fProxyN>(8, Allocator);
            tempfProxyMatrices = new UnsafeList<fProxyMxN>(8, Allocator);
            //-copyReplace

            //+copyReplace
            iProxyVectors = new UnsafeList<iProxyN>(8, Allocator);
            iProxyMatrices = new UnsafeList<iProxyMxN>(8, Allocator);
            tempiProxyVectors = new UnsafeList<iProxyN>(8, Allocator);
            tempiProxyMatrices = new UnsafeList<iProxyMxN>(8, Allocator);
            //-copyReplace

            BoolVectors = new UnsafeList<boolN>(2, Allocator);
            BoolMatrices = new UnsafeList<boolMxN>(2, Allocator);

            TempBoolVectors = new UnsafeList<boolN>(2, Allocator);
            TempBoolMatrices = new UnsafeList<boolMxN>(2, Allocator);
        }

        public void Clear() {

            //+copyReplace
            for (int i = 0; i < fProxyVectors.Length; i++)
                fProxyVectors[i].Dispose();
            fProxyVectors.Clear();

            for(int i = 0; i < fProxyMatrices.Length; i++)
                fProxyMatrices[i].Dispose();
            fProxyMatrices.Clear();
            //-copyReplace

            //+copyReplace
            for (int i = 0; i < iProxyVectors.Length; i++)
                iProxyVectors[i].Dispose();
            iProxyVectors.Clear();

            for (int i = 0; i < iProxyMatrices.Length; i++)
                iProxyMatrices[i].Dispose();
            iProxyMatrices.Clear();
            //-copyReplace

            for (int i = 0; i < BoolVectors.Length; i++)
                BoolVectors[i].Dispose();
            BoolVectors.Clear();

            for (int i = 0; i < BoolMatrices.Length; i++)
                BoolMatrices[i].Dispose();
            BoolMatrices.Clear();

            ClearTemp();
        }

        /// <summary>
        /// dispose only temporary allocations, produced from operations
        /// </summary>
        public void ClearTemp()
        {
            //+copyReplace
            for (int i = 0; i < tempfProxyVectors.Length; i++)
                tempfProxyVectors[i].Dispose();
            tempfProxyVectors.Clear();

            for (int i = 0; i < tempfProxyMatrices.Length; i++)
                tempfProxyMatrices[i].Dispose();
            tempfProxyMatrices.Clear();
            //-copyReplace

            //+copyReplace
            for (int i = 0; i < tempiProxyVectors.Length; i++)
                tempiProxyVectors[i].Dispose();
            tempiProxyVectors.Clear();

            for (int i = 0; i < tempiProxyMatrices.Length; i++)
                tempiProxyMatrices[i].Dispose();
            tempiProxyMatrices.Clear();
            //-copyReplace

            for (int i = 0; i < TempBoolVectors.Length; i++)
                TempBoolVectors[i].Dispose();
            TempBoolVectors.Clear();

            for (int i = 0; i < TempBoolMatrices.Length; i++)
                TempBoolMatrices[i].Dispose();
            TempBoolMatrices.Clear();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    Clear();
                }

                // Dispose unmanged resources here
                //+copyReplace
                fProxyVectors.Dispose();
                fProxyMatrices.Dispose();
                tempfProxyMatrices.Dispose();
                tempfProxyVectors.Dispose();
                //-copyReplace

                //+copyReplace
                iProxyVectors.Dispose();
                iProxyMatrices.Dispose();
                tempiProxyMatrices.Dispose();
                tempiProxyVectors.Dispose();
                //-copyReplace

                BoolVectors.Dispose();
                BoolMatrices.Dispose();
                TempBoolMatrices.Dispose();
                TempBoolVectors.Dispose();

                Initialized = false;
                Allocator = Allocator.Invalid;
                isDisposed = true;
            }
        }
    }
}