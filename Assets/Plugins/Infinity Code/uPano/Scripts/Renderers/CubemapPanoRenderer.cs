/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.Renderers.Base;
using UnityEngine;

namespace InfinityCode.uPano.Renderers
{
    /// <summary>
    /// Pano renderer for displaying a cubemap panorama
    /// </summary>
    [AddComponentMenu("uPano/Renderers/CubemapPanoRenderer")]
    public class CubemapPanoRenderer : CubePanoRenderer<CubemapPanoRenderer>, ISingleTexturePanoRenderer
    {
        [SerializeField]
        private Cubemap _cubemap;

        protected Material material;

        public override Shader defaultShader
        {
            get { return Shader.Find("uPano/Cubemap"); }
        }

        /// <summary>
        /// The cubemap of the panorama
        /// </summary>
        public Cubemap cubemap
        {
            get { return _cubemap; }
            set
            {
                if (_cubemap == value) return;
                _cubemap = value;
                UpdateMesh();
            }
        }

        public Texture texture
        {
            get { return _cubemap; }
            set { _cubemap = value as Cubemap;}
        }

        /// <summary>
        /// Creates a new panorama and pano renderer
        /// </summary>
        /// <param name="cubemap">The cubemap of the panorama</param>
        /// <param name="size">Size of cube mesh</param>
        /// <returns>Instance of pano renderer</returns>
        public static CubemapPanoRenderer Create(Cubemap cubemap, Vector3 size)
        {
            if (cubemap == null) throw new Exception("Cubemap can not be null.");

            CubemapPanoRenderer panoRenderer = CreatePanoInstance();
            panoRenderer._cubemap = cubemap;
            panoRenderer._size = size;
            return panoRenderer;
        }

        protected override void CreateMaterial()
        {
            if (defaultMaterial != null) material = Instantiate(defaultMaterial);
            else material = meshRenderer.material = new Material(shader);
        }

        public override bool CreateMesh()
        {
            if (!base.CreateMesh()) return false;

            if (!material.HasProperty("_cubemap")) throw new Exception("Material does not have a cubemap property.");
            material.SetTexture("_cubemap", cubemap);
            return true;
        }

        protected override Vector2[] CreateUV()
        {
            Vector2[] uv = new Vector2[24];

            // TOP
            uv[0] = Vector2.up;
            uv[1] = Vector2.one;
            uv[2] = Vector2.right;
            uv[3] = Vector2.zero;

            // FRONT
            uv[4] = Vector2.up;
            uv[5] = Vector2.one;
            uv[6] = Vector2.right;
            uv[7] = Vector2.zero;

            // LEFT
            uv[8] = Vector2.up;
            uv[9] = Vector2.one;
            uv[10] = Vector2.right;
            uv[11] = Vector2.zero;

            // BACK
            uv[12] = Vector2.up;
            uv[13] = Vector2.one;
            uv[14] = Vector2.right;
            uv[15] = Vector2.zero;

            // RIGHT
            uv[16] = Vector2.up;
            uv[17] = Vector2.one;
            uv[18] = Vector2.right;
            uv[19] = Vector2.zero;

            // DOWN
            uv[20] = Vector2.up;
            uv[21] = Vector2.one;
            uv[22] = Vector2.right;
            uv[23] = Vector2.zero;

            return uv;
        }

        public void Download(string url)
        {
            Debug.LogError("CubemapPanoRenderer does not support downloading. Use SphericalPanoRenderer instead.");
        }

        public void Download(string lowresURL, string url)
        {
            Debug.LogError("CubemapPanoRenderer does not support downloading. Use SphericalPanoRenderer instead.");
        }

        public bool GetPanTiltByUV(Vector2 uv, out float pan, out float tilt)
        {
            pan = Mathf.Repeat((uv.x - 0.5f) * 360, 360) - pano.northPan;
            tilt = uv.y * 180 - 90;
            return true;
        }

        public Vector2 GetUV(float pan, float tilt)
        {
            return new Vector2(Mathf.Repeat(pan + pano.northPan + 180, 360) / 360, (Mathf.Clamp(tilt, -90, 90) + 90) / 180);
        }

        protected override bool ValidateFields()
        {
            return true;
        }
    }
}