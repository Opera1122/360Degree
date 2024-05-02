/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using UnityEngine;

namespace InfinityCode.uPano.Renderers.Base
{
    /// <summary>
    /// Base generic class of pano renderer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PanoRenderer<T> : PanoRenderer
        where T : PanoRenderer<T>
    {
        protected static T CreatePanoInstance()
        {
            GameObject go = new GameObject("Panorama");
            T renderer = go.AddComponent<T>();
            renderer._pano.UpdateRotation();
            return renderer;
        }
    }
}