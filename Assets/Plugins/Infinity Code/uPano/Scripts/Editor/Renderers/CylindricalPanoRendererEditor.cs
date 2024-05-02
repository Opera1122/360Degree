/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Editors.Utils;
using InfinityCode.uPano.Renderers;
using UnityEditor;

namespace InfinityCode.uPano.Editors.Renderers
{
    [CustomEditor(typeof(CylindricalPanoRenderer))]
    public class CylindricalPanoRendererEditor : SingleTexturePanoRendererEditor<CylindricalPanoRenderer>
    {
        private SerializedProperty mSides;
        private SerializedProperty mHeight;
        private SerializedProperty mRadius;
        private SerializedProperty pUV;

        protected override void CacheSerializedFields()
        {
            base.CacheSerializedFields();

            mSides = FindProperty("_sides");
            mHeight = FindProperty("_height");
            mRadius = FindProperty("_radius");
            pUV = FindProperty("_uv");
        }

        protected override void DrawMeshContent()
        {
            if (EditorUtils.PropertyField(mSides))
            {
                if (mSides.intValue < CylindricalPanoRenderer.minSides) mSides.intValue = CylindricalPanoRenderer.minSides;
                else if (mSides.intValue > CylindricalPanoRenderer.maxSides) mSides.intValue = CylindricalPanoRenderer.maxSides;
            }
            EditorUtils.PropertyField(mHeight);
            if (EditorUtils.PropertyField(mRadius))
            {
                if (mRadius.floatValue < CylindricalPanoRenderer.minRadius) mRadius.floatValue = CylindricalPanoRenderer.minRadius;
            }
        }

        protected virtual void DrawUVContent()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(pUV, true);
            EditorGUI.indentLevel--;
        }

        private void DrawUVLabel()
        {
            EditorUtils.GroupLabel("UV");
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            DrawUVLabel();
            DrawUVContent();
        }
    }
}