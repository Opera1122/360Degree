/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using UnityEditor;

namespace InfinityCode.uPano.Editors.Utils
{
    public class Warning
    {
        public string title = "Warning";
        public string message;
        public string ok = "OK";
        public string cancel = "Cancel";

        public bool Show()
        {
            return EditorUtility.DisplayDialog(title, message, ok, cancel);
        }
    }
}