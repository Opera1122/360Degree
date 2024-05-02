/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors
{
    public abstract class SerializedEditor: Editor
    {
        protected abstract void CacheSerializedFields();

        protected SerializedProperty FindProperty(string propertyPath)
        {
            return serializedObject.FindProperty(propertyPath);
        }

        protected virtual void OnEnable()
        {
            serializedObject.Update();
            CacheSerializedFields();
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            OnGUI();
            serializedObject.ApplyModifiedProperties();
        }

        protected abstract void OnGUI();

        protected bool PropertyField(SerializedProperty serializedProperty)
        {
            return EditorGUILayout.PropertyField(serializedProperty);
        }

        protected bool PropertyField(SerializedProperty serializedProperty, string label)
        {
            return EditorGUILayout.PropertyField(serializedProperty, new GUIContent(label));
        }

        protected bool PropertyField(SerializedProperty serializedProperty, GUIContent label)
        {
            return EditorGUILayout.PropertyField(serializedProperty, label);
        }
    }
}
