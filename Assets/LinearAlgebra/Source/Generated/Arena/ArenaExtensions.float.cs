using Unity.Mathematics;

namespace LinearAlgebra
{

    public static partial class ArenaExtensions {

        #region VECTOR
        public static floatN floatIndexZeroVector(this ref Arena arena, int N, bool isTemp)
        {
            floatN vec;
            if (isTemp)
            {
                vec = arena.tempfloatVec(N, true);
            } else
            {
                vec = arena.floatVec(N, true);
            }

            unsafe {
                mathUnsafefloat.setIndexZero(vec.Data.Ptr, N);
                
            }
            return vec;
        }

        public static floatN floatIndexOneVector(this ref Arena arena, int N, bool isTemp)
        {
            floatN vec;
            if (isTemp)
            {
                vec = arena.tempfloatVec(N, true);
            }
            else
            {
                vec = arena.floatVec(N, true);
            }

            unsafe {
                mathUnsafefloat.setIndexOne(vec.Data.Ptr, N);
            }
            return vec;
        }

        // all zero but the index is one
        public static floatN floatBasisVector(this ref Arena arena, int N, int index, bool isTemp)
        {
            floatN vec;
            if (isTemp)
            {
                vec = arena.tempfloatVec(N, true);
            }
            else
            {
                vec = arena.floatVec(N, true);
            }

            if (index < 0 || index >= N)
                throw new System.Exception("BasisVector: Index out of bounds");

            vec[index] = 1f;

            return vec;
        }

        public static floatN floatRandomUnitVector(this ref Arena arena, int N, uint seed = 34215, bool isTemp = true)
        {
            floatN vec;
            if (isTemp)
            {
                vec = arena.tempfloatVec(N, true);
            }
            else
            {
                vec = arena.floatVec(N, true);
            }

            Random random = new Random(seed);

            float sum = 0;
            for (int i = 0; i < vec.N; i++)
            {
                float p = random.NextFloat(-1f, 1f);
                sum += p*p;
                vec[i] = p;
            }

            float scale = 1 / math.sqrt(sum);

            floatOP.mulInpl(vec, scale);

            return vec;
        }

        public static floatN floatRandomVector(this ref Arena arena, int N, float min, float max, uint seed = 34215, bool isTemp = true)
        {
            floatN vec;
            if (isTemp)
            {
                vec = arena.tempfloatVec(N, true);
            }
            else
            {
                vec = arena.floatVec(N, true);
            }

            Random random = new Random(seed);

            for (int i = 0; i < vec.N; i++)
                vec[i] = random.NextFloat(min, max);

            return vec;
        }

        //linspace
        public static floatN floatLinVector(this ref Arena arena, int N, float start, float end, bool isTemp)
        {
            floatN vec;
            if (isTemp)
            {
                vec = arena.tempfloatVec(N, true);
            }
            else
            {
                vec = arena.floatVec(N, true);
            }

            float scale = 1 / (float)(N - 1);
            for(int i = 0; i < N; i++) {
                vec[i] = math.lerp(start, end, i * scale);
            }

            return vec;
        }

        #endregion

        #region MATRIX
        // constructs identity matrix
        public static floatMxN floatIdentityMatrix(this ref Arena arena, int N, bool isTemp)
        {
            floatMxN matrix;
            if (isTemp)
                matrix = arena.tempfloatMat(N, N, 0);
            else
                matrix = arena.floatMat(N, N, 0);

            for (int i = 0; i < N; i++)
                matrix[i, i] = 1;
            

            return matrix;
        }

        // constructs diagonal matrix with scalar s on diagonal
        public static floatMxN floatDiagonalMatrix(this ref Arena arena, int N, float s, bool isTemp)
        {
            floatMxN matrix;
            if (isTemp)
                matrix = arena.tempfloatMat(N, N, 0);
            else
                matrix = arena.floatMat(N, N, 0);


            for (int i = 0; i < N; i++)
                matrix[i, i] = s;

            return matrix;
        }

        // constructs diagonal matrix based on vector
        public static floatMxN floatDiagonalMatrix(this ref Arena arena, in floatN vec, bool isTemp)
        {
            floatMxN matrix;
            if (isTemp)
                matrix = arena.tempfloatMat(vec.N, vec.N, 0);
            else
                matrix = arena.floatMat(vec.N, vec.N, 0);

            for (int i = 0; i < vec.N; i++)
                matrix[i, i] = vec[i];

            return matrix;
        }

        // constructs matrix with indexes that start at 0
        public static floatMxN floatIndexZeroMatrix(this ref Arena arena, int M_rows, int N_cols, bool isTemp)
        {
            floatMxN mat;
            if (isTemp)
                mat = arena.tempfloatMat(M_rows, N_cols, 0);
            else
                mat = arena.floatMat(M_rows, N_cols, 0);

            int len = mat.Length;

            unsafe
            {
                mathUnsafefloat.setIndexZero(mat.Data.Ptr, len);
            }
            
            return mat;
        }

