using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;



namespace LinearAlgebra
{
    public partial struct Arena {

        private UnsafeList<doubleN> doubleVectors;
        private UnsafeList<doubleMxN> doubleMatrices;
        private UnsafeList<doubleN> tempdoubleVectors;
        private UnsafeList<doubleMxN> tempdoubleMatrices;


        #region VECTOR
        
        public unsafe doubleN doubleVec(int N, bool uninit = false) {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = new doubleN(N, in this, uninit);
            vec.flags.Ptr[0] |= ArrayFlags.isPersistent;
            doubleVectors.Add(in vec);
            return vec;
        }

        // creates vector with s values
        public unsafe doubleN doubleVec(int N, float s)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = new doubleN(N, in this, true);
            vec.flags.Ptr[0] |= ArrayFlags.isPersistent;
            doubleVectors.Add(in vec);
            unsafe {
                mathUnsafedouble.setAll(vec.Data.Ptr, N, s);
            }
            return vec;
        }

        public unsafe doubleN doubleVec(double3 v)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = new doubleN(3, in this, true);
            vec.flags.Ptr[0] |= ArrayFlags.isPersistent;
            vec[0] = v.x;
            vec[1] = v.y;
            vec[2] = v.z;
            doubleVectors.Add(in vec);
            return vec;
        }

        public unsafe doubleN doubleVec(double4 v)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = new doubleN(4, in this, true);
            vec.flags.Ptr[0] |= ArrayFlags.isPersistent;
            vec[0] = v.x;
            vec[1] = v.y;
            vec[2] = v.z;
            vec[3] = v.w;
            doubleVectors.Add(in vec);
            return vec;
        }


        internal unsafe doubleN doubleVec(in doubleN orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = new doubleN(in orig);
            vec.flags.Ptr[0] |= ArrayFlags.isPersistent;
            doubleVectors.Add(in vec);
            return vec;
        }

        public unsafe doubleN doubleVec(float[] orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = doubleVec(orig.Length);
            vec.flags.Ptr[0] |= ArrayFlags.isPersistent;
            for (int i = 0; i < orig.Length; i++)
            {
                vec[i] = orig[i];
            }

            return vec;
        }

        public unsafe doubleN tempdoubleVec(int N, bool uninit = false)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = new doubleN(N, in this, uninit);
            vec.flags.Ptr[0] |= ArrayFlags.isTemp;
            tempdoubleVectors.Add(in vec);
            return vec;
        }

        public unsafe doubleN tempdoubleVec(int N, double s)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = new doubleN(N, in this, true);
            tempdoubleVectors.Add(in vec);
            vec.flags.Ptr[0] |= ArrayFlags.isTemp;
            unsafe
            {
                mathUnsafedouble.setAll(vec.Data.Ptr, N, s);
            }
            return vec;
        }

        public unsafe doubleN tempdoubleVec(in doubleN orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = new doubleN(in orig);
            vec.flags.Ptr[0] |= ArrayFlags.isTemp;
            tempdoubleVectors.Add(in vec);
            return vec;
        }

        public unsafe doubleN tempdoubleVec(double3 v)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = new doubleN(3, in this, true);
            vec.flags.Ptr[0] |= ArrayFlags.isTemp;
            vec[0] = v.x;
            vec[1] = v.y;
            vec[2] = v.z;
            tempdoubleVectors.Add(in vec);
            return vec;
        }

        public unsafe doubleN tempdoubleVec(double4 v)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = new doubleN(4, in this, true);
            vec.flags.Ptr[0] |= ArrayFlags.isTemp;
            vec[0] = v.x;
            vec[1] = v.y;
            vec[2] = v.z;
            vec[3] = v.w;
            tempdoubleVectors.Add(in vec);
            return vec;
        }

        public unsafe doubleN tempdoubleVec(float[] orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = tempdoubleVec(orig.Length);
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
        public unsafe bool DB_isPersistant(in doubleN v)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            if ((v.flags.Ptr[0] & ArrayFlags.isPersistent) == 0) UnityEngine.Debug.LogError("Input vector to DB_isPersistant wasn't flagged as a persistent vector.");
            if ((v.flags.Ptr[0] & ArrayFlags.isDisposed) != 0) UnityEngine.Debug.LogError("Input vector to DB_isPersistant was disposed.");
            for (int i = 0; i < doubleVectors.Length; i++)
            {
                if (doubleVectors[i].Data.Ptr == v.Data.Ptr)
                {
                    if ((doubleVectors[i].flags.Ptr[0] & ArrayFlags.isPersistent) == 0) UnityEngine.Debug.LogError("Vector in Persistent array in DB_isPersistent wasn't flagged as a persistent vector.");
                    if ((doubleVectors[i].flags.Ptr[0] & ArrayFlags.isDisposed) != 0) UnityEngine.Debug.LogError("Vector in Persistent array was Disposed");
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// For debugging checks if a vector is in the persistent list.
        /// </summary>
        public unsafe bool DB_isTemp(in doubleN v)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            if ((v.flags.Ptr[0] & ArrayFlags.isTemp) == 0) UnityEngine.Debug.LogError("Input vector to DB_isTemp wasn't flagged as a temp vector.");
            if ((v.flags.Ptr[0] & ArrayFlags.isDisposed) != 0) UnityEngine.Debug.LogError("Input vector to DB_isTemp was disposed.");
            for (int i = 0; i < tempdoubleVectors.Length; i++)
            {
                if (tempdoubleVectors[i].Data.Ptr == v.Data.Ptr)
                {
                    if ((tempdoubleVectors[i].flags.Ptr[0] & ArrayFlags.isTemp) == 0) UnityEngine.Debug.LogError("Vector in Temp array in DB_isTemp wasn't flagged as a temp vector.");
                    if ((tempdoubleVectors[i].flags.Ptr[0] & ArrayFlags.isDisposed) != 0) UnityEngine.Debug.LogError("Vector in Temp array was Disposed");
                    return true;
                }
            }

            return false;
        }

        #region MATRIX
        public unsafe doubleMxN doubleMat(int dim, bool uninit = false)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var matrix = new doubleMxN(dim, dim, in this, uninit);
            matrix.flags.Ptr[0] |= ArrayFlags.isPersistent;
            doubleMatrices.Add(in matrix);
            return matrix;
        }

        public unsafe doubleMxN doubleMat(int M_rows, int N_cols, bool uninit = false)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var matrix = new doubleMxN(M_rows, N_cols, in this, uninit);
            matrix.flags.Ptr[0] |= ArrayFlags.isPersistent;
            doubleMatrices.Add(in matrix);
            return matrix;
        }

        // creates vector with s values
        public unsafe doubleMxN doubleMat(int M_rows, int N_cols, float s)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var matrix = new doubleMxN(M_rows, N_cols, in this, false);
            matrix.flags.Ptr[0] |= ArrayFlags.isPersistent;
            doubleMatrices.Add(in matrix);
            unsafe
            {
                mathUnsafedouble.setAll(matrix.Data.Ptr, matrix.Length, s);
            }
            return matrix;
        }

        public unsafe doubleMxN doubleMat(in doubleMxN orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var matrix = new doubleMxN(in orig);
            matrix.flags.Ptr[0] |= ArrayFlags.isPersistent;
            doubleMatrices.Add(in matrix);
            return matrix;
        }

        public unsafe doubleMxN doubleMat(in double3x3 orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var m = new doubleMxN(3, 3, in this, false);
            m[0, 0] = orig.c0.x; m[0, 1] = orig.c1.x; m[0, 2] = orig.c2.x;
            m[1, 0] = orig.c0.y; m[1, 1] = orig.c1.y; m[1, 2] = orig.c2.y;
            m[2, 0] = orig.c0.z; m[2, 1] = orig.c1.z; m[2, 2] = orig.c2.z;
            m.flags.Ptr[0] |= ArrayFlags.isPersistent;
            doubleMatrices.Add(in m);
            return m;
        }

        public unsafe doubleMxN doubleMat(in double4x4 orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var m = new doubleMxN(4, 4, in this, false);
            m[0, 0] = orig.c0.x; m[0, 1] = orig.c1.x; m[0, 2] = orig.c2.x; m[0, 3] = orig.c3.x;
            m[1, 0] = orig.c0.y; m[1, 1] = orig.c1.y; m[1, 2] = orig.c2.y; m[1, 3] = orig.c3.y;
            m[2, 0] = orig.c0.z; m[2, 1] = orig.c1.z; m[2, 2] = orig.c2.z; m[2, 3] = orig.c3.z;
            m[3, 0] = orig.c0.w; m[3, 1] = orig.c1.w; m[3, 2] = orig.c2.w; m[3, 3] = orig.c3.w;
            m.flags.Ptr[0] |= ArrayFlags.isPersistent;
            doubleMatrices.Add(in m);
            return m;
        }

        public unsafe doubleMxN doubleMat(float[,] orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            doubleMxN m = doubleMat(orig.GetLength(0), orig.GetLength(1));
            for (int i = 0; i < m.M_Rows; i++)
            {
                for (int j = 0; j < m.N_Cols; j++)
                {
                    m[i, j] = orig[i, j];
                }
            }

            return m;
        }

        public unsafe doubleMxN tempdoubleMat(int M_rows, int M_cols, bool uninit = false)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var matrix = new doubleMxN(M_rows, M_cols, in this, uninit);
            matrix.flags.Ptr[0] |= ArrayFlags.isTemp;
            tempdoubleMatrices.Add(in matrix);
            return matrix;
        }

        public unsafe doubleMxN tempdoubleMat(int M_rows, int N_cols, double s)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var matrix = new doubleMxN(M_rows, N_cols, in this, false);
            matrix.flags.Ptr[0] |= ArrayFlags.isTemp;
            tempdoubleMatrices.Add(in matrix);
            unsafe
            {
                mathUnsafedouble.setAll(matrix.Data.Ptr, matrix.Length, s);
            }
            return matrix;
        }

        public unsafe doubleMxN tempdoubleMat(in doubleMxN orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var matrix = new doubleMxN(orig);
            matrix.flags.Ptr[0] |= ArrayFlags.isTemp;
            tempdoubleMatrices.Add(in matrix);
            return matrix;
        }

        public unsafe doubleMxN tempdoubleMat(in double3x3 orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var m = new doubleMxN(3, 3, in this, false);
            m[0, 0] = orig.c0.x; m[0, 1] = orig.c1.x; m[0, 2] = orig.c2.x;
            m[1, 0] = orig.c0.y; m[1, 1] = orig.c1.y; m[1, 2] = orig.c2.y;
            m[2, 0] = orig.c0.z; m[2, 1] = orig.c1.z; m[2, 2] = orig.c2.z;
            m.flags.Ptr[0] |= ArrayFlags.isTemp;
            tempdoubleMatrices.Add(in m);
            return m;
        }

        public unsafe doubleMxN tempdoubleMat(in double4x4 orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var m = new doubleMxN(4, 4, in this, false);
            m[0, 0] = orig.c0.x; m[0, 1] = orig.c1.x; m[0, 2] = orig.c2.x; m[0, 3] = orig.c3.x;
            m[1, 0] = orig.c0.y; m[1, 1] = orig.c1.y; m[1, 2] = orig.c2.y; m[1, 3] = orig.c3.y;
            m[2, 0] = orig.c0.z; m[2, 1] = orig.c1.z; m[2, 2] = orig.c2.z; m[2, 3] = orig.c3.z;
            m[3, 0] = orig.c0.w; m[3, 1] = orig.c1.w; m[3, 2] = orig.c2.w; m[3, 3] = orig.c3.w;
            m.flags.Ptr[0] |= ArrayFlags.isTemp;
            tempdoubleMatrices.Add(in m);
            return m;
        }

        public doubleMxN tempdoubleMat(float[,] orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            doubleMxN m = tempdoubleMat(orig.GetLength(0), orig.GetLength(1));
            for (int i = 0; i < m.M_Rows; i++)
            {
                for (int j = 0; j < m.N_Cols; j++)
                {
                    m[i, j] = orig[i, j];
                }
            }

            return m;
        }

        public unsafe bool DB_isPersistant(in doubleMxN v)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            if ((v.flags.Ptr[0] & ArrayFlags.isPersistent) == 0) UnityEngine.Debug.LogError("Input matrix to DB_isPersistant wasn't flagged as a persistent vector.");
            if ((v.flags.Ptr[0] & ArrayFlags.isDisposed) != 0) UnityEngine.Debug.LogError("Input matrix to DB_isPersistant was disposed.");
            for (int i = 0; i < doubleMatrices.Length; i++)
            {
                if (doubleMatrices[i].Data.Ptr == v.Data.Ptr)
                {
                    if ((doubleMatrices[i].flags.Ptr[0] & ArrayFlags.isPersistent) == 0) UnityEngine.Debug.LogError("Matrix in Persistent array in DB_isPersistent wasn't flagged as a persistent vector.");
                    if ((doubleMatrices[i].flags.Ptr[0] & ArrayFlags.isDisposed) != 0) UnityEngine.Debug.LogError("Matrix in Persistent array was Disposed");
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// For debugging checks if a vector is in the persistent list.
        /// </summary>
        public unsafe bool DB_isTemp(in doubleMxN v)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            if ((v.flags.Ptr[0] & ArrayFlags.isTemp) == 0) UnityEngine.Debug.LogError("Input matrix to DB_isTemp wasn't flagged as a temp vector.");
            if ((v.flags.Ptr[0] & ArrayFlags.isDisposed) != 0) UnityEngine.Debug.LogError("Input matrix to DB_isTemp was disposed.");

            for (int i = 0; i < tempdoubleMatrices.Length; i++)
            {
                if (tempdoubleMatrices[i].Data.Ptr == v.Data.Ptr)
                {
                    if ((tempdoubleMatrices[i].flags.Ptr[0] & ArrayFlags.isTemp) == 0) UnityEngine.Debug.LogError("Matrix in Temp array in DB_isTemp wasn't flagged as a temp vector.");
                    if ((tempdoubleMatrices[i].flags.Ptr[0] & ArrayFlags.isDisposed) != 0) UnityEngine.Debug.LogError("Matrix in Temp array was Disposed");
                    return true;
                }
            }

            return false;
        }

        [System.Diagnostics.Conditional("LINALG_DEBUG")]
        public static unsafe void CheckValid(doubleN v)
        {
#if LINALG_DEBUG
            if ((v.flags.Ptr[0] & ArrayFlags.isDisposed) != 0) throw new System.Exception("Call on disposed vector");
            if (!v.Data.IsCreated) throw new System.Exception("Call on disposed vector");
            if ((v.N > 0 && float.IsNaN((float) v[0]))) throw new System.Exception("Vector data was NaN. This is likely a disposed vector.");
#endif
        }

        [System.Diagnostics.Conditional("LINALG_DEBUG")]
        public static unsafe void CheckValid(doubleMxN v)
        {
#if LINALG_DEBUG
            if ((v.flags.Ptr[0] & ArrayFlags.isDisposed) != 0) throw new System.Exception("Call on disposed vector");
            if (!v.Data.IsCreated) throw new System.Exception("Call on disposed vector");
            if ((v.Length > 0 && float.IsNaN((float)v[0]))) throw new System.Exception("Vector data was NaN. This is likely a disposed vector.");
#endif
        }
        #endregion

    }

}