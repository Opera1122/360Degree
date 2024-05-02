/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using UnityEngine;

#if FINGERS_TG
using DigitalRubyShared;
#endif

namespace InfinityCode.uPano.Plugins
{
#if FINGERS_TG
    [RequireComponent(typeof(FingersScript))]
#endif
    /// <summary>
    /// Component for integration with Fingers - Touch Gestures asset.
    /// </summary>
    [AddComponentMenu("uPano/Plugins/FingersTouchGesturesConnector")]
    public class FingersTouchGesturesConnector : Plugin
    {
#if FINGERS_TG
        /// <summary>
        /// Scale multiplier
        /// </summary>
        public float scaleSpeed = 1;

        private ScaleGestureRecognizer scaleGesture;

        protected override void Start()
        {
            base.Start();

            scaleGesture = new ScaleGestureRecognizer();
            scaleGesture.StateUpdated += ScaleGestureCallback;
            FingersScript.Instance.AddGesture(scaleGesture);
        }

        private void ScaleGestureCallback(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Executing)
            {
                pano.fov *= (scaleGesture.ScaleMultiplier - 1) * scaleSpeed + 1;
            }
        }
#endif
    }
}