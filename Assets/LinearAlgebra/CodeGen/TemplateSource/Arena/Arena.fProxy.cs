using Unity.Collections.LowLevel.Unsafe;

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

        internal fProxyN fProxyVec(in fProxyN orig)
        {
            var vec = new fProxyN(in orig);
            tempfProxyVectors.Add(in vec);
            return vec;
        }

        internal fProxyN tempfProxyVec(int N, bool uninit = false)
        {
            var vec = new fProxyN(N, in this, uninit);
            tempfProxyVectors.Add(in vec);
            return vec;
        }

        internal fProxyN tempfProxyVec(in fProxyN orig)
        {
            var vec = new fProxyN(in orig);
            tempfProxyVectors.Add(in vec);
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