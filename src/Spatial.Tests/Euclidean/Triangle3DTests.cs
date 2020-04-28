using MathNet.Spatial.Euclidean;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MathNet.Spatial.UnitTests.Euclidean
{
    [TestFixture]
    public class Triangle3DTests
    {
        [Test]
        public void ConstructorTest()
        {
            var triangle = TestTriangle1();
            var checkList = new List<Point3D> { new Point3D(0, 0, 0), new Point3D(1, 1, 0), new Point3D(0.25, 0.5, 0) };
            CollectionAssert.AreEqual(checkList, triangle.Vertices);
        }

        [TestCase("0,0,0;1,0,0;2,0,0")]
        [TestCase("0,0,0;1,1,0;2,2,0")]
        [Test]
        public void CoplanarPointsTest(string stringpoints)
        {
            List<Point3D> points = (from x in stringpoints.Split(';') select Point3D.Parse(x)).ToList();
            Assert.Throws<ArgumentException>(() => new Triangle3D(points));
        }

        [TestCase("1,0,0;0,1,0;0,0,1", 0.86602540378443864676, "0.57735026918962576451,0.57735026918962576451,0.57735026918962576451")] // area = sqrt(3)/2, normal = {1/sqrt(3), 1/sqrt(3), 1/sqrt(3)}
        [TestCase("0,0,0;0,1,0;1,0,0", 0.5, "0,0,-1")]
        [TestCase("1,-2,0;3,-1,0;2,1,0", 2.5, "0,0,1")]
        public void TriangleAreaAndNormalTest(string stringpoints, double area, string unitvector)
        {
            List<Point3D> points = (from x in stringpoints.Split(';') select Point3D.Parse(x)).ToList();
            var triangle = new Triangle3D(points);

            Assert.AreEqual(area, triangle.Area);

            var normal = triangle.Normal;
            var p = UnitVector3D.Parse(unitvector);
            AssertGeometry.AreEqual(normal, p);

            var plane = new Plane(normal, normal.DotProduct(triangle.Vertices[0].ToVector3D()));
            var planeFromPoints = Plane.FromPoints(triangle.Vertices);
            AssertGeometry.AreEqual(plane, planeFromPoints);
        }

        [TestCase(1, 0, 0, true)] // on Vertex
        [TestCase(0, 1, 0, true)] // on Vertex
        [TestCase(0, 0, 1, true)] // on Vertex
        [TestCase(0.5, 0.5, 0, true)] // on Edge
        [TestCase(0, 0.5, 0.5, true)] // on Edge
        [TestCase(0.5, 0, 0.5, true)] // on Edge
        [TestCase(1d/3d, 1d/3d, 1d/3d, true)] // insdie
        [TestCase(1.0, 1.0, 0, false)]
        [TestCase(0.999999, 0, 0, false)]
        [TestCase(0, 0, 0, false)]
        [TestCase(1, 1, 1, false)]
        [TestCase(1d / 3d, 1d / 3d + 0.000001, 1d / 3d, false)]
        [TestCase(1d / 3d, 1d / 3d - 0.000001, 1d / 3d, false)]
        public void IsPointInTriangleTest(double x, double y, double z, bool outcome)
        {
            var testPoint = new Point3D(x, y, z);

            // test for counter-clockwise triangle
            var triangle2 = TestTriangle2();
            Assert.AreEqual(outcome, triangle2.Contains(testPoint));

            // test for clockwise triangle
            var trinagle3 = TestTriangle3();
            Assert.AreEqual(outcome, trinagle3.Contains(testPoint));
        }

        [Test]
        public void CircumCircleTest()
        {
            var triangle = TestTriangle2();
            var actual = triangle.CircumCircle();

            var expected = new Circle3D(new Point3D(1.0 / 3.0, 1.0 / 3.0, 1.0 / 3.0), triangle.Normal, Math.Sqrt(2.0 / 3.0));
            AssertGeometry.AreEqual(expected.CenterPoint, actual.CenterPoint);
            AssertHelpers.AlmostEqualRelative(expected.Radius, actual.Radius, 14);
        }

        [Test]
        public void InCircleTest()
        {
            var triangle = TestTriangle2();
            var actual = triangle.InCircle();

            var expected = new Circle3D(new Point3D(1.0 / 3.0, 1.0 / 3.0, 1.0 / 3.0), triangle.Normal, Math.Sqrt(2.0 / 3.0) / 2.0);
            AssertGeometry.AreEqual(expected.CenterPoint, actual.CenterPoint);
            AssertHelpers.AlmostEqualRelative(expected.Radius, actual.Radius, 14);
        }

        private static Triangle3D TestTriangle1()
        {
            var points = from x in new[] { "0,0,0", "1,1,0", "0.25,0.5,0" } select Point3D.Parse(x);
            return new Triangle3D(points);
        }

        private static Triangle3D TestTriangle2()
        {
            var points = from x in new[] { "1,0,0", "0,1,0", "0,0,1" } select Point3D.Parse(x);
            return new Triangle3D(points);
        }

        private static Triangle3D TestTriangle3()
        {
            var points = from x in new[] { "0,0,1", "0,1,0", "1,0,0" } select Point3D.Parse(x);
            return new Triangle3D(points);
        }
    }
}
