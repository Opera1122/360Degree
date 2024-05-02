/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.InteractiveElements;
using UnityEngine;

namespace InfinityCode.uPano.Transitions
{
    /// <summary>
    /// Base class for transition from and to panorama
    /// </summary>
    public abstract class Transition: MonoBehaviour
    {
        /// <summary>
        /// Action that occurs when transition was finished
        /// </summary>
        public Action<Transition> OnFinish;

        protected bool started;

        /// <summary>
        /// Reference to the interactive element that caused the transition
        /// </summary>
        [HideInInspector, NonSerialized]
        public InteractiveElement element;

        /// <summary>
        /// Transition is finished?
        /// </summary>
        [HideInInspector, NonSerialized]
        public bool finished;

        /// <summary>
        /// Parent transition
        /// </summary>
        [HideInInspector, NonSerialized]
        public Transition parent;

        /// <summary>
        /// Root transition
        /// </summary>
        public Transition rootTransition
        {
            get
            {
                Transition t = this;
                while (t.parent != null)
                {
                    t = t.parent;
                }

                return t;
            }
        }

        protected virtual void Dispose()
        {
            parent = null;
            element = null;
        }

        /// <summary>
        /// Execute transition
        /// </summary>
        /// <param name="element">Interactive element that caused the transition or null</param>
        public void Execute(InteractiveElement element)
        {
            this.element = element;

            Init();
            StartTransition();
            Process();
        }

        /// <summary>
        /// Finish the transition
        /// </summary>
        protected virtual void Finish()
        {
            Dispose();
            started = false;
        }

        protected virtual void FinishBefore()
        {
            
        }

        /// <summary>
        /// Init the transition
        /// </summary>
        public virtual void Init()
        {
            
        }

        /// <summary>
        /// Calculates progress, checks the finish, and causes process the transition
        /// </summary>
        protected virtual void OnProcess()
        {
            Process();
        }

        /// <summary>
        /// Process the transition
        /// </summary>
        public virtual void Process()
        {
            
        }

        /// <summary>
        /// Starts the transition
        /// </summary>
        public virtual void StartTransition()
        {
            started = true;
        }

        private void Update()
        {
            if (!started) return;

            OnProcess();

            if (finished)
            {
                FinishBefore();
                if (OnFinish != null) OnFinish(this);
                Finish();
            }
        }
    }
}
