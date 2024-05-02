/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.Enums;
using InfinityCode.uPano.Renderers.Base;
using UnityEngine;

namespace InfinityCode.uPano.Renderers
{
    /// <summary>
    /// Pano renderer for displaying a cubic panorama, all sides of which are on the same texture
    /// </summary>
    [AddComponentMenu("uPano/Renderers/SingleTextureCubeFacesPanoRenderer")]
    public class SingleTextureCubeFacesPanoRenderer : SingleTexturePanoRenderer<SingleTextureCubeFacesPanoRenderer>
    {
        [SerializeField]
        protected CubeUV _cubeUV = new CubeUV(
            RotatableRectUV.full,
            RotatableRectUV.full,
            RotatableRectUV.full,
            RotatableRectUV.full,
            RotatableRectUV.full,
            RotatableRectUV.full
        );

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

        /// <summary>
        /// UV of the paronama sides
        /// </summary>
        public CubeUV cubeUV
        {
            get { return _cubeUV; }
            set
            {
                if (_cubeUV == value || value == null) return;
                _cubeUV = value;
                UpdateMesh();
            }
        }

        /// <summary>
        /// Creates a new panorama and pano renderer
        /// </summary>
        /// <param name="texture">Texture of the panorama</param>
        /// <param name="size">Size of cube mesh</param>
        /// <param name="cubeUV">UV of the paronama sides</param>
        /// <returns>Instance of pano renderer</returns>
        public static SingleTextureCubeFacesPanoRenderer Create(Texture2D texture, Vector3 size, CubeUV cubeUV)
        {
            SingleTextureCubeFacesPanoRenderer panoRenderer = CreatePanoInstance();
            panoRenderer._texture = texture;
            panoRenderer._size = size;
            panoRenderer._cubeUV = cubeUV;
            return panoRenderer;
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

        protected int[][] CreateTriangles()
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

        protected Vector2[] CreateUV()
        {
            Vector2[] uv = new Vector2[24];

            CubeUV cuv = cubeUV;

            cuv.top.SetUVToArray(uv, 0);
            cuv.front.SetUVToArray(uv, 4);
            cuv.left.SetUVToArray(uv, 8);
            cuv.back.SetUVToArray(uv, 12);
            cuv.right.SetUVToArray(uv, 16);
            cuv.bottom.SetUVToArray(uv, 20);

            return uv;
        }

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

        public override bool GetPanTiltByUV(Vector2 uv, out float pan, out float tilt)
        {
            pan = tilt = 0;
            int sideIndex = cubeUV.GetSideIndex(uv);
            if (sideIndex == -1) return false;
            RotatableRectUV side = cubeUV[sideIndex];

            CubeSide direction = CubeSide.up;

            if (sideIndex == 1) direction = CubeSide.front;
            else if (sideIndex == 2) direction = CubeSide.left;
            else if (sideIndex == 3) direction = CubeSide.back;
            else if (sideIndex == 4) direction = CubeSide.right;
            else if (sideIndex == 5) direction = CubeSide.down;

            Vector3 worldPos = side.GetWorldPosition(uv, direction);
            GetPanTiltByWorldPosition(worldPos, out pan, out tilt);
            return true;
        }

        public override Vector2 GetUV(float pan, float tilt)
        {
            Vector3 p = Quaternion.Euler(-tilt, pan, 0) * new Vector3(0, 0, 1);
            Bounds bounds = new Bounds(Vector3.zero, size);
            float distance;
            bounds.IntersectRay(new Ray(Vector3.zero, p), out distance);
            p *= -distance;

            CubeSide side = CubeSide.up;
            Vector3 halfSize = size / 2;

            float rx, ry;

            if (Mathf.Abs(p.y + halfSize.y) < 0.001)
            {
                side = CubeSide.down;
                rx = (p.x + halfSize.x) / size.x;
                ry = (p.z + halfSize.z) / size.z;
            }
            else if (Mathf.Abs(p.x + halfSize.x) < 0.001)
            {
                side = CubeSide.left;
                rx = (p.z + halfSize.z) / size.z;
                ry = (p.y + halfSize.y) / size.y;
            }
            else if (Mathf.Abs(p.x - halfSize.x) < 0.001)
            {
                side = CubeSide.right;
                rx = 1 - (p.z + halfSize.z) / size.z;
                ry = (p.y + halfSize.y) / size.y;
            }
            else if (Mathf.Abs(p.z - halfSize.z) < 0.001)
            {
                side = CubeSide.front;
                rx = (p.x + halfSize.x) / size.x;
                ry = (p.y + halfSize.y) / size.y;
            }
            else if (Mathf.Abs(p.z + halfSize.z) < 0.001)
            {
                side = CubeSide.back;
                rx = 1 - (p.x + halfSize.x) / size.x;
                ry = (p.y + halfSize.y) / size.y;
            }
            else
            {
                rx = (p.x + halfSize.x) / size.x;
                ry = 1 - (p.z + halfSize.z) / size.z;
            }

            return cubeUV[(int)side].GetUVBilinear(rx, ry);
        }

        public override Vector3 GetWorldPosition(float pan, float tilt)
        {
            Vector3 p = Quaternion.Euler(-tilt, pan, 0) * new Vector3(0, 0, 1);
            Bounds bounds = new Bounds(Vector3.zero, size);
            float distance;
            bounds.IntersectRay(new Ray(Vector3.zero, p), out distance);
            return p * -distance;

        }

        protected override bool ValidateFields()
        {
            return true;
        }
    }
}