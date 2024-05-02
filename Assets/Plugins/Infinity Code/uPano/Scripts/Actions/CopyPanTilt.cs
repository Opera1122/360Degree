/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.InteractiveElements;

namespace InfinityCode.uPano.Actions
{
    /// <summary>
    /// Copy pan and tilt from source to target panorama
    /// </summary>
    public class CopyPanTilt: TransitionAction
    {
        /// <summary>
        /// Source panorama
        /// </summary>
        public Pano source;

        /// <summary>
        /// Target panorama
        /// </summary>
        public Pano target;

        protected override void InvokeAction(InteractiveElement element)
        {
            if (source == null || target == null) return;

            target.pan = source.pan;
            target.tilt = source.tilt;
        }
    }
}