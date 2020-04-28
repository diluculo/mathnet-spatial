using MathNet.Numerics.LinearAlgebra;
using System;
using System.Diagnostics.Contracts;
using HashCode = MathNet.Spatial.Internals.HashCode;

namespace MathNet.Spatial.Euclidean
{
    /// <summary>
    /// Describes a standard 3 dimensional sphere
    /// </summary>
    [Serializable]
    public struct Sphere3D : IEquatable<Sphere3D>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sphere3D"/> struct.
        /// Creates a Sphere of a given radius from a center point
        /// </summary>
        /// <param name="center">The location of the center</param>
        /// <param name="radius">The radius of the sphere</param>
        public Sphere3D(Point3D center, double radius)
        {
            this.Center = center;
            this.Radius = radius;
        }

        /// <summary>
        /// Gets the center point of the sphere
        /// </summary>
        public Point3D Center { get; }

        /// <summary>
        /// Gets the radius of the sphere
        /// </summary>
        public double Radius { get; }

        /// <summary>
        /// Gets the circumference of the sphere
        /// </summary>
        [Pure]
        public double Circumference => 2 * this.Radius * Math.PI;

        /// <summary>
        /// Gets the diameter of the sphere
        /// </summary>
        [Pure]
        public double Diameter => 2 * this.Radius;

        /// <summary>
        /// Gets the surface area of the sphere
        /// </summary>
        [Pure]
        public double SurfaceArea => 4 * this.Radius * this.Radius * Math.PI;

        /// <summary>
        /// Gets the volume of the sphere
        /// </summary>
        [Pure]
        public double Volume => 4 * this.Radius * this.Radius * this.Radius * Math.PI / 3;

        /// <summary>
        /// Test whether a point is enclosed within the circle. 
        /// </summary>
        /// <param name="p">A point.</param>
        /// <param name="tolerance">A tolerance to account for floating point error.</param>
        /// <returns>True if the point is on or in the circle; otherwise false.</returns>
        public bool Contains(Point3D p, double tolerance = float.Epsilon)
        {
            var distance = (p - Center).Length;
            return distance <= Radius + tolerance;
        }

        /// <summary>
        /// Returns a value that indicates whether each pair of elements in two specified spheres is equal.
        /// </summary>
        /// <param name="left">The first sphere to compare.</param>
        /// <param name="right">The second sphere to compare.</param>
        /// <returns>True if the spheres are the same; otherwise false.</returns>
        public static bool operator ==(Sphere3D left, Sphere3D right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value that indicates whether any pair of elements in two specified spheres is not equal.
        /// </summary>
        /// <param name="left">The first sphere to compare.</param>
        /// <param name="right">The second sphere to compare</param>
        /// <returns>True if the spheres are different; otherwise false.</returns>
        public static bool operator !=(Sphere3D left, Sphere3D right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Creates a <see cref="Sphere3D"/> from four noncoplanar points.
        /// </summary>
        /// <param name="pointA">The first point on the sphere.</param>
        /// <param name="pointB">The second point on the sphere.</param>
        /// <param name="pointC">The third point on the sphere.</param>
        /// <param name="pointD">The fourth point on the sphere.</param>
        /// <returns>A sphere which is defined by the four specified points</returns>
        public static Sphere3D FromPoints(Point3D pointA, Point3D pointB, Point3D pointC, Point3D pointD)
        {
            // https://mathworld.wolfram.com/Sphere.html
            // https://mathworld.wolfram.com/Circumsphere.html
            //
            // A sphere containing 4 points is given by the detrminant equation
            //    | x^2 + y^2 + z^2     x   y   z   1 | = 0
            //    | x1^2 + y1^2 + z1^2  x1  y1  z1  1 | = 0
            //    | x2^2 + y2^2 + z2^2  x2  y2  z2  1 | = 0
            //    | x3^2 + y3^2 + z3^2  x3  y3  z3  1 | = 0
            //    | x4^2 + y4^2 + zc^2  x4  y4  z4  1 | = 0
            //
            // By using the Cofactor expansion, we can get the center and radius.
            
            var x1 = pointA.X; var y1 = pointA.Y; var z1 = pointA.Z;
            var x2 = pointB.X; var y2 = pointB.Y; var z2 = pointB.Z;
            var x3 = pointC.X; var y3 = pointC.Y; var z3 = pointC.Z;
            var x4 = pointD.X; var y4 = pointD.Y; var z4 = pointD.Z;

            var sq1 = x1 * x1 + y1 * y1 + z1 * z1;
            var sq2 = x2 * x2 + y2 * y2 + z2 * z2;
            var sq3 = x3 * x3 + y3 * y3 + z3 * z3;
            var sq4 = x4 * x4 + y4 * y4 + z4 * z4;

            var A = Matrix<double>.Build.DenseOfColumnMajor(4, 4,
                new[] { x1, x2, x3, x4, y1, y2, y3, y4, z1, z2, z3, z4, 1, 1, 1, 1 });
            var a = A.Determinant();

            if (Math.Abs(a).Equals(0))
            {
                throw new ArgumentException("A sphere cannot be created from these points, are they coplanar?");
            }

            var Dx = Matrix<double>.Build.DenseOfColumnMajor(4, 4,
                new[] { sq1, sq2, sq3, sq4, y1, y2, y3, y4, z1, z2, z3, z4, 1, 1, 1, 1 });
            var dx = Dx.Determinant();

            var Dy = Matrix<double>.Build.DenseOfColumnMajor(4, 4,
                new[] { x1, x2, x3, x4, sq1, sq2, sq3, sq4, z1, z2, z3, z4, 1, 1, 1, 1 });
            var dy = Dy.Determinant();

            var Dz = Matrix<double>.Build.DenseOfColumnMajor(4, 4,
                new[] { x1, x2, x3, x4, y1, y2, y3, y4, sq1, sq2, sq3, sq4, 1, 1, 1, 1 });
            var dz = Dz.Determinant();

            var C = Matrix<double>.Build.DenseOfColumnMajor(4, 4,
                new[] { sq1, sq2, sq3, sq4, x1, x2, x3, x4, y1, y2, y3, y4, z1, z2, z3, z4 });
            var c = C.Determinant();

            var center = new Point3D(dx / a / 2, dy / a / 2, dz / a / 2);
            var radius = Math.Sqrt(center.X * center.X + center.Y * center.Y + center.Z * center.Z - c / a);

            return new Sphere3D(center, radius);
        }

        /// <summary>
        /// Returns a value to indicate if a pair of spheres are equal
        /// </summary>
        /// <param name="c">The sphere to compare against.</param>
        /// <param name="tolerance">A tolerance (epsilon) to adjust for floating point error</param>
        /// <returns>true if the points are equal; otherwise false</returns>
        [Pure]
        public bool Equals(Sphere3D c, double tolerance)
        {
            if (tolerance < 0)
            {
                throw new ArgumentException("epsilon < 0");
            }

            return Math.Abs(c.Radius - this.Radius) < tolerance && this.Center.Equals(c.Center, tolerance);
        }

        /// <inheritdoc />
        [Pure]
        public bool Equals(Sphere3D c) => this.Radius.Equals(c.Radius) && this.Center.Equals(c.Center);

        /// <inheritdoc />
        [Pure]
        public override bool Equals(object obj) => obj is Sphere3D c && this.Equals(c);

        /// <inheritdoc />
        [Pure]
        public override int GetHashCode() => HashCode.Combine(this.Center, this.Radius);
    }
}
