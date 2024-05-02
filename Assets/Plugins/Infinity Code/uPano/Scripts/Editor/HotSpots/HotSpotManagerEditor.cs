/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Editors.InteractiveElements;
using InfinityCode.uPano.HotSpots;
using InfinityCode.uPano.InteractiveElements;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors.HotSpots
{
    [CustomEditor(typeof(HotSpotManager))]
    public class HotSpotManagerEditor: PrefabElementManagerEditor
    {
        private HotSpotManager _manager;

        protected override IInteractiveElementList manager
        {
            get { return _manager; }
        }

        protected override void CreateItem()
        {
            _manager.Create(0, 0, null);
        }

        protected override void DrawQuickActionsContent(SerializedProperty item)
        {
            base.DrawQuickActionsContent(item);

            SerializedProperty tooltip = item.FindPropertyRelative("tooltip");
            PropertyField(tooltip);
            if (!string.IsNullOrEmpty(tooltip.stringValue))
            {
                EditorGUI.indentLevel++;
                PropertyField(item.FindPropertyRelative("tooltipAction"));
                PropertyField(item.FindPropertyRelative("tooltipPrefab"));
                EditorGUI.indentLevel--;
            }
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
            PropertyField(item.FindPropertyRelative("_tilt"));
            PropertyField(item.FindPropertyRelative("_lookToCenter"));
            if (EditorGUI.EndChangeCheck())
            {
                if (Application.isPlaying)
                {
                    serializedObject.ApplyModifiedProperties();
                    _manager[index].UpdatePosition();
                }
                else if (_manager.isPreview)
                {
                    serializedObject.ApplyModifiedProperties();
                    _manager[index].UpdatePreviewPosition();
                }
            }

            SerializedProperty rotationProperty = item.FindPropertyRelative("_rotation");
            EditorGUI.BeginChangeCheck();
            Vector3 eulerAngles = EditorGUILayout.Vector3Field("Rotation", rotationProperty.quaternionValue.eulerAngles);
            if (EditorGUI.EndChangeCheck())
            {
                rotationProperty.quaternionValue = Quaternion.Euler(eulerAngles);
                if (Application.isPlaying || _manager.isPreview)
                {
                    _manager[index].rotation = rotationProperty.quaternionValue;
                }
            }

            EditorGUI.BeginChangeCheck();
            SerializedProperty scaleProperty = item.FindPropertyRelative("_scale");
            PropertyField(scaleProperty);
            if (EditorGUI.EndChangeCheck())
            {
                if (Application.isPlaying || _manager.isPreview) _manager[index].scale = scaleProperty.vector3Value;
            }

            EditorGUI.BeginChangeCheck();
            EditorGUIUtility.labelWidth += 10;
            SerializedProperty distanceMultiplierProperty = item.FindPropertyRelative("_distanceMultiplier");
            PropertyField(distanceMultiplierProperty);
            EditorGUIUtility.labelWidth -= 10;
            if (EditorGUI.EndChangeCheck())
            {
                if (Application.isPlaying) _manager[index].distanceMultiplier = distanceMultiplierProperty.floatValue;
                else if (_manager.isPreview)
                {
                    serializedObject.ApplyModifiedProperties();
                    _manager[index].UpdatePreviewPosition();
                }
            }
        }

        protected override void OnEnable()
        {
            _manager = (HotSpotManager)target;
            base.OnEnable();
        }

        protected override void RemoveItemAt(int index)
        {
            if (!Application.isPlaying) items.DeleteArrayElementAtIndex(index);
            _manager.RemoveAt(index);
        }
    }
}
