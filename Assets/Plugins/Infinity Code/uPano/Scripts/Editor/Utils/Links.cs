/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using InfinityCode.uPano.Editors.Utils;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors
{
    public static class Links
    {
        private const string aid = "?aid=1100liByC&pubref=up_asset";
        private const string api = "https://infinity-code.com/docs/api/upano";
        private const string assetStore = "https://assetstore.unity.com/packages/tools/integration/upano-126396";
        private const string changelog = "https://infinity-code.com/products_update/get-changelog.php?asset=uPano&from=1.0";
        private const string documentation = "https://infinity-code.com/documentation/upano.html";
        private const string forum = "https://forum.infinity-code.com";
        private const string homepage = "https://infinity-code.com/assets/upano";
        private const string reviews = assetStore + "/reviews";
        private const string support = "mailto:support@infinity-code.com?subject=uPano";
        private const string youtube = "https://www.youtube.com/channel/UCxCID3jp7RXKGqiCGpjPuOg";

        public static void Open(string url)
        {
            Application.OpenURL(url);
        }

        public static void OpenAPIReference()
        {
            Open(api);
        }

        public static void OpenAssetStore()
        {
            Open(assetStore + aid);
        }

        public static void OpenChangelog()
        {
            Open(changelog);
        }

        [MenuItem(EditorUtils.MENU_PATH + "Documentation", false, 120)]
        public static void OpenDocumentation()
        {
            OpenDocumentation(null);
        }

        public static void OpenDocumentation(string anchor)
        {
            string url = documentation;
            if (!string.IsNullOrEmpty(anchor)) url += "#" + anchor;
            Open(url);
        }

        public static void OpenForum()
        {
            Open(forum);
        }

        public static void OpenHomepage()
        {
            Open(homepage);
        }

        public static void OpenReviews()
        {
            Open(reviews + aid);
        }

        public static void OpenSupport()
        {
            Open(support);
        }

        public static void OpenYouTube()
        {
            Open(youtube);
        }
    }
}