using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using System;

namespace LinearAlgebra.MathNet.Numerics
{
    public static class NumericsOPfloat
    {
        [BurstCompile]
        public static void SingularValueDecomposition(ref Arena arena, bool computeVectors, floatMxN aRowMajor, floatN s, floatMxN uRowMajor, floatMxN vtRowMajor)
        {
            // LinearAlgebraBursted stores mats as row major order. 
            // These  Math.Net want column first order. We need to transpose.
            floatN aFlat = arena.tempfloatVec(aRowMajor.N_Cols * aRowMajor.M_Rows);
            aRowMajor.FlattenColumnMajorInpl(aFlat);
            floatN uFlat = arena.tempfloatVec(uRowMajor.N_Cols * uRowMajor.M_Rows);
            floatN vtFlat = arena.tempfloatVec(vtRowMajor.N_Cols * vtRowMajor.M_Rows);
            ManagedLinearAlgebraProviderfloat.SingularValueDecomposition(ref arena, computeVectors, aFlat, aRowMajor.M_Rows, aRowMajor.N_Cols, s, uFlat, vtFlat);
            uFlat.UnFlattenFromColumnMajorInpl(uRowMajor);
            vtFlat.UnFlattenFromColumnMajorInpl(vtRowMajor);
        }

        public static Svdfloat Svd(ref Arena arena, bool computeVectors, floatMxN aRowMajor)
        {
            var svd = new Svdfloat(computeVectors);
            int mRows = aRowMajor.M_Rows;
            int nCols = aRowMajor.N_Cols;
            svd.S = arena.tempfloatVec(Mathf.Min(mRows, nCols));
            svd.U = arena.tempfloatMat(mRows, mRows);
            svd.VT = arena.tempfloatMat(nCols, nCols);

            SingularValueDecomposition(ref arena, computeVectors, aRowMajor, svd.S, svd.U, svd.VT);

            svd.ComputeW(ref arena, true);
            return svd;
        }

        [BurstCompile]
        public static void SvdSolve(ref Arena arena, floatMxN aRowMajor, floatMxN bRowMajor, floatMxN x, float epsilon)
        {
            // LinearAlgebraBursted stores mats as row major order. 
            // These  Math.Net want column first order. We need to transpose.
            var aFlat = arena.tempfloatVec(aRowMajor.Length);
            aRowMajor.FlattenColumnMajorInpl(aFlat);

            var bFlat = arena.tempfloatVec(bRowMajor.Length);
            bRowMajor.FlattenColumnMajorInpl(bFlat);

            var xFlat = arena.tempfloatVec(x.Length);
            ManagedLinearAlgebraProviderfloat.SvdSolve(ref arena, aFlat, aRowMajor.M_Rows, aRowMajor.N_Cols, bFlat, bRowMajor.N_Cols, xFlat, epsilon);

            xFlat.UnFlattenFromColumnMajorInpl(x);
        }

        public static void EigenDecomp(ref Arena arena, floatMxN matrixRowMajor, Symmetricity symmetricity, floatMxN eigenVectors, floatN eigenValuesReal, floatN eigenValuesImaginary, floatMxN blockDiagonal)
        {
            // Debug.LogError("Do Burst Compile");
            if (matrixRowMajor.RowCount != matrixRowMajor.ColumnCount)
            {
                throw new ArgumentException("Matrix must be square.");
            }

            var order = matrixRowMajor.RowCount;

            if (eigenValuesReal.Length != order ||
                eigenValuesImaginary.Length != order)
            {
                throw new ArgumentException("length of eigenValues must match order.");
            }

            // Initialize matrices for eigenvalues and eigenvectors
            var matrixFlat = arena.tempfloatVec(matrixRowMajor.Length);
            matrixRowMajor.FlattenColumnMajorInpl(matrixFlat);

            var eigenVectorsFlat = arena.tempfloatVec(order * order);
            var blockDiagonalFlat = arena.tempfloatVec(order * order);
            //var eigenValuesReal = arena.tempfloatVec(order);
            //var eigenValuesImaginary = arena.tempfloatVec(order);
            

            bool isSymmetric;
            switch (symmetricity)
            {
                case Symmetricity.Symmetric:
                case Symmetricity.Hermitian:
                    isSymmetric = true;
                    break;
                case Symmetricity.Asymmetric:
                    isSymmetric = false;
                    break;
                default:
                    
                    isSymmetric = matrixRowMajor.IsSymmetric();
                    break;
            }

            ManagedLinearAlgebraProviderfloat.EigenDecomp(ref arena, isSymmetric, order, matrixFlat, eigenVectorsFlat, eigenValuesReal, eigenValuesImaginary, blockDiagonalFlat);
            eigenVectorsFlat.UnFlattenFromColumnMajorInpl(eigenVectors);
            blockDiagonalFlat.UnFlattenFromColumnMajorInpl(blockDiagonal);
        }

        public static Evdfloat Evd(ref Arena arena, floatMxN matrixRowMajor, Symmetricity sym)
        {
            var order = matrixRowMajor.RowCount;
            var evd = new Evdfloat();
            evd.EigenVectors = arena.tempfloatMat(order, order);
            evd.EigenValuesReal = arena.tempfloatVec(order);
            evd.EigenValuesImaginary = arena.tempfloatVec(order);
            evd.D = arena.tempfloatMat(order, order);

            EigenDecomp(ref arena, matrixRowMajor, sym, evd.EigenVectors, evd.EigenValuesReal, evd.EigenValuesImaginary, evd.D);

            return evd;
        }


    }
}
