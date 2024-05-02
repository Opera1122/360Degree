/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.Plugins;

namespace InfinityCode.uPano.Controls
{
    /// <summary>
    /// The base class for the components of moving a panorama
    /// </summary>
    public abstract class PanoControl : Plugin
    {
        /// <summary>
        /// Action is triggered by the start of each PanoControl
        /// </summary>
        public static Action<PanoControl> OnControlStarted;

        /// <summary>
        /// Action triggered when moving a panorama
        /// </summary>
        public Action<PanoControl> OnInput;

        protected static PanoControl exclusiveControl;

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Pano.OnPanoEnabled -= OnPanoEnabled;
        }

        protected virtual void OnPanoEnabled(Pano pano)
        {
            _pano = pano;
        }

        protected override void Start()
        {
            base.Start();

            Pano.OnPanoEnabled += OnPanoEnabled;

            if (OnControlStarted != null) OnControlStarted(this);
        }
    }
}