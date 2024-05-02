/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using UnityEngine;

namespace InfinityCode.uPano.Actions
{
    /// <summary>
    /// Sets a new position for the Transform
    /// </summary>
    [AddComponentMenu("uPano/Actions/For Everything/Set Transform Position")]
    public class SetTransformPosition: AnimatedAction<SetTransformPosition, Vector3>
    {
        /// <summary>
        /// Transform for which you want to set the position
        /// </summary>
        public Transform target;

        /// <summary>
        /// From position is the current position
        /// </summary>
        public bool fromIsOriginal = true;

        /// <summary>
        /// From position
        /// </summary>
        public Vector3 fromPosition;

        /// <summary>
        /// To position is the delta to the original position
        /// </summary>
        public bool toIsDelta = false;

        /// <summary>
        /// To position
        /// </summary>
        public Vector3 toPosition;

        /// <summary>
        /// Position in local space?
        /// </summary>
        public bool useLocalSpace = true;

        protected override void SetAnimatedValue(float f)
        {
            if (target == null) return;

            Vector3 t = toPosition;
            if (toIsDelta) t += initialValue;

            Vector3 p = Vector3.Lerp(initialValue, t, f);
            if (useLocalSpace) target.localPosition = p;
            else target.position = p;
        }

        protected override void SetFixedValue()
        {
            Vector3 t = toPosition;
            if (toIsDelta) t += initialValue;

            if (useLocalSpace) target.localPosition = t;
            else target.position = t;
        }

        protected override void StoreInitialValue()
        {
            if (fromIsOriginal)
            {
                if (useLocalSpace) initialValue = target.localPosition;
                else initialValue = target.position;
            }
            else initialValue = fromPosition;
        }
    }
}