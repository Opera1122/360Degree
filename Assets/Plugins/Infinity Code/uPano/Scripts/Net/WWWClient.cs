/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System.Collections.Generic;
using InfinityCode.uPano.Requests;
using UnityEngine;

namespace InfinityCode.uPano.Net
{
    /// <summary>
    /// WWW client that works in edit and play modes
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("")]
    public class WWWClient: MonoBehaviour
    {
        private static WWWClient instance;
        private static List<WWWRequest> requests;

        private WWWRequest AddRequest(WWWRequest request)
        {
            if (requests == null) requests = new List<WWWRequest>();
            requests.Add(request);
            request.routine = request.Download();

            StartCoroutine(request.routine);

            return request;
        }

        internal static WWWClient Add(WWWRequest request)
        {
            Init();
            instance.AddRequest(request);
            return instance;
        }

        /// <summary>
        /// Makes Get request by URL
        /// </summary>
        /// <param name="url">URL</param>
        /// <returns>Instance of the request</returns>
        public static WWWRequest Get(string url)
        {
            Init();
            return instance.GetInternal(url);
        }

        private static void Init()
        {
            if (instance != null) return;

            GameObject go = new GameObject("__Pano WWWClient__");
            go.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSave;
            instance = go.AddComponent<WWWClient>();
        }

        private WWWRequest GetInternal(string url)
        {
            WWWRequest request = new WWWRequest(this, url);
            return AddRequest(request);
        }

        internal static void Remove(WWWRequest request)
        {
            if (instance != null) instance.RemoveRequest(request);
        }

        internal void RemoveRequest(WWWRequest request)
        {
            if (requests == null)
            {
                DestroyImmediate(gameObject);
                return;
            }

            if (request != null)
            {
                request.OnComplete = null;
                request.OnError = null;
                request.OnSuccess = null;

                if (request.routine != null)
                {
                    StopCoroutine(request.routine);
                    request.routine = null;
                }
            }

            requests.Remove(request);

            if (requests.Count == 0)
            {
                instance = null;
                requests = null;
                DestroyImmediate(gameObject);
            }
        }
    }
}
