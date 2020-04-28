using MathNet.Spatial.Euclidean;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MathNet.Spatial.UnitTests.Euclidean
{
    [TestFixture]
    public class Tetrahedron3DTests
    {
        [Test]
        public void ConstructorTest()
        {
            var tetrahedron = TestTetrahedron();
            var checkList = new List<Point3D> { new Point3D(1, 1, 1), new Point3D(-1, -1, 1), new Point3D(-1, 1, -1), new Point3D(1, -1, -1) };
            CollectionAssert.AreEqual(checkList, tetrahedron.Vertices);
        }

        [TestCase("0,0,0;1,0,0;2,0,0;1,1,1")]
        [Test]
        public void CoplanerPointsTest(string stringpoints)
        {
            List<Point3D> points = (from x in stringpoints.Split(';') select Point3D.Parse(x)).ToList();
            Assert.Throws<ArgumentException>(() => new Tetrahedron3D(points));
        }

        [TestCase("1,0,0;0,1,0;0,0,1;0,0,0", 1d / 6d)] 
        [TestCase("1,2,3;2,4,4;4,1,2;3,2,5", 8d / 3d)] 
        [TestCase("1,2,3;2,2,3;1,3,3;1,2,9", -1)]
        public void TetrahedronVolumeTest(string stringpoints, double volume)
        {
            List<Point3D> points = (from x in stringpoints.Split(';') select Point3D.Parse(x)).ToList();
            var tetrahedron = new Tetrahedron3D(points);

            Assert.AreEqual(volume, tetrahedron.SignedVolume);
        }

        [TestCase("0,0,0;1,0,0;0,1,0;0,0,1", 0.5, 0.5, 0.5, 0.86602540378443864676)] // c = (0.5, 0.5, 0.5), r = sqrt(3)/2
        [TestCase("-15,0,0;15,0,0;0,10,0.01;0,-10,0.01", 0, 0, -6249.995, 6250.0129999884794)] // c = (0.5, 0.5, 0.5), r = sqrt(3)/2
        public void TriangleCircumSpereTest(string stringpoints, double x, double y, double  z, double r)
        {
            List<Point3D> points = (from p in stringpoints.Split(';') select Point3D.Parse(p)).ToList();
            var tetrahedron = new Tetrahedron3D(points);

            var circumSphere = tetrahedron.CircumSphere();

            Assert.AreEqual(x, circumSphere.Center.X);
            Assert.AreEqual(y, circumSphere.Center.Y);
            Assert.AreEqual(z, circumSphere.Center.Z);
            Assert.AreEqual(r, circumSphere.Radius);
        }

        [TestCase(0, 0, 0, true)] // on Vertex
        [TestCase(1, 0, 0, true)] // on Vertex
        [TestCase(0, 1, 0, true)] // on Vertex
        [TestCase(0, 0, 1, true)] // on Vertex
        [TestCase(0.5, 0, 0, true)] // on Edge
        [TestCase(0.5, 0.5, 0, true)] // on Edge
        [TestCase(0, 0.5, 0, true)] // on Edge
        [TestCase(0.2, 0.2, 0.2, true)] // inside
        [TestCase(1.0, 1.0, 1.0, false)]
        [TestCase(-0.000001, -0.000001, -0.000001, false)]
        public void IsPointInTetrahedronTest(double x, double y, double z, bool outcome)
        {
            var testPoint = new Point3D(x, y, z);
            var testTriangle = TestTetrahedron2();

            Assert.AreEqual(outcome, testTriangle.Contains(testPoint));
        }

        private static Tetrahedron3D TestTetrahedron()
        {
            var points = from x in new[] { "1,1,1", "-1,-1,1", "-1,1,-1", "1,-1,-1" } select Point3D.Parse(x);
            return new Tetrahedron3D(points);
        }

        private static Tetrahedron3D TestTetrahedron2()
        {
            var points = from x in new[] { "0,0,0", "1,0,0", "0,1,0", "0,0,1" } select Point3D.Parse(x);
            return new Tetrahedron3D(points);
        }
    }
}

