/*           INFINITY CODE           */
/*     https://infinity-code.com     */

namespace InfinityCode.uPano.Controls
{
    /// <summary>
    /// Base class for control on UI
    /// </summary>
    public abstract class UIControl : PanoControl
    {
        /// <summary>
        /// This element will be destroyed with the panorama?
        /// </summary>
        public bool destroyWithPanorama = false;

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Pano.OnPanoDestroy -= OnPanoDestroy;
        }

        protected virtual void OnPanoDestroy(Pano pano)
        {
            if (this.pano == pano)
            {
                if (destroyWithPanorama) Destroy(gameObject);
            }
        }

        protected override void Start()
        {
            base.Start();

            Pano.OnPanoDestroy += OnPanoDestroy;
        }
    }
}