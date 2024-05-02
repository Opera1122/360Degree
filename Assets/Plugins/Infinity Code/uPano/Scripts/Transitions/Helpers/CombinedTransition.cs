/*           INFINITY CODE           */
/*     https://infinity-code.com     */

namespace InfinityCode.uPano.Transitions.Helpers
{
    /// <summary>
    /// A base class for a transition that combines other transitions
    /// </summary>
    public abstract class CombinedTransition : Transition
    {
        /// <summary>
        /// Array of transitions
        /// </summary>
        public Transition[] transitions;

        public override void Init()
        {
            base.Init();

            foreach (Transition t in transitions)
            {
                if (t != null) t.parent = this;
            }
        }
    }
}