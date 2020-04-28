using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using HashCode = MathNet.Spatial.Internals.HashCode;

namespace MathNet.Spatial.Euclidean
{
    /// <summary>
    /// Describes a 3 dimensional triangle.
    /// </summary>
    [Serializable]
    public struct Triangle3D : IEquatable<Triangle3D>
    {
        /// <summary>
        /// A list of vertices.
        /// </summary>
        public Point3D[] Vertices { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Triangle3D"/> struct.
        /// </summary>
        /// <param name="points">the vertices of the triangle.</param>
        public Triangle3D(params Point3D[] points)
        {
            if (points.Length != 3)
            {
                throw new ArgumentException("Three vertices are required for a Triangle3D");
            }

            if (points[0] == points[1] || points[1] == points[2] || points[2] == points[0])
            {
                throw new ArgumentException("The any of vertices of the Triangle3D cannot be identical");
            }

            // 2*Area = |(B-A)x(C-A)|
            var area = (points[1] - points[0]).CrossProduct(points[2] - points[0]).Length / 2;
            if (area == 0)
            {
                throw new ArgumentException("The vertices of the Triangle3D cannot lie on the same line.");
            }

            this.Vertices = points;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Triangle3D"/> struct.
        /// </summary>
        /// <param name="points">the vertices of the triangle.</param>
        public Triangle3D(IEnumerable<Point3D> points) : this(points.ToArray())
        { }

        /// <summary>
        /// Gets the area of the triangle.
        /// </summary>
        [Pure]
        public double Area => (Vertices[1] - Vertices[0]).CrossProduct(Vertices[2] - Vertices[0]).Length / 2;

        /// <summary>
        /// Gets the unit normal vector of the triangle
        /// </summary>
        [Pure]
        public UnitVector3D Normal => UnitVector3D.OfVector((Vertices[1] - Vertices[0]).CrossProduct(Vertices[2] - Vertices[0]).ToVector());

        /// <summary>
        /// Returns a plane on which the triangle lies.
        /// </summary>
        /// <returns>A plane on which the triangle lies</returns>
        [Pure]
        public Plane Plane() => new Plane(Normal, Normal.DotProduct(Vertices[0]));

        /// <summary>
        /// Returns a circumscribed circle of the triangle.
        /// </summary>
        /// <returns>A circumcircle of the triangle</returns>
        [Pure]
        public Circle3D CircumCircle() => Circle3D.FromPoints(Vertices[0], Vertices[1], Vertices[2]);

        /// <summary>
        /// Returns a inscribed circle of the triangle.
        /// </summary>
        /// <returns>A incircle of the triangle</returns>
        [Pure]
        public Circle3D InCircle()
        {
            var a = Vertices[1].DistanceTo(Vertices[2]);
            var b = Vertices[2].DistanceTo(Vertices[0]);
            var c = Vertices[0].DistanceTo(Vertices[1]);
            var l = a + b + c;

            var centerX = (a * Vertices[0].X + b * Vertices[1].X + c * Vertices[2].X) / l;
            var centerY = (a * Vertices[0].Y + b * Vertices[1].Y + c * Vertices[2].Y) / l;
            var centerZ = (a * Vertices[0].Z + b * Vertices[1].Z + c * Vertices[2].Z) / l;
            var center = new Point3D(centerX, centerY, centerZ);
            var r = Math.Sqrt((-a + b + c) * (a + b - c) * (a - b + c) / l) / 2;

            return new Circle3D(center, Normal, r);
        }

        /// <summary>
        /// Test whether a point is enclosed within a triangle. 
        /// </summary>
        /// <param name="p">A point.</param>
        /// <param name="tolerance">A tolerance to account for floating point error.</param>
        /// <returns>True if the point is on vertices, on edges, or inside the triangle; otherwise false.</returns>
        public bool Contains(Point3D p, double tolerance = float.Epsilon)
        {
            // https://mathworld.wolfram.com/BarycentricCoordinates.html
            //
            // a point P can be described with a triangle A-B-C
            //    P = t1*A + t2*B + t3*C where t1 + t2 + t3 = 1
            // P is inside of ABC if 0 <= t1, t2, t3 <= 1
            // where
            //    t1 = (area of BCP)/area 
            //    t2 = (area of CAP)/area 
            //    t3 = (area of ABP)/area 

            if (tolerance < 0)
            {
                throw new ArgumentException("epsilon < 0");
            }

            // TODO: add a BoundingBox for easy containg test.
            if (p.X < Vertices.Select(v => v.X).Min() - tolerance
                && p.Y < Vertices.Select(v => v.Y).Min() - tolerance
                && p.Z < Vertices.Select(v => v.Z).Min() - tolerance)
            {
                return false; // outside
            }
            if (p.X > Vertices.Select(v => v.X).Max() + tolerance
                && p.Y > Vertices.Select(v => v.Y).Max() + tolerance
                && p.Z > Vertices.Select(v => v.Z).Max() + tolerance)
            {
                return false; // outside
            }
            
            var PA = p - Vertices[0]; if (PA.Length <= tolerance) return true; // on vertex
            var PB = p - Vertices[1]; if (PB.Length <= tolerance) return true; // on vertex
            var PC = p - Vertices[2]; if (PC.Length <= tolerance) return true; // on vertex
                      
            var CB = Vertices[2] - Vertices[1];
            var AC = Vertices[0] - Vertices[2];
            var BA = Vertices[1] - Vertices[0];

            var s = new double[3];
            s[0] = CB.CrossProduct(PB).Length / (2d * Area); if (s[0] <= -tolerance) return false;
            s[1] = AC.CrossProduct(PC).Length / (2d * Area); if (s[1] <= -tolerance) return false;
            s[2] = BA.CrossProduct(PA).Length / (2d * Area); if (s[2] <= -tolerance) return false;

            return (1.0 - s.Sum()) >= -tolerance; // non-coplanar
        }

        /// <summary>
        /// Returns a value to indicate if a pair of triangles are equal
        /// </summary>
        /// <param name="other">The triangle to compare against.</param>
        /// <param name="tolerance">A tolerance (epsilon) to adjust for floating point error</param>
        /// <returns>true if the triangles are equal; otherwise false</returns>
        [Pure]
        public bool Equals(Triangle3D other, double tolerance)
        {
            if (tolerance < 0)
            {
                throw new ArgumentException("epsilon < 0");
            }

            for (var i = 0; i < this.Vertices.Length; i++)
            {
                if (!this.Vertices[i].Equals(other.Vertices[i], tolerance))
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc />
        [Pure]
        public bool Equals(Triangle3D c) => this.Vertices[0].Equals(c.Vertices[0]) && this.Vertices[1].Equals(c.Vertices[1]) && this.Vertices[2].Equals(c.Vertices[2]);

        /// <inheritdoc />
        [Pure]
        public override bool Equals(object obj) => obj is Triangle3D c && this.Equals(c);

        /// <inheritdoc />
        [Pure]
        public override int GetHashCode() => HashCode.Combine(this.Vertices[0], this.Vertices[1], this.Vertices[2]);
        
        /// <summary>
        /// Returns a value that indicates whether each pair of elements in two specified triangles is equal.
        /// </summary>
        /// <param name="left">The first triangle to compare.</param>
        /// <param name="right">The second triangle to compare.</param>
        /// <returns>True if the triangles are the same; otherwise false.</returns>
        public static bool operator ==(Triangle3D left, Triangle3D right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value that indicates whether any pair of elements in two specified triangles is not equal.
        /// </summary>
        /// <param name="left">The first triangle to compare.</param>
        /// <param name="right">The second triangle to compare</param>
        /// <returns>True if the triangles are different; otherwise false.</returns>
        public static bool operator !=(Triangle3D left, Triangle3D right)
        {
            return !left.Equals(right);
        }
    }
}
