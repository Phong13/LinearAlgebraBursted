using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Text;
using LinearAlgebra;
using LinearAlgebra.MathNet.Numerics;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Unity.Mathematics;
using System.Linq;

public class TestNumericsOPsfloat
{
    static void ToDotNetMatrix(floatMxN m, out Matrix<double> mm)
    {
        mm = DenseMatrix.Create(m.M_Rows, m.N_Cols, 0);
        for (int i = 0; i < m.M_Rows; i++)
        {
            for (int j = 0; j < m.N_Cols; j++)
            {
                mm[i, j] = m[i, j];
            }
        }
    }

    static void ToDotNetVector(floatN m, out Vector<double> mm)
    {
        mm = DenseVector.Create(m.N, 0);
        for (int i = 0; i < m.N; i++)
        {
            mm[i] = m[i];
        }
    }

    static void DoLinAlgBurstedSVD(double[,] _A, out Vector<double> S, out Matrix<double> U, out Matrix<double> VT)
    {
        Arena arena = new Arena(Unity.Collections.Allocator.Persistent);
        {
            floatMxN A = arena.floatMat(_A);
            int mRows = _A.GetLength(0);
            int nCols = _A.GetLength(1);
            floatN s = arena.tempfloatVec(Mathf.Min(mRows, nCols));
            floatMxN u = arena.tempfloatMat(mRows, mRows);
            floatMxN vt = arena.tempfloatMat(nCols, nCols);
            NumericsOPfloat.SingularValueDecomposition(ref arena, true, A, s, u, vt);


            var w = arena.tempfloatMat(u.M_Rows, vt.N_Cols); 
            for (int i = 0; i < s.N; i++)
            {
                w[i, i] = s[i];
            }

            var _tmp = floatOP.dot(w, vt);
            var AA = floatOP.dot(u, _tmp);
            Assert.IsTrue(A.AlmostEqualsByValue( AA, 10e-10f));

            Debug.Log("A: " + A);
            Debug.Log("AA: " + AA);
            Debug.Log("s: " + s);
            Debug.Log("w: " + w);
            Debug.Log("u: " + u);
            Debug.Log("vt: " + vt);

            ToDotNetMatrix(u, out U);
            ToDotNetVector(s, out S);
            ToDotNetMatrix(vt, out VT);

        }
        arena.Dispose();
    }

    static void DoLinAlgBurstedSVDSolve(double[,] _A, double[,] _b, out Matrix<double> x, float eps)
    {
        Arena arena = new Arena(Unity.Collections.Allocator.Persistent);
        {
            floatMxN A = arena.floatMat(_A);
            floatMxN b = arena.floatMat(_b);

            floatMxN xx = arena.tempfloatMat(_A.GetLength(0), _b.GetLength(1));

            NumericsOPfloat.SvdSolve(ref arena, A, b, xx, eps);

            ToDotNetMatrix(xx, out x);
        }
        arena.Dispose();
    }

    static void DoDotNetSVD(double[,] _A, out Vector<double> S, out Matrix<double> U, out Matrix<double> VT)
    {
        Matrix<double> A = DenseMatrix.OfArray(_A);
        var svd = A.Svd(true);
        S = svd.S;
        U = svd.U;
        VT = svd.VT;

        Debug.Log("DotNet Dims U: " + U.RowCount + ", " + U.ColumnCount + "   vt: " + VT.RowCount + "," + VT.ColumnCount + " s: " + S.Count + "  w: " + svd.W.RowCount + ", " + svd.W.ColumnCount);
        Debug.Log("S: " + S);
        Debug.Log("DotNet _ tmp \n" + (svd.W * VT) + "  \n\n W: \n" + svd.W + " \n\n VT: \n" + VT);

        var AA = U * svd.W * VT;
        Assert.IsTrue(EqualsApproximate(A, AA, 10e-10));
    }

    public static void UnFlattenFromColumnMajorInpl(double[] srcFlat, Matrix<double> toHere)
    {
        for (int cIdx = 0; cIdx < toHere.ColumnCount; cIdx++)       // iterate over columns first
        {
            for (int rIdx = 0; rIdx < toHere.RowCount; rIdx++)   // then rows
            {
                int flatIndex = cIdx * toHere.RowCount + rIdx;
                toHere[rIdx, cIdx] = srcFlat[flatIndex];
            }
        }
    }

