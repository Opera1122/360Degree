/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Editors.Utils;
using InfinityCode.uPano.Renderers;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors.Renderers
{
    [CustomEditor(typeof(SingleTextureCubeFacesPanoRenderer))]
    public class SingleTextureCubeFacesPanoRendererEditor : SingleTexturePanoRendererEditor<SingleTextureCubeFacesPanoRenderer>
    {
        protected SerializedProperty pSize;
        protected SerializedProperty mCubeUV;
        protected SerializedProperty mCubeUp;
        protected SerializedProperty mCubeFront;
        protected SerializedProperty mCubeLeft;
        protected SerializedProperty mCubeBack;
        protected SerializedProperty mCubeRight;
        protected SerializedProperty mCubeDown;

        protected override void CacheSerializedFields()
        {
            base.CacheSerializedFields();

            pSize = FindProperty("_size");
            mCubeUV = FindProperty("_cubeUV");

            mCubeUp = mCubeUV.FindPropertyRelative("top");
            mCubeFront = mCubeUV.FindPropertyRelative("front");
            mCubeLeft = mCubeUV.FindPropertyRelative("left");
            mCubeBack = mCubeUV.FindPropertyRelative("back");
            mCubeRight = mCubeUV.FindPropertyRelative("right");
            mCubeDown = mCubeUV.FindPropertyRelative("bottom");
        }

        protected override void DrawMeshContent()
        {
            EditorUtils.PropertyField(pSize);
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            DrawUVLabel();
            DrawUVContent();
        }

        protected virtual void DrawUVContent()
        {
            if (GUILayout.Button("Presets"))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Horizontal Cross"), false, () =>
                {
                    renderer.cubeUV = CubeUVPresets.horizontalCrossPreset;
                    serializedObject.Update();
                });
                menu.AddItem(new GUIContent("Vertical Cross"), false, () =>
                {
                    renderer.cubeUV = CubeUVPresets.verticalCrossPreset;
                    serializedObject.Update();
                });
                menu.AddItem(new GUIContent("Youtube (3x2)"), false, () =>
                {
                    renderer.cubeUV = CubeUVPresets.youtubePreset;
                    serializedObject.Update();
                });
                menu.ShowAsContext();
            }

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(mCubeUp, true);
            EditorGUILayout.PropertyField(mCubeFront, true);
            EditorGUILayout.PropertyField(mCubeLeft, true);
            EditorGUILayout.PropertyField(mCubeBack, true);
            EditorGUILayout.PropertyField(mCubeRight, true);
            EditorGUILayout.PropertyField(mCubeDown, true);
            EditorGUI.indentLevel--;
        }

        private void DrawUVLabel()
        {
            EditorUtils.GroupLabel("UV");
        }
    }
}