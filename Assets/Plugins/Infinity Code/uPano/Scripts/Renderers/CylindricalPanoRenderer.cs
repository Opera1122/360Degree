/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.Renderers.Base;
using UnityEngine;

namespace InfinityCode.uPano.Renderers
{
    /// <summary>
    /// Pano renderer for displaying a cylindrical panorama
    /// </summary>
    [AddComponentMenu("uPano/Renderers/CylindricalPanoRenderer")]
    public class CylindricalPanoRenderer : SingleTexturePanoRenderer<CylindricalPanoRenderer>
    {
        /// <summary>
        /// Minimum radius
        /// </summary>
        public const float minRadius = 0.0001f;

        /// <summary>
        /// Minimum sides
        /// </summary>
        public const int minSides = 3;

        /// <summary>
        /// Maximum sides
        /// </summary>
        public const int maxSides = 1024;

        [SerializeField]
        protected float _height = 10;

        [SerializeField]
        protected float _radius = 10;

        [SerializeField]
        protected int _sides = 16;

        [SerializeField]
        protected RectUV _uv = RectUV.full;

        /// <summary>
        /// Gets and sets the height of the cylinder
        /// </summary>
        public float height
        {
            get { return _height; }
            set
            {
                if (_height == value) return;
                _height = value;
                UpdateMesh();
            }
        }

        /// <summary>
        /// Gets and sets the radius of the cylinder
        /// </summary>
        public float radius
        {
            get { return _radius; }
            set
            {
                if (value < minRadius) value = minRadius;
                if (_radius == value) return;
                _radius = value;
                UpdateMesh();
            }
        }

        /// <summary>
        /// Gets and sets the number sides of the cylinder
        /// </summary>
        public int sides
        {
            get { return _sides; }
            set
            {
                if (_sides == value) return;
                _sides = Mathf.Clamp(value, minSides, maxSides);
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

        /// <summary>
        /// Creates a new panorama and pano renderer
        /// </summary>
        /// <param name="texture">Texture of the panorama</param>
        /// <param name="radius">Radius of the cylinder</param>
        /// <param name="height">Height of the cylinder</param>
        /// <param name="sides">Number sides of the cylinder</param>
        /// <returns>Instance of pano renderer</returns>
        public static CylindricalPanoRenderer Create(Texture2D texture, float radius, float height, int sides)
        {
            if (radius < minRadius) throw new Exception("The radius must be greater than or equal to " + minRadius + ".");
            if (sides < minSides || sides > maxSides) throw new Exception("The sides must be in range from " + minSides + " to " + maxSides + ".");

            CylindricalPanoRenderer panoRenderer = CreatePanoInstance();
            panoRenderer._texture = texture;
            panoRenderer._radius = radius;
            panoRenderer._height = height;
            panoRenderer._sides = sides;
            return panoRenderer;
        }

        public override bool CreateMesh()
        {
            if (!base.CreateMesh()) return false;

            int s = sides + 1;
            Vector3[] vertices = new Vector3[s * 2];
            int[] triangles = new int[sides * 6];
            Vector2[] uv = new Vector2[s * 2];

            float degreeOffset = 360f / sides;
            float halfHeight = height / 2;

            float uvOffsetX = _uv.width / sides;

            for (int i = 0; i < s; i++)
            {
                int i2 = i * 2;
                float f = i * degreeOffset * Mathf.Deg2Rad;
                Vector3 v = new Vector3(Mathf.Cos(f) * radius, halfHeight, Mathf.Sin(f) * radius);
                vertices[i2] = v;
                vertices[i2 + 1] = new Vector3(v.x, -halfHeight, v.z);

                float uvX = _uv.right - uvOffsetX * i;

                uv[i2] = new Vector2(uvX, _uv.top);
                uv[i2 + 1] = new Vector2(uvX, _uv.bottom);

                if (i < sides)
                {
                    int ni = i + 1;
                    triangles[i * 6] = i2;
                    triangles[i * 6 + 1] = i2 + 1;
                    triangles[i * 6 + 2] = ni * 2;
                    triangles[i * 6 + 3] = i2 + 1;
                    triangles[i * 6 + 4] = ni * 2 + 1;
                    triangles[i * 6 + 5] = ni * 2;
                }
            }

            Vector3[] normals = new Vector3[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 v = vertices[i];
                Vector3 n = v.normalized * -1;
                n.y = v.y;
                normals[i] = n;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;
            mesh.normals = normals;

            mesh.RecalculateBounds();

            meshCollider.sharedMesh = Instantiate(mesh);

            return true;
        }

        public override bool GetPanTiltByUV(Vector2 uv, out float pan, out float tilt)
        {
            pan = uv.x * 360;
            tilt = uv.y * 180 - 90;
            return true;
        }

        public override void GetPanTiltByWorldPosition(Vector3 worldPosition, out float pan, out float tilt)
        {
            Vector3 point = Quaternion.Inverse(transform.rotation) * worldPosition - transform.position;
            pan = 180 - MathHelper.Angle2D(Vector3.zero, point);
            tilt = point.y / height * 180;
        }

        public override Vector2 GetUV(float pan, float tilt)
        {
            return new Vector2(pan / 360, (tilt + 90) / 180);
        }

        public override Vector3 GetWorldPosition(float pan, float tilt)
        {
            Vector3 p = transform.rotation * 
                        Quaternion.Euler(0, pan - 90, 0) * 
                        new Vector3(0, 0, radius) + 
                        new Vector3(0, tilt / 180 * height, 0) + transform.position;
            return p;

        }

        public override void UpdateRotation(Camera activeCamera, float pan, float tilt)
        {
            if (_pano == null) return;

            Transform tf = activeCamera.transform;
            if (_pano.rotationMode == Pano.RotationMode.rotateCamera)
            {
                Vector3 newEulerAngles = new Vector3(0, pan - 90, 0);
                Vector3 newPosition = new Vector3(0, tilt / 180 * height, 0);

                if (newEulerAngles != tf.localEulerAngles) tf.localEulerAngles = newEulerAngles;
                if (newPosition != tf.localPosition) tf.localPosition = newPosition;
            }
            else if (_pano.rotationMode == Pano.RotationMode.rotateGameObject)
            {
                if (_pano.rotateGameObject == null) return;
                Vector3 newEulerAngles = new Vector3(0, pan - 90, 0);
                Vector3 newPosition = new Vector3(0, tilt / 180 * height, 0);

                tf = _pano.rotateGameObject.transform;

                if (newEulerAngles != tf.localEulerAngles) tf.localEulerAngles = newEulerAngles;
                if (newPosition != tf.localPosition) tf.localPosition = newPosition;
            }
            else
            {
                Quaternion newEulerAngles = tf.rotation;
                newEulerAngles *= Quaternion.Euler(0, -pan + 90, 0);
                Vector3 newPosition = tf.position - new Vector3(0, tilt / 180 * height, 0);

                if (newEulerAngles != transform.localRotation) transform.localRotation = newEulerAngles;
                if (newPosition != transform.position) transform.position = newPosition;
            }
        }

        protected override bool ValidateFields()
        {
            if (_sides < minSides || _sides > maxSides) return false;
            if (_radius < minRadius) return false;
            return true;
        }
    }
}