    public static void TruncatedSVDSolve(double[,] _A, double[,] _b, out Matrix<double> x, double epsilon)
    {
        Matrix<double> A = DenseMatrix.OfArray(_A);
        Matrix<double> bb = DenseMatrix.OfArray(_b);
        double[] b = ((MathNet.Numerics.LinearAlgebra.Storage.DenseColumnMajorMatrixStorage<double>)bb.Storage).Data;
        double[] s;
        double[] u;
        double[] vt;
        double l2Norm;


        var svd = A.Svd(true);

        // double tol = 1e-10; // adjust depending on scale of your problem
        s = ((MathNet.Numerics.LinearAlgebra.Storage.DenseVectorStorage<double>) svd.S.Storage).Data;
        u = ((MathNet.Numerics.LinearAlgebra.Storage.DenseColumnMajorMatrixStorage<double>)svd.U.Storage).Data;
        vt = ((MathNet.Numerics.LinearAlgebra.Storage.DenseColumnMajorMatrixStorage<double>)svd.VT.Storage).Data;
        l2Norm = svd.L2Norm;


        double tolerance = System.Math.Max(A.RowCount, A.ColumnCount) * l2Norm * epsilon;

        double[] xx = new double[A.RowCount * bb.ColumnCount];
        // Manual truncated solve: x = V * inv(S) * (U^T * b)

        int rowsA = A.RowCount;
        int columnsA = A.ColumnCount;
        Debug.Log("Tol " + tolerance);
        SvdSolveFactored(rowsA, columnsA, s, u, vt, b, bb.ColumnCount, xx, tolerance);

        x = DenseMatrix.Create(columnsA, bb.ColumnCount, 0);
        //for (int i = 0; i < xx.Length; i++) x[i, 0] = xx[i];
        UnFlattenFromColumnMajorInpl(xx, x);
    }

    // This code was copied from Math.Net with tolerance bit added.
    public static void SvdSolveFactored(int rowsA, int columnsA, double[] s, double[] u, double[] vt, double[] b, int columnsB, double[] x, double tolerance)
    {
        var mn = math.min(rowsA, columnsA);
        var tmp = new double[columnsA];

        for (var k = 0; k < columnsB; k++)
        {
            for (var j = 0; j < columnsA; j++)
            {
                double value = 0;
                if (j < mn)
                {
                    if (tolerance == -1)
                    {
                        // ignore tolerence
                        for (var i = 0; i < rowsA; i++)
                        {
                            value += u[(j * rowsA) + i] * b[(k * rowsA) + i];
                        }

                        value /= s[j];
                    }
                    else
                    {
                        // Truncate small singular values to avoid exploding solutions.
                        if (math.abs(s[j]) > tolerance)
                        {
                            for (var i = 0; i < rowsA; i++)
                            {
                                value += u[(j * rowsA) + i] * b[(k * rowsA) + i];
                            }

                            value /= s[j];
                        }
                    }
                }

                tmp[j] = value;
            }

            for (var j = 0; j < columnsA; j++)
            {
                double value = 0;
                for (var i = 0; i < columnsA; i++)
                {
                    value += vt[(j * columnsA) + i] * tmp[i];
                }

                x[(k * columnsA) + j] = value;
            }
        }
    }

