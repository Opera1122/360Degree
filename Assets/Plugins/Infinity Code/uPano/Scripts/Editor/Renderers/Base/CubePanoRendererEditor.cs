/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Renderers.Base;
using UnityEditor;

namespace InfinityCode.uPano.Editors.Renderers
{
    public abstract class CubePanoRendererEditor<T> : PanoRendererEditor<T>
        where T : CubePanoRenderer<T>
    {
        protected SerializedProperty mSize;

        protected override void CacheSerializedFields()
        {
            base.CacheSerializedFields();

            mSize = FindProperty("_size");
        }

        protected override void DrawMeshContent()
        {
            PropertyField(mSize);
        }

        protected override void DrawTextureContent()
        {
            EditorGUI.BeginChangeCheck();
            PropertyField(shader);
            DrawMaterialContent();
            if (EditorGUI.EndChangeCheck()) needCheckMaterial = true;
        }
    }
}