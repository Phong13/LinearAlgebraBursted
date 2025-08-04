using Unity.Mathematics;

//+deleteThis
using LinearAlgebra.mathProxies;
//-deleteThis

namespace LinearAlgebra
{
    // have to test it all, have to do other conversions too
    public static partial class ArenaExtensions {

        #region CONVERSIONS_FROM_MATH
        public static fProxyN Convert(this ref Arena arena, in fProxy2 mathVec, bool isTemp)
        {
            fProxyN vec;
            if (isTemp)
            {
                vec = arena.tempfProxyVec(2, true);
            } else
            {
                vec = arena.fProxyVec(2, true);
            }

            vec[0] = mathVec.x;
            vec[1] = mathVec.y;

            return vec;
        }

        public static fProxyN Convert(this ref Arena arena, in fProxy3 mathVec, bool isTemp)
        {
            fProxyN vec;
            if (isTemp)
            {
                vec = arena.tempfProxyVec(3, true);
            }
            else
            {
                vec = arena.fProxyVec(3, true);
            }

            vec[0] = mathVec.x;
            vec[1] = mathVec.y;
            vec[2] = mathVec.z;

            return vec;
        }

        public static fProxyN Convert(this ref Arena arena, in fProxy4 mathVec, bool isTemp)
        {
            fProxyN vec;
            if (isTemp)
            {
                vec = arena.tempfProxyVec(4, true);
            }
            else
            {
                vec = arena.fProxyVec(4, true);
            }

            vec[0] = mathVec.x;
            vec[1] = mathVec.y;
            vec[2] = mathVec.z;
            vec[3] = mathVec.w;

            return vec;
        }

        public static fProxyMxN Convert(this ref Arena arena, in fProxy2x2 mathMat, bool isTemp)
        {
            fProxyMxN mat; 
            if (isTemp)
            {
                mat = arena.tempfProxyMat(2, 2, true);
            }
            else
            {
                mat = arena.fProxyMat(2, 2, true);
            }

            mat[0, 0] = mathMat.c0.x;
            mat[1, 0] = mathMat.c0.y;
            mat[0, 1] = mathMat.c1.x;
            mat[1, 1] = mathMat.c1.y;

            return mat;
        }

        public static fProxyMxN Convert(this ref Arena arena, in fProxy3x3 mathMat, bool isTemp)
        {
            fProxyMxN mat;
            if (isTemp)
            {
                mat = arena.tempfProxyMat(3, 3, true);
            }
            else
            {
                mat = arena.fProxyMat(3, 3, true);
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

        public static fProxyMxN Convert(this ref Arena arena, in fProxy4x4 mathMat, bool isTemp)
        {
            fProxyMxN mat;
            if (isTemp)
            {
                mat = arena.tempfProxyMat(4, 4, true);
            }
            else
            {
                mat = arena.fProxyMat(4, 4, true);
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
        public static fProxy2 ToLen2(this ref Arena arena, in fProxyN mathVec) {
            Arena.CheckValid(mathVec);
            fProxy2 vec = new fProxy2();

            vec.x = mathVec[0];
            vec.y = mathVec[1];

            return vec;
        }

        public static fProxy3 ToLen3(this fProxyN mathVec)
        {
            Arena.CheckValid(mathVec);
            fProxy3 vec = new fProxy3();

            vec.x = mathVec[0];
            vec.y = mathVec[1];
            vec.z = mathVec[2];

            return vec;
        }

        public static fProxy4 ToLen4(this fProxyN mathVec)
        {
            Arena.CheckValid(mathVec);
            fProxy4 vec = new fProxy4();

            vec.x = mathVec[0];
            vec.y = mathVec[1];
            vec.z = mathVec[2];
            vec.w = mathVec[3];

            return vec;
        }

        public static fProxy2x2 To2x2(this fProxyMxN mathVec)
        {
            Arena.CheckValid(mathVec);
            fProxy2x2 vec = new fProxy2x2();

            vec.c0.x = mathVec[0, 0]; vec.c1.x = mathVec[0, 1];
            vec.c0.y = mathVec[1, 0]; vec.c1.y = mathVec[1, 1];

            return vec;
        }

        public static fProxy3x3 To3x3(this fProxyMxN mathVec)
        {
            Arena.CheckValid(mathVec);
            fProxy3x3 vec = new fProxy3x3();

            vec.c0.x = mathVec[0, 0]; vec.c1.x = mathVec[0, 1]; vec.c2.x = mathVec[0, 2];
            vec.c0.y = mathVec[1, 0]; vec.c1.y = mathVec[1, 1]; vec.c2.y = mathVec[1, 2];
            vec.c0.z = mathVec[2, 0]; vec.c1.z = mathVec[2, 1]; vec.c2.z = mathVec[2, 2];

            return vec;
        }

        public static fProxy4x4 To4x4(this fProxyMxN mathVec)
        {
            Arena.CheckValid(mathVec);
            fProxy4x4 vec = new fProxy4x4();

            vec.c0.x = mathVec[0, 0]; vec.c1.x = mathVec[0, 1]; vec.c2.x = mathVec[0, 2]; vec.c3.x = mathVec[0, 3];
            vec.c0.y = mathVec[1, 0]; vec.c1.y = mathVec[1, 1]; vec.c2.y = mathVec[1, 2]; vec.c3.y = mathVec[1, 3];
            vec.c0.z = mathVec[2, 0]; vec.c1.z = mathVec[2, 1]; vec.c2.z = mathVec[2, 2]; vec.c3.z = mathVec[2, 3];
            vec.c0.w = mathVec[3, 0]; vec.c1.w = mathVec[3, 1]; vec.c2.w = mathVec[3, 2]; vec.c3.w = mathVec[3, 3];

            return vec;
        }

        #endregion


    }
}