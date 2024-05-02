/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Editors.Utils;
using InfinityCode.uPano.Renderers;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors.Renderers
{
    [CustomEditor(typeof(CubemapPanoRenderer))]
    public class CubemapPanoRendererEditor: PanoRendererEditor<CubemapPanoRenderer>
    {
        protected SerializedProperty pCubemap;
        protected SerializedProperty mSize;

        protected override void CacheSerializedFields()
        {
            base.CacheSerializedFields();

            pCubemap = FindProperty("_cubemap");
            mSize = FindProperty("_size");
        }

        protected override void DrawMeshContent()
        {
            PropertyField(mSize);
        }

        protected override void DrawTextureContent()
        {
            pCubemap.objectReferenceValue = EditorGUILayout.ObjectField("Cubemap", renderer.cubemap, typeof(Cubemap), true) as Cubemap;
            PropertyField(shader);
            DrawMaterialContent();
        }

        protected override void DrawTextureLabel()
        {
            EditorUtils.GroupLabel("Cubemap Settings");
        }
    }
}
