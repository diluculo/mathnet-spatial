using MathNet.Spatial.Euclidean;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MathNet.Spatial.UnitTests.Euclidean
{
    [TestFixture]
    public class Triangle2DTests
    {
        [Test]
        public void ConstructorTest()
        {
            var triangle = TestTriangle1();
            var checkList = new List<Point2D> { new Point2D(0, 0), new Point2D(1, 1), new Point2D(0.25, 0.5) };
            CollectionAssert.AreEqual(checkList, triangle.Vertices);
        }

        [TestCase("0,0;1,0;2,0")]
        [TestCase("0,0;1,1;2,2")]
        [Test]
        public void ColinearPointsTest(string stringpoints)
        {
            List<Point2D> points = (from x in stringpoints.Split(';') select Point2D.Parse(x)).ToList();
            Assert.Throws<ArgumentException>(() => new Triangle2D(points));
        }

        [TestCase("0,0;1,0;0,1", 0.5, "0,0,1")] // counter-clockwise
        [TestCase("0,0;0,1;1,0", -0.5, "0,0,-1")] // clockwise
        [TestCase("1,-2;3,-1;2,1", 2.5, "0,0,1")]
        public void TriangleAreaAndNormalTest(string stringpoints, double area, string unitvector)
        {
            List<Point2D> points = (from x in stringpoints.Split(';') select Point2D.Parse(x)).ToList();
            var triangle = new Triangle2D(points);

            Assert.AreEqual(area, triangle.SignedArea);

            var normal = triangle.Normal;
            var p = UnitVector3D.Parse(unitvector);
            AssertGeometry.AreEqual(normal, p);
        }

        [TestCase(0, 0, true)] // on Vertex
        [TestCase(1, 0, true)] // on Vertex
        [TestCase(0, 1, true)] // on Vertex
        [TestCase(0.5, 0, true)] // on Edge
        [TestCase(0.5, 0.5, true)] // on Edge
        [TestCase(0, 0.5, true)] // on Edge
        [TestCase(0.2, 0.2, true)] // insdie
        [TestCase(1.0, 1.0, false)]
        [TestCase(-0.000001, -0.000001, false)]
        public void IsPointInTriangleTest(double x, double y, bool outcome)
        {
            var testPoint = new Point2D(x, y);

            // test for counter-clockwise triangle
            var triangle2 = TestTriangle2();
            Assert.AreEqual(outcome, triangle2.Contains(testPoint));

            // test for clockwise triangle
            var triangle3 = TestTriangle3();
            Assert.AreEqual(outcome, triangle3.Contains(testPoint));
        }

        [Test]
        public void CircumCircleTest()
        {
            var triangle = TestTriangle2();
            var actual = triangle.CircumCircle();

            var expected = new Circle2D(new Point2D(0.5, 0.5), 1.0 / Math.Sqrt(2.0));
            AssertGeometry.AreEqual(expected.Center, actual.Center);
            AssertHelpers.AlmostEqualRelative(expected.Radius, actual.Radius, 14);
        }

        [Test]
        public void InCircleTest()
        {
            var triangle = TestTriangle2();
            var actual = triangle.InCircle();

            var expected = new Circle2D(new Point2D(1.0 / (2.0 + Math.Sqrt(2.0)), 1.0 / (2.0 + Math.Sqrt(2.0))), Math.Sqrt(3.0 / 2.0 - Math.Sqrt(2.0)));
            AssertGeometry.AreEqual(expected.Center, actual.Center);
            AssertHelpers.AlmostEqualRelative(expected.Radius, actual.Radius, 14);
        }

        private static Triangle2D TestTriangle1()
        {
            var points = from x in new[] { "0,0", "1,1", "0.25,0.5"  } select Point2D.Parse(x);
            return new Triangle2D(points);
        }

        private static Triangle2D TestTriangle2()
        {
            var points = from x in new[] { "0,0", "1,0", "0,1" } select Point2D.Parse(x);
            return new Triangle2D(points);
        }

        private static Triangle2D TestTriangle3() // Clockwised TestTriangle2
        {
            var points = from x in new[] { "0,0", "0,1", "1,0" } select Point2D.Parse(x);
            return new Triangle2D(points);
        }
    }
}
