/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Plugins;
using UnityEditor;

namespace InfinityCode.uPano.Editors.Plugins
{
    [CustomEditor(typeof(OrthographicCameras), true)]
    public class OrthographicCamerasEditor : MultiCameraEditor
    {
        private SerializedProperty pOrthographicSizeCurve;

        protected override void CacheSerializedFields()
        {
            base.CacheSerializedFields();

            pOrthographicSizeCurve = FindProperty("orthographicSizeCurve");
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            PropertyField(pOrthographicSizeCurve);
        }
    }
}