    public static bool EqualsApproximate(Matrix<double> a, Matrix<double> b, double epsilon)
    {
        if (a.RowCount != b.RowCount ||
            a.ColumnCount != b.ColumnCount)
        {
            return false;
        }

        for (int i = 0; i < a.RowCount; i++)
        {
            for (int j = 0; j < b.ColumnCount; j++)
            {
                if (math.abs(a[i, j] - b[i, j]) > epsilon)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public static bool EqualsApproximate(Vector<double> a, Vector<double> b, double epsilon)
    {
        if (a.Count != b.Count)
        {
            return false;
        }

        for (int i = 0; i < a.Count; i++)
        {
            if (math.abs(a[i] - b[i]) > epsilon)
            {
                return false;
            }
        }

        return true;
    }

    [Test]
    public void TestMulMatMat()
    {
        double[,] _A = new double[3, 2] { { 1,2 },
                                        { 4,5 },
                                        { 7,8 } };
        double[,] _B = new double[2, 3] {{ 9, 10, 11 },
                                         {12,13, 13 } };
        Matrix<double> C;
        {
            var A = DenseMatrix.OfArray(_A);
            var B = DenseMatrix.OfArray(_B);
            C = A * B;
        }
        
        Matrix<double> CC;
        Arena arena = new Arena(Unity.Collections.Allocator.Persistent);
        {
            floatMxN A = arena.floatMat(_A);
            floatMxN B = arena.floatMat(_B);
            floatMxN CCC = floatOP.dot(A, B);
            ToDotNetMatrix(CCC, out CC);
        }

        Assert.IsTrue(EqualsApproximate(C, CC, 10e-15));
    }

    [Test]
    public void TestSVD()
    {
        double[,] _A = new double[3, 3] { { 1,2,3 },
                                        { 4,5,6 },
                                        { 7,8,9 } };

        Vector<double> b_S;
        Matrix<double> b_U;
        Matrix<double> b_VT;
        DoDotNetSVD(_A, out b_S, out b_U, out b_VT);

        Vector<double> a_S;
        Matrix<double> a_U;
        Matrix<double> a_VT;
        DoLinAlgBurstedSVD(_A, out a_S, out a_U, out a_VT);



        Assert.IsTrue(EqualsApproximate(a_S, b_S, 10e-10));
        Assert.IsTrue(EqualsApproximate(a_U, b_U, 10e-10));
        Assert.IsTrue(EqualsApproximate(a_VT, b_VT, 10e-10));
    }

    [Test]
    public void TestSVDNonSquareTall()
    {
        double[,] _A = new double[4, 2] { { 1,2 },
                                          { 4,5 },
                                          { 7,8 },
                                          { 9,10} };

        Vector<double> b_S;
        Matrix<double> b_U;
        Matrix<double> b_VT;
        DoDotNetSVD(_A, out b_S, out b_U, out b_VT);

        Vector<double> a_S;
        Matrix<double> a_U;
        Matrix<double> a_VT;
        DoLinAlgBurstedSVD(_A, out a_S, out a_U, out a_VT);

        Debug.Log("AAAA  " + a_S);
        Debug.Log("BBBB  " + b_S);

        Assert.IsTrue(EqualsApproximate(a_S, b_S, 10e-10));
        Assert.IsTrue(EqualsApproximate(a_U, b_U, 10e-10));
        Assert.IsTrue(EqualsApproximate(a_VT, b_VT, 10e-10));
    }

    [Test]
    public void TestSVDNonSquareWide()
    {
        double[,] _A = new double[2, 4] { { 1,2,3,4 },
                                          { 4,5,6,8 }};

        Vector<double> b_S;
        Matrix<double> b_U;
        Matrix<double> b_VT;
        DoDotNetSVD(_A, out b_S, out b_U, out b_VT);

        Vector<double> a_S;
        Matrix<double> a_U;
        Matrix<double> a_VT;
        DoLinAlgBurstedSVD(_A, out a_S, out a_U, out a_VT);

        Assert.IsTrue(EqualsApproximate(a_S, b_S, 10e-10f));
        Assert.IsTrue(EqualsApproximate(a_U, b_U, 10e-10f));
        Assert.IsTrue(EqualsApproximate(a_VT, b_VT, 10e-10f));
    }

    [Test]
    public void TestSVDSolve()
    {
        double[,] _A = new double[3, 3] { { 1,2,3 },
                                        { 4,5,6 },
                                        { 7,8,9 } };

        double[,] _b = new double[3,1] { { 10 }, 
                                         { 11 },
                                         { 12 } };

        Matrix<double> a_x;
        DoLinAlgBurstedSVDSolve(_A, _b, out a_x, 10e-10f);

        Matrix<double> b_x;
        TruncatedSVDSolve(_A, _b, out b_x, 10e-10f);
        Debug.Log(a_x);
        Debug.Log(b_x);
        Assert.IsTrue(EqualsApproximate(a_x, b_x, 10e-10f));
    }

    [Test]
    public void TestSVDSolveTwoColumn()
    {
        double[,] _A = new double[3, 3] { { 1,2,3 },
                                        { 4,5,6 },
                                        { 7,8,9 } };

        double[,] _b = new double[3, 2] { { 10, 13 },
                                         { 11, 14 },
                                         { 12, 15 } };

        Matrix<double> a_x;
        DoLinAlgBurstedSVDSolve(_A, _b, out a_x, 10e-10f);

        Matrix<double> b_x;
        TruncatedSVDSolve(_A, _b, out b_x, 10e-10);

        Assert.IsTrue(EqualsApproximate(a_x, b_x, 10e-10f));
    }

}
