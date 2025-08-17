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
            
            floatVectors.Length + floatMatrices.Length
            +
            doubleVectors.Length + doubleMatrices.Length
            
            +
            
            intVectors.Length + intMatrices.Length
            +
            shortVectors.Length + shortMatrices.Length
            +
            longVectors.Length + longMatrices.Length
            
        ;

        public int TempAllocationsCount => 
            
            tempfloatVectors.Length + tempfloatMatrices.Length
            +
            tempdoubleVectors.Length + tempdoubleMatrices.Length
            
            +
            
            tempintVectors.Length + tempintMatrices.Length
            +
            tempshortVectors.Length + tempshortMatrices.Length
            +
            templongVectors.Length + templongMatrices.Length
            
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

            
            floatVectors = new UnsafeList<floatN>(8, Allocator);
            floatMatrices = new UnsafeList<floatMxN>(8, Allocator);
            tempfloatVectors = new UnsafeList<floatN>(8, Allocator);
            tempfloatMatrices = new UnsafeList<floatMxN>(8, Allocator);
            
            doubleVectors = new UnsafeList<doubleN>(8, Allocator);
            doubleMatrices = new UnsafeList<doubleMxN>(8, Allocator);
            tempdoubleVectors = new UnsafeList<doubleN>(8, Allocator);
            tempdoubleMatrices = new UnsafeList<doubleMxN>(8, Allocator);
            

            
            intVectors = new UnsafeList<intN>(8, Allocator);
            intMatrices = new UnsafeList<intMxN>(8, Allocator);
            tempintVectors = new UnsafeList<intN>(8, Allocator);
            tempintMatrices = new UnsafeList<intMxN>(8, Allocator);
            
            shortVectors = new UnsafeList<shortN>(8, Allocator);
            shortMatrices = new UnsafeList<shortMxN>(8, Allocator);
            tempshortVectors = new UnsafeList<shortN>(8, Allocator);
            tempshortMatrices = new UnsafeList<shortMxN>(8, Allocator);
            
            longVectors = new UnsafeList<longN>(8, Allocator);
            longMatrices = new UnsafeList<longMxN>(8, Allocator);
            templongVectors = new UnsafeList<longN>(8, Allocator);
            templongMatrices = new UnsafeList<longMxN>(8, Allocator);
            

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
            
            for (int i = 0; i < floatVectors.Length; i++)
                floatVectors[i].Dispose();
            floatVectors.Clear();

            for(int i = 0; i < floatMatrices.Length; i++)
                floatMatrices[i].Dispose();
            floatMatrices.Clear();
            
            for (int i = 0; i < doubleVectors.Length; i++)
                doubleVectors[i].Dispose();
            doubleVectors.Clear();

            for(int i = 0; i < doubleMatrices.Length; i++)
                doubleMatrices[i].Dispose();
            doubleMatrices.Clear();
            

            
            for (int i = 0; i < intVectors.Length; i++)
                intVectors[i].Dispose();
            intVectors.Clear();

            for (int i = 0; i < intMatrices.Length; i++)
                intMatrices[i].Dispose();
            intMatrices.Clear();
            
            for (int i = 0; i < shortVectors.Length; i++)
                shortVectors[i].Dispose();
            shortVectors.Clear();

            for (int i = 0; i < shortMatrices.Length; i++)
                shortMatrices[i].Dispose();
            shortMatrices.Clear();
            
            for (int i = 0; i < longVectors.Length; i++)
                longVectors[i].Dispose();
            longVectors.Clear();

            for (int i = 0; i < longMatrices.Length; i++)
                longMatrices[i].Dispose();
            longMatrices.Clear();
            

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
            
            for (int i = 0; i < tempfloatVectors.Length; i++)
                tempfloatVectors[i].Dispose();
            tempfloatVectors.Clear();

            for (int i = 0; i < tempfloatMatrices.Length; i++)
                tempfloatMatrices[i].Dispose();
            tempfloatMatrices.Clear();
            
            for (int i = 0; i < tempdoubleVectors.Length; i++)
                tempdoubleVectors[i].Dispose();
            tempdoubleVectors.Clear();

            for (int i = 0; i < tempdoubleMatrices.Length; i++)
                tempdoubleMatrices[i].Dispose();
            tempdoubleMatrices.Clear();
            

            
            for (int i = 0; i < tempintVectors.Length; i++)
                tempintVectors[i].Dispose();
            tempintVectors.Clear();

            for (int i = 0; i < tempintMatrices.Length; i++)
                tempintMatrices[i].Dispose();
            tempintMatrices.Clear();
            
            for (int i = 0; i < tempshortVectors.Length; i++)
                tempshortVectors[i].Dispose();
            tempshortVectors.Clear();

            for (int i = 0; i < tempshortMatrices.Length; i++)
                tempshortMatrices[i].Dispose();
            tempshortMatrices.Clear();
            
            for (int i = 0; i < templongVectors.Length; i++)
                templongVectors[i].Dispose();
            templongVectors.Clear();

            for (int i = 0; i < templongMatrices.Length; i++)
                templongMatrices[i].Dispose();
            templongMatrices.Clear();
            

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
            
            {
                NativeHashSet<floatN> allVecs = new NativeHashSet<floatN>(tempfloatVectors.Length + floatVectors.Length, Allocator);
                for (int srcIdx = 0; srcIdx < tempfloatVectors.Length; srcIdx++)
                {
                    floatN v = tempfloatVectors[srcIdx];
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

                for (int srcIdx = 0; srcIdx < floatVectors.Length; srcIdx++)
                {
                    floatN v = floatVectors[srcIdx];
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
                NativeHashSet<floatMxN> allMats = new NativeHashSet<floatMxN>(tempfloatMatrices.Length + floatMatrices.Length, Allocator);
                for (int srcIdx = 0; srcIdx < tempfloatMatrices.Length; srcIdx++)
                {
                    floatMxN v = tempfloatMatrices[srcIdx];
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

                for (int srcIdx = 0; srcIdx < floatMatrices.Length; srcIdx++)
                {
                    floatMxN v = floatMatrices[srcIdx];
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
            
            {
                NativeHashSet<doubleN> allVecs = new NativeHashSet<doubleN>(tempdoubleVectors.Length + doubleVectors.Length, Allocator);
                for (int srcIdx = 0; srcIdx < tempdoubleVectors.Length; srcIdx++)
                {
                    doubleN v = tempdoubleVectors[srcIdx];
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

                for (int srcIdx = 0; srcIdx < doubleVectors.Length; srcIdx++)
                {
                    doubleN v = doubleVectors[srcIdx];
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
                NativeHashSet<doubleMxN> allMats = new NativeHashSet<doubleMxN>(tempdoubleMatrices.Length + doubleMatrices.Length, Allocator);
                for (int srcIdx = 0; srcIdx < tempdoubleMatrices.Length; srcIdx++)
                {
                    doubleMxN v = tempdoubleMatrices[srcIdx];
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

                for (int srcIdx = 0; srcIdx < doubleMatrices.Length; srcIdx++)
                {
                    doubleMxN v = doubleMatrices[srcIdx];
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
                
                floatVectors.Dispose();
                floatMatrices.Dispose();
                tempfloatMatrices.Dispose();
                tempfloatVectors.Dispose();
                
                doubleVectors.Dispose();
                doubleMatrices.Dispose();
                tempdoubleMatrices.Dispose();
                tempdoubleVectors.Dispose();
                

                
                intVectors.Dispose();
                intMatrices.Dispose();
                tempintMatrices.Dispose();
                tempintVectors.Dispose();
                
                shortVectors.Dispose();
                shortMatrices.Dispose();
                tempshortMatrices.Dispose();
                tempshortVectors.Dispose();
                
                longVectors.Dispose();
                longMatrices.Dispose();
                templongMatrices.Dispose();
                templongVectors.Dispose();
                

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