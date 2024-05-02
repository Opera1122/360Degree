/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Renderers;
using UnityEngine;

namespace InfinityCode.uPano.Examples
{
    /// <summary>
    /// Animates the transparency of the panorama at startup
    /// </summary>
    [AddComponentMenu("uPano/Examples/AnimatedFadeExample")]
    public class AnimatedFadeExample : MonoBehaviour
    {
        /// <summary>
        /// Reference to PanoRenderer
        /// </summary>
        public SphericalPanoRenderer panoRenderer;

        /// <summary>
        /// Animation curve
        /// </summary>
        public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);

        /// <summary>
        /// Duration of the animation (sec)
        /// </summary>
        public float duration = 1;

        /// <summary>
        /// Current animation progress (0-1)
        /// </summary>
        private float progress;

        /// <summary>
        /// This function is called every fixed framerate frame
        /// </summary>
        private void FixedUpdate()
        {
            // Updating progress
            progress += Time.fixedDeltaTime / duration;

            bool completed = false;
            
            // If the progress >= 1 completes the animation
            if (progress >= 1)
            {
                completed = true;
                progress = 1;
            }

            // Update the transparency of the panorama
            panoRenderer.material.color = new Color(1, 1, 1, progress);

            // If completed, destroy this MonoBehaviour
            if (completed) Destroy(this);
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled
        /// </summary>
        private void Start()
        {
            // Set the initial transparency
            panoRenderer.material.color = new Color(1, 1, 1, 0);
        }
    }
}