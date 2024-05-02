/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.InteractiveElements;
using UnityEngine;

namespace InfinityCode.uPano.Actions
{
    /// <summary>
    /// Base class for animated Interactive Element actions
    /// </summary>
    /// <typeparam name="T">Type of AnimatedAction</typeparam>
    /// <typeparam name="U">Type of animated value</typeparam>
    public abstract class AnimatedAction<T, U> : InteractiveElementAction
        where T: AnimatedAction<T, U>
    {
        /// <summary>
        /// The action must be animated?
        /// </summary>
        public bool animated = true;

        /// <summary>
        /// Animation curve
        /// </summary>
        public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);

        /// <summary>
        /// Delay of the animation
        /// </summary>
        public float delay = 0;

        /// <summary>
        /// Duration of the animation
        /// </summary>
        public float duration = 0.2f;

        protected static AnimatedAction<T, U> activeAnimation;

        protected bool started;
        protected float time;
        protected InteractiveElement element;

        protected U initialValue;

        public float totalDuration
        {
            get { return delay + duration; }
        }

        public override void Invoke(InteractiveElement element)
        {
            StopPreviousAnimation();

            this.element = element;
            if (!animated || totalDuration <= 0) SetFixedValue();
            else
            {
                activeAnimation = this;
                time = 0;
                StoreInitialValue();
                started = true;
            }
        }

        protected abstract void SetAnimatedValue(float f);
        protected abstract void SetFixedValue();

        protected virtual void StopPreviousAnimation()
        {
            if (activeAnimation == null) return;

            activeAnimation.SetFixedValue();
            activeAnimation.started = false;
            activeAnimation = null;
        }

        protected abstract void StoreInitialValue();

        protected virtual void Update()
        {
            if (!started) return;

            time += Time.deltaTime;
            if (time >= totalDuration)
            {
                time = totalDuration;
                started = false;
                activeAnimation = null;
            }

            float f;
            if (duration > 0) f = curve.Evaluate((time - delay) / duration);
            else f = time < delay? 0: 1;

            if (f < 0) f = 0;
            SetAnimatedValue(f);
        }
    }
}