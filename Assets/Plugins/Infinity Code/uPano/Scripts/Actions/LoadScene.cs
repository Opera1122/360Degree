/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.InteractiveElements;
using InfinityCode.uPano.Transitions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InfinityCode.uPano.Actions
{
    /// <summary>
    /// Loads the Scene by its name or index in Build Settings
    /// </summary>
    [AddComponentMenu("uPano/Actions/For Everything/Load Scene")]
    public class LoadScene : TransitionAction
    {
        /// <summary>
        /// Enum of types of scene loading
        /// </summary>
        public enum LoadType
        {
            /// <summary>
            /// Load by name
            /// </summary>
            name,

            /// <summary>
            /// Load by index in Build Settings
            /// </summary>
            index
        }

        /// <summary>
        /// Type of scene loading
        /// </summary>
        public LoadType loadType = LoadType.index;

        /// <summary>
        /// Index of scene in Build Settings
        /// </summary>
        public int sceneIndex;

        /// <summary>
        /// Name of scene
        /// </summary>
        public string sceneName;

        protected override void InvokeAction(InteractiveElement element)
        {
            TryLoadScene();
        }

        private void TryLoadScene()
        {
            if (loadType == LoadType.index) SceneManager.LoadScene(sceneIndex);
            else SceneManager.LoadScene(sceneName);
        }
    }
}