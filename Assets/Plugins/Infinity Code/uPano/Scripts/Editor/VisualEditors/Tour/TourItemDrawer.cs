/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.Editors.Utils;
using InfinityCode.uPano.Editors.VisualEditors.InteractiveElements;
using InfinityCode.uPano.Renderers;
using InfinityCode.uPano.Renderers.Base;
using InfinityCode.uPano.Tours;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InfinityCode.uPano.Editors.VisualEditors.Tours
{
    public class TourItemDrawer
    {
        private static GUIStyle _normalStyle;
        private static GUIStyle _selectedStyle;
        private static GUIStyle _startStyle;

        public Action<TourItemDrawer> OnRightMouseDown;
        public Action<TourItemDrawer> OnRightMouseUp;
        public Action<TourItemDrawer> OnSelect;
        public Action<TourItemDrawer> OnRemove;

        public TourItem item;

        public Rect rect;
        public Rect viewRect;

        public bool isDragged;

        private Vector2 size = new Vector2(200, 150);
        private TourMaker maker;

        public bool isSelected
        {
            get
            {
                return this == TourMaker.selectedDrawer;
            }
        }

        public static GUIStyle normalStyle
        {
            get
            {
                if (_normalStyle == null)
                {
                    _normalStyle = new GUIStyle();
                    _normalStyle.border = new RectOffset(12, 12, 12, 12);
                    _normalStyle.normal.background = EditorUtils.LoadAsset<Texture2D>("Textures/Editor/tour-item-normal.png");
                }

                return _normalStyle;
            }
        }

        public static GUIStyle selectedStyle
        {
            get
            {
                if (_selectedStyle == null)
                {
                    _selectedStyle = new GUIStyle();
                    _selectedStyle.border = new RectOffset(12, 12, 12, 12);
                    _selectedStyle.normal.background = EditorUtils.LoadAsset<Texture2D>("Textures/Editor/tour-item-selected.png");
                }

                return _selectedStyle;
            }
        }

        public static GUIStyle startStyle
        {
            get
            {
                if (_startStyle == null)
                {
                    _startStyle = new GUIStyle();
                    _startStyle.border = new RectOffset(12, 12, 12, 12);
                    _startStyle.normal.background = EditorUtils.LoadAsset<Texture2D>("Textures/Editor/tour-item-start.png");
                }

                return _startStyle;
            }
        }

        public TourItemDrawer(TourMaker maker, TourItem item)
        {
            this.maker = maker;
            this.item = item;
            rect = new Rect(item.position - size / 2, size);
        }

        public void Dispose()
        {
            OnRemove = null;
            OnRightMouseDown = null;
            OnRightMouseUp = null;
            OnSelect = null;
            item = null;
            maker = null;
        }

        public void Destroy()
        {
            if (item.gameObject != null) Undo.DestroyObjectImmediate(item.gameObject);
            Dispose();
        }

        private void Drag(Vector2 delta, float scale)
        {
            Undo.RecordObject(item, "Drag Tour Item");

            item.position += delta / scale;
            rect.position = item.position - size / 2;
            GUI.changed = true;
        }

        public void Draw(Vector2 center, float scale, Vector2 windowSize)
        {
            viewRect = new Rect((center + rect.position) * scale + windowSize / 2, rect.size * scale);
            EditorGUIUtility.AddCursorRect(viewRect, MouseCursor.Link);

            if (item.tour.startItem == item)
            {
                Rect r = new Rect(viewRect);
                r.position -= new Vector2(4, 4);
                r.size += new Vector2(8, 8);

                GUI.Box(r, "", startStyle);
            }

            GUI.Box(viewRect, "", isSelected? selectedStyle: normalStyle);

            Texture texture = item.texture;
            if (texture != null)
            {
                Rect r = new Rect(viewRect);
                r.position += new Vector2(10, 10);
                r.size -= new Vector2(20, 20);

                if (texture is Cubemap) texture = EditorUtils.GetCubemapTexture(texture as Cubemap);

                GUI.DrawTexture(r, texture);
            }
        }

        public void DrawConnections()
        {
            if (item == null)
            {
                //Debug.Log("No item");
                return;
            }

            if (isSelected)
            {
                Handles.color = Color.blue;
            }

            if (item.outLinks == null) item.UpdateOutLinks();

            for (int i = 0; i < item.outLinks.Count; i++)
            {
                TourItem link = item.outLinks[i];
                TourItemDrawer drawer = TourMaker.GetDrawer(link);

                if (drawer == null) continue;
                Vector2 p1 = viewRect.center;
                Vector2 p2 = drawer.viewRect.center;

                float angle = MathHelper.Angle2D(p1.x, p1.y, p2.x, p2.y);
                float a = (angle + 90) * Mathf.Deg2Rad;
                const float offsetLength = 4;
                Vector2 o = new Vector2(Mathf.Cos(a) * offsetLength, Mathf.Sin(a) * offsetLength);
                p1 += o;
                p2 += o;

                Handles.DrawAAPolyLine(4, p1, p2);

                Vector3 center = (p1 + p2) / 2;
                float r = 4;

                float a1 = (angle - 125) * Mathf.Deg2Rad;
                float a2 = angle * Mathf.Deg2Rad;
                float a3 = (angle + 125) * Mathf.Deg2Rad;

                Handles.DrawAAPolyLine(4, new []
                {
                    center + new Vector3(Mathf.Cos(a1) * r, Mathf.Sin(a1) * r),
                    center + new Vector3(Mathf.Cos(a2) * r, Mathf.Sin(a2) * r),
                    center + new Vector3(Mathf.Cos(a3) * r, Mathf.Sin(a3) * r)
                });
            }

            if (isSelected) Handles.color = Color.gray;
        }

        public void ProcessContextMenu()
        {
            GenericMenu genericMenu = new GenericMenu();
            PanoRenderer panoRenderer = item.GetComponent<PanoRenderer>();
            genericMenu.AddItem(new GUIContent("Type/Spherical"), panoRenderer is SphericalPanoRenderer, () =>
            {
                Object.DestroyImmediate(panoRenderer);
                item.gameObject.AddComponent<SphericalPanoRenderer>().texture = item.texture;
                item.Init();
            });
            genericMenu.AddItem(new GUIContent("Type/Single Texture Cube Faces"), panoRenderer is SingleTextureCubeFacesPanoRenderer, () =>
            {
                Object.DestroyImmediate(panoRenderer);
                item.gameObject.AddComponent<SingleTextureCubeFacesPanoRenderer>().texture = item.texture;
                item.Init();
            });
            genericMenu.AddItem(new GUIContent("Type/Cylindrical"), panoRenderer is CylindricalPanoRenderer, () =>
            {
                Object.DestroyImmediate(panoRenderer);
                item.gameObject.AddComponent<CylindricalPanoRenderer>().texture = item.texture;
                item.Init();
            });
            genericMenu.AddItem(new GUIContent("Type/Cubemap"), panoRenderer is CubemapPanoRenderer, () =>
            {
                Object.DestroyImmediate(panoRenderer);
                item.gameObject.AddComponent<CubemapPanoRenderer>().cubemap = item.texture as Cubemap;
                item.Init();
            });
            genericMenu.AddItem(new GUIContent("Interactive Element Editor"), false, VisualInteractiveElementEditor.OpenWindow);
            genericMenu.AddItem(new GUIContent("Set Start Panorama"), false, () =>
            {
                Undo.RecordObject(item.tour, "Set Tour Start Item");
                item.tour.startItem = item;
            });
            genericMenu.AddItem(new GUIContent("Remove item"), false, () =>
            {
                if (OnRemove != null) OnRemove(this);
            });
            genericMenu.ShowAsContext();
        }

        public void ProcessEvents(float scale)
        {
            Event e = Event.current;

            if (e.type == EventType.MouseDown)
            {
                if (viewRect.Contains(e.mousePosition))
                {
                    if (!isSelected) OnSelect(this);
                    TourMaker.selectedDrawer = this;

                    if (e.button == 0)
                    {
                        isDragged = true;
                    }
                    else if (e.button == 1)
                    {
                        OnRightMouseDown(this);
                        e.Use();
                    }
                }
            }
            else if (e.type == EventType.MouseUp)
            {
                isDragged = false;
                if (viewRect.Contains(e.mousePosition))
                {
                    if (e.button == 0)
                    {
                        
                    }
                    else if (e.button == 1)
                    {
                        OnRightMouseUp(this);
                    }
                    e.Use();
                }
            }
            else if (e.type == EventType.MouseDrag)
            {
                if (e.button == 0 && isDragged)
                {
                    Drag(e.delta, scale);
                    e.Use();
                }
            }
        }
    }
}