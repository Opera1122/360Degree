/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using UnityEngine;
using UnityEngine.UI;

namespace InfinityCode.uPano.Transitions.UI
{
    /// <summary>
    /// Tilt transition
    /// </summary>
    [AddComponentMenu("uPano/Transitions/UI/Tint")]
    public class TintTransition : UITransition
    {
        /// <summary>
        /// Initial color
        /// </summary>
        public Color fromColor = new Color(0, 0, 0, 0);

        /// <summary>
        /// Target color
        /// </summary>
        public Color toColor = new Color(0, 0, 0, 1);

        private Image image;

        protected override string containerName
        {
            get { return "__uPano Tint__"; }
        }

        protected override void Dispose()
        {
            base.Dispose();

            image = null;
        }

        protected override void Finish()
        {
            started = false;
        }

        public override void Init()
        {
            base.Init();

            image = displayGameObject.AddComponent<Image>();
            FitToScreen(image);

            rootTransition.OnFinish += t => Dispose();
        }

        public override void Process()
        {
            base.Process();

            image.color = Color.Lerp(fromColor, toColor, curvedProgress);
        }
    }
}