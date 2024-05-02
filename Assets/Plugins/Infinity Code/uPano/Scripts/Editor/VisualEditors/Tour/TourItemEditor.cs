/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Editors.VisualEditors.InteractiveElements;
using InfinityCode.uPano.Tours;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors.VisualEditors.Tours
{
    [CustomEditor(typeof(TourItem))]
    public class TourItemEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Tour Maker")) TourMaker.OpenWindow();
            if (GUILayout.Button("Open Visual Interactive Element Editor")) VisualInteractiveElementEditor.OpenWindow();
        }
    }
}