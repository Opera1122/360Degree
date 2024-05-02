/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using InfinityCode.uPano.Editors.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

namespace InfinityCode.uPano.Editors.Windows
{
    public class Updater : EditorWindow
    {
        private const string packageID = "upano";
        private const string assetPrefix = "uPano";
        private const string lastVersionKey = assetPrefix + "LastVersion";
        private const string lastVersionCheckKey = assetPrefix + "LastVersionCheck";
        private const string channelKey = assetPrefix + "UpdateChannel";
        public const string invoiceNumberKey = assetPrefix + "InvoiceNumber";

        private static string lastVersionID;

        public static bool hasNewVersion;
        private static Channel channel = Channel.stable;
        private string invoiceNumber;
        private Vector2 scrollPosition;
        private List<Item> updates;

        private void CheckNewVersions()
        {
            if (string.IsNullOrEmpty(invoiceNumber))
            {
                EditorUtility.DisplayDialog("Error", "Please enter the Invoice Number.", "OK");
                return;
            }

            SavePrefs();

            string updateKey = GetUpdateKey();
            GetUpdateList(updateKey);
        }

        public static void CheckNewVersionAvailable()
        {
            if (EditorPrefs.HasKey(lastVersionKey))
            {
                lastVersionID = EditorPrefs.GetString(lastVersionKey);

                if (CompareVersions())
                {
                    hasNewVersion = true;
                    return;
                }
            }

            const long ticksInHour = 36000000000;

            if (EditorPrefs.HasKey(lastVersionCheckKey))
            {
                long lastVersionCheck = EditorPrefs.GetInt(lastVersionCheckKey) * ticksInHour;
                if (DateTime.Now.Ticks - lastVersionCheck < 24 * ticksInHour)
                {
                    return;
                }
            }

            EditorPrefs.SetInt(lastVersionCheckKey, (int)(DateTime.Now.Ticks / ticksInHour));

            if (EditorPrefs.HasKey(channelKey)) channel = (Channel)EditorPrefs.GetInt(channelKey);
            else channel = Channel.stable;

            if (channel == Channel.stablePrevious) channel = Channel.stable;

            WebClient client = new WebClient();

            client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            client.UploadDataCompleted += delegate (object sender, UploadDataCompletedEventArgs response)
            {
                if (response.Error != null)
                {
                    Debug.Log(response.Error.Message);
                    return;
                }

                string version = Encoding.UTF8.GetString(response.Result);

                try
                {
                    string[] vars = version.Split(new[] { '.' });
                    string[] vars2 = new string[4];
                    while (vars[1].Length < 8) vars[1] += "0";
                    vars2[0] = vars[0];
                    vars2[1] = int.Parse(vars[1].Substring(0, 2)).ToString();
                    vars2[2] = int.Parse(vars[1].Substring(2, 2)).ToString();
                    vars2[3] = int.Parse(vars[1].Substring(4)).ToString();

                    version = string.Join(".", vars2);
                }
                catch (Exception)
                {
                    Debug.Log("Automatic check for uPano updates: Bad response.");
                    return;
                }

                lastVersionID = version;

                hasNewVersion = CompareVersions();
                EditorApplication.update += SetLastVersion;
            };
            Uri uri = new Uri("http://infinity-code.com/products_update/getlastversion.php");

            string p = UnityWebRequest.EscapeURL(packageID);        
            byte[] postData = Encoding.UTF8.GetBytes("c=" + (int)channel + "&package=" + p);
            client.UploadDataAsync(uri, "POST", postData);
        }

        private static bool CompareVersions()
        {
            double v1 = GetDoubleVersion(Pano.version);
            double v2 = GetDoubleVersion(lastVersionID);
            return v1 < v2;
        }

        private static double GetDoubleVersion(string v)
        {
            string[] vs = v.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            if (vs[1].Length < 2) vs[1] = "0" + vs[1];
            if (vs[2].Length < 2) vs[2] = "0" + vs[2];
            if (vs[3].Length < 4)
            {
                vs[3] = "000" + vs[3];
                vs[3] = vs[3].Substring(vs[3].Length - 4, 4);
            }
            v = vs[0] + "." + vs[1] + vs[2] + vs[3];
            double result;
            if (!double.TryParse(v, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out result)) result = 1;
            return result;
        }

        private string GetUpdateKey()
        {
            WebClient client = new WebClient();
            client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            string updateKey = client.UploadString("http://infinity-code.com/products_update/getupdatekey.php",
                "key=" + invoiceNumber + "&package=" + packageID);

            return updateKey;
        }

