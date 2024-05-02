/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Directions;
using InfinityCode.uPano.HotSpots;
using InfinityCode.uPano.InteractiveElements;
using InfinityCode.uPano.Plugins;
using InfinityCode.uPano.Transitions;
using UnityEngine;

namespace InfinityCode.uPano
{
    /// <summary>
    /// Global settings for all panoramas
    /// </summary>
    public class GlobalSettings : Plugin
    {
        private static GlobalSettings _instance;

        /// <summary>
        /// Prefab by default, which will be used if the Hot Spot prefab is not specified
        /// </summary>
        public GameObject defaultHotSpotPrefab;

        /// <summary>
        /// Prefab by default, which will be used if the Direction prefab is not specified
        /// </summary>
        public GameObject defaultDirectionPrefab;

        /// <summary>
        /// Prefab which contains the transition that is played before the panorama is closed
        /// </summary>
        public GameObject beforeTransitionPrefab;

        /// <summary>
        /// Prefab which contains the transition that is played after the panorama is closed
        /// </summary>
        public GameObject afterTransitionPrefab;

        /// <summary>
        /// Reference to instance
        /// </summary>
        public static GlobalSettings instance
        {
            get { return _instance; }
        }

        protected override void OnEnableLate()
        {
            base.OnEnableLate();

            _instance = this;
        }

        protected override void OnDisable()
        {
            _instance = null;
        }

        public static Transition GetAfterTransition()
        {
            if (_instance == null || _instance.afterTransitionPrefab == null) return null;

            GameObject go = Instantiate(_instance.afterTransitionPrefab);
            return go.GetComponent<Transition>();
        }

        public static Transition GetBeforeTransition()
        {
            if (_instance == null || _instance.beforeTransitionPrefab == null) return null;

            GameObject go = Instantiate(_instance.beforeTransitionPrefab);
            return go.GetComponent<Transition>();
        }

        public static GameObject GetDefaultPrefab<T>() where T : InteractiveElement
        {
            if (_instance == null) return null;
            if (typeof(T) == typeof(HotSpot)) return _instance.defaultHotSpotPrefab;
            if (typeof(T) == typeof(Direction)) return _instance.defaultDirectionPrefab;
            return null;
        }
    }
}