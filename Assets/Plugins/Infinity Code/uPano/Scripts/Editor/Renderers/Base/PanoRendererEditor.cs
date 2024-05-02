/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Editors.Utils;
using InfinityCode.uPano.Renderers.Base;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors.Renderers
{
    public abstract class PanoRendererEditor<T>: SerializedEditor
        where T: PanoRenderer
    {
        protected T renderer;
        protected SerializedProperty defaultMaterial;
        protected SerializedProperty shader;
        protected SerializedProperty compressTextures;

        protected bool needCheckMaterial = true;

        private bool hasMainTexProp;
        private SerializedProperty pMainTex;

        protected override void CacheSerializedFields()
        {
            defaultMaterial = FindProperty("_defaultMaterial");
            compressTextures = FindProperty("compressTextures");
            shader = FindProperty("_shader");
            if (shader.objectReferenceValue == null) shader.objectReferenceValue = renderer.defaultShader;

            pMainTex = FindProperty("_mainTex");
        }

        private void CheckMaterialProps()
        {
            if (defaultMaterial.objectReferenceValue != null)
            {
                hasMainTexProp = (defaultMaterial.objectReferenceValue as Material).HasProperty("_MainTex");
            }
            else if (shader.objectReferenceValue != null)
            {
                Shader s = shader.objectReferenceValue as Shader;
                Material m = new Material(s);
                hasMainTexProp = m.HasProperty("_MainTex");
                DestroyImmediate(m);
            }
            else hasMainTexProp = true;

            needCheckMaterial = false;
        }

        protected void DrawInternalFields(SerializedProperty property)
        {
            SerializedProperty iterator = property.Copy();

            while (iterator.NextVisible(true))
            {
                PropertyField(iterator, iterator.displayName);
            }
        }

        protected void DrawMaterialContent()
        {
            PropertyField(defaultMaterial);
            EditorGUILayout.HelpBox("If the material is specified, the shader will be ignored!", MessageType.Info);
        }

        protected abstract void DrawMeshContent();

        private void DrawMeshLabel()
        {
            EditorUtils.GroupLabel("Mesh Settings");
        }

        protected abstract void DrawTextureContent();
        protected abstract void DrawTextureLabel();

        protected override void OnEnable()
        {
            renderer = (T)target;
            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck()) renderer.UpdateMesh();
        }

        protected override void OnGUI()
        {
            DrawTextureLabel();
            DrawTextureContent();

            if (needCheckMaterial) CheckMaterialProps();
            if (!hasMainTexProp) EditorGUILayout.PropertyField(pMainTex, new GUIContent("Main Texture Prop"));

            DrawMeshLabel();
            DrawMeshContent();
        }
    }
}