        private void GetUpdateList(string updateKey)
        {
            WebClient client = new WebClient();
            client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

            string response;

            try
            {
                string k = UnityWebRequest.EscapeURL(updateKey);
                response = client.UploadString("http://infinity-code.com/products_update/checkupdates.php",
                    "k=" + k + "&v=" + Pano.version + "&c=" + (int) channel);
            }
            catch
            {
                return;
            }

            XmlDocument document = new XmlDocument();
            document.LoadXml(response);

            XmlNode firstChild = document.FirstChild;
            updates = new List<Item>();

            foreach (XmlNode node in firstChild.ChildNodes) updates.Add(new Item(node));
        }

        private void OnEnable()
        {
            if (EditorPrefs.HasKey(invoiceNumberKey)) invoiceNumber = EditorPrefs.GetString(invoiceNumberKey);
            else invoiceNumber = "";

            if (EditorPrefs.HasKey(channelKey)) channel = (Channel) EditorPrefs.GetInt(channelKey);
            else channel = Channel.stable;
        }

        private void OnDestroy()
        {
            SavePrefs();
        }

        private void OnGUI()
        {
            invoiceNumber = EditorGUILayout.TextField("Invoice Number:", invoiceNumber).Trim(' ');
            channel = (Channel) EditorGUILayout.EnumPopup("Channel:", channel);

            if (GUILayout.Button("Check new versions")) CheckNewVersions();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            if (updates != null)
            {
                foreach (Item update in updates) update.Draw();
                if (updates.Count == 0) GUILayout.Label("No updates");
            }

            EditorGUILayout.EndScrollView();
        }

        [MenuItem(EditorUtils.MENU_PATH + "Check Updates", false, 100)]
        public static void OpenWindow()
        {
            GetWindow<Updater>(false, "uPano Updater", true);
        }

        private void SavePrefs()
        {
            EditorPrefs.SetString(invoiceNumberKey, invoiceNumber);
            EditorPrefs.SetInt(channelKey, (int) channel);
        }

        private static void SetLastVersion()
        {
            EditorPrefs.SetString(lastVersionKey, lastVersionID);
            EditorApplication.update -= SetLastVersion;
        }

        private static string Unescape(string str)
        {
            return Regex.Replace(str, @"&(\w+);", delegate (Match match)
            {
                string v = match.Groups[1].Value;
                if (v == "amp") return "&";
                if (v == "lt") return "<";
                if (v == "gt") return ">";
                if (v == "quot") return "\"";
                if (v == "apos") return "'";
                return match.Value;
            });
        }

        public class Item
        {
            private string version;
            private int type;
            private string changelog;
            private string download;
            private string date;

            private static GUIStyle _changelogStyle;
            private static GUIStyle _titleStyle;

            private static GUIStyle changelogStyle
            {
                get
                {
                    if (_changelogStyle == null)
                    {
                        _changelogStyle = new GUIStyle(EditorStyles.label)
                        {
                            wordWrap = true
                        };
                    }
                    return _changelogStyle;
                }
            }

            private static GUIStyle titleStyle
            {
                get
                {
                    if (_titleStyle == null)
                    {
                        _titleStyle = new GUIStyle(EditorStyles.boldLabel)
                        {
                            alignment = TextAnchor.MiddleCenter
                        };
                    }

                    return _titleStyle;
                }
            }

            public Item(XmlNode node)
            {
                version = node.SelectSingleNode("Version").InnerText;
                type = int.Parse(node.SelectSingleNode("Type").InnerText);
                changelog = Unescape(node.SelectSingleNode("ChangeLog").InnerText);
                download = node.SelectSingleNode("Download").InnerText;
                date = node.SelectSingleNode("Date").InnerText;

                string[] vars = version.Split('.');
                string[] vars2 = new string[4];
                vars2[0] = vars[0];
                vars2[1] = int.Parse(vars[1].Substring(0, 2)).ToString();
                vars2[2] = int.Parse(vars[1].Substring(2, 2)).ToString();
                vars2[3] = int.Parse(vars[1].Substring(4, 4)).ToString();
                version = string.Join(".", vars2);
            }

            public void Draw()
            {
                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.Label("Version: " + version + " (" + typeStr + "). " + date, titleStyle);

                GUILayout.Label(changelog, changelogStyle);

                if (GUILayout.Button("Download"))
                {
                    Application.OpenURL("https://infinity-code.com/products_update/download.php?k=" + download);
                }

                GUILayout.EndVertical();
            }

            public string typeStr
            {
                get { return Enum.GetName(typeof(Channel), type); }
            }


        }

        public enum Channel
        {
            stable = 10,
            stablePrevious = 15,
            releaseCandidate = 20,
            beta = 30,
            alpha = 40,
            working = 50,
        }
    }
}