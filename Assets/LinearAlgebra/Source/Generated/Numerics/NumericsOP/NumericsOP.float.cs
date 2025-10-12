using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using System;

namespace LinearAlgebra.MathNet.Numerics
{
    [BurstCompile]
    public static class NumericsOPfloat
    {
        [BurstCompile]
        public static void SingularValueDecomposition(ref Arena arena, bool computeVectors, ref floatMxN aRowMajor, ref floatN s, ref floatMxN uRowMajor, ref floatMxN vtRowMajor)
        {
            // LinearAlgebraBursted stores mats as row major order. 
            // These  Math.Net want column first order. We need to transpose.
            floatN aFlat = arena.tempfloatVec(aRowMajor.N_Cols * aRowMajor.M_Rows);
            aRowMajor.FlattenColumnMajorInpl(aFlat);
            floatN uFlat = arena.tempfloatVec(uRowMajor.N_Cols * uRowMajor.M_Rows);
            floatN vtFlat = arena.tempfloatVec(vtRowMajor.N_Cols * vtRowMajor.M_Rows);
            ManagedLinearAlgebraProviderfloat.SingularValueDecomposition(ref arena, computeVectors, ref aFlat, aRowMajor.M_Rows, aRowMajor.N_Cols, ref s, ref uFlat, ref vtFlat);
            uFlat.UnFlattenFromColumnMajorInpl(uRowMajor);
            vtFlat.UnFlattenFromColumnMajorInpl(vtRowMajor);
        }

        [BurstCompile]
        public static void Svd(ref Arena arena, bool computeVectors, ref floatMxN aRowMajor, out Svdfloat svd)
        {
            svd = new Svdfloat();
            svd.Init(computeVectors);
            int mRows = aRowMajor.M_Rows;
            int nCols = aRowMajor.N_Cols;
            svd.S = arena.tempfloatVec(Mathf.Min(mRows, nCols));
            svd.U = arena.tempfloatMat(mRows, mRows);
            svd.VT = arena.tempfloatMat(nCols, nCols);

            SingularValueDecomposition(ref arena, computeVectors, ref aRowMajor, ref svd.S, ref svd.U, ref svd.VT);

            svd.ComputeW(ref arena, true);
        }

        [BurstCompile]
        public static void SvdSolve(ref Arena arena, ref floatMxN aRowMajor, ref floatMxN bRowMajor, ref floatMxN x, float epsilon)
        {
            // LinearAlgebraBursted stores mats as row major order. 
            // These  Math.Net want column first order. We need to transpose.
            var aFlat = arena.tempfloatVec(aRowMajor.Length);
            aRowMajor.FlattenColumnMajorInpl(aFlat);

            var bFlat = arena.tempfloatVec(bRowMajor.Length);
            bRowMajor.FlattenColumnMajorInpl(bFlat);

            var xFlat = arena.tempfloatVec(x.Length);
            ManagedLinearAlgebraProviderfloat.SvdSolve(ref arena, ref aFlat, aRowMajor.M_Rows, aRowMajor.N_Cols, ref bFlat, bRowMajor.N_Cols, ref xFlat, epsilon);

            xFlat.UnFlattenFromColumnMajorInpl(x);
        }

        [BurstCompile]
        public static void EigenDecomp(ref Arena arena, ref floatMxN matrixRowMajor, Symmetricity symmetricity, ref floatMxN eigenVectors, ref floatN eigenValuesReal, ref floatN eigenValuesImaginary, ref floatMxN blockDiagonal)
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

            ManagedLinearAlgebraProviderfloat.EigenDecomp(ref arena, isSymmetric, order, ref matrixFlat, ref eigenVectorsFlat, ref eigenValuesReal, ref eigenValuesImaginary, ref blockDiagonalFlat);
            eigenVectorsFlat.UnFlattenFromColumnMajorInpl(eigenVectors);
            blockDiagonalFlat.UnFlattenFromColumnMajorInpl(blockDiagonal);
        }

        [BurstCompile]
        public static void Evd(ref Arena arena, ref floatMxN matrixRowMajor, Symmetricity sym, out Evdfloat evd)
        {
            var order = matrixRowMajor.RowCount;
            evd = new Evdfloat();
            evd.EigenVectors = arena.tempfloatMat(order, order);
            evd.EigenValuesReal = arena.tempfloatVec(order);
            evd.EigenValuesImaginary = arena.tempfloatVec(order);
            evd.D = arena.tempfloatMat(order, order);

            EigenDecomp(ref arena, ref matrixRowMajor, sym, ref evd.EigenVectors, ref evd.EigenValuesReal, ref evd.EigenValuesImaginary, ref evd.D);
        }


    }
}
