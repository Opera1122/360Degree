/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using UnityEngine;

namespace InfinityCode.uPano.Transitions.Helpers
{
    /// <summary>
    /// A composite transition, each next element of which starts after the end of the previous one. The transition finish when the last element is finished.
    /// </summary>
    [AddComponentMenu("uPano/Transitions/Helpers/Consecutive Transitions")]
    public class ConsecutiveTransitions : CombinedTransition
    {
        private int index = -1;
        private Transition active;

        private Transition GetNextTransition()
        {
            while (index < transitions.Length - 1)
            {
                index++;
                if (transitions[index] != null) return transitions[index];
            }

            return null;
        }

        protected override void OnProcess()
        {
            if (active != null && !active.finished) return;

            active = GetNextTransition();
            if (active == null)
            {
                finished = true;
                return;
            }

            active.element = element;
            active.Init();
            active.StartTransition();
        }
    }
}