/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.Editors.Utils;
using InfinityCode.uPano.Editors.VisualEditors.Tours;
using InfinityCode.uPano.InteractiveElements;
using InfinityCode.uPano.Renderers.Base;
using InfinityCode.uPano.Tours;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors.VisualEditors.InteractiveElements
{
    public partial class VisualInteractiveElementEditor : EditorWindow
    {
        public enum Mode
        {
            idle,
            connection,
            createHotArea,
            editHotArea
        }

        internal static VisualInteractiveElementEditor wnd;
        
        private static GUIStyle _hintBodyStyle;
        private static GUIStyle _hintHeaderStyle;

        [NonSerialized]
        internal Mode mode = Mode.idle;

        internal PanoRenderer panoRenderer;
        internal ISingleTexturePanoRenderer singleTexturePanoRenderer;
        internal Rect viewRect;

        private TourItem connectionFrom;
        private TourItem connectionTarget;
        private bool isTextureDrag;
        private Vector2 rectPosition;
        private float scale = 1;
        private bool waitReturn;

        public static GUIStyle hintHeaderStyle
        {
            get
            {
                if (_hintHeaderStyle == null)
                {
                    _hintHeaderStyle = new GUIStyle(EditorStyles.whiteLargeLabel)
                    {
                        alignment = TextAnchor.MiddleCenter, 
                        fontStyle = FontStyle.Bold, 
                        fontSize = 24,
                        wordWrap = true
                    };
                }

                return _hintHeaderStyle;
            }
        }

        public static GUIStyle hintBodyStyle
        {
            get
            {
                if (_hintBodyStyle == null)
                {
                    _hintBodyStyle = new GUIStyle(EditorStyles.whiteLargeLabel)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        wordWrap = true
                    };
                }

                return _hintBodyStyle;
            }
        }

        private void DrawHint(string header, string message, float extraHeight = 0)
        {
            float windowCenter = position.width / 2;
            float headerWidth = 500;
            GUILayout.BeginArea(new Rect(windowCenter - headerWidth / 2, 25, headerWidth, 70 + extraHeight), TourMaker.helpStyle);

            GUILayout.Label(header, hintHeaderStyle);
            GUILayout.Label(message, hintBodyStyle);

            GUILayout.EndArea();
        }

        private void DrawNoPano()
        {
            EditorGUI.LabelField(new Rect(position.width / 2 - 150, position.height / 2 - 50, 300, 100), "No PanoRenderer", TourMaker.hugeWhiteLabelStyle);
        }

        private bool DrawReturnButton(Rect rect, string text)
        {
            if (GUI.Button(rect, text))
            {
                GetWindow<TourMaker>();
                InteractiveElementSettings.CloseWindow();
                return true;
            }

            return false;
        }

        private void DrawTexture()
        {
            if (Event.current.type != EventType.Repaint) return;

            Texture texture = singleTexturePanoRenderer.texture;

            if (texture == null)
            {
                GUI.Box(new Rect(0, 0, position.width, position.height), GUIContent.none);
                return;
            }

            if (texture is Cubemap) texture = EditorUtils.GetCubemapTexture(texture as Cubemap);

            float ratio = texture.width / (float)texture.height;
            float screenRatio = position.width / position.height;

            float width;
            float height;
            float offsetX = 0;
            float offsetY = 0;

            if (screenRatio < ratio)
            {
                width = position.width;
                height = position.width / ratio;
                offsetY = (position.height - height) / 2;
            }
            else
            {
                height = position.height;
                width = position.height * ratio;
                offsetX = (position.width - width) / 2;
            }

            viewRect = new Rect(offsetX, offsetY, width, height);
            viewRect.position += rectPosition;
            viewRect.size *= scale;
            GUI.DrawTexture(viewRect, texture, ScaleMode.ScaleToFit);
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button("Reset view", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
                scale = 1;
                rectPosition = Vector2.zero;
            }

            EditorGUILayout.Space();

            EditorGUILayout.EndHorizontal();
        }

        public Vector2 GetUV()
        {
            Event e = Event.current;

            return new Vector2(
                (e.mousePosition.x - viewRect.x) / viewRect.width,
                1 - (e.mousePosition.y - viewRect.y) / viewRect.height);
        }

        private void InitPanoRenderer()
        {
            serializedHotSpotManager = null;
            serializedHotSpotItems = null;

            if (Selection.activeGameObject == null)
            {
                panoRenderer = null;
                singleTexturePanoRenderer = null;
                return;
            }

            panoRenderer = Selection.activeGameObject.GetComponent<PanoRenderer>();
            if (panoRenderer == null) return;

            singleTexturePanoRenderer = panoRenderer as ISingleTexturePanoRenderer;
            if (singleTexturePanoRenderer == null)
            {
                panoRenderer = null;
                return;
            }

            InitManagers();
        }

        private void LoadIcons()
        {
            hotSpotIcon = EditorUtils.LoadAsset<Texture2D>("Textures/Editor/hotspot-icon.png");
            hotSpotSelectedIcon = EditorUtils.LoadAsset<Texture2D>("Textures/Editor/hotspot-selected-icon.png");
            northPanIcon = EditorUtils.LoadAsset<Texture2D>("Textures/Editor/north-icon.png");
        }

        private void OnDestroy()
        {
            InteractiveElementSettings.CloseWindow();
        }

        private void OnDragTexture(Vector2 delta)
        {
            rectPosition += delta;
            GUI.changed = true;
        }

        private void OnEnable()
        {
            wnd = this;
            wantsMouseMove = true;

            Selection.selectionChanged -= OnSelectionChanged;
            Selection.selectionChanged += OnSelectionChanged;

            InitPanoRenderer();
            LoadIcons();
        }

        private void OnFocus()
        {
            OnEnable();
        }

        private void OnGUI()
        {
            if (panoRenderer == null)
            {
                DrawNoPano();
                return;
            }

            bool drawContent = singleTexturePanoRenderer != null;

            if (drawContent)
            {
                DrawTexture();
                DrawNorthPan();
                DrawElements();
            }

            DrawToolbar();

            Rect rect = new Rect(position.width / 2 - 80, position.height - 60, 180, 30);

            if (mode == Mode.connection)
            {
                DrawConnectionHint();

                if (DrawReturnButton(rect, "Cancel connection creation"))
                {
                    mode = Mode.idle;
                }
            }
            else if (mode == Mode.createHotArea || mode == Mode.editHotArea)
            {
                if (mode == Mode.editHotArea) DrawEditHotAreaHint();
                else DrawCreateHotAreaHint();

                if (GUI.Button(rect, "Stop edit hot area"))
                {
                    mode = Mode.idle;
                    activeElement = null;
                }
            }

            if (waitReturn && mode == Mode.idle && DrawReturnButton(rect, "Return to Tour Maker"))
            {
                waitReturn = false;
            }

            if (drawContent)
            {
                ProcessNorthPanEvents();
                ProcessElementEvents();
                ProcessEvents();
            }

            if (GUI.changed) Repaint();
        }

        private void OnScrollWheel()
        {
            Event e = Event.current;

            float oldScale = scale;
            scale = Mathf.Clamp(scale * (1 - e.delta.y / 100), 0.1f, 10);

            float scaleDelta = scale - oldScale;
            if (Mathf.Abs(scaleDelta) < float.Epsilon) return;

            scaleDelta = scale / oldScale;

            Vector2 p1 = e.mousePosition - viewRect.position;
            p1.x /= viewRect.width;
            p1.y /= viewRect.height;

            Vector2 p2 = e.mousePosition - viewRect.position;
            p2.x /= viewRect.width * scaleDelta;
            p2.y /= viewRect.height * scaleDelta;

            Vector2 offset = p2 - p1;
            offset.x *= viewRect.width * scaleDelta;
            offset.y *= viewRect.height * scaleDelta;

            rectPosition += offset;

            GUI.changed = true;
        }

        [MenuItem(EditorUtils.MENU_PATH + "Interactive Element Editor", false, 2)]
        public static void OpenWindow()
        {
            wnd = GetWindow<VisualInteractiveElementEditor>("Interactive Element Editor");
            wnd.autoRepaintOnSceneChange = true;
            wnd.wantsMouseMove = true;
            wnd.waitReturn = false;
        }

        public static void OpenWindow(TourItem item1, TourItem item2)
        {
            OpenWindow();
            wnd.connectionFrom = item1;
            wnd.connectionTarget = item2;
            wnd.mode = Mode.connection;
        }

        internal static void Redraw()
        {
            if (wnd != null) wnd.Repaint();
        }

        public Vector2 PanTiltToPoint(float pan, float tilt)
        {
            Vector2 uv = singleTexturePanoRenderer.GetUV(pan, tilt);
            uv.x = viewRect.width * uv.x + viewRect.position.x;
            uv.y = (1 - uv.y) * viewRect.height + viewRect.position.y;
            return uv;
        }

        public Vector2 PanTiltToPoint(PanTilt panTilt)
        {
            return PanTiltToPoint(panTilt.pan, panTilt.tilt);
        }

        public PanTilt PointToPanTilt(Vector2 mousePosition)
        {
            Vector2 uv = new Vector2(
                (mousePosition.x - viewRect.x) / viewRect.width,
                1 - (mousePosition.y - viewRect.y) / viewRect.height);
            float pan, tilt;
            singleTexturePanoRenderer.GetPanTiltByUV(uv, out pan, out tilt);
            return new PanTilt(pan, tilt);
        }

        public void PointToPanTilt(Vector2 mousePosition, out float pan, out float tilt)
        {
            Vector2 uv = new Vector2(
                (mousePosition.x - viewRect.x) / viewRect.width,
                1 - (mousePosition.y - viewRect.y) / viewRect.height);
            singleTexturePanoRenderer.GetPanTiltByUV(uv, out pan, out tilt);
        }

        private void ProcessContextMenu(Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Create Direction"), false, () => { CreateDirectionFromContextMenu(mousePosition); });
            genericMenu.AddItem(new GUIContent("Create Hot Area"), false, () => { CreateHotAreaFromContextMenu(mousePosition); });
            genericMenu.AddItem(new GUIContent("Create Hot Spot"), false, () => { CreateHotSpotFromContextMenu(mousePosition); });
            genericMenu.ShowAsContext();
        }

        private void ProcessEvents()
        {
            Event e = Event.current;

            if (e.type == EventType.MouseDown)
            {
                if (e.button == 0)
                {
                    if (mode == Mode.createHotArea)
                    {
                        AddActiveAreaPoint();
                        e.Use();
                    }
                    else
                    {
                        isTextureDrag = true;
                        activeElement = null;
                        mode = Mode.idle;
                        InteractiveElementSettings.Redraw();
                        e.Use();
                    }
                }
                else if (e.button == 1)
                {
                    ProcessContextMenu(e.mousePosition);
                    e.Use();
                }
                else if (e.button == 2)
                {
                    isTextureDrag = true;
                    e.Use();
                }
            }
            else if (e.type == EventType.MouseDrag)
            {
                if ((e.button == 0 || e.button == 2) && isTextureDrag)
                {
                    OnDragTexture(e.delta);
                    e.Use();
                }
            }
            else if (e.type == EventType.MouseUp)
            {
                isTextureDrag = false;
                e.Use();
            }
            else if (e.type == EventType.ScrollWheel)
            {
                OnScrollWheel();
                e.Use();
            }
            else if (e.type == EventType.KeyUp)
            {
                if (e.keyCode == KeyCode.Delete || e.keyCode == KeyCode.Backspace)
                {
                    if (activeElement != null)
                    {
                        RemoveActiveElement();
                        mode = Mode.idle;
                        
                        InteractiveElementSettings.CloseWindow();
                        GUI.changed = true;
                        e.Use();
                    }
                }
            }
        }

        private void TrySetConnection(InteractiveElement el)
        {
            if (mode != Mode.connection) return;

            el.switchToPanorama = connectionTarget.gameObject;
            connectionFrom.UpdateOutLinks();
            mode = Mode.idle;
            waitReturn = true;
            connectionFrom = null;
            connectionTarget = null;
        }
    }
}