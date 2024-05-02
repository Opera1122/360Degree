/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using UnityEngine;

namespace InfinityCode.uPano.Transitions
{
    /// <summary>
    /// Base class for time-based transitions
    /// </summary>
    public abstract class TimeBasedTransition : Transition
    {
        [SerializeField]
        private float _duration = 1;

        [SerializeField]
        private AnimationCurve _curve = AnimationCurve.Linear(0, 0, 1, 1);

        /// <summary>
        /// Transition progress (0-1)
        /// </summary>
        protected float progress;

        /// <summary>
        /// Total duration of the transition
        /// </summary>
        public float duration
        {
            get { return _duration; }
        }

        /// <summary>
        /// Animation curve
        /// </summary>
        public AnimationCurve curve
        {
            get { return _curve; }
        }

        /// <summary>
        /// Progress after applying the curve.Evaluate
        /// </summary>
        public float curvedProgress
        {
            get { return curve.Evaluate(progress); }
        }

        public override void Init()
        {
            progress = 0;

            base.Init();
        }

        protected override void OnProcess()
        {
            progress += Time.deltaTime / duration;
            finished = progress >= 1;
            if (finished) progress = 1;

            Process();
        }
    }
}