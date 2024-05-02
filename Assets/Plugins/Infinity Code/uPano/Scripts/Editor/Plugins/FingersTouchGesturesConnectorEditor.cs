/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using InfinityCode.uPano.Plugins;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors.Plugins
{
    [CustomEditor(typeof(FingersTouchGesturesConnector))]
    public class FingersTouchGesturesConnectorEditor : Editor
    {
#if FINGERS_TG
        private FingersTouchGesturesConnector connector;

        private void OnEnable()
        {
            connector = target as FingersTouchGesturesConnector;
        }
#endif

        public override void OnInspectorGUI()
        {
#if !FINGERS_TG
            if (GUILayout.Button("Enable Fingers - Touch Gestures"))
            {
                if (EditorUtility.DisplayDialog("Enable Fingers - Touch Gestures", "You have Fingers - Touch Gestures in your project?", "Yes, I have Fingers - Touch Gestures", "Cancel"))
                {
                    PanoEditor.AddCompilerDirective("FINGERS_TG");
                }
            }
#else
            base.OnInspectorGUI();
#endif
        }
    }
}