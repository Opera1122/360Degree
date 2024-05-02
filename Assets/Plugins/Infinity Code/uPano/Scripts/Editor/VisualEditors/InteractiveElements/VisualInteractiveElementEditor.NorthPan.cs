/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Directions;
using InfinityCode.uPano.Editors.Utils;
using InfinityCode.uPano.HotAreas;
using InfinityCode.uPano.HotSpots;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors.VisualEditors.InteractiveElements
{
    public partial class VisualInteractiveElementEditor
    {
        private bool isNorthPanDrag;
        private Texture northPanIcon;
        private bool showNorthPanLabel;

        private void DrawNorthPan()
        {
            if (singleTexturePanoRenderer == null) return;

            Event e = Event.current;

            Vector2 northUV = singleTexturePanoRenderer.GetUV(0, 0);
            northUV.x = viewRect.width * northUV.x + viewRect.position.x;
            northUV.y = viewRect.position.y;

            if (northUV.y < 32) northUV.y = 32;

            Rect rect = new Rect(northUV.x - 16, northUV.y - 16, 32, 32);
            if (e.type == EventType.Repaint)
            {
                EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeHorizontal);
            }

            if (isNorthPanDrag || rect.Contains(e.mousePosition))
            {
                if (!showNorthPanLabel)
                {
                    showNorthPanLabel = true;
                    GUI.changed = true;
                }

                if (e.type == EventType.Repaint)
                {
                    Color color = Handles.color;
                    Handles.color = Color.cyan;

                    Handles.DrawAAPolyLine(2, new Vector3(rect.center.x, 0), new Vector3(rect.center.x, position.height));

                    Handles.color = color;

                    GUIContent content = new GUIContent("North Pan: " + panoRenderer.pano.northPan);
                    Vector2 size = normalPrefabStyle.CalcSize(content);
                    Rect labelRect = new Rect(rect.center.x - size.x / 2, rect.y + size.y + 15, size.x, size.y);
                    normalPrefabStyle.Draw(labelRect, content, false, false, false, false);
                }
            }
            else if (showNorthPanLabel)
            {
                showNorthPanLabel = false;
                GUI.changed = true;
            }

            if (e.type == EventType.Repaint)
            {
                GUI.DrawTexture(rect, northPanIcon);
            }
        }

        private void ProcessNorthPanEvents()
        {
            Event e = Event.current;
            if (e.type == EventType.MouseDown && showNorthPanLabel)
            {
                if (e.button == 0)
                {
                    isNorthPanDrag = true;
                    e.Use();
                }
                else if (e.button == 1)
                {
                    e.Use();
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Reset"), false, () => SetNorthPan(0));
                    menu.ShowAsContext();
                }
            }
            else if (e.type == EventType.MouseUp)
            {
                if (isNorthPanDrag)
                {
                    isNorthPanDrag = false;
                    e.Use();
                }
            }
            else if (e.type == EventType.MouseDrag)
            {
                if (isNorthPanDrag)
                {
                    Vector2 uv = GetUV();
                    float newNorth = Mathf.Repeat((uv.x - 0.5f) * 360, 360);
                    SetNorthPan(newNorth);
                    e.Use();
                }
            }
        }

        private void SetNorthPan(float northPan)
        {
            Undo.SetCurrentGroupName("Set North Pan");
            int group = Undo.GetCurrentGroup();

            float delta = northPan - panoRenderer.pano.northPan;
            Undo.RecordObject(panoRenderer.pano, "Set North Pan");
            panoRenderer.pano.northPan = northPan;
            EditorUtils.SetDirty(panoRenderer.pano);

            float pan, tilt;

            if (directionManager != null)
            {
                Undo.RecordObject(directionManager, "Set North Pan");
                foreach (Direction direction in directionManager)
                {
                    direction.GetPanTilt(out pan, out tilt);
                    direction.SetPanTilt(pan - delta, tilt);
                }
            }

            if (hotSpotManager != null)
            {
                Undo.RecordObject(hotSpotManager, "Set North Pan");
                foreach (HotSpot hotSpot in hotSpotManager)
                {
                    hotSpot.GetPanTilt(out pan, out tilt);
                    hotSpot.SetPanTilt(pan - delta, tilt);
                }
            }

            if (hotAreaManager != null)
            {
                Undo.RecordObject(hotAreaManager, "Set North Pan");
                foreach (HotArea area in hotAreaManager)
                {
                    area.GetPanTilt(out pan, out tilt);
                    area.SetPanTilt(pan - delta, tilt);
                }
            }

            Undo.CollapseUndoOperations(group);

            GUI.changed = true;
        }
    }
}