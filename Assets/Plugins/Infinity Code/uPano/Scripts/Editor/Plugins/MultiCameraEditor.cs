/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Plugins;
using UnityEditor;

namespace InfinityCode.uPano.Editors.Plugins
{
    [CustomEditor(typeof(MultiCamera), true)]
    public class MultiCameraEditor : SerializedEditor
    {
        private SerializedProperty pItemsType;
        private SerializedProperty pCameras;
        private SerializedProperty pCamerasContainer;
        private SerializedProperty pUpdateDistance;
        private SerializedProperty pDistanceCurve;
        private SerializedProperty pInitType;
        private SerializedProperty pExcludeContainer;

        protected override void CacheSerializedFields()
        {
            pItemsType = FindProperty("itemsType");
            pCameras = FindProperty("cameras");
            pCamerasContainer = FindProperty("camerasContainer");
            pUpdateDistance = FindProperty("updateDistance");
            pDistanceCurve = FindProperty("distanceCurve");
            pInitType = FindProperty("initType");
            pExcludeContainer = FindProperty("excludeContainer");
        }

        protected override void OnGUI()
        {
            PropertyField(pItemsType);
            if (pItemsType.enumValueIndex == (int) MultiCamera.ItemsType.manual)
            {
                EditorGUILayout.PropertyField(pCameras, true);
            }
            else
            {
                PropertyField(pCamerasContainer);
                PropertyField(pInitType);
                PropertyField(pExcludeContainer);
            }

            PropertyField(pUpdateDistance);
            EditorGUI.BeginDisabledGroup(!pUpdateDistance.boolValue);
            PropertyField(pDistanceCurve);
            EditorGUI.EndDisabledGroup();
        }
    }
}