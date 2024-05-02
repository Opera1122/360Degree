/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors
{
    [CustomPropertyDrawer(typeof(PanTilt), true)]
    public class PanTiltDrawer : PropertyDrawer
    {
        private GUIContent _panContent;
        private GUIContent _tiltContent;

        public GUIContent panContent
        {
            get
            {
                if (_panContent == null) _panContent = new GUIContent("Pan");
                return _panContent;
            }
        }

        public GUIContent tiltContent
        {
            get
            {
                if (_tiltContent == null) _tiltContent = new GUIContent("Tilt");
                return _tiltContent;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            const float margin = 2;

            Rect panRect = new Rect(position.x, position.y, position.width / 2 - margin, position.height - 1);
            Rect tiltRect = new Rect(position.x + position.width / 2 + margin * 2, position.y, position.width / 2 - margin, position.height - 1);

            float labelWidth = EditorGUIUtility.labelWidth;

            EditorGUIUtility.labelWidth = 25;
            EditorGUI.PropertyField(panRect, property.FindPropertyRelative("pan"), panContent);
            EditorGUI.PropertyField(tiltRect, property.FindPropertyRelative("tilt"), tiltContent);

            EditorGUIUtility.labelWidth = labelWidth;

            EditorGUI.EndProperty();
        }
    }
}