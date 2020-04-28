using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using HashCode = MathNet.Spatial.Internals.HashCode;

namespace MathNet.Spatial.Euclidean
{
    /// <summary>
    /// Describes a 3 dimensional tetrahedron.
    /// </summary>
    [Serializable]
    public struct Tetrahedron3D : IEquatable<Tetrahedron3D>
    {
        /// <summary>
        /// A list of vertices.
        /// </summary>
        public Point3D[] Vertices { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tetrahedron3D"/> struct.
        /// </summary>
        /// <param name="points">the vertices of the triangle.</param>
        public Tetrahedron3D(params Point3D[] points)
        {
            if (points.Length != 4)
            {
                throw new ArgumentException("Four vertices are required for a Tetrahedron3D");
            }

            if (points[0] == points[1] || points[0] == points[2] || points[0] == points[3] ||
                points[1] == points[2] || points[1] == points[3] || points[2] == points[3])
            {
                throw new ArgumentException("The any of vertices of the Tetrahedron3D cannot be identical");
            }

            // 6*Volume = |J| = |(A-D)∙(B-D)x(C-D)| = | x1 y1 z1 1 |
            //                                        | x2 y2 z2 1 |
            //                                        | x3 y3 z3 1 |
            //                                        | x4 y4 z4 1 |

            //var x1 = points[0].X; var y1 = points[0].Y; var z1 = points[0].Z;
            //var x2 = points[1].X; var y2 = points[1].Y; var z2 = points[1].Z;
            //var x3 = points[2].X; var y3 = points[2].Y; var z3 = points[2].Z;
            //var x4 = points[3].X; var y4 = points[3].Y; var z4 = points[3].Z;

            //var J = - (z1 - z4) * (y2 - y4) * (x3 - x4) + (y1 - y4) * (z2 - z4) * (x3 - x4)
            //        + (z1 - z4) * (x2 - x4) * (y3 - y4) - (x1 - x4) * (z2 - z4) * (y3 - y4)
            //        - (y1 - y4) * (x2 - x4) * (z3 - z4) + (x1 - x4) * (y2 - y4) * (z3 - z4);

            var AD = (points[0] - points[3]);
            var BD = (points[1] - points[3]);
            var CD = (points[2] - points[3]);
            var J = AD.DotProduct(BD.CrossProduct(CD));

            this.SignedVolume = J / 6d;
            
            if (SignedVolume == 0)
            {
                throw new ArgumentException("The vertices of the Tetrahedron3D cannot lie on the same plane.");
            }

            this.Vertices = points;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tetrahedron3D"/> struct.
        /// </summary>
        /// <param name="points">the vertices of the triangle.</param>
        public Tetrahedron3D(IEnumerable<Point3D> points) : this(points.ToArray())
        { }

        /// <summary>
        /// Gets the signed volume of the tetrahedron.
        /// If the first three vertices form a counterclockwise circuit 
        /// when viewed from the side away from the last vertex, the volume is positive.
        /// </summary>
        [Pure]
        public double SignedVolume { get; }

        /// <summary>
        /// Returns a value that indicates whether each pair of elements in two specified tetrahedrons is equal.
        /// </summary>
        /// <param name="left">The first tetrahedron to compare.</param>
        /// <param name="right">The second tetrahedron to compare.</param>
        /// <returns>True if the tetrahedrons are the same; otherwise false.</returns>
        public static bool operator ==(Tetrahedron3D left, Tetrahedron3D right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value that indicates whether any pair of elements in two specified tetrahedrons is not equal.
        /// </summary>
        /// <param name="left">The first tetrahedron to compare.</param>
        /// <param name="right">The second tetrahedron to compare</param>
        /// <returns>True if the tetrahedrons are different; otherwise false.</returns>
        public static bool operator !=(Tetrahedron3D left, Tetrahedron3D right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Returns a circumscribed sphere of the tetrahedron.
        /// </summary>
        /// <returns>A circumspehere of the tetrahedron</returns>
        public Sphere3D CircumSphere() => Sphere3D.FromPoints(Vertices[0], Vertices[1], Vertices[2], Vertices[3]);

        /// <summary>
        /// Test whether a point is enclosed within a tetrahedron. 
        /// </summary>
        /// <param name="p">A point.</param>
        /// <returns>True if the point is on vertices, on edges, or inside the tetrahedron; otherwise false.</returns>
        public bool Contains(Point3D p, double tolerance = float.Epsilon)
        {
            // a point P can be described with a tetrahedron A-B-C-D
            //    P = A + s*(B - A) + t*(C - A) + u*(D - A)
            //    s*(B - A) + t*(C - A) + u*(D - A) = (P - A)
            //    s*BA + t*CA + u*DA = PA
            //
            // In matrix form,
            //    [ x21 x31 x41 ] [ s ] = [ xp1 ]  
            //    [ y21 y31 y41 ] [ t ] = [ xp1 ]  
            //    [ z21 z31 z41 ] [ u ] = [ xp1 ]  
            // i.e
            //    J'x = b
            // gives
            //    [ u ] = 1/|J| [ y31*z41-y41*z31 x41*z31-x31*z41 x31*y41-x41*y31 ] [ xp1 ]
            //    [ v ]         [ y41*z21-y21*z41 x21*z41-x41*z21 x41*y21-x21*y41 ] [ yp1 ]
            //    [ w ]         [ y21*z31-y31*z21 x31*z21-x21*z31 x21*y31-x31*y21 ] [ zp1 ]
            //
            // if (s >= 0 && t >= 0 && u >= 0 && s + t + u <= 1), then P is inside the tetrahedron ABCD.

            if (tolerance < 0)
            {
                throw new ArgumentException("epsilon < 0");
            }

            // TODO: add a BoundingBox for easy containg test.
            if (p.X < Vertices.Select(v => v.X).Min() - tolerance &&
                p.Y < Vertices.Select(v => v.Y).Min() - tolerance &&
                p.Z < Vertices.Select(v => v.Z).Min() - tolerance)
            {
                return false; // outside
            }
            if (p.X > Vertices.Select(v => v.X).Max() + tolerance &&
                p.Y > Vertices.Select(v => v.Y).Max() + tolerance &&
                p.Z > Vertices.Select(v => v.Z).Max() + tolerance)
            {
                return false; // outside
            }

            var PA = p - Vertices[0]; if (PA.Length <= tolerance) return true; // on vertex
            var PB = p - Vertices[1]; if (PB.Length <= tolerance) return true; // on vertex
            var PC = p - Vertices[2]; if (PC.Length <= tolerance) return true; // on vertex

            var BA = Vertices[1] - Vertices[0];
            var CA = Vertices[2] - Vertices[0];
            var DA = Vertices[3] - Vertices[0];

            var x21 = BA.X;
            var y21 = BA.Y;
            var z21 = BA.Z;
            var x31 = CA.X;
            var y31 = CA.Y;
            var z31 = CA.Z;
            var x41 = DA.X;
            var y41 = DA.Y;
            var z41 = DA.Z;
            var xp1 = PA.X;
            var yp1 = PA.Y;
            var zp1 = PA.Z;

            var volume = (x21 * (y31 * z41 - z31 * y41) - y21 * (x31 * z41 - z31 * x41) - z21 * (x31 * y41 - y31 * x41)) / 6.0;
            var s = ((y31 * z41 - y41 * z31) * xp1 + (x41 * z31 - x31 * z41) * yp1 + (x31 * y41 - x41 * y31) * zp1) / (6d * volume);
            var t = ((y41 * z21 - y21 * z41) * xp1 + (x21 * z41 - x41 * z21) * yp1 + (x41 * y21 - x21 * y41) * zp1) / (6d * volume);
            var u = ((y21 * z31 - y31 * z21) * xp1 + (x31 * z21 - x21 * z31) * yp1 + (x21 * y31 - x31 * y21) * zp1) / (6d * volume);

            return s >= -tolerance
                && t >= -tolerance
                && u >= -tolerance
                && (1.0 - s - t - u) >= -tolerance;
        }

        /// <summary>
        /// Returns a value to indicate if a pair of tetrahedrons are equal
        /// </summary>
        /// <param name="other">The tetrahedron to compare against.</param>
        /// <param name="tolerance">A tolerance (epsilon) to adjust for floating point error</param>
        /// <returns>true if the tetrahedrons are equal; otherwise false</returns>
        [Pure]
        public bool Equals(Tetrahedron3D other, double tolerance)
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
        public bool Equals(Tetrahedron3D c) =>
            this.Vertices[0].Equals(c.Vertices[0])
            && this.Vertices[1].Equals(c.Vertices[1])
            && this.Vertices[2].Equals(c.Vertices[2])
            && this.Vertices[3].Equals(c.Vertices[3]);

        /// <inheritdoc />
        [Pure]
        public override bool Equals(object obj) => obj is Tetrahedron3D c && this.Equals(c);

        /// <inheritdoc />
        [Pure]
        public override int GetHashCode() => HashCode.Combine(this.Vertices[0], this.Vertices[1], this.Vertices[2], this.Vertices[3]);
    }
}

