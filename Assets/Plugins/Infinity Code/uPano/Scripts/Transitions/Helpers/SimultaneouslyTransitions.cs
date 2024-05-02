/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System.Linq;
using UnityEngine;

namespace InfinityCode.uPano.Transitions.Helpers
{
    /// <summary>
    /// A compound transition, all elements of which begin simultaneously. The transition finish at the finish of the longest element.
    /// </summary>
    [AddComponentMenu("uPano/Transitions/Helpers/Simultaneously Transitions")]
    public class SimultaneouslyTransitions : CombinedTransition
    {
        public override void Init()
        {
            base.Init();

            foreach (Transition t in transitions)
            {
                if (t != null)
                {
                    t.element = element;
                    t.Init();
                }
            }
        }

        protected override void OnProcess()
        {
            finished = transitions.All(t => t == null || t.finished);
        }

        public override void StartTransition()
        {
            started = true;

            foreach (Transition t in transitions)
            {
                if (t != null)
                {
                    t.StartTransition();
                    t.Process();
                }
            }
        }
    }
}