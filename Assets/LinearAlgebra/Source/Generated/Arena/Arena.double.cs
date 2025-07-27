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
        
        public doubleN doubleVec(int N, bool uninit = false) {

            var vec = new doubleN(N, in this, uninit);
            doubleVectors.Add(in vec);
            return vec;
        }

        // creates vector with s values
        public doubleN doubleVec(int N, float s)
        {
            var vec = new doubleN(N, in this, true);
            doubleVectors.Add(in vec);
            unsafe {
                mathUnsafedouble.setAll(vec.Data.Ptr, N, s);
            }
            return vec;
        }

        public doubleN doubleVec(double3 v)
        {
            var vec = new doubleN(3, in this, true);
            vec[0] = v.x;
            vec[1] = v.y;
            vec[2] = v.z;
            doubleVectors.Add(in vec);
            return vec;
        }

        public doubleN doubleVec(double4 v)
        {
            var vec = new doubleN(4, in this, true);
            vec[0] = v.x;
            vec[1] = v.y;
            vec[2] = v.z;
            vec[3] = v.w;
            doubleVectors.Add(in vec);
            return vec;
        }


        internal doubleN doubleVec(in doubleN orig)
        {
            var vec = new doubleN(in orig);
            doubleVectors.Add(in vec);
            return vec;
        }

        public doubleN doubleVec(float[] orig)
        {
            var vec = doubleVec(orig.Length);
            for (int i = 0; i < orig.Length; i++)
            {
                vec[i] = orig[i];
            }

            return vec;
        }

        public doubleN tempdoubleVec(int N, bool uninit = false)
        {
            var vec = new doubleN(N, in this, uninit);
            tempdoubleVectors.Add(in vec);
            return vec;
        }

        public doubleN tempdoubleVec(in doubleN orig)
        {
            var vec = new doubleN(in orig);
            tempdoubleVectors.Add(in vec);
            return vec;
        }

        public doubleN tempdoubleVec(double3 v)
        {
            var vec = new doubleN(3, in this, true);
            vec[0] = v.x;
            vec[1] = v.y;
            vec[2] = v.z;
            tempdoubleVectors.Add(in vec);
            return vec;
        }

        public doubleN tempdoubleVec(double4 v)
        {
            var vec = new doubleN(4, in this, true);
            vec[0] = v.x;
            vec[1] = v.y;
            vec[2] = v.z;
            vec[3] = v.w;
            tempdoubleVectors.Add(in vec);
            return vec;
        }

        public doubleN tempdoubleVec(float[] orig)
        {
            var vec = tempdoubleVec(orig.Length);
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
            for (int i = 0; i < doubleVectors.Length; i++)
            {
                if (doubleVectors[i].Data.Ptr == v.Data.Ptr) return true;
            }

            return false;
        }

        /// <summary>
        /// For debugging checks if a vector is in the persistent list.
        /// </summary>
        public unsafe bool DB_isTemp(in doubleN v)
        {
            for (int i = 0; i < tempdoubleVectors.Length; i++)
            {
                if (tempdoubleVectors[i].Data.Ptr == v.Data.Ptr) return true;
            }

            return false;
        }

        #region MATRIX
        public doubleMxN doubleMat(int dim, bool uninit = false)
        {
            return new doubleMxN(dim, dim, in this, uninit);
        }

        public doubleMxN doubleMat(int M_rows, int N_cols, bool uninit = false)
        {
            var matrix = new doubleMxN(M_rows, N_cols, in this, uninit);
            doubleMatrices.Add(in matrix);
            return matrix;
        }

        // creates vector with s values
        public doubleMxN doubleMat(int M_rows, int N_cols, float s)
        {
            var matrix = new doubleMxN(M_rows, N_cols, in this, false);
            doubleMatrices.Add(in matrix);
            unsafe
            {
                mathUnsafedouble.setAll(matrix.Data.Ptr, matrix.Length, s);
            }
            return matrix;
        }

        public doubleMxN doubleMat(in doubleMxN orig)
        {
            var matrix = new doubleMxN(in orig);
            doubleMatrices.Add(in matrix);
            return matrix;
        }

        public doubleMxN doubleMat(in double3x3 orig)
        {
            var m = new doubleMxN(3, 3, in this, false);
            m[0, 0] = orig.c0.x; m[0, 1] = orig.c1.x; m[0, 2] = orig.c2.x;
            m[1, 0] = orig.c0.y; m[1, 1] = orig.c1.y; m[1, 2] = orig.c2.y;
            m[2, 0] = orig.c0.z; m[2, 1] = orig.c1.z; m[2, 2] = orig.c2.z;
            doubleMatrices.Add(in m);
            return m;
        }

        public doubleMxN doubleMat(in double4x4 orig)
        {
            var m = new doubleMxN(4, 4, in this, false);
            m[0, 0] = orig.c0.x; m[0, 1] = orig.c1.x; m[0, 2] = orig.c2.x; m[0, 3] = orig.c3.x;
            m[1, 0] = orig.c0.y; m[1, 1] = orig.c1.y; m[1, 2] = orig.c2.y; m[1, 3] = orig.c3.y;
            m[2, 0] = orig.c0.z; m[2, 1] = orig.c1.z; m[2, 2] = orig.c2.z; m[2, 3] = orig.c3.z;
            m[3, 0] = orig.c0.w; m[3, 1] = orig.c1.w; m[3, 2] = orig.c2.w; m[3, 3] = orig.c3.w;
            doubleMatrices.Add(in m);
            return m;
        }

        public doubleMxN doubleMat(float[,] orig)
        {
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

        public doubleMxN tempdoubleMat(int M_rows, int M_cols, bool uninit = false)
        {
            var matrix = new doubleMxN(M_rows, M_cols, in this, uninit);
            tempdoubleMatrices.Add(in matrix);
            return matrix;
        }

        public doubleMxN tempdoubleMat(in doubleMxN orig)
        {
            var matrix = new doubleMxN(orig);
            tempdoubleMatrices.Add(in matrix);
            return matrix;
        }

        public doubleMxN tempdoubleMat(in double3x3 orig)
        {
            var m = new doubleMxN(3, 3, in this, false);
            m[0, 0] = orig.c0.x; m[0, 1] = orig.c1.x; m[0, 2] = orig.c2.x;
            m[1, 0] = orig.c0.y; m[1, 1] = orig.c1.y; m[1, 2] = orig.c2.y;
            m[2, 0] = orig.c0.z; m[2, 1] = orig.c1.z; m[2, 2] = orig.c2.z;
            tempdoubleMatrices.Add(in m);
            return m;
        }

        public doubleMxN tempdoubleMat(in double4x4 orig)
        {
            var m = new doubleMxN(4, 4, in this, false);
            m[0, 0] = orig.c0.x; m[0, 1] = orig.c1.x; m[0, 2] = orig.c2.x; m[0, 3] = orig.c3.x;
            m[1, 0] = orig.c0.y; m[1, 1] = orig.c1.y; m[1, 2] = orig.c2.y; m[1, 3] = orig.c3.y;
            m[2, 0] = orig.c0.z; m[2, 1] = orig.c1.z; m[2, 2] = orig.c2.z; m[2, 3] = orig.c3.z;
            m[3, 0] = orig.c0.w; m[3, 1] = orig.c1.w; m[3, 2] = orig.c2.w; m[3, 3] = orig.c3.w;
            tempdoubleMatrices.Add(in m);
            return m;
        }

        public doubleMxN tempdoubleMat(float[,] orig)
        {
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
            for (int i = 0; i < doubleMatrices.Length; i++)
            {
                if (doubleMatrices[i].Data.Ptr == v.Data.Ptr) return true;
            }

            return false;
        }

        /// <summary>
        /// For debugging checks if a vector is in the persistent list.
        /// </summary>
        public unsafe bool DB_isTemp(in doubleMxN v)
        {
            for (int i = 0; i < tempdoubleMatrices.Length; i++)
            {
                if (tempdoubleMatrices[i].Data.Ptr == v.Data.Ptr) return true;
            }

            return false;
        }
        #endregion

    }

}