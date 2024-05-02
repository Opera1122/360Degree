/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using UnityEngine;

namespace InfinityCode.uPano.Renderers.Base
{
    /// <summary>
    /// Base class of cubic pano renderers
    /// </summary>
    /// <typeparam name="T">Type of pano renderer</typeparam>
    public abstract class CubePanoRenderer<T> : PanoRenderer<T>
        where T : CubePanoRenderer<T>
    {
        [SerializeField]
        protected Vector3 _size = new Vector3(10, 10, 10);

        /// <summary>
        /// Size of cube mesh
        /// </summary>
        public Vector3 size
        {
            get { return _size; }
            set
            {
                if (_size == value) return;
                _size = value;
                UpdateMesh();
            }
        }

        protected virtual int[][] CreateTriangles()
        {
            int[][] triangles = new int[1][];
            int[] t = new int[36];
            for (int i = 0; i < 6; i++)
            {
                int i3 = i * 6;
                int i4 = i * 4;

                t[i3] = i4;
                t[i3 + 1] = i4 + 1;
                t[i3 + 2] = i4 + 2;
                t[i3 + 3] = i4;
                t[i3 + 4] = i4 + 2;
                t[i3 + 5] = i4 + 3;
            }
            triangles[0] = t;
            return triangles;
        }

        protected abstract Vector2[] CreateUV();

        protected Vector3[] CreateVertices()
        {
            Vector3[] vertices = new Vector3[24];
            Vector3 hs = size / 2;

            // UP
            vertices[0] = new Vector3(-hs.x, hs.y, -hs.z);
            vertices[1] = new Vector3(hs.x, hs.y, -hs.z);
            vertices[2] = new Vector3(hs.x, hs.y, hs.z);
            vertices[3] = new Vector3(-hs.x, hs.y, hs.z);

            // FRONT
            vertices[4] = new Vector3(-hs.x, hs.y, hs.z);
            vertices[5] = new Vector3(hs.x, hs.y, hs.z);
            vertices[6] = new Vector3(hs.x, -hs.y, hs.z);
            vertices[7] = new Vector3(-hs.x, -hs.y, hs.z);

            // LEFT
            vertices[8] = new Vector3(-hs.x, hs.y, -hs.z);
            vertices[9] = new Vector3(-hs.x, hs.y, hs.z);
            vertices[10] = new Vector3(-hs.x, -hs.y, hs.z);
            vertices[11] = new Vector3(-hs.x, -hs.y, -hs.z);

            // BACK
            vertices[12] = new Vector3(hs.x, hs.y, -hs.z);
            vertices[13] = new Vector3(-hs.x, hs.y, -hs.z);
            vertices[14] = new Vector3(-hs.x, -hs.y, -hs.z);
            vertices[15] = new Vector3(hs.x, -hs.y, -hs.z);

            // RIGHT
            vertices[18] = new Vector3(hs.x, -hs.y, -hs.z);
            vertices[19] = new Vector3(hs.x, -hs.y, hs.z);
            vertices[16] = new Vector3(hs.x, hs.y, hs.z);
            vertices[17] = new Vector3(hs.x, hs.y, -hs.z);

            // DOWN
            vertices[20] = new Vector3(-hs.x, -hs.y, hs.z);
            vertices[21] = new Vector3(hs.x, -hs.y, hs.z);
            vertices[22] = new Vector3(hs.x, -hs.y, -hs.z);
            vertices[23] = new Vector3(-hs.x, -hs.y, -hs.z);

            return vertices;
        }

        public override bool CreateMesh()
        {
            if (!base.CreateMesh()) return false;

            Vector3[] vertices = CreateVertices();

            int[][] triangles = CreateTriangles();

            Vector2[] uv = CreateUV();
            Vector3[] normals = new Vector3[vertices.Length];

            for (int i = 0; i < vertices.Length; i++) normals[i] = vertices[i].normalized * -1;

            mesh.vertices = vertices;
            mesh.subMeshCount = triangles.Length;
            for (int i = 0; i < triangles.Length; i++) mesh.SetTriangles(triangles[i], i);
            mesh.uv = uv;
            mesh.normals = normals;

            mesh.RecalculateBounds();
            meshCollider.sharedMesh = Instantiate(mesh);

            return true;
        }

        public override Vector3 GetWorldPosition(float pan, float tilt)
        {
            Vector3 p = transform.rotation * Quaternion.Euler(-tilt, pan, 0) * new Vector3(0, 0, 1);
            Bounds bounds = new Bounds(Vector3.zero, size);
            float distance;
            bounds.IntersectRay(new Ray(Vector3.zero, p), out distance);
            return p * -distance;
        }
    }
}