using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;



namespace LinearAlgebra
{
    public partial struct Arena {

        private UnsafeList<floatN> floatVectors;
        private UnsafeList<floatMxN> floatMatrices;
        private UnsafeList<floatN> tempfloatVectors;
        private UnsafeList<floatMxN> tempfloatMatrices;


        #region VECTOR
        
        public unsafe floatN floatVec(int N, bool uninit = false) {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = new floatN(N, in this, uninit);
            vec.flags.Ptr[0] |= ArrayFlags.isPersistent;
            floatVectors.Add(in vec);
            return vec;
        }

        // creates vector with s values
        public unsafe floatN floatVec(int N, float s)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = new floatN(N, in this, true);
            vec.flags.Ptr[0] |= ArrayFlags.isPersistent;
            floatVectors.Add(in vec);
            unsafe {
                mathUnsafefloat.setAll(vec.Data.Ptr, N, s);
            }
            return vec;
        }

        public unsafe floatN floatVec(float3 v)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = new floatN(3, in this, true);
            vec.flags.Ptr[0] |= ArrayFlags.isPersistent;
            vec[0] = v.x;
            vec[1] = v.y;
            vec[2] = v.z;
            floatVectors.Add(in vec);
            return vec;
        }

        public unsafe floatN floatVec(float4 v)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = new floatN(4, in this, true);
            vec.flags.Ptr[0] |= ArrayFlags.isPersistent;
            vec[0] = v.x;
            vec[1] = v.y;
            vec[2] = v.z;
            vec[3] = v.w;
            floatVectors.Add(in vec);
            return vec;
        }


        internal unsafe floatN floatVec(in floatN orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = new floatN(in orig);
            vec.flags.Ptr[0] |= ArrayFlags.isPersistent;
            floatVectors.Add(in vec);
            return vec;
        }

        public unsafe floatN floatVec(float[] orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = floatVec(orig.Length);
            vec.flags.Ptr[0] |= ArrayFlags.isPersistent;
            for (int i = 0; i < orig.Length; i++)
            {
                vec[i] = orig[i];
            }

            return vec;
        }

        public unsafe floatN floatVec(double[] orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = floatVec(orig.Length);
            vec.flags.Ptr[0] |= ArrayFlags.isPersistent;
            for (int i = 0; i < orig.Length; i++)
            {
                vec[i] = (float) orig[i];
            }

            return vec;
        }


        public unsafe floatN tempfloatVec(int N, bool uninit = false)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = new floatN(N, in this, uninit);
            vec.flags.Ptr[0] |= ArrayFlags.isTemp;
            tempfloatVectors.Add(in vec);
            return vec;
        }

        public unsafe floatN tempfloatVec(int N, float s)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = new floatN(N, in this, true);
            tempfloatVectors.Add(in vec);
            vec.flags.Ptr[0] |= ArrayFlags.isTemp;
            unsafe
            {
                mathUnsafefloat.setAll(vec.Data.Ptr, N, s);
            }
            return vec;
        }

        public unsafe floatN tempfloatVec(in floatN orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = new floatN(in orig);
            vec.flags.Ptr[0] |= ArrayFlags.isTemp;
            tempfloatVectors.Add(in vec);
            return vec;
        }

        public unsafe floatN tempfloatVec(float3 v)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = new floatN(3, in this, true);
            vec.flags.Ptr[0] |= ArrayFlags.isTemp;
            vec[0] = v.x;
            vec[1] = v.y;
            vec[2] = v.z;
            tempfloatVectors.Add(in vec);
            return vec;
        }

        public unsafe floatN tempfloatVec(float4 v)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = new floatN(4, in this, true);
            vec.flags.Ptr[0] |= ArrayFlags.isTemp;
            vec[0] = v.x;
            vec[1] = v.y;
            vec[2] = v.z;
            vec[3] = v.w;
            tempfloatVectors.Add(in vec);
            return vec;
        }

        public unsafe floatN tempfloatVec(float[] orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = tempfloatVec(orig.Length);
            vec.flags.Ptr[0] |= ArrayFlags.isTemp;
            for (int i = 0; i < orig.Length; i++)
            {
                vec[i] = orig[i];
            }
            return vec;
        }
        #endregion

        /// <summary>
        /// For debugging checks if a vector is in the persistent list.
        /// </summary>
        public unsafe bool DB_isPersistant(in floatN v)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            if ((v.flags.Ptr[0] & ArrayFlags.isPersistent) == 0) UnityEngine.Debug.LogError("Input vector to DB_isPersistant wasn't flagged as a persistent vector.");
            if ((v.flags.Ptr[0] & ArrayFlags.isDisposed) != 0) UnityEngine.Debug.LogError("Input vector to DB_isPersistant was disposed.");
            for (int i = 0; i < floatVectors.Length; i++)
            {
                if (floatVectors[i].Data.Ptr == v.Data.Ptr)
                {
                    if ((floatVectors[i].flags.Ptr[0] & ArrayFlags.isPersistent) == 0) UnityEngine.Debug.LogError("Vector in Persistent array in DB_isPersistent wasn't flagged as a persistent vector.");
                    if ((floatVectors[i].flags.Ptr[0] & ArrayFlags.isDisposed) != 0) UnityEngine.Debug.LogError("Vector in Persistent array was Disposed");
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// For debugging checks if a vector is in the persistent list.
        /// </summary>
        public unsafe bool DB_isTemp(in floatN v)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            if ((v.flags.Ptr[0] & ArrayFlags.isTemp) == 0) UnityEngine.Debug.LogError("Input vector to DB_isTemp wasn't flagged as a temp vector.");
            if ((v.flags.Ptr[0] & ArrayFlags.isDisposed) != 0) UnityEngine.Debug.LogError("Input vector to DB_isTemp was disposed.");
            for (int i = 0; i < tempfloatVectors.Length; i++)
            {
                if (tempfloatVectors[i].Data.Ptr == v.Data.Ptr)
                {
                    if ((tempfloatVectors[i].flags.Ptr[0] & ArrayFlags.isTemp) == 0) UnityEngine.Debug.LogError("Vector in Temp array in DB_isTemp wasn't flagged as a temp vector.");
                    if ((tempfloatVectors[i].flags.Ptr[0] & ArrayFlags.isDisposed) != 0) UnityEngine.Debug.LogError("Vector in Temp array was Disposed");
                    return true;
                }
            }

            return false;
        }

        #region MATRIX
        public unsafe floatMxN floatMat(int dim, bool uninit = false)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var matrix = new floatMxN(dim, dim, in this, uninit);
            matrix.flags.Ptr[0] |= ArrayFlags.isPersistent;
            floatMatrices.Add(in matrix);
            return matrix;
        }

        public unsafe floatMxN floatMat(int M_rows, int N_cols, bool uninit = false)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var matrix = new floatMxN(M_rows, N_cols, in this, uninit);
            matrix.flags.Ptr[0] |= ArrayFlags.isPersistent;
            floatMatrices.Add(in matrix);
            return matrix;
        }

        // creates vector with s values
        public unsafe floatMxN floatMat(int M_rows, int N_cols, float s)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var matrix = new floatMxN(M_rows, N_cols, in this, false);
            matrix.flags.Ptr[0] |= ArrayFlags.isPersistent;
            floatMatrices.Add(in matrix);
            unsafe
            {
                mathUnsafefloat.setAll(matrix.Data.Ptr, matrix.Length, s);
            }
            return matrix;
        }

        public unsafe floatMxN floatMat(in floatMxN orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var matrix = new floatMxN(in orig);
            matrix.flags.Ptr[0] |= ArrayFlags.isPersistent;
            floatMatrices.Add(in matrix);
            return matrix;
        }

        public unsafe floatMxN floatMat(in float3x3 orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var m = new floatMxN(3, 3, in this, false);
            m[0, 0] = orig.c0.x; m[0, 1] = orig.c1.x; m[0, 2] = orig.c2.x;
            m[1, 0] = orig.c0.y; m[1, 1] = orig.c1.y; m[1, 2] = orig.c2.y;
            m[2, 0] = orig.c0.z; m[2, 1] = orig.c1.z; m[2, 2] = orig.c2.z;
            m.flags.Ptr[0] |= ArrayFlags.isPersistent;
            floatMatrices.Add(in m);
            return m;
        }

        public unsafe floatMxN floatMat(in float4x4 orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var m = new floatMxN(4, 4, in this, false);
            m[0, 0] = orig.c0.x; m[0, 1] = orig.c1.x; m[0, 2] = orig.c2.x; m[0, 3] = orig.c3.x;
            m[1, 0] = orig.c0.y; m[1, 1] = orig.c1.y; m[1, 2] = orig.c2.y; m[1, 3] = orig.c3.y;
            m[2, 0] = orig.c0.z; m[2, 1] = orig.c1.z; m[2, 2] = orig.c2.z; m[2, 3] = orig.c3.z;
            m[3, 0] = orig.c0.w; m[3, 1] = orig.c1.w; m[3, 2] = orig.c2.w; m[3, 3] = orig.c3.w;
            m.flags.Ptr[0] |= ArrayFlags.isPersistent;
            floatMatrices.Add(in m);
            return m;
        }

        public unsafe floatMxN floatMat(float[,] orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            floatMxN m = floatMat(orig.GetLength(0), orig.GetLength(1));
            for (int i = 0; i < m.M_Rows; i++)
            {
                for (int j = 0; j < m.N_Cols; j++)
                {
                    m[i, j] = orig[i, j];
                }
            }

            return m;
        }

        public unsafe floatMxN floatMat(double[,] orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            floatMxN m = floatMat(orig.GetLength(0), orig.GetLength(1));
            for (int i = 0; i < m.M_Rows; i++)
            {
                for (int j = 0; j < m.N_Cols; j++)
                {
                    m[i, j] = (float) orig[i, j];
                }
            }

            return m;
        }

        public unsafe floatMxN tempfloatMat(int M_rows, int M_cols, bool uninit = false)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var matrix = new floatMxN(M_rows, M_cols, in this, uninit);
            matrix.flags.Ptr[0] |= ArrayFlags.isTemp;
            tempfloatMatrices.Add(in matrix);
            return matrix;
        }

        public unsafe floatMxN tempfloatMat(int M_rows, int N_cols, float s)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var matrix = new floatMxN(M_rows, N_cols, in this, false);
            matrix.flags.Ptr[0] |= ArrayFlags.isTemp;
            tempfloatMatrices.Add(in matrix);
            unsafe
            {
                mathUnsafefloat.setAll(matrix.Data.Ptr, matrix.Length, s);
            }
            return matrix;
        }

        public unsafe floatMxN tempfloatMat(in floatMxN orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var matrix = new floatMxN(orig);
            matrix.flags.Ptr[0] |= ArrayFlags.isTemp;
            tempfloatMatrices.Add(in matrix);
            return matrix;
        }

        public unsafe floatMxN tempfloatMat(in float3x3 orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var m = new floatMxN(3, 3, in this, false);
            m[0, 0] = orig.c0.x; m[0, 1] = orig.c1.x; m[0, 2] = orig.c2.x;
            m[1, 0] = orig.c0.y; m[1, 1] = orig.c1.y; m[1, 2] = orig.c2.y;
            m[2, 0] = orig.c0.z; m[2, 1] = orig.c1.z; m[2, 2] = orig.c2.z;
            m.flags.Ptr[0] |= ArrayFlags.isTemp;
            tempfloatMatrices.Add(in m);
            return m;
        }

        public unsafe floatMxN tempfloatMat(in float4x4 orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var m = new floatMxN(4, 4, in this, false);
            m[0, 0] = orig.c0.x; m[0, 1] = orig.c1.x; m[0, 2] = orig.c2.x; m[0, 3] = orig.c3.x;
            m[1, 0] = orig.c0.y; m[1, 1] = orig.c1.y; m[1, 2] = orig.c2.y; m[1, 3] = orig.c3.y;
            m[2, 0] = orig.c0.z; m[2, 1] = orig.c1.z; m[2, 2] = orig.c2.z; m[2, 3] = orig.c3.z;
            m[3, 0] = orig.c0.w; m[3, 1] = orig.c1.w; m[3, 2] = orig.c2.w; m[3, 3] = orig.c3.w;
            m.flags.Ptr[0] |= ArrayFlags.isTemp;
            tempfloatMatrices.Add(in m);
            return m;
        }

        public floatMxN tempfloatMat(float[,] orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            floatMxN m = tempfloatMat(orig.GetLength(0), orig.GetLength(1));
            for (int i = 0; i < m.M_Rows; i++)
            {
                for (int j = 0; j < m.N_Cols; j++)
                {
                    m[i, j] = orig[i, j];
                }
            }

            return m;
        }

        public unsafe bool DB_isPersistant(in floatMxN v)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            if ((v.flags.Ptr[0] & ArrayFlags.isPersistent) == 0) UnityEngine.Debug.LogError("Input matrix to DB_isPersistant wasn't flagged as a persistent vector.");
            if ((v.flags.Ptr[0] & ArrayFlags.isDisposed) != 0) UnityEngine.Debug.LogError("Input matrix to DB_isPersistant was disposed.");
            for (int i = 0; i < floatMatrices.Length; i++)
            {
                if (floatMatrices[i].Data.Ptr == v.Data.Ptr)
                {
                    if ((floatMatrices[i].flags.Ptr[0] & ArrayFlags.isPersistent) == 0) UnityEngine.Debug.LogError("Matrix in Persistent array in DB_isPersistent wasn't flagged as a persistent vector.");
                    if ((floatMatrices[i].flags.Ptr[0] & ArrayFlags.isDisposed) != 0) UnityEngine.Debug.LogError("Matrix in Persistent array was Disposed");
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// For debugging checks if a vector is in the persistent list.
        /// </summary>
        public unsafe bool DB_isTemp(in floatMxN v)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            if ((v.flags.Ptr[0] & ArrayFlags.isTemp) == 0) UnityEngine.Debug.LogError("Input matrix to DB_isTemp wasn't flagged as a temp vector.");
            if ((v.flags.Ptr[0] & ArrayFlags.isDisposed) != 0) UnityEngine.Debug.LogError("Input matrix to DB_isTemp was disposed.");

            for (int i = 0; i < tempfloatMatrices.Length; i++)
            {
                if (tempfloatMatrices[i].Data.Ptr == v.Data.Ptr)
                {
                    if ((tempfloatMatrices[i].flags.Ptr[0] & ArrayFlags.isTemp) == 0) UnityEngine.Debug.LogError("Matrix in Temp array in DB_isTemp wasn't flagged as a temp vector.");
                    if ((tempfloatMatrices[i].flags.Ptr[0] & ArrayFlags.isDisposed) != 0) UnityEngine.Debug.LogError("Matrix in Temp array was Disposed");
                    return true;
                }
            }

            return false;
        }

        [System.Diagnostics.Conditional("LINALG_DEBUG")]
        public static unsafe void CheckValid(floatN v)
        {
#if LINALG_DEBUG
            if ((v.flags.Ptr[0] & ArrayFlags.isDisposed) != 0) throw new System.Exception("Call on disposed vector");
            if (!v.Data.IsCreated) throw new System.Exception("Call on disposed vector");
            if ((v.N > 0 && float.IsNaN((float) v[0]))) UnityEngine.Debug.LogError("Vector data was NaN. This is likely a disposed vector.");
            bool t = v.IsTemp();
            bool p = v.IsPersistent();
            if (t && p) throw new System.Exception("Vector shouldn't be persistent and temp");
            if (!t && !p) throw new System.Exception("Vector must be temp or persistent");
#endif
        }

        [System.Diagnostics.Conditional("LINALG_DEBUG")]
        public static unsafe void CheckValid(floatMxN v)
        {
#if LINALG_DEBUG
            if ((v.flags.Ptr[0] & ArrayFlags.isDisposed) != 0) throw new System.Exception("Call on disposed matrix");
            if (!v.Data.IsCreated) throw new System.Exception("Call on disposed matrix");
            if ((v.Length > 0 && float.IsNaN((float)v[0]))) UnityEngine.Debug.LogError("Matrix data was NaN. This is likely a disposed vector.");
            bool t = v.IsTemp();
            bool p = v.IsPersistent();
            if (t && p) throw new System.Exception("Matrix shouldn't be persistent and temp");
            if (!t && !p) throw new System.Exception("Matrix must be temp or persistent");
#endif
        }
        #endregion

    }

}