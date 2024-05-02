/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.InteractiveElements;
using UnityEngine;
using UnityEngine.UI;

namespace InfinityCode.uPano.Actions
{
    /// <summary>
    /// Sets the value for UI.Text.
    /// </summary>
    [AddComponentMenu("uPano/Actions/For Everything/Set Text")]
    public class SetText : InteractiveElementAction
    {
        /// <summary>
        /// UI.Text
        /// </summary>
        public Text textfield;

        /// <summary>
        /// New value
        /// </summary>
        public string value;

        public override void Invoke(InteractiveElement element)
        {
            if (textfield != null) textfield.text = value;
        }
    }
}