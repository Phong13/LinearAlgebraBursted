using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

//+deleteThis
using LinearAlgebra.mathProxies;
//-deleteThis

namespace LinearAlgebra
{
    public partial struct Arena {

        private UnsafeList<fProxyN> fProxyVectors;
        private UnsafeList<fProxyMxN> fProxyMatrices;
        private UnsafeList<fProxyN> tempfProxyVectors;
        private UnsafeList<fProxyMxN> tempfProxyMatrices;


        #region VECTOR
        
        public unsafe fProxyN fProxyVec(int N, bool uninit = false) {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = new fProxyN(N, in this, uninit);
            vec.flags.Ptr[0] |= ArrayFlags.isPersistent;
            fProxyVectors.Add(in vec);
            return vec;
        }

        // creates vector with s values
        public unsafe fProxyN fProxyVec(int N, float s)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = new fProxyN(N, in this, true);
            vec.flags.Ptr[0] |= ArrayFlags.isPersistent;
            fProxyVectors.Add(in vec);
            unsafe {
                mathUnsafefProxy.setAll(vec.Data.Ptr, N, s);
            }
            return vec;
        }

        public unsafe fProxyN fProxyVec(fProxy3 v)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = new fProxyN(3, in this, true);
            vec.flags.Ptr[0] |= ArrayFlags.isPersistent;
            vec[0] = v.x;
            vec[1] = v.y;
            vec[2] = v.z;
            fProxyVectors.Add(in vec);
            return vec;
        }

        public unsafe fProxyN fProxyVec(fProxy4 v)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = new fProxyN(4, in this, true);
            vec.flags.Ptr[0] |= ArrayFlags.isPersistent;
            vec[0] = v.x;
            vec[1] = v.y;
            vec[2] = v.z;
            vec[3] = v.w;
            fProxyVectors.Add(in vec);
            return vec;
        }


        internal unsafe fProxyN fProxyVec(in fProxyN orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = new fProxyN(in orig);
            vec.flags.Ptr[0] |= ArrayFlags.isPersistent;
            fProxyVectors.Add(in vec);
            return vec;
        }

        public unsafe fProxyN fProxyVec(float[] orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = fProxyVec(orig.Length);
            vec.flags.Ptr[0] |= ArrayFlags.isPersistent;
            for (int i = 0; i < orig.Length; i++)
            {
                vec[i] = orig[i];
            }

            return vec;
        }

        public unsafe fProxyN tempfProxyVec(int N, bool uninit = false)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = new fProxyN(N, in this, uninit);
            vec.flags.Ptr[0] |= ArrayFlags.isTemp;
            tempfProxyVectors.Add(in vec);
            return vec;
        }

        public unsafe fProxyN tempfProxyVec(int N, fProxy s)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = new fProxyN(N, in this, true);
            tempfProxyVectors.Add(in vec);
            vec.flags.Ptr[0] |= ArrayFlags.isTemp;
            unsafe
            {
                mathUnsafefProxy.setAll(vec.Data.Ptr, N, s);
            }
            return vec;
        }

        public unsafe fProxyN tempfProxyVec(in fProxyN orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = new fProxyN(in orig);
            vec.flags.Ptr[0] |= ArrayFlags.isTemp;
            tempfProxyVectors.Add(in vec);
            return vec;
        }

        public unsafe fProxyN tempfProxyVec(fProxy3 v)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = new fProxyN(3, in this, true);
            vec.flags.Ptr[0] |= ArrayFlags.isTemp;
            vec[0] = v.x;
            vec[1] = v.y;
            vec[2] = v.z;
            tempfProxyVectors.Add(in vec);
            return vec;
        }

        public unsafe fProxyN tempfProxyVec(fProxy4 v)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = new fProxyN(4, in this, true);
            vec.flags.Ptr[0] |= ArrayFlags.isTemp;
            vec[0] = v.x;
            vec[1] = v.y;
            vec[2] = v.z;
            vec[3] = v.w;
            tempfProxyVectors.Add(in vec);
            return vec;
        }

        public unsafe fProxyN tempfProxyVec(float[] orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var vec = tempfProxyVec(orig.Length);
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
        public unsafe bool DB_isPersistant(in fProxyN v)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            if ((v.flags.Ptr[0] & ArrayFlags.isPersistent) == 0) UnityEngine.Debug.LogError("Input vector to DB_isPersistant wasn't flagged as a persistent vector.");
            if ((v.flags.Ptr[0] & ArrayFlags.isDisposed) != 0) UnityEngine.Debug.LogError("Input vector to DB_isPersistant was disposed.");
            for (int i = 0; i < fProxyVectors.Length; i++)
            {
                if (fProxyVectors[i].Data.Ptr == v.Data.Ptr)
                {
                    if ((fProxyVectors[i].flags.Ptr[0] & ArrayFlags.isPersistent) == 0) UnityEngine.Debug.LogError("Vector in Persistent array in DB_isPersistent wasn't flagged as a persistent vector.");
                    if ((fProxyVectors[i].flags.Ptr[0] & ArrayFlags.isDisposed) != 0) UnityEngine.Debug.LogError("Vector in Persistent array was Disposed");
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// For debugging checks if a vector is in the persistent list.
        /// </summary>
        public unsafe bool DB_isTemp(in fProxyN v)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            if ((v.flags.Ptr[0] & ArrayFlags.isTemp) == 0) UnityEngine.Debug.LogError("Input vector to DB_isTemp wasn't flagged as a temp vector.");
            if ((v.flags.Ptr[0] & ArrayFlags.isDisposed) != 0) UnityEngine.Debug.LogError("Input vector to DB_isTemp was disposed.");
            for (int i = 0; i < tempfProxyVectors.Length; i++)
            {
                if (tempfProxyVectors[i].Data.Ptr == v.Data.Ptr)
                {
                    if ((tempfProxyVectors[i].flags.Ptr[0] & ArrayFlags.isTemp) == 0) UnityEngine.Debug.LogError("Vector in Temp array in DB_isTemp wasn't flagged as a temp vector.");
                    if ((tempfProxyVectors[i].flags.Ptr[0] & ArrayFlags.isDisposed) != 0) UnityEngine.Debug.LogError("Vector in Temp array was Disposed");
                    return true;
                }
            }

            return false;
        }

        #region MATRIX
        public unsafe fProxyMxN fProxyMat(int dim, bool uninit = false)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var matrix = new fProxyMxN(dim, dim, in this, uninit);
            matrix.flags.Ptr[0] |= ArrayFlags.isPersistent;
            fProxyMatrices.Add(in matrix);
            return matrix;
        }

        public unsafe fProxyMxN fProxyMat(int M_rows, int N_cols, bool uninit = false)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var matrix = new fProxyMxN(M_rows, N_cols, in this, uninit);
            matrix.flags.Ptr[0] |= ArrayFlags.isPersistent;
            fProxyMatrices.Add(in matrix);
            return matrix;
        }

        // creates vector with s values
        public unsafe fProxyMxN fProxyMat(int M_rows, int N_cols, float s)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var matrix = new fProxyMxN(M_rows, N_cols, in this, false);
            matrix.flags.Ptr[0] |= ArrayFlags.isPersistent;
            fProxyMatrices.Add(in matrix);
            unsafe
            {
                mathUnsafefProxy.setAll(matrix.Data.Ptr, matrix.Length, s);
            }
            return matrix;
        }

        public unsafe fProxyMxN fProxyMat(in fProxyMxN orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var matrix = new fProxyMxN(in orig);
            matrix.flags.Ptr[0] |= ArrayFlags.isPersistent;
            fProxyMatrices.Add(in matrix);
            return matrix;
        }

        public unsafe fProxyMxN fProxyMat(in fProxy3x3 orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var m = new fProxyMxN(3, 3, in this, false);
            m[0, 0] = orig.c0.x; m[0, 1] = orig.c1.x; m[0, 2] = orig.c2.x;
            m[1, 0] = orig.c0.y; m[1, 1] = orig.c1.y; m[1, 2] = orig.c2.y;
            m[2, 0] = orig.c0.z; m[2, 1] = orig.c1.z; m[2, 2] = orig.c2.z;
            m.flags.Ptr[0] |= ArrayFlags.isPersistent;
            fProxyMatrices.Add(in m);
            return m;
        }

        public unsafe fProxyMxN fProxyMat(in fProxy4x4 orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var m = new fProxyMxN(4, 4, in this, false);
            m[0, 0] = orig.c0.x; m[0, 1] = orig.c1.x; m[0, 2] = orig.c2.x; m[0, 3] = orig.c3.x;
            m[1, 0] = orig.c0.y; m[1, 1] = orig.c1.y; m[1, 2] = orig.c2.y; m[1, 3] = orig.c3.y;
            m[2, 0] = orig.c0.z; m[2, 1] = orig.c1.z; m[2, 2] = orig.c2.z; m[2, 3] = orig.c3.z;
            m[3, 0] = orig.c0.w; m[3, 1] = orig.c1.w; m[3, 2] = orig.c2.w; m[3, 3] = orig.c3.w;
            m.flags.Ptr[0] |= ArrayFlags.isPersistent;
            fProxyMatrices.Add(in m);
            return m;
        }

        public unsafe fProxyMxN fProxyMat(float[,] orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            fProxyMxN m = fProxyMat(orig.GetLength(0), orig.GetLength(1));
            for (int i = 0; i < m.M_Rows; i++)
            {
                for (int j = 0; j < m.N_Cols; j++)
                {
                    m[i, j] = orig[i, j];
                }
            }

            return m;
        }

        public unsafe fProxyMxN tempfProxyMat(int M_rows, int M_cols, bool uninit = false)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var matrix = new fProxyMxN(M_rows, M_cols, in this, uninit);
            matrix.flags.Ptr[0] |= ArrayFlags.isTemp;
            tempfProxyMatrices.Add(in matrix);
            return matrix;
        }

        public unsafe fProxyMxN tempfProxyMat(int M_rows, int N_cols, fProxy s)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var matrix = new fProxyMxN(M_rows, N_cols, in this, false);
            matrix.flags.Ptr[0] |= ArrayFlags.isTemp;
            tempfProxyMatrices.Add(in matrix);
            unsafe
            {
                mathUnsafefProxy.setAll(matrix.Data.Ptr, matrix.Length, s);
            }
            return matrix;
        }

        public unsafe fProxyMxN tempfProxyMat(in fProxyMxN orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var matrix = new fProxyMxN(orig);
            matrix.flags.Ptr[0] |= ArrayFlags.isTemp;
            tempfProxyMatrices.Add(in matrix);
            return matrix;
        }

        public unsafe fProxyMxN tempfProxyMat(in fProxy3x3 orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var m = new fProxyMxN(3, 3, in this, false);
            m[0, 0] = orig.c0.x; m[0, 1] = orig.c1.x; m[0, 2] = orig.c2.x;
            m[1, 0] = orig.c0.y; m[1, 1] = orig.c1.y; m[1, 2] = orig.c2.y;
            m[2, 0] = orig.c0.z; m[2, 1] = orig.c1.z; m[2, 2] = orig.c2.z;
            m.flags.Ptr[0] |= ArrayFlags.isTemp;
            tempfProxyMatrices.Add(in m);
            return m;
        }

        public unsafe fProxyMxN tempfProxyMat(in fProxy4x4 orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            var m = new fProxyMxN(4, 4, in this, false);
            m[0, 0] = orig.c0.x; m[0, 1] = orig.c1.x; m[0, 2] = orig.c2.x; m[0, 3] = orig.c3.x;
            m[1, 0] = orig.c0.y; m[1, 1] = orig.c1.y; m[1, 2] = orig.c2.y; m[1, 3] = orig.c3.y;
            m[2, 0] = orig.c0.z; m[2, 1] = orig.c1.z; m[2, 2] = orig.c2.z; m[2, 3] = orig.c3.z;
            m[3, 0] = orig.c0.w; m[3, 1] = orig.c1.w; m[3, 2] = orig.c2.w; m[3, 3] = orig.c3.w;
            m.flags.Ptr[0] |= ArrayFlags.isTemp;
            tempfProxyMatrices.Add(in m);
            return m;
        }

        public fProxyMxN tempfProxyMat(float[,] orig)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            fProxyMxN m = tempfProxyMat(orig.GetLength(0), orig.GetLength(1));
            for (int i = 0; i < m.M_Rows; i++)
            {
                for (int j = 0; j < m.N_Cols; j++)
                {
                    m[i, j] = orig[i, j];
                }
            }

            return m;
        }

        public unsafe bool DB_isPersistant(in fProxyMxN v)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            if ((v.flags.Ptr[0] & ArrayFlags.isPersistent) == 0) UnityEngine.Debug.LogError("Input matrix to DB_isPersistant wasn't flagged as a persistent vector.");
            if ((v.flags.Ptr[0] & ArrayFlags.isDisposed) != 0) UnityEngine.Debug.LogError("Input matrix to DB_isPersistant was disposed.");
            for (int i = 0; i < fProxyMatrices.Length; i++)
            {
                if (fProxyMatrices[i].Data.Ptr == v.Data.Ptr)
                {
                    if ((fProxyMatrices[i].flags.Ptr[0] & ArrayFlags.isPersistent) == 0) UnityEngine.Debug.LogError("Matrix in Persistent array in DB_isPersistent wasn't flagged as a persistent vector.");
                    if ((fProxyMatrices[i].flags.Ptr[0] & ArrayFlags.isDisposed) != 0) UnityEngine.Debug.LogError("Matrix in Persistent array was Disposed");
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// For debugging checks if a vector is in the persistent list.
        /// </summary>
        public unsafe bool DB_isTemp(in fProxyMxN v)
        {
            if (isDisposed) throw new System.Exception("Call on disposed Arena");
            if ((v.flags.Ptr[0] & ArrayFlags.isTemp) == 0) UnityEngine.Debug.LogError("Input matrix to DB_isTemp wasn't flagged as a temp vector.");
            if ((v.flags.Ptr[0] & ArrayFlags.isDisposed) != 0) UnityEngine.Debug.LogError("Input matrix to DB_isTemp was disposed.");

            for (int i = 0; i < tempfProxyMatrices.Length; i++)
            {
                if (tempfProxyMatrices[i].Data.Ptr == v.Data.Ptr)
                {
                    if ((tempfProxyMatrices[i].flags.Ptr[0] & ArrayFlags.isTemp) == 0) UnityEngine.Debug.LogError("Matrix in Temp array in DB_isTemp wasn't flagged as a temp vector.");
                    if ((tempfProxyMatrices[i].flags.Ptr[0] & ArrayFlags.isDisposed) != 0) UnityEngine.Debug.LogError("Matrix in Temp array was Disposed");
                    return true;
                }
            }

            return false;
        }

        [System.Diagnostics.Conditional("LINALG_DEBUG")]
        public static unsafe void CheckValid(fProxyN v)
        {
#if LINALG_DEBUG
            if ((v.flags.Ptr[0] & ArrayFlags.isDisposed) != 0) throw new System.Exception("Call on disposed vector");
            if (!v.Data.IsCreated) throw new System.Exception("Call on disposed vector");
            if ((v.N > 0 && float.IsNaN((float) v[0]))) throw new System.Exception("Vector data was NaN. This is likely a disposed vector.");
#endif
        }

        [System.Diagnostics.Conditional("LINALG_DEBUG")]
        public static unsafe void CheckValid(fProxyMxN v)
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