/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System.Collections.Generic;
using System.Linq;
using System.Net;
using InfinityCode.uPano.Editors.Utils;
using InfinityCode.uPano.Json;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors.Windows
{
    public class ExtensionManager: EditorWindow
    {
        private string invoiceNumber = "";
        private Group[] groups = new Group[0];

        private void OnEnable()
        {
            if (EditorPrefs.HasKey(Updater.invoiceNumberKey)) invoiceNumber = EditorPrefs.GetString(Updater.invoiceNumberKey);
            else invoiceNumber = "";

            WebClient client = new WebClient();

            string response;

            try
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                response = client.DownloadString("http://infinity-code.com/extensions/get-extensions.php?asset=uPano");
                JSONItem json = JSON.Parse(response);
                groups = json[0].Deserialize<Group[]>();
                Item[] items = json[1].Deserialize<Item[]>();
                foreach (Item item in items)
                {
                    Group cg = groups.FirstOrDefault(g => g.id == item.@group);
                    if (cg != null) cg.items.Add(item);
                }
            }
            catch
            {
                return;
            }
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            invoiceNumber = EditorGUILayout.TextField("Invoice Number", invoiceNumber);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetString(Updater.invoiceNumberKey, invoiceNumber);
            }
            foreach (Group g in groups) g.Draw(this);
        }

        [MenuItem(EditorUtils.MENU_PATH + "Extension Manager", false, 98)]
        public static void OpenWindow()
        {
            GetWindow<ExtensionManager>("Extension Manager");
        }

        internal class Group
        {
            public string id = string.Empty;
            public string title = string.Empty;
            public string description = string.Empty;
            private bool expanded = true;

            public List<Item> items = new List<Item>();

            public void Draw(ExtensionManager manager)
            {
                if (items.Count == 0) return;

                GUILayout.BeginVertical(GUI.skin.box);

                expanded = EditorGUILayout.Foldout(expanded, title);

                if (expanded)
                {
                    if (!string.IsNullOrEmpty(description))
                    {
                        EditorGUILayout.HelpBox(description, MessageType.Info);
                    }

                    foreach (Item item in items) item.Draw(manager);
                }

                GUILayout.EndVertical();
            }
        }

        internal class Item
        {
            public string id = string.Empty;
            public string title = string.Empty;
            public string group = string.Empty;
            public string description = string.Empty;
            public string version = string.Empty;
            public string datetime = string.Empty;
            public string requirments = string.Empty;

            public void Draw(ExtensionManager manager)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.LabelField(title);
                if (!string.IsNullOrEmpty(description)) EditorGUILayout.HelpBox(description, MessageType.None);
                EditorGUILayout.LabelField("Version ", version);
                EditorGUILayout.LabelField("Published", datetime);
                if (!string.IsNullOrEmpty(requirments)) EditorGUILayout.LabelField("Requirements", requirments);
                if (GUILayout.Button("Download"))
                {
                    Application.OpenURL("http://infinity-code.com/extensions/download.php?asset=uPano&id=" + id + "&invoice=" + manager.invoiceNumber.Trim());
                }
                GUILayout.EndVertical();
            }
        }
    }
}