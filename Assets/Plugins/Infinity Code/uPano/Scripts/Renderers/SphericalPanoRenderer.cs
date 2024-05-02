/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using System.Collections.Generic;
using InfinityCode.uPano.Renderers.Base;
using UnityEngine;

namespace InfinityCode.uPano.Renderers
{
    /// <summary>
    /// Pano renderer for displaying a spherical panorama
    /// </summary>
    [AddComponentMenu("uPano/Renderers/SphericalPanoRenderer")]
    public class SphericalPanoRenderer : SingleTexturePanoRenderer<SphericalPanoRenderer>
    {
        #region Enums

        /// <summary>
        /// Enum of mesh types
        /// </summary>
        public enum MeshType
        {
            /// <summary>
            /// Sphere mesh
            /// </summary>
            sphere,

            /// <summary>
            /// Icosahedron mesh
            /// </summary>
            icosahedron
        }

        #endregion

        #region Constants

        /// <summary>
        /// Maximum icosahedron quality
        /// </summary>
        public const int maxQuality = 4;

        /// <summary>
        /// Minimum radius
        /// </summary>
        public const float minRadius = 0.001f;

        /// <summary>
        /// Minimum number of sphere segments
        /// </summary>
        public const int minSegments = 4;

        #endregion

        #region Fields

        [SerializeField]
        private MeshType _meshType = MeshType.sphere;

        [SerializeField]
        [Range(0, maxQuality)]
        private int _quality = 2;

        [SerializeField]
        protected float _radius = 10;

        [SerializeField]
        private int _segments = 64;

        [SerializeField]
        protected RectUV _uv = RectUV.full;

        #endregion

        #region Properties

        /// <summary>
        /// Gets and sets the type of the mesh
        /// </summary>
        public MeshType meshType
        {
            get { return _meshType; }
            set { _meshType = value; }
        }

        /// <summary>
        /// Gets and sets the quality of icosahedron mesh
        /// </summary>
        public int quality
        {
            get { return _quality; }
            set
            {
                value = Mathf.Clamp(value, 0, maxQuality);
                if (_quality == value) return;
                _quality = value;
                UpdateMesh();
            }
        }

        /// <summary>
        /// Gets and sets the radius of the mesh
        /// </summary>
        public float radius
        {
            get { return _radius; }
            set
            {
                if (value < minRadius) value = minRadius;
                if (Math.Abs(value - _radius) < float.Epsilon) return;
                _radius = value;
                UpdateMesh();
            }
        }

        /// <summary>
        /// Gets and sets the number of segments of sphere mesh
        /// </summary>
        public int segments
        {
            get { return _segments; }
            set
            {
                if (value < minSegments) value = minSegments;
                if (_segments == value) return;
                _segments = value;
                UpdateMesh();
            }
        }

