/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.Renderers;
using InfinityCode.uPano.Renderers.Base;
using UnityEngine;

namespace InfinityCode.uPano.Plugins
{
    [Serializable]
    [AddComponentMenu("uPano/Plugins/Downloader")]
    public class Downloader : Plugin
    {
        public bool useLowRes = true;
        public string url;
        public string lowresUrl;

        public string[] sidesURLs = new string[6];
        public string[] lowresSidesURLs = new string[6];

        protected override void Start()
        {
            base.Start();
            StartDownload();
        }

        public void StartDownload()
        {
            if (panoRenderer == null) return;

            if (panoRenderer is ISingleTexturePanoRenderer)
            {
                if (!string.IsNullOrEmpty(url))
                {
                    ISingleTexturePanoRenderer r = panoRenderer as ISingleTexturePanoRenderer;
                    if (useLowRes && !string.IsNullOrEmpty(lowresUrl)) r.Download(lowresUrl, url);
                    else r.Download(url);
                }
            }
            else if (panoRenderer is CubeFacesPanoRenderer)
            {
                if (sidesURLs == null || sidesURLs.Length != 6) return;
                CubeFacesPanoRenderer r = panoRenderer as CubeFacesPanoRenderer;
                if (useLowRes) r.Download(lowresSidesURLs, sidesURLs);
                else r.Download(sidesURLs);
            }
        }
    }
}