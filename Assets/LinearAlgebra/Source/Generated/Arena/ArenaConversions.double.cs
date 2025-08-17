using Unity.Mathematics;



namespace LinearAlgebra
{
    // have to test it all, have to do other conversions too
    public static partial class ArenaExtensions {

        #region CONVERSIONS_FROM_MATH
        public static doubleN Convert(this ref Arena arena, in double2 mathVec, bool isTemp)
        {
            doubleN vec;
            if (isTemp)
            {
                vec = arena.tempdoubleVec(2, true);
            } else
            {
                vec = arena.doubleVec(2, true);
            }

            vec[0] = mathVec.x;
            vec[1] = mathVec.y;

            return vec;
        }

        public static doubleN Convert(this ref Arena arena, in double3 mathVec, bool isTemp)
        {
            doubleN vec;
            if (isTemp)
            {
                vec = arena.tempdoubleVec(3, true);
            }
            else
            {
                vec = arena.doubleVec(3, true);
            }

            vec[0] = mathVec.x;
            vec[1] = mathVec.y;
            vec[2] = mathVec.z;

            return vec;
        }

        public static doubleN Convert(this ref Arena arena, in double4 mathVec, bool isTemp)
        {
            doubleN vec;
            if (isTemp)
            {
                vec = arena.tempdoubleVec(4, true);
            }
            else
            {
                vec = arena.doubleVec(4, true);
            }

            vec[0] = mathVec.x;
            vec[1] = mathVec.y;
            vec[2] = mathVec.z;
            vec[3] = mathVec.w;

            return vec;
        }

        public static doubleMxN Convert(this ref Arena arena, in double2x2 mathMat, bool isTemp)
        {
            doubleMxN mat; 
            if (isTemp)
            {
                mat = arena.tempdoubleMat(2, 2, true);
            }
            else
            {
                mat = arena.doubleMat(2, 2, true);
            }

            mat[0, 0] = mathMat.c0.x;
            mat[1, 0] = mathMat.c0.y;
            mat[0, 1] = mathMat.c1.x;
            mat[1, 1] = mathMat.c1.y;

            return mat;
        }

        public static doubleMxN Convert(this ref Arena arena, in double3x3 mathMat, bool isTemp)
        {
            doubleMxN mat;
            if (isTemp)
            {
                mat = arena.tempdoubleMat(3, 3, true);
            }
            else
            {
                mat = arena.doubleMat(3, 3, true);
            }

            mat[0, 0] = mathMat.c0.x;
            mat[1, 0] = mathMat.c0.y;
            mat[2, 0] = mathMat.c0.z;
            mat[0, 1] = mathMat.c1.x;
            mat[1, 1] = mathMat.c1.y;
            mat[2, 1] = mathMat.c1.z;
            mat[0, 2] = mathMat.c2.x;
            mat[1, 2] = mathMat.c2.y;
            mat[2, 2] = mathMat.c2.z;

            return mat;
        }

        public static doubleMxN Convert(this ref Arena arena, in double4x4 mathMat, bool isTemp)
        {
            doubleMxN mat;
            if (isTemp)
            {
                mat = arena.tempdoubleMat(4, 4, true);
            }
            else
            {
                mat = arena.doubleMat(4, 4, true);
            }

            mat[0, 0] = mathMat.c0.x;
            mat[1, 0] = mathMat.c0.y;
            mat[2, 0] = mathMat.c0.z;
            mat[3, 0] = mathMat.c0.w;
            mat[0, 1] = mathMat.c1.x;
            mat[1, 1] = mathMat.c1.y;
            mat[2, 1] = mathMat.c1.z;
            mat[3, 1] = mathMat.c1.w;
            mat[0, 2] = mathMat.c2.x;
            mat[1, 2] = mathMat.c2.y;
            mat[2, 2] = mathMat.c2.z;
            mat[3, 2] = mathMat.c2.w;
            mat[0, 3] = mathMat.c3.x;
            mat[1, 3] = mathMat.c3.y;
            mat[2, 3] = mathMat.c3.z;
            mat[3, 3] = mathMat.c3.w;

            return mat;
        }

        #endregion

        #region CONVERSIONS_TO_MATH
        public static double2 ToLen2(this ref Arena arena, in doubleN mathVec) {
            Arena.CheckValid(mathVec);
            double2 vec = new double2();

            vec.x = mathVec[0];
            vec.y = mathVec[1];

            return vec;
        }

        public static double3 ToLen3(this doubleN mathVec)
        {
            Arena.CheckValid(mathVec);
            double3 vec = new double3();

            vec.x = mathVec[0];
            vec.y = mathVec[1];
            vec.z = mathVec[2];

            return vec;
        }

        public static double4 ToLen4(this doubleN mathVec)
        {
            Arena.CheckValid(mathVec);
            double4 vec = new double4();

            vec.x = mathVec[0];
            vec.y = mathVec[1];
            vec.z = mathVec[2];
            vec.w = mathVec[3];

            return vec;
        }

