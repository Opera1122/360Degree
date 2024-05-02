/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Controls;
using UnityEditor;

namespace InfinityCode.uPano.Editors.Controls
{
    public abstract class SensitivityControlEditor : SerializedEditor
    {
        protected SerializedProperty pAxes;
        protected SerializedProperty pSensitivityX;
        protected SerializedProperty pSensitivityY;
        protected SerializedProperty pSensitivityZoom;
        protected SerializedProperty pAdaptiveSensitivity;
        protected SerializedProperty pFovSensitivityMulCurve;

        protected override void CacheSerializedFields()
        {
            pAxes = FindProperty("axes");
            pSensitivityX = FindProperty("sensitivityPan");
            pSensitivityY = FindProperty("sensitivityTilt");
            pSensitivityZoom = FindProperty("sensitivityFov");
            pAdaptiveSensitivity = FindProperty("adaptiveSensitivity");
            pFovSensitivityMulCurve = FindProperty("fovSensitivityMulCurve");
        }

        protected void DrawAxes()
        {
            PropertyField(pAxes);
            if (pAxes.enumValueIndex == (int)SensitivityControl.Axes.PanTilt ||
                pAxes.enumValueIndex == (int)SensitivityControl.Axes.Pan)
            {
                PropertyField(pSensitivityX);
            }

            if (pAxes.enumValueIndex == (int)SensitivityControl.Axes.PanTilt ||
                pAxes.enumValueIndex == (int)SensitivityControl.Axes.Tilt)
            {
                PropertyField(pSensitivityY);
            }

            PropertyField(pSensitivityZoom);
            PropertyField(pAdaptiveSensitivity);
            if (pAdaptiveSensitivity.boolValue) PropertyField(pFovSensitivityMulCurve);

        }

        protected override void OnGUI()
        {
            DrawAxes();
        }
    }
}