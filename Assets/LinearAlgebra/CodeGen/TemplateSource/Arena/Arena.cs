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

        private int expectedPersistentAllocations;

        public Arena(Allocator allocator) {

            Initialized = true;
            Allocator = allocator;
            isDisposed = false;

            expectedPersistentAllocations = -1;

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
            ClearPersistent();
            ClearTemp();
        }

        public void ClearPersistent()
        {
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

        /// <summary>
        /// Call this once all persistent matricies and vectors have been allocated
        /// </summary>
        public void SetExpectedPersistentAllocationCount()
        {
            expectedPersistentAllocations = AllocationsCount;
        }

        /// <summary>
        /// Call this to check allocations.
        /// </summary>
        public bool CheckPersistentAllocationCount()
        {
            if (expectedPersistentAllocations != AllocationsCount)
            {
                UnityEngine.Debug.LogError($"Persistent allocations {AllocationsCount} did not match expected {expectedPersistentAllocations}");
                return false;
            }

            return true;
        }

        public unsafe bool CheckIntegrity()
        {
            // Check for duplicates and disposed
            bool good = true;
            //+copyReplace
            {
                NativeHashSet<fProxyN> allVecs = new NativeHashSet<fProxyN>(tempfProxyVectors.Length + fProxyVectors.Length, Allocator);
                for (int srcIdx = 0; srcIdx < tempfProxyVectors.Length; srcIdx++)
                {
                    fProxyN v = tempfProxyVectors[srcIdx];
                    if ((v.flags.Ptr[0] & ArrayFlags.isTemp) == 0)
                    {
                        UnityEngine.Debug.LogError("Arena temp vector was not flagged temp");
                        good = false;
                    }
                    if ((v.flags.Ptr[0] & ArrayFlags.isDisposed) != 0)
                    {
                        UnityEngine.Debug.LogError("Arena had disposed vector");
                        good = false;
                    }

                    if (allVecs.Contains(v))
                    {
                        UnityEngine.Debug.LogError("Arena had duplicate vector");
                        good = false;
                    }

                    allVecs.Add(v);
                }

                for (int srcIdx = 0; srcIdx < fProxyVectors.Length; srcIdx++)
                {
                    fProxyN v = fProxyVectors[srcIdx];
                    if ((v.flags.Ptr[0] & ArrayFlags.isPersistent) == 0)
                    {
                        UnityEngine.Debug.LogError("Arena temp vector was not flagged peristent");
                        good = false;
                    }
                    if ((v.flags.Ptr[0] & ArrayFlags.isDisposed) != 0)
                    {
                        UnityEngine.Debug.LogError("Arena had disposed vector");
                        good = false;
                    }

                    if (allVecs.Contains(v))
                    {
                        UnityEngine.Debug.LogError("Arena had duplicate vector");
                        good = false;
                    }

                    allVecs.Add(v);
                }

                allVecs.Dispose();
            }

            {
                NativeHashSet<fProxyMxN> allMats = new NativeHashSet<fProxyMxN>(tempfProxyMatrices.Length + fProxyMatrices.Length, Allocator);
                for (int srcIdx = 0; srcIdx < tempfProxyMatrices.Length; srcIdx++)
                {
                    fProxyMxN v = tempfProxyMatrices[srcIdx];
                    if ((v.flags.Ptr[0] & ArrayFlags.isTemp) == 0)
                    {
                        UnityEngine.Debug.LogError("Arena temp vector was not flagged temp");
                        good = false;
                    }
                    if ((v.flags.Ptr[0] & ArrayFlags.isDisposed) != 0)
                    {
                        UnityEngine.Debug.LogError("Arena had disposed vector");
                        good = false;
                    }

                    if (allMats.Contains(v))
                    {
                        UnityEngine.Debug.LogError("Arena had duplicate vector");
                        good = false;
                    }

                    allMats.Add(v);
                }

                for (int srcIdx = 0; srcIdx < fProxyMatrices.Length; srcIdx++)
                {
                    fProxyMxN v = fProxyMatrices[srcIdx];
                    if ((v.flags.Ptr[0] & ArrayFlags.isPersistent) == 0)
                    {
                        UnityEngine.Debug.LogError("Arena temp vector was not flagged peristent");
                        good = false;
                    }
                    if ((v.flags.Ptr[0] & ArrayFlags.isDisposed) != 0)
                    {
                        UnityEngine.Debug.LogError("Arena had disposed vector");
                        good = false;
                    }

                    if (allMats.Contains(v))
                    {
                        UnityEngine.Debug.LogError("Arena had duplicate vector");
                        good = false;
                    }

                    allMats.Add(v);
                }

                allMats.Dispose();
            }
            //-copyReplace

            return good;
        }

        public bool IsInitializedAndNotDisposed()
        {
            return !isDisposed && Initialized;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
#if LINALG_DEBUG
                CheckIntegrity();
#endif

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