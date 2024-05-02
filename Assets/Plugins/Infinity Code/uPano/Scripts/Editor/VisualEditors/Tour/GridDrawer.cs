/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors.VisualEditors.Tours
{
    public static class GridDrawer
    {
        public static void Draw(Rect screenRect, Vector2 center, float scale)
        {
            DrawGrid(20, 0.2f, Color.gray, screenRect, center, scale);
            DrawGrid(100, 0.4f, Color.gray, screenRect, center, scale);
        }

        private static void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor, Rect screenRect, Vector2 center, float scale)
        {
            gridSpacing *= scale;

            int widthDivs = Mathf.CeilToInt(screenRect.width / gridSpacing) + 1;
            int heightDivs = Mathf.CeilToInt(screenRect.height / gridSpacing) + 1;

            Handles.BeginGUI();

            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            float sx = (center.x * scale + screenRect.width / 2) % gridSpacing;
            float sy = (center.y * scale + screenRect.height / 2) % gridSpacing;

            for (int i = 0; i < widthDivs; i++)
            {
                Vector3 p1 = new Vector3(gridSpacing * i + sx, sy - gridSpacing, 0);
                Vector3 p2 = new Vector3(gridSpacing * i + sx, sy + screenRect.height + gridSpacing, 0f);
                Handles.DrawLine(p1, p2);
            }

            for (int i = 0; i < heightDivs; i++)
            {
                Vector3 p1 = new Vector3(sx - gridSpacing, sy + gridSpacing * i, 0);
                Vector3 p2 = new Vector3(sx + screenRect.width + gridSpacing, sy + gridSpacing * i, 0f);
                Handles.DrawLine(p1, p2);
            }

            Handles.EndGUI();
        }
    }
}