using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathNet.Spatial.UnitTests
{
    using Complex = System.Numerics.Complex;

    /// <summary>
    /// A class which includes some assertion helper methods particularly for numerical code.
    /// </summary>
    public static class AssertHelpers
    {
        public static void AlmostEqual(Complex expected, Complex actual)
        {
            if (expected.IsNaN() && actual.IsNaN() || expected.IsInfinity() && expected.IsInfinity())
            {
                return;
            }

            if (!expected.Real.AlmostEqual(actual.Real))
            {
                Assert.Fail("Real components are not equal. Expected:{0}; Actual:{1}", expected.Real, actual.Real);
            }

            if (!expected.Imaginary.AlmostEqual(actual.Imaginary))
            {
                Assert.Fail("Imaginary components are not equal. Expected:{0}; Actual:{1}", expected.Imaginary, actual.Imaginary);
            }
        }

        public static void AlmostEqual(Complex32 expected, Complex32 actual)
        {
            if (expected.IsNaN() && actual.IsNaN() || expected.IsInfinity() && expected.IsInfinity())
            {
                return;
            }

            if (!expected.Real.AlmostEqual(actual.Real))
            {
                Assert.Fail("Real components are not equal. Expected:{0}; Actual:{1}", expected.Real, actual.Real);
            }

            if (!expected.Imaginary.AlmostEqual(actual.Imaginary))
            {
                Assert.Fail("Imaginary components are not equal. Expected:{0}; Actual:{1}", expected.Imaginary, actual.Imaginary);
            }
        }


        public static void AlmostEqual(double expected, double actual, int decimalPlaces)
        {
            if (double.IsNaN(expected) && double.IsNaN(actual))
            {
                return;
            }

            if (!expected.AlmostEqual(actual, decimalPlaces))
            {
                Assert.Fail("Not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected, actual);
            }
        }

        public static void AlmostEqual(float expected, float actual, int decimalPlaces)
        {
            if (float.IsNaN(expected) && float.IsNaN(actual))
            {
                return;
            }

            if (!expected.AlmostEqual(actual, decimalPlaces))
            {
                Assert.Fail("Not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected, actual);
            }
        }

        public static void AlmostEqual(Complex expected, Complex actual, int decimalPlaces)
        {
            if (!expected.Real.AlmostEqual(actual.Real, decimalPlaces))
            {
                Assert.Fail("Real components are not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected.Real, actual.Real);
            }

            if (!expected.Imaginary.AlmostEqual(actual.Imaginary, decimalPlaces))
            {
                Assert.Fail("Imaginary components are not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected.Imaginary, actual.Imaginary);
            }
        }

        public static void AlmostEqual(Complex32 expected, Complex32 actual, int decimalPlaces)
        {
            if (!expected.Real.AlmostEqual(actual.Real, decimalPlaces))
            {
                Assert.Fail("Real components are not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected.Real, actual.Real);
            }

            if (!expected.Imaginary.AlmostEqual(actual.Imaginary, decimalPlaces))
            {
                Assert.Fail("Imaginary components are not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected.Imaginary, actual.Imaginary);
            }
        }

        public static void AlmostEqualRelative(double expected, double actual, int decimalPlaces)
        {
            if (double.IsNaN(expected) && double.IsNaN(actual))
            {
                return;
            }

            if (!expected.AlmostEqualRelative(actual, decimalPlaces))
            {
                Assert.Fail("Not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected, actual);
            }
        }

        public static void AlmostEqualRelative(float expected, float actual, int decimalPlaces)
        {
            if (float.IsNaN(expected) && float.IsNaN(actual))
            {
                return;
            }

            if (!expected.AlmostEqualRelative(actual, decimalPlaces))
            {
                Assert.Fail("Not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected, actual);
            }
        }

        public static void AlmostEqualRelative(Complex expected, Complex actual, int decimalPlaces)
        {
            if (!expected.Real.AlmostEqualRelative(actual.Real, decimalPlaces))
            {
                Assert.Fail("Real components are not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected.Real, actual.Real);
            }

            if (!expected.Imaginary.AlmostEqualRelative(actual.Imaginary, decimalPlaces))
            {
                Assert.Fail("Imaginary components are not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected.Imaginary, actual.Imaginary);
            }
        }

        public static void AlmostEqualRelative(Complex32 expected, Complex32 actual, int decimalPlaces)
        {
            if (!expected.Real.AlmostEqualRelative(actual.Real, decimalPlaces))
            {
                Assert.Fail("Real components are not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected.Real, actual.Real);
            }

            if (!expected.Imaginary.AlmostEqualRelative(actual.Imaginary, decimalPlaces))
            {
                Assert.Fail("Imaginary components are not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected.Imaginary, actual.Imaginary);
            }
        }


        public static void AlmostEqual(IList<double> expected, IList<double> actual, int decimalPlaces)
        {
            for (var i = 0; i < expected.Count; i++)
            {
                if (!actual[i].AlmostEqual(expected[i], decimalPlaces))
                {
                    Assert.Fail("Not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected[i], actual[i]);
                }
            }
        }

        public static void AlmostEqual(IList<float> expected, IList<float> actual, int decimalPlaces)
        {
            for (var i = 0; i < expected.Count; i++)
            {
                if (!actual[i].AlmostEqual(expected[i], decimalPlaces))
                {
                    Assert.Fail("Not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected[i], actual[i]);
                }
            }
        }

        public static void AlmostEqual(IList<Complex> expected, IList<Complex> actual, int decimalPlaces)
        {
            for (var i = 0; i < expected.Count; i++)
            {
                if (!actual[i].AlmostEqual(expected[i], decimalPlaces))
                {
                    Assert.Fail("Not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected[i], actual[i]);
                }
            }
        }

        public static void AlmostEqual(IList<Complex32> expected, IList<Complex32> actual, int decimalPlaces)
        {
            for (var i = 0; i < expected.Count; i++)
            {
                if (!actual[i].AlmostEqual(expected[i], decimalPlaces))
                {
                    Assert.Fail("Not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected[i], actual[i]);
                }
            }
        }

        public static void AlmostEqualRelative(IList<double> expected, IList<double> actual, int decimalPlaces)
        {
            for (var i = 0; i < expected.Count; i++)
            {
                if (!actual[i].AlmostEqualRelative(expected[i], decimalPlaces))
                {
                    Assert.Fail("Not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected[i], actual[i]);
                }
            }
        }

        public static void AlmostEqualRelative(IList<float> expected, IList<float> actual, int decimalPlaces)
        {
            for (var i = 0; i < expected.Count; i++)
            {
                if (!actual[i].AlmostEqualRelative(expected[i], decimalPlaces))
                {
                    Assert.Fail("Not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected[i], actual[i]);
                }
            }
        }

        public static void AlmostEqualRelative(IList<Complex> expected, IList<Complex> actual, int decimalPlaces)
        {
            for (var i = 0; i < expected.Count; i++)
            {
                if (!actual[i].AlmostEqualRelative(expected[i], decimalPlaces))
                {
                    Assert.Fail("Not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected[i], actual[i]);
                }
            }
        }

        public static void AlmostEqualRelative(IList<Complex32> expected, IList<Complex32> actual, int decimalPlaces)
        {
            for (var i = 0; i < expected.Count; i++)
            {
                if (!actual[i].AlmostEqualRelative(expected[i], decimalPlaces))
                {
                    Assert.Fail("Not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected[i], actual[i]);
                }
            }
        }

        public static void AlmostEqual(Matrix<double> expected, Matrix<double> actual, int decimalPlaces)
        {
            if (expected.ColumnCount != actual.ColumnCount || expected.RowCount != actual.RowCount)
            {
                Assert.Fail("Matrix dimensions mismatch. Expected: {0}; Actual: {1}", expected.ToTypeString(), actual.ToTypeString());
            }

            for (var i = 0; i < expected.RowCount; i++)
            {
                for (var j = 0; j < expected.ColumnCount; j++)
                {
                    if (!actual.At(i, j).AlmostEqual(expected.At(i, j), decimalPlaces))
                    {
                        Assert.Fail("Not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected.At(i, j), actual.At(i, j));
                    }
                }
            }
        }

        public static void AlmostEqual(Matrix<float> expected, Matrix<float> actual, int decimalPlaces)
        {
            if (expected.ColumnCount != actual.ColumnCount || expected.RowCount != actual.RowCount)
            {
                Assert.Fail("Matrix dimensions mismatch. Expected: {0}; Actual: {1}", expected.ToTypeString(), actual.ToTypeString());
            }

            for (var i = 0; i < expected.RowCount; i++)
            {
                for (var j = 0; j < expected.ColumnCount; j++)
                {
                    if (!actual.At(i, j).AlmostEqual(expected.At(i, j), decimalPlaces))
                    {
                        Assert.Fail("Not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected.At(i, j), actual.At(i, j));
                    }
                }
            }
        }

        public static void AlmostEqual(Matrix<Complex> expected, Matrix<Complex> actual, int decimalPlaces)
        {
            if (expected.ColumnCount != actual.ColumnCount || expected.RowCount != actual.RowCount)
            {
                Assert.Fail("Matrix dimensions mismatch. Expected: {0}; Actual: {1}", expected.ToTypeString(), actual.ToTypeString());
            }

            for (var i = 0; i < expected.RowCount; i++)
            {
                for (var j = 0; j < expected.ColumnCount; j++)
                {
                    if (!actual.At(i, j).AlmostEqual(expected.At(i, j), decimalPlaces))
                    {
                        Assert.Fail("Not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected.At(i, j), actual.At(i, j));
                    }
                }
            }
        }

        public static void AlmostEqual(Matrix<Complex32> expected, Matrix<Complex32> actual, int decimalPlaces)
        {
            if (expected.ColumnCount != actual.ColumnCount || expected.RowCount != actual.RowCount)
            {
                Assert.Fail("Matrix dimensions mismatch. Expected: {0}; Actual: {1}", expected.ToTypeString(), actual.ToTypeString());
            }

            for (var i = 0; i < expected.RowCount; i++)
            {
                for (var j = 0; j < expected.ColumnCount; j++)
                {
                    if (!actual.At(i, j).AlmostEqual(expected.At(i, j), decimalPlaces))
                    {
                        Assert.Fail("Not equal within {0} places. Expected:{1}; Actual:{2}", decimalPlaces, expected.At(i, j), actual.At(i, j));
                    }
                }
            }
        }

        public static void AlmostEqualRelative(Matrix<double> expected, Matrix<double> actual, int decimalPlaces)
        {
            if (expected.ColumnCount != actual.ColumnCount || expected.RowCount != actual.RowCount)
            {
                Assert.Fail("Matrix dimensions mismatch. Expected: {0}; Actual: {1}", expected.ToTypeString(), actual.ToTypeString());
            }

            for (var i = 0; i < expected.RowCount; i++)
            {
                for (var j = 0; j < expected.ColumnCount; j++)
                {
                    if (!actual.At(i, j).AlmostEqualRelative(expected.At(i, j), decimalPlaces))
                    {
                        Assert.Fail("Not equal within {0} relative places. Expected:{1}; Actual:{2}", decimalPlaces, expected.At(i, j), actual.At(i, j));
                    }
                }
            }
        }

        public static void AlmostEqualRelative(Matrix<float> expected, Matrix<float> actual, int decimalPlaces)
        {
            if (expected.ColumnCount != actual.ColumnCount || expected.RowCount != actual.RowCount)
            {
                Assert.Fail("Matrix dimensions mismatch. Expected: {0}; Actual: {1}", expected.ToTypeString(), actual.ToTypeString());
            }

            for (var i = 0; i < expected.RowCount; i++)
            {
                for (var j = 0; j < expected.ColumnCount; j++)
                {
                    if (!actual.At(i, j).AlmostEqualRelative(expected.At(i, j), decimalPlaces))
                    {
                        Assert.Fail("Not equal within {0} relative places. Expected:{1}; Actual:{2}", decimalPlaces, expected.At(i, j), actual.At(i, j));
                    }
                }
            }
        }

        public static void AlmostEqualRelative(Matrix<Complex> expected, Matrix<Complex> actual, int decimalPlaces)
        {
            if (expected.ColumnCount != actual.ColumnCount || expected.RowCount != actual.RowCount)
            {
                Assert.Fail("Matrix dimensions mismatch. Expected: {0}; Actual: {1}", expected.ToTypeString(), actual.ToTypeString());
            }

            for (var i = 0; i < expected.RowCount; i++)
            {
                for (var j = 0; j < expected.ColumnCount; j++)
                {
                    if (!actual.At(i, j).AlmostEqualRelative(expected.At(i, j), decimalPlaces))
                    {
                        Assert.Fail("Not equal within {0} relative places. Expected:{1}; Actual:{2}", decimalPlaces, expected.At(i, j), actual.At(i, j));
                    }
                }
            }
        }

        public static void AlmostEqualRelative(Matrix<Complex32> expected, Matrix<Complex32> actual, int decimalPlaces)
        {
            if (expected.ColumnCount != actual.ColumnCount || expected.RowCount != actual.RowCount)
            {
                Assert.Fail("Matrix dimensions mismatch. Expected: {0}; Actual: {1}", expected.ToTypeString(), actual.ToTypeString());
            }

            for (var i = 0; i < expected.RowCount; i++)
            {
                for (var j = 0; j < expected.ColumnCount; j++)
                {
                    if (!actual.At(i, j).AlmostEqualRelative(expected.At(i, j), decimalPlaces))
                    {
                        Assert.Fail("Not equal within {0} relative places. Expected:{1}; Actual:{2}", decimalPlaces, expected.At(i, j), actual.At(i, j));
                    }
                }
            }
        }
    }
}