        public static double2x2 To2x2(this doubleMxN mathVec)
        {
            Arena.CheckValid(mathVec);
            double2x2 vec = new double2x2();

            vec.c0.x = mathVec[0, 0]; vec.c1.x = mathVec[0, 1];
            vec.c0.y = mathVec[1, 0]; vec.c1.y = mathVec[1, 1];

            return vec;
        }

        public static double3x3 To3x3(this doubleMxN mathVec)
        {
            Arena.CheckValid(mathVec);
            double3x3 vec = new double3x3();

            vec.c0.x = mathVec[0, 0]; vec.c1.x = mathVec[0, 1]; vec.c2.x = mathVec[0, 2];
            vec.c0.y = mathVec[1, 0]; vec.c1.y = mathVec[1, 1]; vec.c2.y = mathVec[1, 2];
            vec.c0.z = mathVec[2, 0]; vec.c1.z = mathVec[2, 1]; vec.c2.z = mathVec[2, 2];

            return vec;
        }

        public static double4x4 To4x4(this doubleMxN mathVec)
        {
            Arena.CheckValid(mathVec);
            double4x4 vec = new double4x4();

            vec.c0.x = mathVec[0, 0]; vec.c1.x = mathVec[0, 1]; vec.c2.x = mathVec[0, 2]; vec.c3.x = mathVec[0, 3];
            vec.c0.y = mathVec[1, 0]; vec.c1.y = mathVec[1, 1]; vec.c2.y = mathVec[1, 2]; vec.c3.y = mathVec[1, 3];
            vec.c0.z = mathVec[2, 0]; vec.c1.z = mathVec[2, 1]; vec.c2.z = mathVec[2, 2]; vec.c3.z = mathVec[2, 3];
            vec.c0.w = mathVec[3, 0]; vec.c1.w = mathVec[3, 1]; vec.c2.w = mathVec[3, 2]; vec.c3.w = mathVec[3, 3];

            return vec;
        }

        public static double[,] ToDoubleArray(this doubleMxN mathVec)
        {
            Arena.CheckValid(mathVec);
            double[,] vec = new double[mathVec.M_Rows, mathVec.N_Cols];

            for (int i = 0; i < mathVec.M_Rows; i++)
            {
                for (int j = 0; j < mathVec.N_Cols; j++)
                {
                    vec[i, j] = mathVec[i, j];
                }
            }

            return vec;
        }

        public static float[,] ToFloatArray(this doubleMxN mathVec)
        {
            Arena.CheckValid(mathVec);
            float[,] vec = new float[mathVec.M_Rows, mathVec.N_Cols];

            for (int i = 0; i < mathVec.M_Rows; i++)
            {
                for (int j = 0; j < mathVec.N_Cols; j++)
                {
                    vec[i, j] = (float) mathVec[i, j];
                }
            }

            return vec;
        }

        /// <summary>
        /// Matricies are stored in row-major format but some libraries want column major format
        /// </summary>
        public static void FlattenColumnMajorInpl(this doubleMxN srcMat, doubleN toHereFlat)
        {
            Arena.CheckValid(srcMat);
            Arena.CheckValid(toHereFlat);
            Assume.IsTrue(toHereFlat.N == srcMat.Length);
            for (int cIdx = 0; cIdx < srcMat.N_Cols; cIdx++)       // iterate over columns first
            {
                for (int rIdx = 0; rIdx < srcMat.M_Rows; rIdx++)   // then rows
                {
                    int flatIndex = cIdx * srcMat.M_Rows + rIdx;
                    toHereFlat[flatIndex] = srcMat[rIdx, cIdx];
                }
            }
        }

        public static void UnFlattenFromColumnMajorInpl(this doubleN srcFlat, doubleMxN toHere)
        {
            Arena.CheckValid(srcFlat);
            Arena.CheckValid(toHere);
            Assume.IsTrue(srcFlat.N == toHere.Length);
            for (int cIdx = 0; cIdx < toHere.N_Cols; cIdx++)       // iterate over columns first
            {
                for (int rIdx = 0; rIdx < toHere.M_Rows; rIdx++)   // then rows
                {
                    int flatIndex = cIdx * toHere.M_Rows + rIdx;
                    toHere[rIdx, cIdx] = srcFlat[flatIndex];
                }
            }
        }

        public static float[] ToFloatArray(this doubleN mathVec)
        {
            Arena.CheckValid(mathVec);
            float[] vec = new float[mathVec.N];

            for (int i = 0; i < mathVec.N; i++)
            {
                vec[i] = (float) mathVec[i];
            }

            return vec;
        }

        public static double[] ToDoubleArray(this doubleN mathVec)
        {
            Arena.CheckValid(mathVec);
            double[] vec = new double[mathVec.N];

            for (int i = 0; i < mathVec.N; i++)
            {
                vec[i] = (double)mathVec[i];
            }

            return vec;
        }

        #endregion


    }
}