/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Controls;
using UnityEditor;

namespace InfinityCode.uPano.Editors.Controls
{
    [CustomEditor(typeof(MouseControl))]
    public class MouseControlEditor : SensitivityControlEditor
    {
        private SerializedProperty pMode;
        private SerializedProperty pNotInteractUnderUI;
        private SerializedProperty pWheelZoom;
        private SerializedProperty pPinchToZoom;
        private SerializedProperty pInertia;
        private SerializedProperty pInertiaLerpSpeed;

        protected override void CacheSerializedFields()
        {
            base.CacheSerializedFields();
            pMode = FindProperty("mode");
            pNotInteractUnderUI = FindProperty("notInteractUnderUI");
            pWheelZoom = FindProperty("wheelZoom");
            pPinchToZoom = FindProperty("pinchToZoom");
            pInertia = FindProperty("inertia");
            pInertiaLerpSpeed = FindProperty("inertiaLerpSpeed");
        }

        protected override void OnGUI()
        {
            PropertyField(pMode);
            if (pMode.enumValueIndex == (int)MouseControl.Mode.Free)
            {
                DrawAxes();
            }
            else if (pMode.enumValueIndex == (int)MouseControl.Mode.LeftMouseButtonDown)
            {
                PropertyField(pNotInteractUnderUI);
                DrawAxes();
            }
            else if (pMode.enumValueIndex == (int) MouseControl.Mode.Drag)
            {
                PropertyField(pNotInteractUnderUI);
                PropertyField(pSensitivityZoom);
            }

            PropertyField(pWheelZoom);
            PropertyField(pPinchToZoom);

            PropertyField(pInertia);
            if (pInertia.boolValue) PropertyField(pInertiaLerpSpeed);
        }
    }

}
