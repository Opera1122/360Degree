/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace InfinityCode.uPano.Requests
{
    /// <summary>
    /// Base class of a request
    /// </summary>
    /// <typeparam name="T">Type of a request</typeparam>
    public abstract class Request<T>: CustomYieldInstruction, IDisposable
        where T: Request<T>
    {
        /// <summary>
        /// Action that occurs when the request is completed
        /// </summary>
        public Action<T> OnComplete;

        /// <summary>
        /// Do not destroy the request after completion. If true, you must call Dispose yourself when you no longer need it to avoid memory leaks.
        /// </summary>
        public bool keepAlive = false;

        private Dictionary<string, object> _customFields;

        /// <summary>
        /// Gets and sets the user value to be stored in a request
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Value</returns>
        public object this[string key]
        {
            get
            {
                object val;
                return customFields.TryGetValue(key, out val) ? val : null;
            }
            set { customFields[key] = value; }
        }

        /// <summary>
        /// Gets a dictionary of custom fields
        /// </summary>
        public Dictionary<string, object> customFields
        {
            get
            {
                if (_customFields == null) _customFields = new Dictionary<string, object>();
                return _customFields;
            }
        }

        protected void BroadcastAction(Action<T> action)
        {
            try
            {
                if (action != null) action((T) this);
            }
            catch
            {
            }
        }

        protected virtual void BroadcastActions()
        {
            BroadcastAction(OnComplete);
        }

        /// <summary>
        /// Disposes a request
        /// </summary>
        public virtual void Dispose()
        {
            _customFields = null;
        }
    }
}
