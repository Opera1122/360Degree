/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Editors.Utils;
using InfinityCode.uPano.Renderers.Base;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors.Renderers
{
    public abstract class SingleTexturePanoRendererEditor<T> : PanoRendererEditor<T>
        where T : SingleTexturePanoRenderer<T>
    {
        protected SerializedProperty texture;


        protected override void CacheSerializedFields()
        {
            base.CacheSerializedFields();

            texture = FindProperty("_texture");
        }

        protected override void DrawTextureLabel()
        {
            EditorUtils.GroupLabel("Texture Settings");
        }

        protected override void DrawTextureContent()
        {
            texture.objectReferenceValue = EditorGUILayout.ObjectField("Texture", renderer.texture, typeof(Texture), true) as Texture;
            PropertyField(compressTextures);

            EditorGUI.BeginChangeCheck();
            PropertyField(shader);
            DrawMaterialContent();
            if (EditorGUI.EndChangeCheck()) needCheckMaterial = true;
        }
    }
}