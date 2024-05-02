/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System.IO;
using InfinityCode.uPano.Components;
using InfinityCode.uPano.Editors.Utils;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors
{
    [CustomEditor(typeof(Documentation))]
    [InitializeOnLoad]
    public class DocumentationEditor : Editor
    {
        private static GUIStyle style;

        static DocumentationEditor()
        {
            EditorApplication.delayCall += TryRemoveOldDoc;
        }

        private void DrawDocumentation()
        {
            if (GUILayout.Button("Local Documentation"))
            {
                string assetPath = AssetDatabase.GetAssetPath(target);
                FileInfo fileInfo = new FileInfo (assetPath);
                Application.OpenURL(fileInfo.DirectoryName + "/Content/Documentation-Content.html");
            }

            if (GUILayout.Button("Online Documentation"))
            {
                Links.OpenDocumentation();
            }

            if (GUILayout.Button("API Reference"))
            {
                Links.OpenAPIReference();
            }

            GUILayout.Space(10);
        }

        private static void DrawExtra()
        {
            if (GUILayout.Button("Homepage"))
            {
                Links.OpenHomepage();
            }

            if (GUILayout.Button("Asset Store"))
            {
                Links.OpenAssetStore();
            }

            if (GUILayout.Button("Changelog"))
            {
                Links.OpenChangelog();
            }

            GUILayout.Space(10);
        }

        private new static void DrawHeader()
        {
            if (style == null)
            {
                style = new GUIStyle(EditorStyles.label);
                style.alignment = TextAnchor.MiddleCenter;
            }

            GUILayout.Label("uPano", style);
            GUILayout.Label("version: " + Pano.version, style);
            GUILayout.Space(10);
        }

        private void DrawRateAndReview()
        {
            EditorGUILayout.HelpBox("Please don't forget to leave a review on the store page if you liked uPano, this helps us a lot!", MessageType.Warning);

            if (GUILayout.Button("Rate & Review"))
            {
                Links.OpenReviews();
            }
        }

        private void DrawSupport()
        {
            if (GUILayout.Button("Support"))
            {
                Links.OpenSupport();
            }

            if (GUILayout.Button("Forum"))
            {
                Links.OpenForum();
            }

            GUILayout.Space(10);
        }

        public override void OnInspectorGUI()
        {
            DrawHeader();
            DrawDocumentation();
            DrawExtra();
            DrawSupport();
            DrawRateAndReview();
        }

        private static void TryRemoveOldDoc()
        {
            string filename = EditorUtils.assetPath + "/Documentation/Readme.pdf";
            if (!File.Exists(filename)) return;

            try
            {
                File.Delete(filename);
            }
            catch
            {
                return;
            }

            filename += ".meta";
            if (!File.Exists(filename)) return;

            try
            {
                File.Delete(filename);
            }
            catch
            {

            }
        }
    }
}
