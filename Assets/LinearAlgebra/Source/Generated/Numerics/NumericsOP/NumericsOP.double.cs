using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using System;

namespace LinearAlgebra.MathNet.Numerics
{
    public static class NumericsOPdouble
    {
        [BurstCompile]
        public static void SingularValueDecomposition(ref Arena arena, bool computeVectors, doubleMxN aRowMajor, doubleN s, doubleMxN uRowMajor, doubleMxN vtRowMajor)
        {
            // LinearAlgebraBursted stores mats as row major order. 
            // These  Math.Net want column first order. We need to transpose.
            doubleN aFlat = arena.tempdoubleVec(aRowMajor.N_Cols * aRowMajor.M_Rows);
            aRowMajor.FlattenColumnMajorInpl(aFlat);
            doubleN uFlat = arena.tempdoubleVec(uRowMajor.N_Cols * uRowMajor.M_Rows);
            doubleN vtFlat = arena.tempdoubleVec(vtRowMajor.N_Cols * vtRowMajor.M_Rows);
            ManagedLinearAlgebraProviderdouble.SingularValueDecomposition(ref arena, computeVectors, aFlat, aRowMajor.M_Rows, aRowMajor.N_Cols, s, uFlat, vtFlat);
            uFlat.UnFlattenFromColumnMajorInpl(uRowMajor);
            vtFlat.UnFlattenFromColumnMajorInpl(vtRowMajor);
        }

        public static Svddouble Svd(ref Arena arena, bool computeVectors, doubleMxN aRowMajor)
        {
            var svd = new Svddouble(computeVectors);
            int mRows = aRowMajor.M_Rows;
            int nCols = aRowMajor.N_Cols;
            svd.S = arena.tempdoubleVec(Mathf.Min(mRows, nCols));
            svd.U = arena.tempdoubleMat(mRows, mRows);
            svd.VT = arena.tempdoubleMat(nCols, nCols);

            SingularValueDecomposition(ref arena, computeVectors, aRowMajor, svd.S, svd.U, svd.VT);

            svd.ComputeW(ref arena, true);
            return svd;
        }

        [BurstCompile]
        public static void SvdSolve(ref Arena arena, doubleMxN aRowMajor, doubleMxN bRowMajor, doubleMxN x, double epsilon)
        {
            // LinearAlgebraBursted stores mats as row major order. 
            // These  Math.Net want column first order. We need to transpose.
            var aFlat = arena.tempdoubleVec(aRowMajor.Length);
            aRowMajor.FlattenColumnMajorInpl(aFlat);

            var bFlat = arena.tempdoubleVec(bRowMajor.Length);
            bRowMajor.FlattenColumnMajorInpl(bFlat);

            var xFlat = arena.tempdoubleVec(x.Length);
            ManagedLinearAlgebraProviderdouble.SvdSolve(ref arena, aFlat, aRowMajor.M_Rows, aRowMajor.N_Cols, bFlat, bRowMajor.N_Cols, xFlat, epsilon);

            xFlat.UnFlattenFromColumnMajorInpl(x);
        }

        public static void EigenDecomp(ref Arena arena, doubleMxN matrixRowMajor, Symmetricity symmetricity, doubleMxN eigenVectors, doubleN eigenValuesReal, doubleN eigenValuesImaginary, doubleMxN blockDiagonal)
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
            var matrixFlat = arena.tempdoubleVec(matrixRowMajor.Length);
            matrixRowMajor.FlattenColumnMajorInpl(matrixFlat);

            var eigenVectorsFlat = arena.tempdoubleVec(order * order);
            var blockDiagonalFlat = arena.tempdoubleVec(order * order);
            //var eigenValuesReal = arena.tempdoubleVec(order);
            //var eigenValuesImaginary = arena.tempdoubleVec(order);
            

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

            ManagedLinearAlgebraProviderdouble.EigenDecomp(ref arena, isSymmetric, order, matrixFlat, eigenVectorsFlat, eigenValuesReal, eigenValuesImaginary, blockDiagonalFlat);
            eigenVectorsFlat.UnFlattenFromColumnMajorInpl(eigenVectors);
            blockDiagonalFlat.UnFlattenFromColumnMajorInpl(blockDiagonal);
        }

        public static Evddouble Evd(ref Arena arena, doubleMxN matrixRowMajor, Symmetricity sym)
        {
            var order = matrixRowMajor.RowCount;
            var evd = new Evddouble();
            evd.EigenVectors = arena.tempdoubleMat(order, order);
            evd.EigenValuesReal = arena.tempdoubleVec(order);
            evd.EigenValuesImaginary = arena.tempdoubleVec(order);
            evd.D = arena.tempdoubleMat(order, order);

            EigenDecomp(ref arena, matrixRowMajor, sym, evd.EigenVectors, evd.EigenValuesReal, evd.EigenValuesImaginary, evd.D);

            return evd;
        }


    }
}
