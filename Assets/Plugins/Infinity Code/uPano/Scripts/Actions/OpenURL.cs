/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.InteractiveElements;
using UnityEngine;

namespace InfinityCode.uPano.Actions
{
    /// <summary>
    /// Opens URL using the default browser
    /// </summary>
    [AddComponentMenu("uPano/Actions/For Everything/Open URL")]
    public class OpenURL : InteractiveElementAction
    {
        /// <summary>
        /// URL
        /// </summary>
        public string url;

        public override void Invoke(InteractiveElement element)
        {
            Application.OpenURL(url);
        }
    }
}