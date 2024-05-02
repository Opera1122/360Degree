/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Editors.Utils;
using InfinityCode.uPano.Plugins;
using UnityEditor;

namespace InfinityCode.uPano.Editors.Plugins
{
    [CustomEditor(typeof(Limits))]
    public class LimitsEditor : Editor
    {
        private Limits limits;

        private void OnEnable()
        {
            limits = target as Limits;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            limits.limitPan = EditorGUILayout.Toggle("Pan", limits.limitPan);
            if (limits.limitPan)
            {
                EditorGUI.indentLevel++;
                if (limits.panLimits == null) limits.panLimits = new FloatRange(0, 360);
                limits.panLimits.min = EditorGUILayout.FloatField("Min", limits.panLimits.min);
                limits.panLimits.max = EditorGUILayout.FloatField("Max", limits.panLimits.max);
                EditorGUI.indentLevel--;
            }

            limits.limitTilt = EditorGUILayout.Toggle("Tilt", limits.limitTilt);
            if (limits.limitTilt)
            {
                EditorGUI.indentLevel++;
                if (limits.tiltLimits == null) limits.tiltLimits = new FloatRange(-90, 90);
                limits.tiltLimits.min = EditorGUILayout.FloatField("Min", limits.tiltLimits.min);
                limits.tiltLimits.max = EditorGUILayout.FloatField("Max", limits.tiltLimits.max);
                EditorGUI.indentLevel--;
            }

            limits.limitFOV = EditorGUILayout.Toggle("FOV", limits.limitFOV);
            if (limits.limitFOV)
            {
                EditorGUI.indentLevel++;
                if (limits.fovLimits == null) limits.fovLimits = new FloatRange(5, 60);
                limits.fovLimits.min = EditorGUILayout.FloatField("Min", limits.fovLimits.min);
                limits.fovLimits.max = EditorGUILayout.FloatField("Max", limits.fovLimits.max);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            if (EditorGUI.EndChangeCheck()) EditorUtils.SetDirty(target);
        }
    }
}