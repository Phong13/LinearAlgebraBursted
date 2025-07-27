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
        
        public floatN floatVec(int N, bool uninit = false) {

            var vec = new floatN(N, in this, uninit);
            floatVectors.Add(in vec);
            return vec;
        }

        // creates vector with s values
        public floatN floatVec(int N, float s)
        {
            var vec = new floatN(N, in this, true);
            floatVectors.Add(in vec);
            unsafe {
                mathUnsafefloat.setAll(vec.Data.Ptr, N, s);
            }
            return vec;
        }

        public floatN floatVec(float3 v)
        {
            var vec = new floatN(3, in this, true);
            vec[0] = v.x;
            vec[1] = v.y;
            vec[2] = v.z;
            floatVectors.Add(in vec);
            return vec;
        }

        public floatN floatVec(float4 v)
        {
            var vec = new floatN(4, in this, true);
            vec[0] = v.x;
            vec[1] = v.y;
            vec[2] = v.z;
            vec[3] = v.w;
            floatVectors.Add(in vec);
            return vec;
        }


        internal floatN floatVec(in floatN orig)
        {
            var vec = new floatN(in orig);
            floatVectors.Add(in vec);
            return vec;
        }

        public floatN floatVec(float[] orig)
        {
            var vec = floatVec(orig.Length);
            for (int i = 0; i < orig.Length; i++)
            {
                vec[i] = orig[i];
            }

            return vec;
        }

        public floatN tempfloatVec(int N, bool uninit = false)
        {
            var vec = new floatN(N, in this, uninit);
            tempfloatVectors.Add(in vec);
            return vec;
        }

        public floatN tempfloatVec(in floatN orig)
        {
            var vec = new floatN(in orig);
            tempfloatVectors.Add(in vec);
            return vec;
        }

        public floatN tempfloatVec(float3 v)
        {
            var vec = new floatN(3, in this, true);
            vec[0] = v.x;
            vec[1] = v.y;
            vec[2] = v.z;
            tempfloatVectors.Add(in vec);
            return vec;
        }

        public floatN tempfloatVec(float4 v)
        {
            var vec = new floatN(4, in this, true);
            vec[0] = v.x;
            vec[1] = v.y;
            vec[2] = v.z;
            vec[3] = v.w;
            tempfloatVectors.Add(in vec);
            return vec;
        }

        public floatN tempfloatVec(float[] orig)
        {
            var vec = tempfloatVec(orig.Length);
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
            for (int i = 0; i < floatVectors.Length; i++)
            {
                if (floatVectors[i].Data.Ptr == v.Data.Ptr) return true;
            }

            return false;
        }

        /// <summary>
        /// For debugging checks if a vector is in the persistent list.
        /// </summary>
        public unsafe bool DB_isTemp(in floatN v)
        {
            for (int i = 0; i < tempfloatVectors.Length; i++)
            {
                if (tempfloatVectors[i].Data.Ptr == v.Data.Ptr) return true;
            }

            return false;
        }

        #region MATRIX
        public floatMxN floatMat(int dim, bool uninit = false)
        {
            return new floatMxN(dim, dim, in this, uninit);
        }

        public floatMxN floatMat(int M_rows, int N_cols, bool uninit = false)
        {
            var matrix = new floatMxN(M_rows, N_cols, in this, uninit);
            floatMatrices.Add(in matrix);
            return matrix;
        }

        // creates vector with s values
        public floatMxN floatMat(int M_rows, int N_cols, float s)
        {
            var matrix = new floatMxN(M_rows, N_cols, in this, false);
            floatMatrices.Add(in matrix);
            unsafe
            {
                mathUnsafefloat.setAll(matrix.Data.Ptr, matrix.Length, s);
            }
            return matrix;
        }

        public floatMxN floatMat(in floatMxN orig)
        {
            var matrix = new floatMxN(in orig);
            floatMatrices.Add(in matrix);
            return matrix;
        }

        public floatMxN floatMat(in float3x3 orig)
        {
            var m = new floatMxN(3, 3, in this, false);
            m[0, 0] = orig.c0.x; m[0, 1] = orig.c1.x; m[0, 2] = orig.c2.x;
            m[1, 0] = orig.c0.y; m[1, 1] = orig.c1.y; m[1, 2] = orig.c2.y;
            m[2, 0] = orig.c0.z; m[2, 1] = orig.c1.z; m[2, 2] = orig.c2.z;
            floatMatrices.Add(in m);
            return m;
        }

        public floatMxN floatMat(in float4x4 orig)
        {
            var m = new floatMxN(4, 4, in this, false);
            m[0, 0] = orig.c0.x; m[0, 1] = orig.c1.x; m[0, 2] = orig.c2.x; m[0, 3] = orig.c3.x;
            m[1, 0] = orig.c0.y; m[1, 1] = orig.c1.y; m[1, 2] = orig.c2.y; m[1, 3] = orig.c3.y;
            m[2, 0] = orig.c0.z; m[2, 1] = orig.c1.z; m[2, 2] = orig.c2.z; m[2, 3] = orig.c3.z;
            m[3, 0] = orig.c0.w; m[3, 1] = orig.c1.w; m[3, 2] = orig.c2.w; m[3, 3] = orig.c3.w;
            floatMatrices.Add(in m);
            return m;
        }

        public floatMxN floatMat(float[,] orig)
        {
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

        public floatMxN tempfloatMat(int M_rows, int M_cols, bool uninit = false)
        {
            var matrix = new floatMxN(M_rows, M_cols, in this, uninit);
            tempfloatMatrices.Add(in matrix);
            return matrix;
        }

        public floatMxN tempfloatMat(in floatMxN orig)
        {
            var matrix = new floatMxN(orig);
            tempfloatMatrices.Add(in matrix);
            return matrix;
        }

        public floatMxN tempfloatMat(in float3x3 orig)
        {
            var m = new floatMxN(3, 3, in this, false);
            m[0, 0] = orig.c0.x; m[0, 1] = orig.c1.x; m[0, 2] = orig.c2.x;
            m[1, 0] = orig.c0.y; m[1, 1] = orig.c1.y; m[1, 2] = orig.c2.y;
            m[2, 0] = orig.c0.z; m[2, 1] = orig.c1.z; m[2, 2] = orig.c2.z;
            tempfloatMatrices.Add(in m);
            return m;
        }

        public floatMxN tempfloatMat(in float4x4 orig)
        {
            var m = new floatMxN(4, 4, in this, false);
            m[0, 0] = orig.c0.x; m[0, 1] = orig.c1.x; m[0, 2] = orig.c2.x; m[0, 3] = orig.c3.x;
            m[1, 0] = orig.c0.y; m[1, 1] = orig.c1.y; m[1, 2] = orig.c2.y; m[1, 3] = orig.c3.y;
            m[2, 0] = orig.c0.z; m[2, 1] = orig.c1.z; m[2, 2] = orig.c2.z; m[2, 3] = orig.c3.z;
            m[3, 0] = orig.c0.w; m[3, 1] = orig.c1.w; m[3, 2] = orig.c2.w; m[3, 3] = orig.c3.w;
            tempfloatMatrices.Add(in m);
            return m;
        }

        public floatMxN tempfloatMat(float[,] orig)
        {
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
            for (int i = 0; i < floatMatrices.Length; i++)
            {
                if (floatMatrices[i].Data.Ptr == v.Data.Ptr) return true;
            }

            return false;
        }

        /// <summary>
        /// For debugging checks if a vector is in the persistent list.
        /// </summary>
        public unsafe bool DB_isTemp(in floatMxN v)
        {
            for (int i = 0; i < tempfloatMatrices.Length; i++)
            {
                if (tempfloatMatrices[i].Data.Ptr == v.Data.Ptr) return true;
            }

            return false;
        }
        #endregion

    }

}