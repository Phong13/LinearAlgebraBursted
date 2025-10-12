using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using System;

namespace LinearAlgebra.MathNet.Numerics
{
    [BurstCompile]
    public static class NumericsOPdouble
    {
        [BurstCompile]
        public static void SingularValueDecomposition(ref Arena arena, bool computeVectors, ref doubleMxN aRowMajor, ref doubleN s, ref doubleMxN uRowMajor, ref doubleMxN vtRowMajor)
        {
            // LinearAlgebraBursted stores mats as row major order. 
            // These  Math.Net want column first order. We need to transpose.
            doubleN aFlat = arena.tempdoubleVec(aRowMajor.N_Cols * aRowMajor.M_Rows);
            aRowMajor.FlattenColumnMajorInpl(aFlat);
            doubleN uFlat = arena.tempdoubleVec(uRowMajor.N_Cols * uRowMajor.M_Rows);
            doubleN vtFlat = arena.tempdoubleVec(vtRowMajor.N_Cols * vtRowMajor.M_Rows);
            ManagedLinearAlgebraProviderdouble.SingularValueDecomposition(ref arena, computeVectors, ref aFlat, aRowMajor.M_Rows, aRowMajor.N_Cols, ref s, ref uFlat, ref vtFlat);
            uFlat.UnFlattenFromColumnMajorInpl(uRowMajor);
            vtFlat.UnFlattenFromColumnMajorInpl(vtRowMajor);
        }

        [BurstCompile]
        public static void Svd(ref Arena arena, bool computeVectors, ref doubleMxN aRowMajor, out Svddouble svd)
        {
            svd = new Svddouble();
            svd.Init(computeVectors);
            int mRows = aRowMajor.M_Rows;
            int nCols = aRowMajor.N_Cols;
            svd.S = arena.tempdoubleVec(Mathf.Min(mRows, nCols));
            svd.U = arena.tempdoubleMat(mRows, mRows);
            svd.VT = arena.tempdoubleMat(nCols, nCols);

            SingularValueDecomposition(ref arena, computeVectors, ref aRowMajor, ref svd.S, ref svd.U, ref svd.VT);

            svd.ComputeW(ref arena, true);
        }

        [BurstCompile]
        public static void SvdSolve(ref Arena arena, ref doubleMxN aRowMajor, ref doubleMxN bRowMajor, ref doubleMxN x, double epsilon)
        {
            // LinearAlgebraBursted stores mats as row major order. 
            // These  Math.Net want column first order. We need to transpose.
            var aFlat = arena.tempdoubleVec(aRowMajor.Length);
            aRowMajor.FlattenColumnMajorInpl(aFlat);

            var bFlat = arena.tempdoubleVec(bRowMajor.Length);
            bRowMajor.FlattenColumnMajorInpl(bFlat);

            var xFlat = arena.tempdoubleVec(x.Length);
            ManagedLinearAlgebraProviderdouble.SvdSolve(ref arena, ref aFlat, aRowMajor.M_Rows, aRowMajor.N_Cols, ref bFlat, bRowMajor.N_Cols, ref xFlat, epsilon);

            xFlat.UnFlattenFromColumnMajorInpl(x);
        }

        [BurstCompile]
        public static void EigenDecomp(ref Arena arena, ref doubleMxN matrixRowMajor, Symmetricity symmetricity, ref doubleMxN eigenVectors, ref doubleN eigenValuesReal, ref doubleN eigenValuesImaginary, ref doubleMxN blockDiagonal)
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

            ManagedLinearAlgebraProviderdouble.EigenDecomp(ref arena, isSymmetric, order, ref matrixFlat, ref eigenVectorsFlat, ref eigenValuesReal, ref eigenValuesImaginary, ref blockDiagonalFlat);
            eigenVectorsFlat.UnFlattenFromColumnMajorInpl(eigenVectors);
            blockDiagonalFlat.UnFlattenFromColumnMajorInpl(blockDiagonal);
        }

        [BurstCompile]
        public static void Evd(ref Arena arena, ref doubleMxN matrixRowMajor, Symmetricity sym, out Evddouble evd)
        {
            var order = matrixRowMajor.RowCount;
            evd = new Evddouble();
            evd.EigenVectors = arena.tempdoubleMat(order, order);
            evd.EigenValuesReal = arena.tempdoubleVec(order);
            evd.EigenValuesImaginary = arena.tempdoubleVec(order);
            evd.D = arena.tempdoubleMat(order, order);

            EigenDecomp(ref arena, ref matrixRowMajor, sym, ref evd.EigenVectors, ref evd.EigenValuesReal, ref evd.EigenValuesImaginary, ref evd.D);
        }


    }
}
