/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.InteractiveElements;
using UnityEngine;

namespace InfinityCode.uPano.Actions
{
    /// <summary>
    /// Sets the scale of the current IScalableElement
    /// </summary>
    [AddComponentMenu("uPano/Actions/For Everything/Set Scale")]
    public class SetScale : AnimatedAction<SetScale, Vector3>
    {
        /// <summary>
        /// The scale that must be set
        /// </summary>
        public Vector3 scale = Vector3.one;

        private IScalableElement scalableElement;

        public override void Invoke(InteractiveElement element)
        {
            scalableElement = element as IScalableElement;
            base.Invoke(element);
        }

        protected override void SetFixedValue()
        {
            if (scalableElement != null) scalableElement.scale = scale;
        }

        protected override void SetAnimatedValue(float f)
        {
            if (scalableElement != null) scalableElement.scale = Vector3.Lerp(initialValue, scale, f);
        }

        protected override void StoreInitialValue()
        {
            if (scalableElement != null) initialValue = scalableElement.scale;
        }
    }
}