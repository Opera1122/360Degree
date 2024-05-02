/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Directions;
using InfinityCode.uPano.Editors.InteractiveElements;
using InfinityCode.uPano.InteractiveElements;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors.Directions
{
    [CustomEditor(typeof(DirectionManager))]
    public class DirectionManagerEditor: PrefabElementManagerEditor
    {
        private DirectionManager _manager;

        private SerializedProperty internalRadius;
        private SerializedProperty verticalOffset;
        private SerializedProperty externalRadius;

        protected override IInteractiveElementList manager
        {
            get { return _manager; }
        }

        protected override void CacheSerializedFields()
        {
            base.CacheSerializedFields();

            verticalOffset = FindProperty("_verticalOffset");
            internalRadius = FindProperty("_internalRadius");
            externalRadius = FindProperty("_externalRadius");
        }

        protected override void CreateItem()
        {
            _manager.Create(0, null);
        }

        protected override void DrawItemContent(SerializedProperty item, int index)
        {
            EditorGUI.BeginChangeCheck();
            SerializedProperty prefabProperty = item.FindPropertyRelative("_prefab");
            PropertyField(prefabProperty);
            if (EditorGUI.EndChangeCheck() && Application.isPlaying)
            {
                _manager[index].prefab = prefabProperty.objectReferenceValue as GameObject;
            }

            EditorGUI.BeginChangeCheck();
            PropertyField(item.FindPropertyRelative("_pan"));
            if (EditorGUI.EndChangeCheck())
            {
                if (Application.isPlaying)
                {
                    serializedObject.ApplyModifiedProperties();
                    _manager.items[index].UpdatePosition();
                }
            }

            EditorGUI.BeginChangeCheck();
            SerializedProperty scaleProperty = item.FindPropertyRelative("_scale");
            PropertyField(scaleProperty);
            if (EditorGUI.EndChangeCheck())
            {
                if (Application.isPlaying) _manager[index].scale = scaleProperty.vector3Value;
            }
        }

        protected override void OnEnable()
        {
            _manager = target as DirectionManager;
            base.OnEnable();
        }

        protected override void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            PropertyField(internalRadius);
            PropertyField(externalRadius);
            PropertyField(verticalOffset);
            if (EditorGUI.EndChangeCheck())
            {
                if (Application.isPlaying)
                {
                    serializedObject.ApplyModifiedProperties();
                    _manager.UpdatePosition();
                }
            }

            base.OnGUI();
        }

        protected override void RemoveItemAt(int index)
        {
            if (!Application.isPlaying) items.DeleteArrayElementAtIndex(index);
            _manager.RemoveAt(index);
        }
    }
}
