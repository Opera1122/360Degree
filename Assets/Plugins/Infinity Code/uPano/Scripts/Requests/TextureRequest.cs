/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System.Collections;
using InfinityCode.uPano.Net;
using UnityEngine;
using UnityEngine.Networking;

namespace InfinityCode.uPano.Requests
{
    /// <summary>
    /// Texture request wrapper of WWW, which works in edit and play mode
    /// </summary>
    public class TextureRequest : WWWRequest
    {
        public override Texture2D texture
        {
            get
            {
                if (!isDone) return null;
                if (_texture == null)
                {
                    _texture = DownloadHandlerTexture.GetContent(www);
                }
                return _texture;
            }
        }

        public TextureRequest(string url) : base(url)
        {
        }

        internal TextureRequest(WWWClient client, string url) : base(client, url)
        {
        }

        internal override IEnumerator Download()
        {
            www = UnityWebRequestTexture.GetTexture(url);
            UnityWebRequestAsyncOperation asyncOperation = www.SendWebRequest();
            while (www != null && !asyncOperation.isDone)
            {
                yield return null;
            }

            _isDone = true;
            _error = www.error;

            BroadcastActions();

            Dispose();
        }
    }
}