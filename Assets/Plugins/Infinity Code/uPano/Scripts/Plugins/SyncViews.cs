/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using UnityEngine;

namespace InfinityCode.uPano.Plugins
{
    /// <summary>
    /// Component for synchronously changing pan, tilt and fov for multiple panoramas.
    /// </summary>
    [AddComponentMenu("uPano/Plugins/SyncViews")]
    public class SyncViews : MonoBehaviour
    {
        /// <summary>
        /// Panoramas to be synced.
        /// </summary>
        public Pano[] panoramas;

        private bool ignoreEvents;

        private void OnFovChanged(float fov)
        {
            if (ignoreEvents) return;

            ignoreEvents = true;
            foreach (Pano pano in panoramas) pano.fov = fov;
            ignoreEvents = false;
        }

        private void OnPanChanged(float pan)
        {
            if (ignoreEvents) return;

            ignoreEvents = true;
            foreach (Pano pano in panoramas) pano.pan = pan;
            ignoreEvents = false;
        }

        private void OnTiltChanged(float tilt)
        {
            if (ignoreEvents) return;

            ignoreEvents = true;
            foreach (Pano pano in panoramas) pano.tilt = tilt;
            ignoreEvents = false;
        }

        private void Start()
        {
            foreach (Pano pano in panoramas)
            {
                pano.OnPanChanged += OnPanChanged;
                pano.OnTiltChanged += OnTiltChanged;
                pano.OnFOVChanged += OnFovChanged;
            }
        }
    }
}