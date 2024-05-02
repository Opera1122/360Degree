/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Editors.Utils;
using InfinityCode.uPano.Renderers;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors.Renderers
{
    [CustomEditor(typeof(CubeFacesPanoRenderer))]
    public class CubeFacesPanoRendererEditor : CubePanoRendererEditor<CubeFacesPanoRenderer>
    {
        private SerializedProperty mTextures;

        protected override void CacheSerializedFields()
        {
            base.CacheSerializedFields();

            mTextures = FindProperty("_textures");
        }

        protected override void DrawTextureContent()
        {
            if (mTextures.arraySize != 6) mTextures.arraySize = 6;

            EditorGUI.BeginChangeCheck();
            for (int i = 0; i < 6; i++)
            {
                SerializedProperty property = mTextures.GetArrayElementAtIndex(i);
                PropertyField(property, CubeUV.sides[i]);
            }
            if (EditorGUI.EndChangeCheck())
            {
                renderer.UpdateMesh();
            }

            base.DrawTextureContent();
        }

        protected override void DrawTextureLabel()
        {
            EditorUtils.GroupLabel("Textures");
        }
    }
}