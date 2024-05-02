/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.InteractiveElements;
using UnityEngine;

namespace InfinityCode.uPano.Actions
{
    /// <summary>
    /// Sets the mouse cursor to the given texture
    /// </summary>
    [AddComponentMenu("uPano/Actions/For Everything/Set Cursor")]
    public class SetCursor : InteractiveElementAction
    {
        /// <summary>
        /// The texture to use for the cursor or null to set the default cursor. Note that a texture needs to be imported with "Read/Write enabled" in the texture importer (or using the "Cursor" defaults), in order to be used as a cursor.
        /// </summary>
        public Texture2D texture;

        /// <summary>
        /// The offset from the top left of the texture to use as the target point (must be within the bounds of the cursor).
        /// </summary>
        public Vector2 offset;

        /// <summary>
        /// Allow this cursor to render as a hardware cursor on supported platforms, or force software cursor.
        /// </summary>
        public CursorMode cursorMode = CursorMode.Auto;

        public override void Invoke(InteractiveElement element)
        {
            Cursor.SetCursor(texture, offset, cursorMode);
        }
    }
}