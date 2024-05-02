/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.Renderers.Base;
using UnityEngine;

namespace InfinityCode.uPano.Plugins
{
    /// <summary>
    /// Base class for uPano plugins
    /// </summary>
    public abstract class Plugin : MonoBehaviour
    {
        public static Action<Plugin> OnPluginStarted;

        protected Pano _pano;
        protected PanoRenderer _panoRenderer;

        /// <summary>
        /// Reference to the panorama
        /// </summary>
        public Pano pano
        {
            get { return _pano; }
        }

        /// <summary>
        /// Reference to the pano renderer
        /// </summary>
        public PanoRenderer panoRenderer
        {
            get { return _panoRenderer; }
        }

        protected virtual void OnDisable()
        {

        }

        protected virtual void OnDestroy()
        {

        }

        private void OnEnable()
        {
            GetPanoInstance();

            if (_pano == null) _pano = FindObjectOfType<Pano>();
            if (_pano != null) _panoRenderer = _pano.GetComponent<PanoRenderer>();

            OnEnableLate();
        }

        protected virtual void GetPanoInstance()
        {
            _pano = GetComponent<Pano>();
        }

        protected virtual void OnEnableLate()
        {

        }

        protected virtual void Start()
        {
            if (OnPluginStarted != null) OnPluginStarted(this);
        }
    }
}