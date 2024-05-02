/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InfinityCode.uPano.Editors.Utils
{
    public static class EditorUtils
    {
        public const string MENU_PATH = "Window/Infinity Code/uPano/";

        private static string _assetPath;
        private static GUIStyle _groupStyle;
        private static Dictionary<Cubemap, Texture2D> cubemapCache = new Dictionary<Cubemap, Texture2D>();

        public static string assetPath
        {
            get
            {
                if (_assetPath == null)
                {
                    string[] dirs = Directory.GetDirectories("Assets", "uPano", SearchOption.AllDirectories);
                    _assetPath = dirs.Length > 0 ? dirs[0] : string.Empty;
                }
                return _assetPath;
            }
        }

        public static GUIStyle groupStyle
        {
            get
            {
                if (_groupStyle == null)
                {
                    _groupStyle = new GUIStyle(GUI.skin.box);
                    _groupStyle.normal.textColor = EditorStyles.label.normal.textColor;
                    _groupStyle.margin = new RectOffset(0, 0, 10, 5);
                }
                return _groupStyle;
            }
        }

        public static Texture2D GetCubemapTexture(Cubemap cubemap)
        {
            Texture2D t;
            if (cubemapCache.TryGetValue(cubemap, out t)) return t;

            string path = AssetDatabase.GetAssetPath(cubemap);
            byte[] bytes = File.ReadAllBytes(path);
            t = new Texture2D(1, 1);
            t.LoadImage(bytes);
            cubemapCache.Add(cubemap, t);
            return t;
        }

        public static void GroupLabel(string label)
        {
            GUILayout.Label(label, groupStyle, GUILayout.ExpandWidth(true));
        }

        public static T LoadAsset<T>(string path, bool throwOnMissed = false) where T : Object
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                if (throwOnMissed) throw new FileNotFoundException(assetPath);
                return default(T);
            }
            string filename = assetPath + "\\" + path;
            if (!File.Exists(filename))
            {
                if (throwOnMissed) throw new FileNotFoundException(assetPath);
                return default(T);
            }
            return (T)AssetDatabase.LoadAssetAtPath(filename, typeof(T));
        }

        public static void ImportPackage(string path, Warning warning = null, string errorMessage = null)
        {
            if (warning != null && !warning.Show()) return;
            if (string.IsNullOrEmpty(assetPath))
            {
                if (!string.IsNullOrEmpty(errorMessage)) Debug.LogError(errorMessage);
                return;
            }

            string filaname = assetPath + "\\" + path;
            if (!File.Exists(filaname))
            {
                if (!string.IsNullOrEmpty(errorMessage)) Debug.LogError(errorMessage);
                return;
            }

            AssetDatabase.ImportPackage(filaname, true);
        }

        public static bool PropertyField(SerializedProperty property, GUIContent label = null)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(property, label);
            return EditorGUI.EndChangeCheck();
        }

        public static void SetDirty(Object target)
        {
            EditorUtility.SetDirty(target);
            if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
    }
}
