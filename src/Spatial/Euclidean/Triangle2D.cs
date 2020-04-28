using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using HashCode = MathNet.Spatial.Internals.HashCode;

namespace MathNet.Spatial.Euclidean
{
    /// <summary>
    /// Describes a 2 dimensional triangle.
    /// </summary>
    [Serializable]
    public struct Triangle2D : IEquatable<Triangle2D>
    {
        /// <summary>
        /// A list of vertices.
        /// </summary>
        public Point2D[] Vertices { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Triangle2D"/> struct.
        /// </summary>
        /// <param name="points">the vertices of the triangle.</param>
        public Triangle2D(params Point2D[] points)
        {
            if (points.Length != 3)
            {
                throw new ArgumentException("Three vertices are required for a Triangle2D");
            }

            if (points[0] == points[1] || points[1] == points[2] || points[2] == points[0])
            {
                throw new ArgumentException("The any of vertices of the Triangle2D cannot be identical");
            }

            // 2*Area = |(B-A)x(C-A)| = | x1 y1 1 | = x21*y31 - y21*x31
            //                          | x2 y2 1 |
            //                          | x3 y3 1 |

            var area = (points[1] - points[0]).CrossProduct(points[2] - points[0]) / 2;
            if (area == 0)
            {
                throw new ArgumentException("The vertices of the Triangle2D cannot lie on the same line.");
            }

            this.Vertices = points;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Triangle2D"/> struct.
        /// </summary>
        /// <param name="points">the vertices of the triangle.</param>
        public Triangle2D(IEnumerable<Point2D> points) : this(points.ToArray())
        { }

        /// <summary>
        /// Gets the signed area of the triangle.
        /// The area is positive if the vertices are located sequentially in the counter-clockwise direction.
        /// </summary>
        [Pure]
        public double SignedArea => (Vertices[1] - Vertices[0]).CrossProduct(Vertices[2] - Vertices[0]) / 2;

        /// <summary>
        /// Gets the area of the triangle.
        /// </summary>
        [Pure]
        public double Area => Math.Abs(SignedArea);

        /// <summary>
        /// Gets the unit normal vector of the triangle
        /// </summary>
        [Pure]
        public UnitVector3D Normal => UnitVector3D.Create(0, 0, SignedArea);

        /// <summary>
        /// Returns a circumscribed circle of the triangle.
        /// </summary>
        /// <returns>A circumcircle of the triangle</returns>
        [Pure]
        public Circle2D CircumCircle() => Circle2D.FromPoints(Vertices[0], Vertices[1], Vertices[2]);

        /// <summary>
        /// Returns an inscribed circle of the triangle.
        /// </summary>
        /// <returns>An incircle of the triangle</returns>
        [Pure]
        public Circle2D InCircle()
        {
            var a = Vertices[1].DistanceTo(Vertices[2]);
            var b = Vertices[2].DistanceTo(Vertices[0]);
            var c = Vertices[0].DistanceTo(Vertices[1]);
            var l = a + b + c;

            var centerX = (a * Vertices[0].X + b * Vertices[1].X + c * Vertices[2].X) / l;
            var centerY = (a * Vertices[0].Y + b * Vertices[1].Y + c * Vertices[2].Y) / l;
            var center = new Point2D(centerX, centerY);

            var r = Math.Sqrt((-a + b + c) * (a + b - c) * (a - b + c) / l) / 2d;

            return new Circle2D(center, r);
        }

        /// <summary>
        /// Test whether a point is enclosed within a triangle. 
        /// </summary>
        /// <param name="p">A point.</param>
        /// <param name="tolerance">A tolerance to account for floating point error.</param>
        /// <returns>True if the point is on vertices, on edges, or inside the triangle; otherwise false.</returns>
        public bool Contains(Point2D p, double tolerance = float.Epsilon)
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
                && p.Y < Vertices.Select(v => v.Y).Min() - tolerance)
            {
                return false;
            }
            if (p.X > Vertices.Select(v => v.X).Max() + tolerance
                && p.Y > Vertices.Select(v => v.Y).Max() + tolerance)
            {
                return false;
            }

            var PA = p - Vertices[0]; if (PA.Length <= tolerance) return true; // on vertex
            var PB = p - Vertices[1]; if (PB.Length <= tolerance) return true; // on vertex
            var PC = p - Vertices[2]; if (PC.Length <= tolerance) return true; // on vertex

            var CB = Vertices[2] - Vertices[1];
            var AC = Vertices[0] - Vertices[2];
            var BA = Vertices[1] - Vertices[0];

            var t = new double[3];
            t[0] = CB.CrossProduct(PB) / (2d * SignedArea); if (t[0] <= -tolerance) return false; // outside
            t[1] = AC.CrossProduct(PC) / (2d * SignedArea); if (t[1] <= -tolerance) return false; // outside
            t[2] = BA.CrossProduct(PA) / (2d * SignedArea); if (t[2] <= -tolerance) return false; // outside

            // TODO: Identify the 'on edge' and 'inside' cases
            // if (t.Min() <= tolerance) return true // on edge                
            return true;
        }

        /// <summary>
        /// Returns a value to indicate if a pair of triangles are equal
        /// </summary>
        /// <param name="other">The triangle to compare against.</param>
        /// <param name="tolerance">A tolerance (epsilon) to adjust for floating point error</param>
        /// <returns>true if the triangles are equal; otherwise false</returns>
        [Pure]
        public bool Equals(Triangle2D other, double tolerance)
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
        public bool Equals(Triangle2D c) => this.Vertices[0].Equals(c.Vertices[0]) && this.Vertices[1].Equals(c.Vertices[1]) && this.Vertices[2].Equals(c.Vertices[2]);

        /// <inheritdoc />
        [Pure]
        public override bool Equals(object obj) => obj is Triangle2D c && this.Equals(c);

        /// <inheritdoc />
        [Pure]
        public override int GetHashCode() => HashCode.Combine(this.Vertices[0], this.Vertices[1], this.Vertices[2]);

        /// <summary>
        /// Returns a value that indicates whether each pair of elements in two specified triangles is equal.
        /// </summary>
        /// <param name="left">The first triangle to compare.</param>
        /// <param name="right">The second triangle to compare.</param>
        /// <returns>True if the triangles are the same; otherwise false.</returns>
        public static bool operator ==(Triangle2D left, Triangle2D right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value that indicates whether any pair of elements in two specified triangles is not equal.
        /// </summary>
        /// <param name="left">The first triangle to compare.</param>
        /// <param name="right">The second triangle to compare</param>
        /// <returns>True if the triangles are different; otherwise false.</returns>
        public static bool operator !=(Triangle2D left, Triangle2D right)
        {
            return !left.Equals(right);
        }
    }
}
