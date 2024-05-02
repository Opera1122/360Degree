/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Editors.Utils;
using InfinityCode.uPano.Renderers;
using UnityEditor;

namespace InfinityCode.uPano.Editors.Renderers
{
    [CustomEditor(typeof(SphericalPanoRenderer))]
    public class SphericalPanoRendererEditor : SingleTexturePanoRendererEditor<SphericalPanoRenderer>
    {
        protected SerializedProperty pSegments;
        protected SerializedProperty pRadius;
        protected SerializedProperty pUV;
        private SerializedProperty pMeshType;
        private SerializedProperty pQuality;

        protected override void CacheSerializedFields()
        {
            base.CacheSerializedFields();

            pMeshType = FindProperty("_meshType");
            pRadius = FindProperty("_radius");
            pUV = FindProperty("_uv");
            pSegments = FindProperty("_segments");
            pQuality = FindProperty("_quality");
        }

        protected override void DrawMeshContent()
        {
            PropertyField(pMeshType);

            if (EditorUtils.PropertyField(pRadius))
            {
                if (pRadius.floatValue < SphericalPanoRenderer.minRadius) pRadius.floatValue = SphericalPanoRenderer.minRadius;
            }

            if (pMeshType.enumValueIndex == (int) SphericalPanoRenderer.MeshType.sphere)
            {
                if (EditorUtils.PropertyField(pSegments))
                {
                    if (pSegments.intValue < SphericalPanoRenderer.minSegments) pSegments.intValue = SphericalPanoRenderer.minSegments;
                }
            }
            else
            {
                if (EditorUtils.PropertyField(pQuality))
                {
                    if (pQuality.intValue < 0) pQuality.intValue = 0;
                    else if (pQuality.intValue > SphericalPanoRenderer.maxQuality) pQuality.intValue = SphericalPanoRenderer.maxQuality;
                }
            }
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            DrawUVLabel();
            DrawUVContent();
        }

        protected virtual void DrawUVContent()
        {
            DrawInternalFields(pUV);
        }

        private void DrawUVLabel()
        {
            EditorUtils.GroupLabel("UV");
        }
    }
}