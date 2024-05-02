/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;

namespace InfinityCode.uPano.Requests
{
    /// <summary>
    /// Base class of a request that has the result status
    /// </summary>
    /// <typeparam name="T">Type of request</typeparam>
    public abstract class StatusRequest<T>: Request<T>
        where T: StatusRequest<T>
    {
        /// <summary>
        /// Action that occurs when the request is successful
        /// </summary>
        public Action<T> OnSuccess;

        /// <summary>
        /// Action that occurs when the request is failed
        /// </summary>
        public Action<T> OnError;

        /// <summary>
        /// Gets whether the request contains an error
        /// </summary>
        public abstract bool hasErrors { get; }

        /// <summary>
        /// Gets an error of the request
        /// </summary>
        public abstract string error { get; }

        protected override void BroadcastActions()
        {
            base.BroadcastActions();

            BroadcastAction(!hasErrors ? OnSuccess : OnError);
        }
    }
}