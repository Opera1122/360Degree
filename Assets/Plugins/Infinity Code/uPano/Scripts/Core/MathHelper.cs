/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InfinityCode.uPano
{
    /// <summary>
    /// Mathematical methods
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// Degrees-to-radians conversion constant.
        /// </summary>
        public const double Deg2Rad = Math.PI / 180;

        /// <summary>
        /// Radians-to-degrees conversion constant.
        /// </summary>
        public const double Rad2Deg = 180 / Math.PI;

        /// <summary>
        /// Math.PI / 4
        /// </summary>
        public const double PID4 = Math.PI / 4;

        /// <summary>
        /// The angle between the two points in degree
        /// </summary>
        /// <param name="point1">Point 1</param>
        /// <param name="point2">Point 2</param>
        /// <returns>Angle in degree</returns>
        public static float Angle2D(Vector3 point1, Vector3 point2)
        {
            return Mathf.Atan2(point2.z - point1.z, point2.x - point1.x) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// The angle between the two points in degree
        /// </summary>
        /// <param name="p1x">Point 1 X</param>
        /// <param name="p1y">Point 1 Y</param>
        /// <param name="p2x">Point 2 X</param>
        /// <param name="p2y">Point 2 Y</param>
        /// <returns>Angle in degree</returns>
        public static float Angle2D(float p1x, float p1y, float p2x, float p2y)
        {
            return Mathf.Atan2(p2y - p1y, p2x - p1x) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// The angle between the two points in degree
        /// </summary>
        /// <param name="p1x">Point 1 X</param>
        /// <param name="p1y">Point 1 Y</param>
        /// <param name="p2x">Point 2 X</param>
        /// <param name="p2y">Point 2 Y</param>
        /// <returns>Angle in degree</returns>
        public static double Angle2D(double p1x, double p1y, double p2x, double p2y)
        {
            return Math.Atan2(p2y - p1y, p2x - p1x) * Rad2Deg;
        }

        /// <summary>
        /// Checks if a point is inside a polygon
        /// </summary>
        /// <param name="poly">Polygon points list</param>
        /// <param name="x">Point X</param>
        /// <param name="y">Point Y</param>
        /// <returns>True - point inside the polygon, false - otherwise</returns>
        public static bool IsPointInPolygon(List<Vector2> poly, float x, float y)
        {
            int i, j;
            bool c = false;
            for (i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
            {
                if (((poly[i].y <= y && y < poly[j].y) || (poly[j].y <= y && y < poly[i].y)) &&
                    x < (poly[j].x - poly[i].x) * (y - poly[i].y) / (poly[j].y - poly[i].y) + poly[i].x)
                    c = !c;
            }
            return c;
        }

        /// <summary>
        /// Gets the nearest point on a line segment
        /// </summary>
        /// <param name="point">The point for which need to find the nearest point</param>
        /// <param name="lineStart">Segment start point</param>
        /// <param name="lineEnd">Segment end point</param>
        /// <returns>Nearest point on a line segment</returns>
        public static Vector2 NearestPointStrict(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
        {
            Vector2 fullDirection = lineEnd - lineStart;
            Vector2 lineDirection = fullDirection.normalized;
            float closestPoint = Vector2.Dot(point - lineStart, lineDirection) / Vector2.Dot(lineDirection, lineDirection);
            return lineStart + Mathf.Clamp(closestPoint, 0, fullDirection.magnitude) * lineDirection;
        }

        /// <summary>
        /// Triangulates a polygon
        /// </summary>
        /// <param name="points">Polygon point</param>
        /// <returns>List of vertex indices</returns>
        public static List<int> Triangulate(List<Vector3> points)
        {
            List<int> indices = new List<int>(18);

            int n = points.Count;
            if (n < 3) return indices;

            int[] V = new int[n];
            if (TriangulateArea(points) > 0)
            {
                for (int v = 0; v < n; v++) V[v] = v;
            }
            else
            {
                for (int v = 0; v < n; v++) V[v] = n - 1 - v;
            }

            int nv = n;
            int count = 2 * nv;

            for (int v = nv - 1; nv > 2;)
            {
                if (count-- <= 0) return indices;

                int u = v;
                if (nv <= u) u = 0;
                v = u + 1;
                if (nv <= v) v = 0;
                int w = v + 1;
                if (nv <= w) w = 0;

                if (TriangulateSnip(points, u, v, w, nv, V))
                {
                    int s, t;
                    indices.Add(V[u]);
                    indices.Add(V[v]);
                    indices.Add(V[w]);
                    for (s = v, t = v + 1; t < nv; s++, t++) V[s] = V[t];
                    nv--;
                    count = 2 * nv;
                }
            }

            indices.Reverse();
            return indices;
        }

        private static float TriangulateArea(List<Vector3> points)
        {
            int n = points.Count;
            float A = 0.0f;
            for (int p = n - 1, q = 0; q < n; p = q++)
            {
                Vector3 pval = points[p];
                Vector3 qval = points[q];
                A += pval.x * qval.y - qval.x * pval.y;
            }
            return A * 0.5f;
        }

        private static bool TriangulateSnip(List<Vector3> points, int u, int v, int w, int n, int[] V)
        {
            Vector3 A = points[V[u]];
            Vector3 B = points[V[v]];
            Vector3 C = points[V[w]];
            if (Mathf.Epsilon > (B.x - A.x) * (C.y - A.y) - (B.y - A.y) * (C.x - A.x)) return false;
            for (int p = 0; p < n; p++)
            {
                if (p == u || p == v || p == w) continue;
                if (TriangulateInsideTriangle(A, B, C, points[V[p]])) return false;
            }
            return true;
        }

        private static bool TriangulateInsideTriangle(Vector3 a, Vector3 b, Vector3 c, Vector3 p)
        {
            float bp = (c.x - b.x) * (p.y - b.y) - (c.y - b.y) * (p.x - b.x);
            float ap = (b.x - a.x) * (p.y - a.y) - (b.y - a.y) * (p.x - a.x);
            float cp = (a.x - c.x) * (p.y - c.y) - (a.y - c.y) * (p.x - c.x);
            return bp >= 0.0f && cp >= 0.0f && ap >= 0.0f;
        }
    }
}
