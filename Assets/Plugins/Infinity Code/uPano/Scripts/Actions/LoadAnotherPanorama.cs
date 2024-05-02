/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.InteractiveElements;
using InfinityCode.uPano.Transitions;
using UnityEngine;

namespace InfinityCode.uPano.Actions
{
    /// <summary>
    /// Destroys the current one and loads an another panorama
    /// </summary>
    [AddComponentMenu("uPano/Actions/For Everything/Load Another Panorama")]
    public class LoadAnotherPanorama : TransitionAction
    {
        /// <summary>
        /// Prefab of the panorama which must be instantiated
        /// </summary>
        public GameObject prefab;

        /// <summary>
        /// Keep the orientation of the current panorama
        /// </summary>
        public bool keepOrientation = true;

        private float pan;
        private float tilt;
        private float fov;

        public override void Invoke(InteractiveElement element)
        {
            if (prefab == null) throw new Exception("Prefab can not be null");

            SaveOrientation(element);
            base.Invoke(element);
        }

        protected override void InvokeAction(InteractiveElement element)
        {
            LoadPano(element);
        }

        private void LoadPano(InteractiveElement element)
        {
            Pano pano = element.pano;

            DestroyImmediate(pano.gameObject);
            GameObject go = Instantiate(prefab);
            pano = go.GetComponent<Pano>();
            if (pano != null && keepOrientation)
            {
                pano.pan = pan;
                pano.tilt = tilt;
                pano.fov = fov;
            }
        }

        private void SaveOrientation(InteractiveElement element)
        {
            Pano pano = element.pano;
            pan = pano.pan;
            tilt = pano.tilt;
            fov = pano.fov;
        }
    }
}