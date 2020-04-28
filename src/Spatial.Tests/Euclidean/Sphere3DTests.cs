using System;
using MathNet.Spatial.Euclidean;
using NUnit.Framework;

namespace MathNet.Spatial.UnitTests.Euclidean
{
    [TestFixture]
    public class Sphere3DTests
    {
        [TestCase("0, 0, 0", 2.5)]
        [TestCase("2, -4, 5", 4)]
        public void SphereCenterRadius(string p1s, double radius)
        {
            var center = Point3D.Parse(p1s);
            var sphere = new Sphere3D(center, radius);
            Assert.AreEqual(2 * radius, sphere.Diameter, double.Epsilon);
            Assert.AreEqual(2 * Math.PI * radius, sphere.Circumference, double.Epsilon);
            Assert.AreEqual(4 * Math.PI * radius * radius, sphere.SurfaceArea, double.Epsilon);
            Assert.AreEqual(4 * Math.PI * radius * radius * radius / 3, sphere.Volume, double.Epsilon);
        }

        [TestCase("0, 0, 0", 1)]
        [TestCase("2, -4, 5", 4.7)]
        public void SphereEquality(string center, double radius)
        {
            var cp = Point3D.Parse(center);
            var c = new Sphere3D(cp, radius);
            var c2 = new Sphere3D(cp, radius);
            Assert.True(c == c2);
            Assert.True(c.Equals(c2));
        }

        [TestCase("1,0,0", "0,1,0", "0,0,1", "-1,0,0", "0,0,0", 1)] // center = (0, 0, 0), radius = 1
        [TestCase("-5,4,1", "3,4,-5", "0,0,4", "0,0,0", "2,7.25,2", 7.7821912081366903407)] // center = (2, 29/4, 2), radius = sqrt(969)/4
        [TestCase("4,-1,2", "0,-2,3", "1,-5,-1", "2,0,1", "2,-3,1", 3)] // center = (2, -3, 1), radius = 3
        public void SphereFromFourPoints(string p1s, string p2s, string p3s, string p4s, string centers, double radius)
        {
            var p1 = Point3D.Parse(p1s);
            var p2 = Point3D.Parse(p2s);
            var p3 = Point3D.Parse(p3s);
            var p4 = Point3D.Parse(p4s);
            var center = Point3D.Parse(centers);

            var sphere = Sphere3D.FromPoints(p1, p2, p3, p4);

            AssertGeometry.AreEqual(center, sphere.Center);
            Assert.AreEqual(radius, sphere.Radius, 1e-6);
        }

        [Test]
        public void SphereFromFourPointsArgumentException()
        {
            var p1 = new Point3D(0, 0, 0);
            var p2 = new Point3D(-1, 0, 0);
            var p3 = new Point3D(1, 0, 0);
            var p4 = new Point3D(1, 1, 1);

            Assert.Throws<ArgumentException>(() => { Sphere3D.FromPoints(p1, p2, p3, p4); });
        }
    }
}
