/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using InfinityCode.uPano.Editors.Utils;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors.Windows
{
    public class Downloader : EditorWindow
    {
        private string url;

        private List<Provider> providers;

        public Color emptyColor = Color.gray;
        public bool pleaseWaitMessage;

        private int maxTextureSize = 4096;
        private bool generateMipMaps = false;
        private Provider activeProvider;

        private void DetectPanoProvider()
        {
            foreach (Provider p in providers)
            {
                if (p.Validate(url))
                {
                    activeProvider = p;
                    return;
                }
            }

            activeProvider = null;
        }

        private void OnEnable()
        {
            Type[] allTypes = Assembly.GetExecutingAssembly().GetTypes();
            providers = new List<Provider>();
            foreach (Type type in allTypes.Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(Provider))))
            {
                providers.Add(Activator.CreateInstance(type) as Provider);
            }
        }

        private void OnGUI()
        {
            if (providers.Count == 0)
            {
                EditorGUILayout.HelpBox("No providers found. Open Extension Manager to download providers.", MessageType.Error);
                if (GUILayout.Button("Open Extension Manager")) ExtensionManager.OpenWindow();
            }

            EditorGUIUtility.labelWidth = 210;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("URL");
            url = EditorGUILayout.TextArea(url);
            if (EditorGUI.EndChangeCheck()) DetectPanoProvider();

            EditorGUI.BeginDisabledGroup(true);
            string providerString = activeProvider != null ? activeProvider.title : "Unknown";
            EditorGUILayout.TextField("Panorama Provider: ", providerString);
            EditorGUI.EndDisabledGroup();

            if (activeProvider != null) activeProvider.OnGUI();

            emptyColor = EditorGUILayout.ColorField("Empty Color: ", emptyColor);

            maxTextureSize = EditorGUILayout.IntPopup("Max Size:", maxTextureSize,
                new[] { "512", "1024", "2048", "4096", "8192" },
                new[] { 512, 1024, 2048, 4096, 8192 });

            generateMipMaps = EditorGUILayout.Toggle("Generate Mip Maps: ", generateMipMaps);

            EditorGUI.BeginDisabledGroup(activeProvider == null || pleaseWaitMessage);
            if (GUILayout.Button("Download") && activeProvider != null) activeProvider.Download(this, url);
            EditorGUI.EndDisabledGroup();

            if (pleaseWaitMessage)
            {
                EditorGUILayout.LabelField("Please wait, the panorama is being downloaded ...");
                EditorGUILayout.HelpBox("If the panorama does not download for a long time, help Unity Editor send out coroutines by changing the size of this window.", MessageType.Info);

                Repaint();
            }
        }

        [MenuItem(EditorUtils.MENU_PATH + "Pano Downloader", false, 2)]
        private static void OpenWindow()
        {
            GetWindow<Downloader>(true, "Pano Downloader", true);
        }

        public void SaveTexture(string filename, Texture2D texture)
        {
            string directory = Path.Combine("Assets", "Panoramas");
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            string filePath = Path.Combine(directory, filename + ".png");

            File.WriteAllBytes(filePath, texture.EncodeToPNG());

            AssetDatabase.Refresh();

            Debug.Log("Panorama save to " + filePath);

            TextureImporter importer = AssetImporter.GetAtPath(filePath) as TextureImporter;
            importer.mipmapEnabled = generateMipMaps;
            importer.maxTextureSize = maxTextureSize;

            AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceUpdate);

            Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(filePath);
            EditorGUIUtility.PingObject(Selection.activeObject);
        }

        public abstract class Provider
        {
            public abstract string title { get; }

            public abstract void Download(Downloader downloader, string url);

            public virtual void OnGUI()
            {

            }

            public abstract bool Validate(string url);
        }
    }
}