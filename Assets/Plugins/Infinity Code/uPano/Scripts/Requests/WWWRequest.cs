/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using System.Collections;
using InfinityCode.uPano.Net;
using UnityEngine;
using UnityEngine.Networking;

namespace InfinityCode.uPano.Requests
{
    /// <summary>
    /// Request wrapper of WWW, which works in edit and play mode
    /// </summary>
    public class WWWRequest : StatusRequest<WWWRequest>
    {
        /// <summary>
        /// Hook to intercept and modify the URL of any request
        /// </summary>
        public static Func<string, string> OnPrepareURL;

        /// <summary>
        /// URL of request
        /// </summary>
        public readonly string url;

        /// <summary>
        /// Request routine
        /// </summary>
        public IEnumerator routine;

        protected bool _isDone = false;
        protected Texture2D _texture;
        protected UnityWebRequest www;

        private WWWClient client;
        protected string _error;

        /// <summary>
        /// Returns the contents of the fetched web page as a byte array
        /// </summary>
        public byte[] bytes
        {
            get { return isDone ? www.downloadHandler.data : null; }
        }

        public override string error
        {
            get { return isDone ? _error : null; }
        }

        public override bool hasErrors
        {
            get
            {
                return isDone && !string.IsNullOrEmpty(_error);
            }
        }

        /// <summary>
        /// Gets whether the request is completed
        /// </summary>
        public bool isDone
        {
            get { return _isDone; }
        }

        /// <summary>
        /// Indicates if coroutine should be kept suspended
        /// </summary>
        public override bool keepWaiting
        {
            get { return !_isDone; }
        }

        /// <summary>
        /// Returns the contents of the fetched web page as a string
        /// </summary>
        public string text
        {
            get { return isDone ? www.downloadHandler.text : null; }
        }

        /// <summary>
        /// Returns a Texture2D generated from the downloaded data
        /// </summary>
        public virtual Texture2D texture
        {
            get
            {
                if (!isDone) return null;
                if (_texture == null)
                {
                    _texture = new Texture2D(1, 1);
                    _texture.LoadImage(bytes);
                }
                return _texture;
            }
        }

        /// <summary>
        /// Makes a new GET request by url
        /// </summary>
        /// <param name="url">URL</param>
        public WWWRequest(string url)
        {
            if (OnPrepareURL != null) url = OnPrepareURL(url);
            this.url = url;
            client = WWWClient.Add(this);
        }

        internal WWWRequest(WWWClient client, string url)
        {
            if (OnPrepareURL != null) url = OnPrepareURL(url);
            this.url = url;
            this.client = client;
        }

        internal virtual IEnumerator Download()
        {
            www = UnityWebRequest.Get(url);
            UnityWebRequestAsyncOperation asyncOperation = www.SendWebRequest();
            while (www != null && !asyncOperation.isDone)
            {
                yield return null;
            }

            _isDone = true;
            _error = www.error;

            BroadcastActions();

            if (!keepAlive) Dispose();
        }

        public override void Dispose()
        {
            if (client != null)
            {
                client.RemoveRequest(this);
                client = null;
            }

            if (www != null)
            {
                www.Dispose();
                www = null;
            }
        }
    }
}