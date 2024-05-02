/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors.Utils
{
    [InitializeOnLoad]
    public static class UpgradeCompatibility
    {
        private const string CHECK_ISSUES_KEY = "uPano.UpgradeCompatibility.CheckIssues";
        private static string[] obsoleteFilenames =
        {
            "Scripts/Services/GoogleStreetView.cs",
            "Scripts/Services/GoogleStreetViewDirection.cs",
            "Scripts/Editor/Services/GoogleStreetViewEditor.cs",
            "Scripts/Services/GoogleStreetViewMeta.cs",
            "Scripts/Requests/GoogleStreetViewMetaRequest.cs",
            "Scripts/Requests/GoogleStreetViewRequest.cs",
            "Examples/Scenes/08. Google Street View.unity"
        };

        static UpgradeCompatibility()
        {
            EditorApplication.delayCall += CheckOldFiles;
        }

        private static void CheckOldFiles()
        {
            EditorPrefs.SetBool(CHECK_ISSUES_KEY, true);
            if (!EditorPrefs.GetBool(CHECK_ISSUES_KEY, true)) return;

            string assetPath = EditorUtils.assetPath + "/";

            bool hasObsoleteFiles = false;

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < obsoleteFilenames.Length; i++)
            {
                builder.Length = 0;
                builder.Append(assetPath).Append(obsoleteFilenames[i]);
                if (File.Exists(builder.ToString()))
                {
                    hasObsoleteFiles = true;
                    break;
                }
            }

            if (!hasObsoleteFiles)
            {
                EditorPrefs.SetBool(CHECK_ISSUES_KEY, false);
                return;
            }

#if UPANO
            string[] connectors = AssetDatabase.FindAssets("OnlineMapsPanoConnector t:Script");
            bool hasPanoConnector = connectors.Length > 0;

            if (hasPanoConnector)
            {
                string msg = 
@"uPano has detected that you have updated an asset and are using the Online Maps Pano Connector.

What you need to do, step by step:
1. Close the current scene (open an empty one).
2. Open Edit / Project Settings / Player / Other Settings / Scripting Define Symbols, delete UPANO key and click Apply.
3. Window / Infinity Code / uPano / Fix Upgrade Issues.
4. Open Window / Infinity Code / uPano / Extension Manager, download and import Google Street View Service.
5. Open the map scene, and in the Online Maps Pano Connector click Enable uPano.";
                if (!EditorUtility.DisplayDialog("Update problem detected", msg, "OK", "Don't Show Again"))
                {
                    EditorPrefs.SetBool(CHECK_ISSUES_KEY, false);
                }
                return; 
            } 
#endif

            string message =
@"uPano has detected that you have updated an asset and the project contains obsolete files.
We recommend deleting obsolete files, and if you want to use Google Street View, open Window / Infinity Code / uPano / Extension Manager, download and import Google Street View Service using Extension Manager.";
            if (EditorUtility.DisplayDialog("Update problem detected", message, "Delete Obsolete Files", "Don't Show Again"))
            {
                FixUpgradeIssues();
            }
            else
            {
                EditorPrefs.SetBool(CHECK_ISSUES_KEY, false);
            }
        }

        [MenuItem(EditorUtils.MENU_PATH + "Fix Upgrade Issues", false, 98)]
        public static void FixUpgradeIssues()
        {
            try
            {
                string assetPath = EditorUtils.assetPath + "/";

                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < obsoleteFilenames.Length; i++)
                {
                    builder.Length = 0;
                    builder.Append(assetPath).Append(obsoleteFilenames[i]);
                    string filename = builder.ToString();
                    if (File.Exists(filename))
                    {
                        File.Delete(filename);
                    }

                    filename += ".meta";
                    if (File.Exists(filename))
                    {
                        File.Delete(filename);
                    }
                }

                AssetDatabase.Refresh();
                EditorPrefs.SetBool(CHECK_ISSUES_KEY, false);
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Error", e.Message + "\n" + e.StackTrace, "OK");
            }
        }
    }
}