        /// <summary>
        /// Gets and sets UV of the panorama
        /// </summary>
        public RectUV uv
        {
            get { return _uv; }
            set
            {
                _uv = value;
                UpdateMesh();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new panorama and pano renderer with icosahedron mesh type
        /// </summary>
        /// <param name="texture">Texture of the panorama</param>
        /// <param name="radius">Radius of the mesh</param>
        /// <param name="quality">Quality of the icosahedron mesh</param>
        /// <returns>Instance of pano renderer</returns>
        public static SphericalPanoRenderer CreateIcosahedron(Texture texture, float radius, int quality)
        {
            if (radius < minRadius) throw new Exception("The radius must be greater than or equal to " + minRadius + ".");
            if (quality < 0 || quality > maxQuality) throw new Exception("The quality must be in range from 0 to " + maxQuality + ".");

            SphericalPanoRenderer panoRenderer = CreatePanoInstance();
            panoRenderer._texture = texture;
            panoRenderer._meshType = MeshType.icosahedron;
            panoRenderer._radius = radius;
            panoRenderer._segments = quality;
            return panoRenderer;
        }

        private void CreateIcosahedronMesh()
        {
            int[] triangles = {
                0,11,5,
                0,5,1,
                0,1,7,
                0,7,10,
                0,10,11,

                1,5,9,
                5,11,4,
                11,10,2,
                10,7,6,
                7,1,8,

                3,9,4,
                3,4,2,
                3,2,6,
                3,6,8,
                3,8,9,

                4,9,5,
                2,4,11,
                6,2,10,
                8,6,7,
                9,8,1
            };

            float t = (1f + Mathf.Sqrt(5f)) / 2f;

            int[] verticesCapacity = { 15, 49, 175, 667, 2611 };
            List<Vector3> vertices = new List<Vector3>(verticesCapacity[quality])
            {
                new Vector3(-1f, t, 0f).normalized * radius,
                new Vector3(1f, t, 0f).normalized * radius,

                new Vector3(-1f, -t, 0f).normalized * radius,
                new Vector3(1f, -t, 0f).normalized * radius,

                new Vector3(0f, -1f, t).normalized * radius,
                new Vector3(0f, 1f, t).normalized * radius,

                new Vector3(0f, -1f, -t).normalized * radius,
                new Vector3(0f, 1f, -t).normalized * radius,

                new Vector3(t, 0f, -1f).normalized * radius,
                new Vector3(t, 0f, 1f).normalized * radius,

                new Vector3(-t, 0f, -1f).normalized * radius,
                new Vector3(-t, 0f, 1f).normalized * radius
            };

            int[] cache;
            if (quality > 0)
            {
                int[] cacheCapacity = { 60, 240, 960, 3840 };
                cache = new int[cacheCapacity[quality - 1]];
            }
            else cache = new int[60];

            for (int i = 0; i < quality; i++) SubdivideIcosahedron(vertices, ref triangles, cache);

            List<Vector2> uv = new List<Vector2>(vertices.Count);

            for (int i = 0; i < vertices.Count; i++)
            {
                Vector3 v = vertices[i].normalized;
                vertices[i] = v * radius;

                float a1 = Mathf.Atan2(v.z, v.x) * Mathf.Rad2Deg;

                Vector3 p = Quaternion.Euler(0, a1, 0) * v;
                float a2 = Mathf.Atan2(p.y, p.x) * Mathf.Rad2Deg;

                a1 = (a1 + 90) / -360;
                a2 = a2 / 180 + 0.5f;

                a1 = Mathf.Repeat(_uv.width * a1 + _uv.left, 1);
                a2 = _uv.bottom + _uv.height * a2;

                uv.Add(new Vector2(a1, a2));
            }

            int ci = 0;
            int trianglesCount = triangles.Length;
            for (int i = 0; i < trianglesCount; i += 3)
            {
                int i2 = i + 1;
                int i3 = i + 2;
                int v1 = triangles[i];
                int v2 = triangles[i2];
                int v3 = triangles[i3];
                Vector2 uv1 = uv[v1];
                Vector2 uv2 = uv[v2];
                Vector2 uv3 = uv[v3];

                if (Math.Abs(uv1.x - uv2.x) > 0.5) FixWrappedUV(triangles, vertices, uv, cache, ref ci, i, i2, v1, v2, uv1, uv2);
                if (Math.Abs(uv1.x - uv3.x) > 0.5) FixWrappedUV(triangles, vertices, uv, cache, ref ci, i, i3, v1, v3, uv1, uv3);
                if (Math.Abs(uv2.x - uv3.x) > 0.5) FixWrappedUV(triangles, vertices, uv, cache, ref ci, i2, i3, v2, v3, uv2, uv3);
            }

            Vector3[] normals = new Vector3[vertices.Count];
            for (int i = 0; i < vertices.Count; i++)
            {
                normals[i] = vertices[i].normalized * -1;
            }

            for (int i = 0, i2 = trianglesCount - 1; i < trianglesCount / 2; i++, i2--)
            {
                int ti1 = triangles[i];
                int ti2 = triangles[i2];
                triangles[i] = ti2;
                triangles[i2] = ti1;
            }

            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uv);
            mesh.normals = normals;
            mesh.SetTriangles(triangles, 0);
        }

        public override bool CreateMesh()
        {
            if (!base.CreateMesh()) return false;

            if (_meshType == MeshType.sphere) CreateSphereMesh();
            else CreateIcosahedronMesh();

            mesh.RecalculateBounds();

            meshCollider.sharedMesh = Instantiate(mesh);

            return true;
        }

        /// <summary>
        /// Creates a new panorama and pano renderer with sphere mesh type
        /// </summary>
        /// <param name="texture">Texture of the panorama</param>
        /// <param name="radius">Radius of the panorama</param>
        /// <param name="segments">Number of the panorama segments</param>
        /// <returns>Instance of pano renderer</returns>
        public static SphericalPanoRenderer CreateSphere(Texture texture, float radius, int segments)
        {
            if (radius < minRadius) throw new Exception("The radius must be greater than or equal to " + minRadius + ".");
            if (segments < minSegments) throw new Exception("The segments must be greater than or equal to " + minSegments + ".");

            SphericalPanoRenderer panoRenderer = CreatePanoInstance();
            panoRenderer._texture = texture;
            panoRenderer._meshType = MeshType.sphere;
            panoRenderer._radius = radius;
            panoRenderer._segments = segments;
            return panoRenderer;
        }

        private void CreateSphereMesh()
        {
            int s = _segments + 1;
            int s2 = s * s;

            Vector3[] vertices = new Vector3[s2];
            Vector3[] normals = new Vector3[s2];
            Vector2[] uv = new Vector2[s2];
            int[] triangles = new int[_segments * _segments * 6];

            float degreeOffset = 360f / _segments;
            float uvOffsetX = _uv.width / _segments;
            float uvOffsetY = _uv.height / _segments;

            Vector3 pt = new Vector3();

            for (int v = 0; v < s; v++)
            {
                float theta = v * degreeOffset / 2;
                float snt = Mathf.Sin(theta * Mathf.Deg2Rad);
                float cnt = Mathf.Cos(theta * Mathf.Deg2Rad);
                pt.y = _radius * cnt;

                for (int u = 0; u < s; u++)
                {
                    float phi = u * degreeOffset + 90;
                    
                    float snp = Mathf.Sin(phi * Mathf.Deg2Rad);
                    float cnp = Mathf.Cos(phi * Mathf.Deg2Rad);
                    pt.x = _radius * snt * cnp;
                    pt.z = -_radius * snt * snp;
                    int i = v * s + u;
                    vertices[i] = pt;
                    normals[i] = pt.normalized * -1;
                    uv[i] = new Vector2(uvOffsetX * u + _uv.left, _uv.top - uvOffsetY * v);

                    if (u < _segments && v < _segments)
                    {
                        int ti = (v * _segments + u) * 6;
                        int ni = (v + 1) * s + u;

                        triangles[ti] = i;
                        triangles[ti + 1] = i + 1;
                        triangles[ti + 2] = ni;
                        triangles[ti + 3] = i + 1;
                        triangles[ti + 4] = ni + 1;
                        triangles[ti + 5] = ni;
                    }
                }
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;
            mesh.normals = normals;
        }

        private void FixWrappedUV(int[] triangles, List<Vector3> vertices, List<Vector2> uv, int[] cache, ref int ci, int i1, int i2, int v1, int v2, Vector2 uv1, Vector2 uv2)
        {
            bool firstIsSmaller = uv1.x < uv2.x;
            Vector3 v = vertices[firstIsSmaller ? v1 : v2];
            Vector2 u = uv[firstIsSmaller ? v1 : v2];

            int key = firstIsSmaller ? v1 : v2;

            for (int i = 0; i < ci; i += 2)
            {
                if (cache[i] == key)
                {
                    triangles[firstIsSmaller ? i1 : i2] = cache[i + 1];
                    return;
                }
            }

            int vc = vertices.Count;
            triangles[firstIsSmaller ? i1 : i2] = vc;
            vertices.Add(v);
            uv.Add(new Vector2(u.x + 1, u.y));
            cache[ci] = key;
            cache[ci + 1] = vc;
            ci += 2;
        }

        private int GetMidpointIndex(List<Vector3> vertices, int i0, int i1, int[] cache, ref int ci)
        {
            int smallerIndex = i0;
            int greaterIndex = i1;
            if (i0 < i1)
            {
                smallerIndex = i1;
                greaterIndex = i0;
            }
            int key = (smallerIndex << 16) + greaterIndex;

            for (int i = 0; i < ci; i += 2)
            {
                if (cache[i] == key) return cache[i + 1];
            }

            Vector3 v0 = vertices[i0];
            Vector3 v1 = vertices[i1];

            float mx = (v0.x + v1.x) / 2;
            float my = (v0.y + v1.y) / 2;
            float mz = (v0.z + v1.z) / 2;

            int midpointIndex = vertices.Count;
            vertices.Add(new Vector3(mx, my, mz).normalized * radius);
            cache[ci] = key;
            cache[ci + 1] = midpointIndex;
            ci += 2;

            return midpointIndex;
        }

        public override bool GetPanTiltByUV(Vector2 uv, out float pan, out float tilt)
        {
            pan = Mathf.Repeat((uv.x - 0.5f) * 360, 360) - pano.northPan;
            tilt = uv.y * 180 - 90;
            return true;
        }

        public override Vector2 GetUV(float pan, float tilt)
        {
            return new Vector2(Mathf.Repeat(pan + pano.northPan + 180, 360) / 360, (Mathf.Clamp(tilt, -90, 90) + 90) / 180);
        }

        public override Vector3 GetWorldPosition(float pan, float tilt)
        {
            return transform.rotation * Quaternion.Euler(-tilt, pan + pano.northPan, 0) * new Vector3(0, 0, radius) + transform.position;
        }

        private void SubdivideIcosahedron(List<Vector3> vertices, ref int[] triangles, int[] cache)
        {
            int[] indexList = new int[4 * triangles.Length];

            int ci = 0;
            int ti = 0;

            for (int i = 0; i < triangles.Length; i += 3)
            {
                int i0 = triangles[i];
                int i1 = triangles[i + 1];
                int i2 = triangles[i + 2];

                int m01 = GetMidpointIndex(vertices, i0, i1, cache, ref ci);
                int m12 = GetMidpointIndex(vertices, i1, i2, cache, ref ci);
                int m02 = GetMidpointIndex(vertices, i2, i0, cache, ref ci);

                indexList[ti++] = i0;
                indexList[ti++] = m01;
                indexList[ti++] = m02;

                indexList[ti++] = i1;
                indexList[ti++] = m12;
                indexList[ti++] = m01;

                indexList[ti++] = i2;
                indexList[ti++] = m02;
                indexList[ti++] = m12;

                indexList[ti++] = m02;
                indexList[ti++] = m01;
                indexList[ti++] = m12;
            }

            triangles = indexList;
        }

        protected override bool ValidateFields()
        {
            if (_radius < minRadius) return false;

            if (_meshType == MeshType.sphere)
            {
                if (_segments < minSegments) return false;
            }
            else
            {
                if (_quality < 0 || _quality > maxQuality) return false;
            }
            return true;
        }

        #endregion
    }
}