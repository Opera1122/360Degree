/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using UnityEngine;

namespace InfinityCode.uPano.Actions
{
    /// <summary>
    /// Sets a new position for the Transform
    /// </summary>
    [AddComponentMenu("uPano/Actions/For Everything/Set Transform Rotation")]
    public class SetTransformRotation : AnimatedAction<SetTransformPosition, Vector3>
    {
        /// <summary>
        /// Transform for which you want to set the rotation
        /// </summary>
        public Transform target;

        /// <summary>
        /// From rotation is the current rotation
        /// </summary>
        public bool fromIsOriginal = true;

        /// <summary>
        /// From rotation
        /// </summary>
        public Vector3 fromRotation;

        /// <summary>
        /// To rotation is the delta to the original rotation
        /// </summary>
        public bool toIsDelta = false;

        /// <summary>
        /// To rotation
        /// </summary>
        public Vector3 toRotation;

        /// <summary>
        /// Rotation in local space?
        /// </summary>
        public bool useLocalSpace = true;

        protected override void SetAnimatedValue(float f)
        {
            if (target == null) return;

            Vector3 t = toRotation;
            if (toIsDelta) t += initialValue;

            Quaternion r = Quaternion.Lerp(Quaternion.Euler(initialValue), Quaternion.Euler(t), f);
            if (useLocalSpace) target.localRotation = r;
            else target.rotation = r;
        }

        protected override void SetFixedValue()
        {
            Vector3 t = toRotation;
            if (toIsDelta) t += initialValue;

            Quaternion r = Quaternion.Euler(t);
            if (useLocalSpace) target.localRotation = r;
            else target.rotation = r;
        }

        protected override void StoreInitialValue()
        {
            if (fromIsOriginal)
            {
                if (useLocalSpace) initialValue = target.localRotation.eulerAngles;
                else initialValue = target.rotation.eulerAngles;
            }
            else initialValue = fromRotation;
        }
    }
}