/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using System.Collections.Generic;
using InfinityCode.uPano.InteractiveElements;
using UnityEngine;

namespace InfinityCode.uPano.HotAreas
{
    /// <summary>
    /// Hot area is a polygon area defined by a set of points that have pan and tilt.
    /// </summary>
    [Serializable]
    public class HotArea : InteractiveElement
    {
        /// <summary>
        /// Set of points that have pan and tilt
        /// </summary>
        public List<PanTilt> points = new List<PanTilt>();

        /// <summary>
        /// The color of the area in the Visual Element Editor.
        /// </summary>
        public Color color = Color.red;

        [NonSerialized]
        private Rect rect;

        [NonSerialized]
        private List<Vector2> normalizedPoints;

        private HotAreaManager _manager;

        /// <summary>
        /// Gets the screen position of HotArea
        /// </summary>
        public Vector3 screenPosition
        {
            get { return GetScreenPosition(); }
        }

        /// <summary>
        /// Reference to HotArea manager
        /// </summary>
        public HotAreaManager manager
        {
            get { return _manager; }
            set { _manager = value; }
        }

        /// <summary>
        /// Checks if the specified pan and tilt are in the area.
        /// </summary>
        /// <param name="pan">Pan</param>
        /// <param name="tilt">Tilt</param>
        /// <returns></returns>
        public bool Contain(float pan, float tilt)
        {
            float panOffset = pan - rect.center.x;

            if (panOffset > 180) pan -= 360;
            else if (panOffset < -180) pan += 360;

            if (!rect.Contains(new Vector2(pan, tilt))) return false;

            return MathHelper.IsPointInPolygon(normalizedPoints, pan, tilt);
        }

        public override void GetPanTilt(out float pan, out float tilt)
        {
            pan = rect.center.x;
            tilt = rect.center.y;
        }

        /// <summary>
        /// Gets the screen position of HotArea
        /// </summary>
        /// <param name="camera">Camera</param>
        /// <returns>Screen position of HotArea</returns>
        public Vector2 GetScreenPosition(Camera camera = null)
        {
            return manager.panoRenderer.GetScreenPosition(rect.center.x, rect.center.y, camera);
        }

        public override void Reinit()
        {
            if (normalizedPoints == null) normalizedPoints = new List<Vector2>(points.Count);
            else normalizedPoints.Clear();

            if (points.Count < 3) return;

            Vector2 pp = points[0];
            rect = new Rect(pp, Vector2.zero);
            normalizedPoints.Add(pp);

            for (int i = 1; i < points.Count; i++)
            {
                Vector2 p = points[i];

                do
                {
                    float ox = p.x - pp.x;
                    if (ox > 180)
                    {
                        p.x -= 360;
                        continue;
                    }

                    if (ox < -180)
                    {
                        p.x += 360;
                        continue;
                    }
                } while (false);

                if (p.x < rect.x) rect.xMin = p.x;
                else if (p.x > rect.xMax) rect.xMax = p.x;

                if (p.y < rect.y) rect.yMin = p.y;
                else if (p.y > rect.yMax) rect.yMax = p.y;

                normalizedPoints.Add(p);
                pp = p;
            }
        }

        public override void SetPanTilt(float pan, float tilt)
        {
            float panDelta = pan - rect.center.x;
            float tiltDelta = tilt - rect.center.y;

            for (int i = 0; i < points.Count; i++)
            {
                PanTilt p = points[i];
                p.pan += panDelta;
                p.tilt += tiltDelta;
                points[i] = p;
            }

            if (Application.isPlaying) Reinit();
        }
    }
}