        // constructs matrix with indexes that start at 1
        public static floatMxN floatIndexOneMatrix(this ref Arena arena, int M_rows, int N_cols, bool isTemp)
        {
            floatMxN mat;
            if (isTemp)
                mat = arena.tempfloatMat(M_rows, N_cols, 0);
            else
                mat = arena.floatMat(M_rows, N_cols, 0);

            int len = mat.Length;

            unsafe
            {
                mathUnsafefloat.setIndexOne(mat.Data.Ptr, len);
            }

            return mat;
        }

        // random matrix

        public static floatMxN floatRandomMatrix(this ref Arena arena, int M_rows, int N_cols, uint seed = 121312, bool isTemp = true)
        {
            return floatRandomMatrix(ref arena, M_rows, N_cols, -1, 1, seed, isTemp);
        }

        // constructs diagonal matrix with scalar s on diagonal
        public static floatMxN floatRandomDiagonalMatrix(this ref Arena arena, int N, float min, float max, uint seed = 65792, bool isTemp = true)
        {
            floatMxN mat;
            if (isTemp)
                mat = arena.tempfloatMat(N, N, 0);
            else
                mat = arena.floatMat(N, N, 0);

            Random rand = new Random(seed);

            for (int i = 0; i < N; i++)
                mat[i, i] = rand.NextFloat(min, max);

            return mat;
        }

        public static floatMxN floatRandomMatrix(this ref Arena arena, int M_rows, int N_cols, float min, float max, uint seed = 121312, bool isTemp = true)
        {
            floatMxN mat;
            if (isTemp)
                mat = arena.tempfloatMat(M_rows, N_cols, 0);
            else
                mat = arena.floatMat(M_rows, N_cols, 0);

            Random random = new Random(seed);

            int len = mat.Length;
            for (int i = 0; i < len; i++)
                mat[i] = random.NextFloat(min, max);

            return mat;
        }

        // i and j are axis indexes to rotate
        public static floatMxN floatRotationMatrix(this ref Arena arena, int M, int i, int j, float radians, bool isTemp)
        {
            var matrix = arena.floatIdentityMatrix(M, isTemp);

            if (M < 2)
                throw new System.Exception("RotationMatrix: Matrix must be at least 2x2");

            if(i < 0 || i >= M || j < 0 || j >= M)
                throw new System.Exception("RotationMatrix: Index out of bounds");

            if(i == j) {
                return matrix;
            }

            float c = math.cos(radians);
            float s = math.sin(radians);

            matrix[i, i] = c;
            matrix[j, j] = c;
            matrix[i, j] = -s;
            matrix[j, i] = s;

            return matrix;
        }

        // i and j are axis indexes to swap
        public static floatMxN floatPermutationMatrix(this ref Arena arena, int M, int i, int j, bool isTemp)
        {
            var matrix = arena.floatIdentityMatrix(M, isTemp);

            if (M < 2)
                throw new System.Exception("PermutationMatrix: Matrix must be at least 2x2");

            if (i < 0 || i >= M || j < 0 || j >= M)
                throw new System.Exception("PermutationMatrix: Index out of bounds");

            if (i == j)
            {
                return matrix;
            }

            matrix[i, j] = 1;
            matrix[j, i] = 1;
            matrix[i, i] = 0;
            matrix[j, j] = 0;
            
            return matrix;
        }

        public static floatMxN floatHouseholderMatrix(this ref Arena arena, int M, in floatN v, bool isTemp)
        {
            if(M < 2)
                throw new System.Exception("HouseholderMatrix: Matrix must be at least 2x2");

            // Compute the Householder matrix: H = I - 2 * vvT / (vTv)
            if (v.N != M)
                throw new System.Exception("HouseholderMatrix: Vector length must match matrix dimension.");

            var matrix = arena.floatIdentityMatrix(M, isTemp);

            // Compute the outer product of v
            float vTv = floatOP.dot(v, v);
            
            float scaleFactor = 2 / vTv;
            
            // Rank 1 update
            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    float vvT_element = scaleFactor * v[i] * v[j];
                    matrix[i, j] -= vvT_element;
                }
            }

            return matrix;
        }

        // very ill conditioned matrix, used for testing numerical stability
        public static floatMxN floatHilbertMatrix(this ref Arena arena, int M, bool isTemp)
        {
            if (M < 2)
                throw new System.Exception("HilbertMatrix: Matrix must be at least 2x2");
            floatMxN hilbert;
            if (isTemp)
                hilbert = arena.tempfloatMat(M, M, true);
            else
                hilbert = arena.floatMat(M, true);

            for (int i = 0; i < M; i++) {
                for (int j = 0; j < M; j++) {
                    hilbert[i, j] = (float) 1.0 / (float)(i + j + 1);
                }
            }

            return hilbert;
        }

        #endregion

    }

}