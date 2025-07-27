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
        
        public fProxyN fProxyVec(int N, bool uninit = false) {

            var vec = new fProxyN(N, in this, uninit);
            fProxyVectors.Add(in vec);
            return vec;
        }

        // creates vector with s values
        public fProxyN fProxyVec(int N, float s)
        {
            var vec = new fProxyN(N, in this, true);
            fProxyVectors.Add(in vec);
            unsafe {
                mathUnsafefProxy.setAll(vec.Data.Ptr, N, s);
            }
            return vec;
        }

        public fProxyN fProxyVec(fProxy3 v)
        {
            var vec = new fProxyN(3, in this, true);
            vec[0] = v.x;
            vec[1] = v.y;
            vec[2] = v.z;
            fProxyVectors.Add(in vec);
            return vec;
        }

        public fProxyN fProxyVec(fProxy4 v)
        {
            var vec = new fProxyN(4, in this, true);
            vec[0] = v.x;
            vec[1] = v.y;
            vec[2] = v.z;
            vec[3] = v.w;
            fProxyVectors.Add(in vec);
            return vec;
        }


        internal fProxyN fProxyVec(in fProxyN orig)
        {
            var vec = new fProxyN(in orig);
            fProxyVectors.Add(in vec);
            return vec;
        }

        public fProxyN fProxyVec(float[] orig)
        {
            var vec = fProxyVec(orig.Length);
            for (int i = 0; i < orig.Length; i++)
            {
                vec[i] = orig[i];
            }

            return vec;
        }

        public fProxyN tempfProxyVec(int N, bool uninit = false)
        {
            var vec = new fProxyN(N, in this, uninit);
            tempfProxyVectors.Add(in vec);
            return vec;
        }

        public fProxyN tempfProxyVec(in fProxyN orig)
        {
            var vec = new fProxyN(in orig);
            tempfProxyVectors.Add(in vec);
            return vec;
        }

        public fProxyN tempfProxyVec(fProxy3 v)
        {
            var vec = new fProxyN(3, in this, true);
            vec[0] = v.x;
            vec[1] = v.y;
            vec[2] = v.z;
            tempfProxyVectors.Add(in vec);
            return vec;
        }

        public fProxyN tempfProxyVec(fProxy4 v)
        {
            var vec = new fProxyN(4, in this, true);
            vec[0] = v.x;
            vec[1] = v.y;
            vec[2] = v.z;
            vec[3] = v.w;
            tempfProxyVectors.Add(in vec);
            return vec;
        }

        public fProxyN tempfProxyVec(float[] orig)
        {
            var vec = tempfProxyVec(orig.Length);
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
            for (int i = 0; i < fProxyVectors.Length; i++)
            {
                if (fProxyVectors[i].Data.Ptr == v.Data.Ptr) return true;
            }

            return false;
        }

        /// <summary>
        /// For debugging checks if a vector is in the persistent list.
        /// </summary>
        public unsafe bool DB_isTemp(in fProxyN v)
        {
            for (int i = 0; i < tempfProxyVectors.Length; i++)
            {
                if (tempfProxyVectors[i].Data.Ptr == v.Data.Ptr) return true;
            }

            return false;
        }

        #region MATRIX
        public fProxyMxN fProxyMat(int dim, bool uninit = false)
        {
            return new fProxyMxN(dim, dim, in this, uninit);
        }

        public fProxyMxN fProxyMat(int M_rows, int N_cols, bool uninit = false)
        {
            var matrix = new fProxyMxN(M_rows, N_cols, in this, uninit);
            fProxyMatrices.Add(in matrix);
            return matrix;
        }

        // creates vector with s values
        public fProxyMxN fProxyMat(int M_rows, int N_cols, float s)
        {
            var matrix = new fProxyMxN(M_rows, N_cols, in this, false);
            fProxyMatrices.Add(in matrix);
            unsafe
            {
                mathUnsafefProxy.setAll(matrix.Data.Ptr, matrix.Length, s);
            }
            return matrix;
        }

        public fProxyMxN fProxyMat(in fProxyMxN orig)
        {
            var matrix = new fProxyMxN(in orig);
            fProxyMatrices.Add(in matrix);
            return matrix;
        }

        public fProxyMxN fProxyMat(in fProxy3x3 orig)
        {
            var m = new fProxyMxN(3, 3, in this, false);
            m[0, 0] = orig.c0.x; m[0, 1] = orig.c1.x; m[0, 2] = orig.c2.x;
            m[1, 0] = orig.c0.y; m[1, 1] = orig.c1.y; m[1, 2] = orig.c2.y;
            m[2, 0] = orig.c0.z; m[2, 1] = orig.c1.z; m[2, 2] = orig.c2.z;
            fProxyMatrices.Add(in m);
            return m;
        }

        public fProxyMxN fProxyMat(in fProxy4x4 orig)
        {
            var m = new fProxyMxN(4, 4, in this, false);
            m[0, 0] = orig.c0.x; m[0, 1] = orig.c1.x; m[0, 2] = orig.c2.x; m[0, 3] = orig.c3.x;
            m[1, 0] = orig.c0.y; m[1, 1] = orig.c1.y; m[1, 2] = orig.c2.y; m[1, 3] = orig.c3.y;
            m[2, 0] = orig.c0.z; m[2, 1] = orig.c1.z; m[2, 2] = orig.c2.z; m[2, 3] = orig.c3.z;
            m[3, 0] = orig.c0.w; m[3, 1] = orig.c1.w; m[3, 2] = orig.c2.w; m[3, 3] = orig.c3.w;
            fProxyMatrices.Add(in m);
            return m;
        }

        public fProxyMxN fProxyMat(float[,] orig)
        {
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

        internal fProxyMxN tempfProxyMat(int M_rows, int M_cols, bool uninit = false)
        {
            var matrix = new fProxyMxN(M_rows, M_cols, in this, uninit);
            tempfProxyMatrices.Add(in matrix);
            return matrix;
        }

        internal fProxyMxN tempfProxyMat(in fProxyMxN orig)
        {
            var matrix = new fProxyMxN(orig);
            tempfProxyMatrices.Add(in matrix);
            return matrix;
        }

        public fProxyMxN tempfProxyMat(in fProxy3x3 orig)
        {
            var m = new fProxyMxN(3, 3, in this, false);
            m[0, 0] = orig.c0.x; m[0, 1] = orig.c1.x; m[0, 2] = orig.c2.x;
            m[1, 0] = orig.c0.y; m[1, 1] = orig.c1.y; m[1, 2] = orig.c2.y;
            m[2, 0] = orig.c0.z; m[2, 1] = orig.c1.z; m[2, 2] = orig.c2.z;
            tempfProxyMatrices.Add(in m);
            return m;
        }

        public fProxyMxN tempfProxyMat(in fProxy4x4 orig)
        {
            var m = new fProxyMxN(4, 4, in this, false);
            m[0, 0] = orig.c0.x; m[0, 1] = orig.c1.x; m[0, 2] = orig.c2.x; m[0, 3] = orig.c3.x;
            m[1, 0] = orig.c0.y; m[1, 1] = orig.c1.y; m[1, 2] = orig.c2.y; m[1, 3] = orig.c3.y;
            m[2, 0] = orig.c0.z; m[2, 1] = orig.c1.z; m[2, 2] = orig.c2.z; m[2, 3] = orig.c3.z;
            m[3, 0] = orig.c0.w; m[3, 1] = orig.c1.w; m[3, 2] = orig.c2.w; m[3, 3] = orig.c3.w;
            tempfProxyMatrices.Add(in m);
            return m;
        }

        public fProxyMxN tempfProxyMat(float[,] orig)
        {
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
            for (int i = 0; i < fProxyMatrices.Length; i++)
            {
                if (fProxyMatrices[i].Data.Ptr == v.Data.Ptr) return true;
            }

            return false;
        }

        /// <summary>
        /// For debugging checks if a vector is in the persistent list.
        /// </summary>
        public unsafe bool DB_isTemp(in fProxyMxN v)
        {
            for (int i = 0; i < tempfProxyMatrices.Length; i++)
            {
                if (tempfProxyMatrices[i].Data.Ptr == v.Data.Ptr) return true;
            }

            return false;
        }
        #endregion

    }

}