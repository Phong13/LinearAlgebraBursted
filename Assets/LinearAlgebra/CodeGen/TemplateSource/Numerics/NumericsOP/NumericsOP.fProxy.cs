using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using System;

namespace LinearAlgebra.MathNet.Numerics
{
    public static class NumericsOPfProxy
    {
        [BurstCompile]
        public static void SingularValueDecomposition(ref Arena arena, bool computeVectors, fProxyMxN aRowMajor, fProxyN s, fProxyMxN uRowMajor, fProxyMxN vtRowMajor)
        {
            // LinearAlgebraBursted stores mats as row major order. 
            // These  Math.Net want column first order. We need to transpose.
            fProxyN aFlat = arena.tempfProxyVec(aRowMajor.N_Cols * aRowMajor.M_Rows);
            aRowMajor.FlattenColumnMajorInpl(aFlat);
            fProxyN uFlat = arena.tempfProxyVec(uRowMajor.N_Cols * uRowMajor.M_Rows);
            fProxyN vtFlat = arena.tempfProxyVec(vtRowMajor.N_Cols * vtRowMajor.M_Rows);
            ManagedLinearAlgebraProviderfProxy.SingularValueDecomposition(ref arena, computeVectors, aFlat, aRowMajor.M_Rows, aRowMajor.N_Cols, s, uFlat, vtFlat);
            uFlat.UnFlattenFromColumnMajorInpl(uRowMajor);
            vtFlat.UnFlattenFromColumnMajorInpl(vtRowMajor);
        }

        public static SvdfProxy Svd(ref Arena arena, bool computeVectors, fProxyMxN aRowMajor)
        {
            var svd = new SvdfProxy(computeVectors);
            int mRows = aRowMajor.M_Rows;
            int nCols = aRowMajor.N_Cols;
            svd.S = arena.tempfProxyVec(Mathf.Min(mRows, nCols));
            svd.U = arena.tempfProxyMat(mRows, mRows);
            svd.VT = arena.tempfProxyMat(nCols, nCols);

            SingularValueDecomposition(ref arena, computeVectors, aRowMajor, svd.S, svd.U, svd.VT);

            svd.ComputeW(ref arena, true);
            return svd;
        }

        [BurstCompile]
        public static void SvdSolve(ref Arena arena, fProxyMxN aRowMajor, fProxyMxN bRowMajor, fProxyMxN x, double epsilon)
        {
            // LinearAlgebraBursted stores mats as row major order. 
            // These  Math.Net want column first order. We need to transpose.
            var aFlat = arena.tempfProxyVec(aRowMajor.Length);
            aRowMajor.FlattenColumnMajorInpl(aFlat);

            var bFlat = arena.tempfProxyVec(bRowMajor.Length);
            bRowMajor.FlattenColumnMajorInpl(bFlat);

            var xFlat = arena.tempfProxyVec(x.Length);
            ManagedLinearAlgebraProviderfProxy.SvdSolve(ref arena, aFlat, aRowMajor.M_Rows, aRowMajor.N_Cols, bFlat, bRowMajor.N_Cols, xFlat, epsilon);

            xFlat.UnFlattenFromColumnMajorInpl(x);
        }

        public static void EigenDecomp(ref Arena arena, fProxyMxN matrixRowMajor, Symmetricity symmetricity, fProxyMxN eigenVectors, fProxyN eigenValuesReal, fProxyN eigenValuesImaginary, fProxyMxN blockDiagonal)
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
            var matrixFlat = arena.tempfProxyVec(matrixRowMajor.Length);
            matrixRowMajor.FlattenColumnMajorInpl(matrixFlat);

            var eigenVectorsFlat = arena.tempfProxyVec(order * order);
            var blockDiagonalFlat = arena.tempfProxyVec(order * order);
            //var eigenValuesReal = arena.tempfProxyVec(order);
            //var eigenValuesImaginary = arena.tempfProxyVec(order);
            

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

            ManagedLinearAlgebraProviderfProxy.EigenDecomp(ref arena, isSymmetric, order, matrixFlat, eigenVectorsFlat, eigenValuesReal, eigenValuesImaginary, blockDiagonalFlat);
            eigenVectorsFlat.UnFlattenFromColumnMajorInpl(eigenVectors);
            blockDiagonalFlat.UnFlattenFromColumnMajorInpl(blockDiagonal);
        }

        public static EvdfProxy Evd(ref Arena arena, fProxyMxN matrixRowMajor, Symmetricity sym)
        {
            var order = matrixRowMajor.RowCount;
            var evd = new EvdfProxy();
            evd.EigenVectors = arena.tempfProxyMat(order, order);
            evd.EigenValuesReal = arena.tempfProxyVec(order);
            evd.EigenValuesImaginary = arena.tempfProxyVec(order);
            evd.D = arena.tempfProxyMat(order, order);

            EigenDecomp(ref arena, matrixRowMajor, sym, evd.EigenVectors, evd.EigenValuesReal, evd.EigenValuesImaginary, evd.D);

            return evd;
        }


    }
}
