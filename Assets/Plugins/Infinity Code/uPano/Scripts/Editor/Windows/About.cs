/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.Editors.Utils;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors.Windows
{
    public class About:EditorWindow
    {
        private string years;

        [MenuItem(EditorUtils.MENU_PATH + "About", false, 300)]
        public static void OpenWindow()
        {
            About window = GetWindow<About>(true, "About", true);
            window.minSize = new Vector2(200, 100);
            window.maxSize = new Vector2(200, 100);
            window.years = "2018";
            if (DateTime.Now.Year != 2018) window.years += "-" + DateTime.Now.Year;
        }

        public void OnGUI()
        {
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
            titleStyle.alignment = TextAnchor.MiddleCenter;

            GUIStyle textStyle = new GUIStyle(EditorStyles.label);
            textStyle.alignment = TextAnchor.MiddleCenter;

            GUILayout.Label("uPano", titleStyle);
            GUILayout.Label("version " + Pano.version, textStyle);
            GUILayout.Label("created Infinity Code", textStyle);
            GUILayout.Label(years, textStyle);
        }
    }
}
