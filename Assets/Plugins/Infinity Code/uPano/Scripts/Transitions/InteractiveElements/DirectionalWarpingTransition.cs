/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using System.Linq;
using InfinityCode.uPano.Renderers;
using UnityEngine;

namespace InfinityCode.uPano.Transitions.InteractiveElements
{
    /// <summary>
    /// Performs directional warping transition from one panorama to another.
    /// </summary>
    public class DirectionalWarpingTransition : TimeBasedTransition
    {
        /// <summary>
        /// Distortion strength
        /// </summary>
        public float distortion = 0.04f;

        protected Material[] materials;

        protected SphericalPanoRenderer panoRenderer;

        protected Mesh mesh;
        protected float pan;
        protected float northPanOffset;
        protected Texture secondTexture;

        protected override void Dispose()
        {
            base.Dispose();

            materials = null;
            mesh = null;
            panoRenderer = null;
        }

        protected override void FinishBefore()
        {
            progress = 0;
            Process();

            materials[0].SetTexture("_SecondTexture", null);
        }

        private Vector2 GetUV(float u, float v, float m, RectUV uv, float panOffset)
        {
            float u1 = 180 + u * 360 - pan + panOffset;
            float su = Mathf.Sin(u1 * Mathf.Deg2Rad);
            float ou = su * m;

            float v1 = 180 + v * 360;
            float sv = Mathf.Sin(v1 * Mathf.Deg2Rad);
            float ov = sv * m * (0.25f - Mathf.Sin((270 + u1) * Mathf.Deg2Rad));

            return new Vector2(u + ou + uv.left + panOffset, uv.top - (v + ov));
        }

        public override void Init()
        {
            if (element == null) throw new Exception("Element cannot be null.");

            base.Init();

            panoRenderer = element.pano.panoRenderer as SphericalPanoRenderer;
            MeshFilter meshFilter = panoRenderer.meshGameObject.GetComponent<MeshFilter>();
            mesh = meshFilter.sharedMesh;

            float tilt;
            element.GetPanTilt(out pan, out tilt);
            pan += element.pano.northPan;

            northPanOffset = element.pano.northPan;
            if (element.switchToPanorama != null)
            {
                northPanOffset -= element.switchToPanorama.GetComponent<Pano>().northPan;
                secondTexture = element.switchToPanorama.GetComponent<SphericalPanoRenderer>().texture;
            }

            MeshRenderer mr = panoRenderer.meshGameObject.GetComponent<MeshRenderer>();
            materials = mr.sharedMaterials;

            materials[0].SetTexture("_SecondTexture", secondTexture);
        }

        public override void Process()
        {
            Vector2[] uv1 = mesh.uv;
            Vector2[] uv2 = new Vector2[uv1.Length];

            int s = panoRenderer.segments + 1;

            RectUV uv = panoRenderer.uv;
            float uvOffsetX = uv.width / panoRenderer.segments;
            float uvOffsetY = uv.height / panoRenderer.segments;

            float m1 = Mathf.Lerp(0, -distortion, progress);
            float m2 = Mathf.Lerp(distortion, 0, progress);

            float northPanShift = northPanOffset / 360;

            for (int v = 0; v < s; v++)
            {
                for (int u = 0; u < s; u++)
                {
                    int i = v * s + u;
                    uv1[i] = GetUV(uvOffsetX * u, uvOffsetY * v, m1, uv, 0);
                    uv2[i] = GetUV(uvOffsetX * u - northPanShift, uvOffsetY * v, m2, uv, northPanOffset);
                }
            }

            mesh.SetUVs(0, uv1);
            mesh.SetUVs(1, uv2);

            foreach (Material m in materials) m.SetFloat("_progress", progress);
        }
    }
}