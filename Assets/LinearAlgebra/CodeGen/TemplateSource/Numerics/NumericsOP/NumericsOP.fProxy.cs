using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using System;

namespace LinearAlgebra.MathNet.Numerics
{
    [BurstCompile]
    public static class NumericsOPfProxy
    {
        [BurstCompile]
        public static void SingularValueDecomposition(ref Arena arena, bool computeVectors, ref fProxyMxN aRowMajor, ref fProxyN s, ref fProxyMxN uRowMajor, ref fProxyMxN vtRowMajor)
        {
            // LinearAlgebraBursted stores mats as row major order. 
            // These  Math.Net want column first order. We need to transpose.
            fProxyN aFlat = arena.tempfProxyVec(aRowMajor.N_Cols * aRowMajor.M_Rows);
            aRowMajor.FlattenColumnMajorInpl(aFlat);
            fProxyN uFlat = arena.tempfProxyVec(uRowMajor.N_Cols * uRowMajor.M_Rows);
            fProxyN vtFlat = arena.tempfProxyVec(vtRowMajor.N_Cols * vtRowMajor.M_Rows);
            ManagedLinearAlgebraProviderfProxy.SingularValueDecomposition(ref arena, computeVectors, ref aFlat, aRowMajor.M_Rows, aRowMajor.N_Cols, ref s, ref uFlat, ref vtFlat);
            uFlat.UnFlattenFromColumnMajorInpl(uRowMajor);
            vtFlat.UnFlattenFromColumnMajorInpl(vtRowMajor);
        }

        [BurstCompile]
        public static void Svd(ref Arena arena, bool computeVectors, ref fProxyMxN aRowMajor, out SvdfProxy svd)
        {
            svd = new SvdfProxy();
            svd.Init(computeVectors);
            int mRows = aRowMajor.M_Rows;
            int nCols = aRowMajor.N_Cols;
            svd.S = arena.tempfProxyVec(Mathf.Min(mRows, nCols));
            svd.U = arena.tempfProxyMat(mRows, mRows);
            svd.VT = arena.tempfProxyMat(nCols, nCols);

            SingularValueDecomposition(ref arena, computeVectors, ref aRowMajor, ref svd.S, ref svd.U, ref svd.VT);

            svd.ComputeW(ref arena, true);
        }

        [BurstCompile]
        public static void SvdSolve(ref Arena arena, ref fProxyMxN aRowMajor, ref fProxyMxN bRowMajor, ref fProxyMxN x, fProxy epsilon)
        {
            // LinearAlgebraBursted stores mats as row major order. 
            // These  Math.Net want column first order. We need to transpose.
            var aFlat = arena.tempfProxyVec(aRowMajor.Length);
            aRowMajor.FlattenColumnMajorInpl(aFlat);

            var bFlat = arena.tempfProxyVec(bRowMajor.Length);
            bRowMajor.FlattenColumnMajorInpl(bFlat);

            var xFlat = arena.tempfProxyVec(x.Length);
            ManagedLinearAlgebraProviderfProxy.SvdSolve(ref arena, ref aFlat, aRowMajor.M_Rows, aRowMajor.N_Cols, ref bFlat, bRowMajor.N_Cols, ref xFlat, epsilon);

            xFlat.UnFlattenFromColumnMajorInpl(x);
        }

        [BurstCompile]
        public static void EigenDecomp(ref Arena arena, ref fProxyMxN matrixRowMajor, Symmetricity symmetricity, ref fProxyMxN eigenVectors, ref fProxyN eigenValuesReal, ref fProxyN eigenValuesImaginary, ref fProxyMxN blockDiagonal)
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

            ManagedLinearAlgebraProviderfProxy.EigenDecomp(ref arena, isSymmetric, order, ref matrixFlat, ref eigenVectorsFlat, ref eigenValuesReal, ref eigenValuesImaginary, ref blockDiagonalFlat);
            eigenVectorsFlat.UnFlattenFromColumnMajorInpl(eigenVectors);
            blockDiagonalFlat.UnFlattenFromColumnMajorInpl(blockDiagonal);
        }

        [BurstCompile]
        public static void Evd(ref Arena arena, ref fProxyMxN matrixRowMajor, Symmetricity sym, out EvdfProxy evd)
        {
            var order = matrixRowMajor.RowCount;
            evd = new EvdfProxy();
            evd.EigenVectors = arena.tempfProxyMat(order, order);
            evd.EigenValuesReal = arena.tempfProxyVec(order);
            evd.EigenValuesImaginary = arena.tempfProxyVec(order);
            evd.D = arena.tempfProxyMat(order, order);

            EigenDecomp(ref arena, ref matrixRowMajor, sym, ref evd.EigenVectors, ref evd.EigenValuesReal, ref evd.EigenValuesImaginary, ref evd.D);
        }


    }
}
