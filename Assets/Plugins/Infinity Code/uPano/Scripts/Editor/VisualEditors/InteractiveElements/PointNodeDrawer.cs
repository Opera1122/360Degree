/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Directions;
using InfinityCode.uPano.Editors.Utils;
using InfinityCode.uPano.HotSpots;
using InfinityCode.uPano.InteractiveElements;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors.VisualEditors.InteractiveElements
{
    public static class PointNodeDrawer
    {
        public static void Draw(VisualInteractiveElementEditor editor, InteractiveElement element)
        {
            Rect rect = GetRect(editor, element);

            GUIStyle titleStyle = editor.normalPrefabStyle;
            HotSpot hotSpot = element as HotSpot;
            if (hotSpot != null && hotSpot.prefab == null) titleStyle = editor.missedPrefabStyle;

            GUIContent content = new GUIContent(element.title);
            Vector2 size = titleStyle.CalcSize(content);

            Rect labelRect = new Rect(rect.center.x - size.x / 2, rect.y - size.y - 5, size.x, size.y);

            titleStyle.Draw(labelRect, content, false, false, false, false);
            GUI.DrawTexture(rect, VisualInteractiveElementEditor.activeElement == element ? VisualInteractiveElementEditor.hotSpotSelectedIcon : VisualInteractiveElementEditor.hotSpotIcon);
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
        }

        public static Rect GetRect(VisualInteractiveElementEditor editor, InteractiveElement element)
        {
            float pan, tilt;
            element.GetPanTilt(out pan, out tilt);
            Vector2 uv = editor.PanTiltToPoint(pan, tilt);
            return new Rect(uv.x - 16, uv.y - 16, 32, 32);
        }

        public static bool HitTest(VisualInteractiveElementEditor editor, InteractiveElement element, Vector2 mousePosition)
        {
            return GetRect(editor, element).Contains(mousePosition);
        }

        public static void ProcessEvents(VisualInteractiveElementEditor editor, InteractiveElement element)
        {
            Event e = Event.current;

            if (e.type == EventType.MouseDown)
            {
                if (HitTest(editor, element, e.mousePosition) && !HotAreaNodeDrawer.IsInsertPointMode())
                {
                    if (e.button == 0)
                    {
                        editor.SelectElement(element);
                        editor.dragElement = element;
                        e.Use();
                    }
                    else if (e.button == 1)
                    {
                        editor.ProcessElementContextMenu(element);
                        e.Use();
                    }
                }
            }
            else if (e.type == EventType.MouseDrag)
            {
                if (editor.dragElement == element)
                {
                    Vector2 uv = editor.GetUV();
                    float pan, tilt;
                    if (editor.singleTexturePanoRenderer.GetPanTiltByUV(uv, out pan, out tilt))
                    {
                        Undo.SetCurrentGroupName("Set Pan and Tilt");
                        int group = Undo.GetCurrentGroup();

                        if (VisualInteractiveElementEditor.directionManager != null) Undo.RecordObject(VisualInteractiveElementEditor.directionManager, "Set Pan and Tilt");
                        if (VisualInteractiveElementEditor.hotSpotManager != null) Undo.RecordObject(VisualInteractiveElementEditor.hotSpotManager, "Set Pan and Tilt");

                        Undo.CollapseUndoOperations(group);

                        element.SetPanTilt(pan, tilt);
                        InteractiveElementSettings.Redraw();
                        if (element is Direction) EditorUtils.SetDirty(VisualInteractiveElementEditor.directionManager);
                        else if (element is HotSpot) EditorUtils.SetDirty(VisualInteractiveElementEditor.hotSpotManager);
                    }
                    GUI.changed = true;
                    e.Use();
                }
            }
            else if (e.type == EventType.MouseUp)
            {
                if (editor.dragElement == element)
                {
                    editor.dragElement = null;
                    e.Use();
                }
            }
        }
    }
}