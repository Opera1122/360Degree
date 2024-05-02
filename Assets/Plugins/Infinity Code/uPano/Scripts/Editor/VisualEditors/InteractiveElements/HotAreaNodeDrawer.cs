/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System.Collections.Generic;
using InfinityCode.uPano.Editors.Utils;
using InfinityCode.uPano.HotAreas;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors.VisualEditors.InteractiveElements
{
    public static class HotAreaNodeDrawer
    {
        private static List<Vector3> polygon = new List<Vector3>(128);
        private static List<Vector2> hitTestPolygon = new List<Vector2>(128);

        public static void Draw(VisualInteractiveElementEditor editor, HotArea area)
        {
            List<PanTilt> points = area.points;
            polygon.Clear();
            Vector3[] polygon2 = new Vector3[points.Count + 1];

            for (int i = 0; i < points.Count; i++)
            {
                Vector2 v = editor.PanTiltToPoint(points[i]);
                polygon.Add(v);
                polygon2[i] = v;
            }

            polygon2[polygon2.Length - 1] = polygon2[0];

            Color color = Handles.color;

            Color c = area.color;
            Handles.color = new Color(c.r, c.g, c.b, 0.3f);

            List<int> indices = MathHelper.Triangulate(polygon);
            int j = 0;

            for (int i = 0; i < indices.Count / 3; i++)
            {
                Handles.DrawAAConvexPolygon(polygon[indices[j++]], polygon[indices[j++]], polygon[indices[j++]]);
            }

            if (VisualInteractiveElementEditor.activeElement == area)
            {
                Handles.color = Color.black;
                Handles.DrawAAPolyLine(2, polygon2);

                bool isRemoveMode = Event.current.modifiers == EventModifiers.Alt;
                bool isInsertMode = IsInsertPointMode();
                bool isMoveMode = !isRemoveMode && !isInsertMode;

                if (isInsertMode)
                {
                    PanTilt panTilt;
                    int nearestIndex = NearestPointIndex(editor, area, out panTilt);
                    Handles.color = Color.green;
                    Handles.DrawAAPolyLine(3, polygon2[nearestIndex], polygon2[nearestIndex + 1]);
                }

                for (int i = 0; i < points.Count; i++)
                {
                    Handles.color = Color.black;
                    Vector3 p = polygon[i];
                    Handles.DrawSolidDisc(p, Vector3.forward, 5);
                    Handles.color = Color.white;
                    Handles.DrawSolidDisc(p, Vector3.forward, 3);

                    Rect rect = new Rect(p.x - 5, p.y - 5, 10, 10);
                    if (isMoveMode) EditorGUIUtility.AddCursorRect(rect, MouseCursor.MoveArrow);
                    else if (isRemoveMode) EditorGUIUtility.AddCursorRect(rect, MouseCursor.ArrowMinus);
                }

                if (isInsertMode)
                {
                    EditorGUIUtility.AddCursorRect(new Rect(Event.current.mousePosition, Vector2.one), MouseCursor.ArrowPlus);
                }
            }

            Handles.color = color;
        }

        private static int GetControlPoint(VisualInteractiveElementEditor editor, HotArea area, Vector2 mousePosition)
        {
            List<PanTilt> points = area.points;
            for (int i = 0; i < points.Count; i++)
            {
                PanTilt p = points[i];
                Vector2 point = editor.PanTiltToPoint(p.pan, p.tilt);
                if (Mathf.Abs(point.x - mousePosition.x) <= 5 &&
                    Mathf.Abs(point.y - mousePosition.y) <= 5)
                {
                    return i;
                }
            }

            return -1;
        }

        public static bool HitTest(VisualInteractiveElementEditor editor, HotArea hotArea, Vector2 mousePosition)
        {
            if (hotArea == null || hotArea.points == null || hotArea.points.Count < 3) return false;

            hitTestPolygon.Clear();
            for (int i = 0; i < hotArea.points.Count; i++)
            {
                PanTilt p = hotArea.points[i];
                hitTestPolygon.Add(editor.PanTiltToPoint(p.pan, p.tilt));
            }

            return MathHelper.IsPointInPolygon(hitTestPolygon, mousePosition.x, mousePosition.y);
        }

        public static bool IsInsertPointMode()
        {
            Event e = Event.current;
            return VisualInteractiveElementEditor.activeElement is HotArea && (e.modifiers == EventModifiers.Control || e.modifiers == EventModifiers.Command);
        }

        private static int NearestPointIndex(VisualInteractiveElementEditor editor, HotArea area, out PanTilt point)
        {
            Event e = Event.current;
            Vector2 mousePos = e.mousePosition;

            float bestDistance = float.MaxValue;
            int bestIndex = -1;

            int count = area.points.Count;
            for (int i = 0; i < count; i++)
            {
                int i1 = i;
                int i2 = i + 1;
                if (i2 >= count) i2 = 0;

                Vector2 p1 = editor.PanTiltToPoint(area.points[i1]);
                Vector2 p2 = editor.PanTiltToPoint(area.points[i2]);

                Vector2 nearestPoint = MathHelper.NearestPointStrict(mousePos, p1, p2);
                float d = (mousePos - nearestPoint).sqrMagnitude;

                if (d < bestDistance)
                {
                    bestDistance = d;
                    bestIndex = i;
                }
            }

            point = editor.PointToPanTilt(mousePos);
            return bestIndex;
        }

        private static void ProcessDrag(VisualInteractiveElementEditor editor, HotArea area)
        {
            if (editor.dragElement != area) return;

            Vector2 uv = editor.GetUV();
            float pan, tilt;
            if (editor.singleTexturePanoRenderer.GetPanTiltByUV(uv, out pan, out tilt))
            {
                Undo.SetCurrentGroupName("Set Pan and Tilt");
                int group = Undo.GetCurrentGroup();

                if (VisualInteractiveElementEditor.hotAreaManager != null) Undo.RecordObject(VisualInteractiveElementEditor.hotAreaManager, "Set Pan and Tilt");

                Undo.CollapseUndoOperations(@group);

                if (editor.dragControlPoint != -1)
                {
                    PanTilt panTilt = area.points[editor.dragControlPoint];
                    panTilt.Set(pan, tilt);
                    area.points[editor.dragControlPoint] = panTilt;
                }
                else
                {
                    area.SetPanTilt(pan, tilt);
                }

                InteractiveElementSettings.Redraw();
                EditorUtils.SetDirty(VisualInteractiveElementEditor.directionManager);
                EditorUtils.SetDirty(VisualInteractiveElementEditor.hotSpotManager);
            }

            GUI.changed = true;
            Event.current.Use();
        }

        public static void ProcessEvents(VisualInteractiveElementEditor editor, HotArea area)
        {
            Event e = Event.current;

            if (e.type == EventType.MouseDown)
            {
                ProcessMouseDown(editor, area);
            }
            else if (e.type == EventType.MouseDrag)
            {
                ProcessDrag(editor, area);
            }
            else if (e.type == EventType.MouseUp)
            {
                if (editor.dragElement == area)
                {
                    editor.dragControlPoint = -1;
                    editor.dragElement = null;
                    e.Use();
                }
            }
            else if (e.type == EventType.MouseMove)
            {
                if (VisualInteractiveElementEditor.activeElement == area && IsInsertPointMode())
                {
                    editor.Repaint();
                    e.Use();
                }
            }
            else if (e.type == EventType.KeyDown || e.type == EventType.KeyUp)
            {
                if (VisualInteractiveElementEditor.activeElement == area)
                {
                    if (e.keyCode == KeyCode.LeftControl ||
                        e.keyCode == KeyCode.LeftAlt ||
                        e.keyCode == KeyCode.RightAlt ||
                        e.keyCode == KeyCode.RightControl ||
                        e.keyCode == KeyCode.LeftCommand ||
                        e.keyCode == KeyCode.RightControl)
                    {
                        editor.Repaint();
                        e.Use();
                    }
                }
            }
        }

        private static void ProcessMouseDown(VisualInteractiveElementEditor editor, HotArea area)
        {
            Event e = Event.current;
            int controlPoint = -1;

            if (VisualInteractiveElementEditor.activeElement == area)
            {
                if (IsInsertPointMode())
                {
                    PanTilt point;
                    int bestIndex = NearestPointIndex(editor, area, out point);
                    area.points.Insert(bestIndex + 1, point);
                    e.Use();
                }
                else
                {
                    controlPoint = GetControlPoint(editor, area, e.mousePosition);
                    if (controlPoint != -1)
                    {
                        if (e.modifiers == EventModifiers.Alt)
                        {
                            area.points.RemoveAt(controlPoint);
                        }
                        else
                        {
                            editor.dragElement = area;
                            editor.dragControlPoint = controlPoint;
                        }

                        e.Use();
                    }
                }
            }

            if (controlPoint == -1 && HitTest(editor, area, e.mousePosition))
            {
                if (e.button == 0)
                {
                    editor.SelectElement(area);
                    editor.dragElement = area;
                    e.Use();
                }
                else if (e.button == 1)
                {
                    editor.ProcessElementContextMenu(area);
                    e.Use();
                }
            }
        }
    }
}