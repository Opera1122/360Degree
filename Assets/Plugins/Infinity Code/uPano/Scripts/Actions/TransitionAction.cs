/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.InteractiveElements;
using InfinityCode.uPano.Transitions;
using UnityEngine;

namespace InfinityCode.uPano.Actions
{
    /// <summary>
    /// Base class for actions that support transitions
    /// </summary>
    public abstract class TransitionAction : InteractiveElementAction
    {
        /// <summary>
        /// Use transitions for this action.
        /// </summary>
        public bool useTransition = false;

        /// <summary>
        /// Prefab which contains the transition that is played before the panorama is closed
        /// </summary>
        public GameObject beforeTransitionPrefab;

        /// <summary>
        /// Prefab which contains the transition that is played after the panorama is closed
        /// </summary>
        public GameObject afterTransitionPrefab;

        public override void Invoke(InteractiveElement element)
        {
            if (!useTransition)
            {
                InvokeAction(element);
                return;
            }

            Transition beforeTransition = GetTransition(beforeTransitionPrefab);
            if (beforeTransition == null) beforeTransition = GlobalSettings.GetBeforeTransition();

            if (beforeTransition != null)
            {
                element.pano.locked = true;
                beforeTransition.OnFinish += t =>
                {
                    element.pano.locked = false;
                    InvokeAction(element);
                    Destroy(beforeTransition.gameObject);
                    StartAfterTransition();
                };
                beforeTransition.Execute(element);
            }
            else
            {
                InvokeAction(element);
                StartAfterTransition();
            }
        }

        private void StartAfterTransition()
        {
            Transition afterTransition = GetTransition(afterTransitionPrefab);
            if (afterTransition == null) afterTransition = GlobalSettings.GetAfterTransition();

            if (afterTransition != null)
            {
                afterTransition.OnFinish += t => { Destroy(afterTransition.gameObject); };
                afterTransition.Execute(null);
            }
        }

        protected Transition GetTransition(GameObject prefab)
        {
            if (prefab != null)
            {
                GameObject go = Instantiate(prefab);
                return go.GetComponent<Transition>();
            }

            return null;
        }

        protected abstract void InvokeAction(InteractiveElement element);
    }
}