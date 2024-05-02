/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Plugins;
using InfinityCode.uPano.Renderers;
using InfinityCode.uPano.Renderers.Base;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors.Plugins
{
    [CustomEditor(typeof(Downloader))]
    public class DownloaderEditor : Editor
    {
        private SerializedProperty useLowRes;
        private SerializedProperty url;
        private SerializedProperty lowresUrl;
        private SerializedProperty sidesURLs;
        private SerializedProperty lowresSidesURLs;
        private PanoRenderer panoRenderer;

        private void OnEnable()
        {
            useLowRes = serializedObject.FindProperty("useLowRes");
            url = serializedObject.FindProperty("url");
            lowresUrl = serializedObject.FindProperty("lowresUrl");
            sidesURLs = serializedObject.FindProperty("sidesURLs");
            lowresSidesURLs = serializedObject.FindProperty("lowresSidesURLs");

            panoRenderer = (target as Downloader).GetComponent<PanoRenderer>();
        }

        public override void OnInspectorGUI()
        {
            if (panoRenderer == null)
            {
                EditorGUILayout.HelpBox("Can not find PanoRenderer", MessageType.Error);
                return;
            }

            serializedObject.Update();

            if (panoRenderer is ISingleTexturePanoRenderer)
            {
                EditorGUILayout.PropertyField(useLowRes, new GUIContent("Use low-resolution?"));
                if (useLowRes.boolValue) EditorGUILayout.PropertyField(lowresUrl, new GUIContent("Low-resolution url"));
                EditorGUILayout.PropertyField(url, new GUIContent("Full-resolution url"));
            }
            else if (panoRenderer is CubeFacesPanoRenderer)
            {
                EditorGUILayout.PropertyField(useLowRes, new GUIContent("Use low-resolution?"));
                if (sidesURLs.arraySize != 6) sidesURLs.arraySize = 6;
                if (lowresSidesURLs.arraySize != 6) lowresSidesURLs.arraySize = 6;

                if (useLowRes.boolValue)
                {
                    EditorGUILayout.LabelField("Low-resolution urls");
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < 6; i++)
                    {
                        EditorGUILayout.PropertyField(lowresSidesURLs.GetArrayElementAtIndex(i), new GUIContent(CubeUV.sides[i]));
                    }
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.LabelField("Full-resolution urls");
                EditorGUI.indentLevel++;
                for (int i = 0; i < 6; i++)
                {
                    EditorGUILayout.PropertyField(sidesURLs.GetArrayElementAtIndex(i), new GUIContent(CubeUV.sides[i]));
                }
                EditorGUI.indentLevel--;
            }
            else EditorGUILayout.HelpBox("Downloader does not support this PanoRenderer", MessageType.Error);

            serializedObject.ApplyModifiedProperties();
        }
    }
}