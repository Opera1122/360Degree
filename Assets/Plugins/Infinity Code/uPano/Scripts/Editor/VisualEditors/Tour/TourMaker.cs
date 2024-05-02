/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using System.Collections.Generic;
using System.IO;
using InfinityCode.uPano.Actions;
using InfinityCode.uPano.Actions.HotSpots;
using InfinityCode.uPano.Controls;
using InfinityCode.uPano.Directions;
using InfinityCode.uPano.Editors.Utils;
using InfinityCode.uPano.HotSpots;
using InfinityCode.uPano.InteractiveElements;
using InfinityCode.uPano.Plugins;
using InfinityCode.uPano.Tours;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InfinityCode.uPano.Editors.VisualEditors.Tours
{
    public class TourMaker: EditorWindow
    {
        public static TourMaker instance;

        private static GUIStyle _helpStyle;
        private static GUIStyle _hugeWhiteLabelStyle;
        private static List<TourItemDrawer> drawers = new List<TourItemDrawer>();

        public static TourItemDrawer selectedDrawer; 

        private TourItemDrawer startConnectionItem;
        private Tour tour;
        private bool isDragged;
        private bool isFirstCheck = true;
        private bool showHelp = true;

        public static GUIStyle hugeWhiteLabelStyle
        {
            get
            {
                if (_hugeWhiteLabelStyle == null)
                {
                    _hugeWhiteLabelStyle = new GUIStyle(GUI.skin.label)
                    {
                        fontSize = 32,
                        normal =
                        {
                            textColor = Color.white
                        },
                        alignment = TextAnchor.MiddleCenter
                    };
                }

                return _hugeWhiteLabelStyle;
            }
        }

        public static GUIStyle helpStyle
        {
            get
            {
                if (_helpStyle == null)
                {
                    Texture2D t = EditorUtils.LoadAsset<Texture2D>("Textures/Editor/helpbox.png");
                    _helpStyle = new GUIStyle
                    {
                        normal =
                        {
                            background = t
                        }, 
                        border = new RectOffset(4, 4, 4, 4)
                    };
                }

                return _helpStyle;
            }
        }

        private bool CheckTour()
        {
            if (tour == null || tour.gameObject == null)
            {
                if (isFirstCheck)
                {
                    tour = FindObjectOfType<Tour>();
                    if (tour == null) isFirstCheck = false;
                    else
                    {
                        isFirstCheck = true;
                        return true;
                    }
                }

                DrawCreateTour();
                return false;
            }

            return true;
        }

        private TourItemDrawer CreateItemDrawer(TourItem item)
        {
            item.tour = tour;
            TourItemDrawer drawer = new TourItemDrawer(this, item);
            drawers.Add(drawer);
            drawer.OnSelect += OnItemSelect;
            drawer.OnRightMouseDown += OnStartConnection;
            drawer.OnRightMouseUp += OnStopConnection;
            drawer.OnRemove += OnItemRemove;

            return drawer;
        }

        private void CreateTour()
        {
            GameObject go = new GameObject("uPano Tour");
            tour = go.AddComponent<Tour>();
            GlobalSettings globalSettings = go.AddComponent<GlobalSettings>();
            globalSettings.defaultHotSpotPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(EditorUtils.assetPath + "\\Examples\\Prefabs\\Hot Spots\\Hot Spot Arrow.prefab");
            globalSettings.defaultDirectionPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(EditorUtils.assetPath + "\\Examples\\Prefabs\\Directions\\Direction Arrow.prefab");
            globalSettings.beforeTransitionPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(EditorUtils.assetPath + "\\Examples\\Transitions\\ExampleFadeOut.prefab");
            globalSettings.afterTransitionPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(EditorUtils.assetPath + "\\Examples\\Transitions\\ExampleFadeIn.prefab");
            go.AddComponent<MouseControl>();
            go.AddComponent<KeyboardControl>();
            go.AddComponent<Limits>();
            HotSpotGlobalActions hsga = go.AddComponent<HotSpotGlobalActions>();
            DirectionGlobalActions dga = go.AddComponent<DirectionGlobalActions>();

            GameObject events = new GameObject("Events");
            events.transform.parent = go.transform;

            GameObject enterGO = new GameObject("Pointer Enter");
            enterGO.transform.parent = events.transform;
            SetScale enterScale = enterGO.AddComponent<SetScale>();
            enterScale.scale = new Vector3(1.2f, 1.2f, 1.2f);
            UnityEventTools.AddPersistentListener(hsga.OnPointerEnter, enterScale.Invoke);
            UnityEventTools.AddPersistentListener(dga.OnPointerEnter, enterScale.Invoke);

            GameObject exitGO = new GameObject("Pointer Exit");
            exitGO.transform.parent = events.transform;
            SetScale exitScale = exitGO.AddComponent<SetScale>();
            UnityEventTools.AddPersistentListener(hsga.OnPointerExit, exitScale.Invoke);
            UnityEventTools.AddPersistentListener(dga.OnPointerExit, exitScale.Invoke);

            GameObject tooltip = new GameObject("Show Tooltip");
            tooltip.transform.parent = events.transform;
            ShowTooltip showTooltip = tooltip.AddComponent<ShowTooltip>();
            showTooltip.tooltipPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(EditorUtils.assetPath + "\\Examples\\Prefabs\\Tooltips\\Tooltip.prefab");
            showTooltip.getTextFromElement = true;
            UnityEventTools.AddPersistentListener(hsga.OnPointerEnter, showTooltip.Invoke);
            UnityEventTools.AddPersistentListener(hsga.OnPointerExit, showTooltip.Hide);
        }

        private void DrawConnections()
        {
            Handles.color = Color.gray;
            foreach (TourItemDrawer item in drawers)
            {
                if (item != selectedDrawer) item.DrawConnections();
            }
            if (selectedDrawer != null) selectedDrawer.DrawConnections();
        }

        private void DrawCreateTour()
        {
            EditorGUI.LabelField(new Rect(position.width / 2 - 150, position.height / 2 - 50, 300, 100), "No Tour", hugeWhiteLabelStyle);
            if (GUI.Button(new Rect(position.width / 2 - 150, position.height / 2 + 50, 300, 50), "Create a new tour"))
            {
                CreateTour();
                isFirstCheck = true;
            }
        }

        private void DrawHelp()
        {
            float helpWidth = 280;
            float helpHeight = 130;
            GUILayout.BeginArea(new Rect(position.width - helpWidth - 1, 22, helpWidth, helpHeight), helpStyle);

            GUILayout.Label("Drag-and-drop panorama images from Project into this window.", EditorStyles.wordWrappedLabel);
            GUILayout.Label("Hold the left mouse button to move the view or elements.", EditorStyles.wordWrappedLabel);
            GUILayout.Label("Hold the mouse wheel to zoom view.", EditorStyles.wordWrappedLabel);
            GUILayout.Label("Hold the right mouse button and drag from one item to another to connect them.", EditorStyles.wordWrappedLabel);

            GUILayout.EndArea();
        }

        private bool DrawItem(TourItemDrawer drawer)
        {
            if (drawer.item == null || drawer.item.gameObject == null)
            {
                tour.items.Remove(drawer.item);
                drawers.Remove(drawer);
                return false;
            }

            if (drawer.item.tour == null) drawer.item.tour = tour;

            drawer.Draw(tour.center, tour.scale, position.size);
            return true;
        }

        private void DrawItems()
        {
            for (int i = 0; i < drawers.Count; i++)
            {
                TourItemDrawer drawer = drawers[i];
                if (selectedDrawer != drawer)
                {
                    if (!DrawItem(drawer)) i--;
                }
            }

            if (selectedDrawer != null) DrawItem(selectedDrawer);
        }

        private void DrawNewConnection()
        {
            if (startConnectionItem == null) return;

            Handles.color = Color.green;

            Handles.DrawAAPolyLine(
                4,
                startConnectionItem.viewRect.center,
                Event.current.mousePosition
            );

            GUI.changed = true;
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button("Fit view", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) FitView();

            if (GUILayout.Button("Reset view", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
                Undo.RecordObject(tour, "Reset Tour View");

                tour.scale = 1;
                tour.center = Vector2.zero;
            }

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            showHelp = GUILayout.Toggle(showHelp, "Help", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));

            if (EditorGUI.EndChangeCheck()) EditorPrefs.SetBool("uPano.TourMaker.ShowHelp", showHelp);

            if (showHelp) DrawHelp();

            EditorGUILayout.EndHorizontal();
        }

        private static void FreeDrawers()
        {
            if (drawers == null)
            {
                drawers = new List<TourItemDrawer>();
                return;
            }

            selectedDrawer = null;

            foreach (TourItemDrawer drawer in drawers) drawer.Dispose();
            drawers.Clear();
        }

        private void FitView()
        {
            Undo.RecordObject(tour, "Fit Tour View");

            if (drawers.Count == 0)
            {
                tour.scale = 1;
                tour.center = Vector2.zero;
                return;
            }

            Rect r = drawers[0].rect;

            for (int i = 1; i < drawers.Count; i++)
            {
                Rect v = drawers[i].rect;
                if (r.xMin > v.xMin) r.xMin = v.xMin;
                if (r.xMax < v.xMax) r.xMax = v.xMax;
                if (r.yMin > v.yMin) r.yMin = v.yMin;
                if (r.yMax < v.yMax) r.yMax = v.yMax;
            }

            tour.center = -r.center;

            Vector2 s = r.size;
            float cx = (position.width - 30) / s.x;
            float cy = (position.height - 30) / s.y;

            tour.scale = Mathf.Min(cx, cy);
        }

        public static TourItemDrawer GetDrawer(TourItem item)
        {
            foreach (TourItemDrawer d in drawers)
            {
                if (d.item == item) return d;
            }

            return null;
        }

        private Vector2 GetPositionUnderCursor(Vector2 mousePosition)
        {
            return (mousePosition - position.size / 2) / tour.scale - tour.center;
        }

        public void InitItems()
        {
            TourItem selectedItem = selectedDrawer != null ? selectedDrawer.item : null;

            FreeDrawers();

            if (tour != null)
            {
                tour.ClearItems();

                foreach (TourItem item in tour.items)
                {
                    item.Init();
                    TourItemDrawer drawer = CreateItemDrawer(item);
                    if (selectedItem == item) selectedDrawer = drawer;
                }
            }

            Repaint();
        }

        private void OnDestroy()
        {
            Undo.undoRedoPerformed -= Repaint;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnDrag(Vector2 delta)
        {
            if (focusedWindow != this) return;

            Undo.RecordObject(tour, "Change Tour View");

            isDragged = true;
            tour.center += delta / tour.scale;
            GUI.changed = true;
        }

        private void OnEnable()
        {
            instance = this;
            
            tour = FindObjectOfType<Tour>();

            FreeDrawers();

            if (tour != null)
            {
                selectedDrawer = null;
                tour.ClearItems();

                foreach (TourItem item in tour.items)
                {
                    item.Init();
                    TourItemDrawer drawer = CreateItemDrawer(item);
                    if (Selection.activeGameObject == item.gameObject)
                    {
                        selectedDrawer = drawer;
                    }
                }
            }

            showHelp = EditorPrefs.GetBool("uPano.TourMaker.ShowHelp", true);

            Selection.selectionChanged -= OnSelectionChanged;
            Selection.selectionChanged += OnSelectionChanged;

            Undo.undoRedoPerformed -= InitItems;
            Undo.undoRedoPerformed += InitItems;

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnFocus()
        {
            if (tour == null || drawers == null) OnEnable();
        }

        private void OnGUI()
        {
            bool hasTour = CheckTour();
            if (hasTour)
            {
                Color oldColor = Handles.color;

                GridDrawer.Draw(position, tour.center, tour.scale);

                DrawNewConnection();
                DrawConnections();
                DrawItems();

                Handles.color = oldColor;
            }

            DrawToolbar();

            if (hasTour)
            {
                ProcessItemsEvents();
                ProcessEvents();
            }

            if (GUI.changed) Repaint();
        }

        private void OnItemRemove(TourItemDrawer drawer)
        {
            Undo.SetCurrentGroupName("Remove Tour Item");
            int group = Undo.GetCurrentGroup();

            Undo.RecordObject(tour, "Remove Tour Item");

            tour.items.Remove(drawer.item);
            drawers.Remove(drawer);
            drawer.Destroy();

            Undo.CollapseUndoOperations(group);

            if (selectedDrawer == drawer) selectedDrawer = null;
            if (tour.startItem == drawer.item)
            {
                if (tour.items.Count > 0) tour.startItem = tour.items[0];
            }
        }

        private void OnItemSelect(TourItemDrawer drawer)
        {
            if (drawer != null) Selection.activeGameObject = drawer.item.gameObject;
            else Selection.activeGameObject = null;

            GUI.changed = true;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange mode)
        {
            if (mode == PlayModeStateChange.EnteredPlayMode || mode == PlayModeStateChange.EnteredEditMode)
            {
                OnEnable();
                Repaint();
            }
        }

        private void OnScrollWheel(Event e)
        {
            Undo.RecordObject(tour, "Scroll Tour View");

            float oldScale = tour.scale;
            tour.scale = Mathf.Clamp(tour.scale * (1 - e.delta.y / 100), 0.1f, 10);

            float scaleDelta = tour.scale - oldScale;
            if (Math.Abs(scaleDelta) < float.Epsilon) return;

            Vector2 p1 = (e.mousePosition - position.size / 2) / oldScale;
            Vector2 p2 = (e.mousePosition - position.size / 2) / tour.scale;

            tour.center += p2 - p1;

            GUI.changed = true;
        }

        private void OnSelectionChanged()
        {
            if (tour == null)
            {
                if (Selection.activeGameObject != null)
                {
                    tour = Selection.activeGameObject.GetComponent<Tour>();
                    if (tour != null) InitItems();
                }
            }
            UpdateSelectedItem();
            Repaint();
        }

        private void OnStartConnection(TourItemDrawer item)
        {
            startConnectionItem = item;
        }

        private void OnStopConnection(TourItemDrawer item)
        {
            if (startConnectionItem == item)
            {
                item.ProcessContextMenu();
            }
            else if (startConnectionItem != null)
            {
                InteractiveElements.VisualInteractiveElementEditor.OpenWindow(startConnectionItem.item, item.item);
            }

            startConnectionItem = null;
        }

        [MenuItem(EditorUtils.MENU_PATH + "Tour Maker", false, 1)]
        public static void OpenWindow()
        {
            GetWindow<TourMaker>("Tour Maker");
        }

        private void ProcessContextMenu(Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Add item"), false, () =>
            {
                TourItem item = TourItem.Create(tour);
                item.position = GetPositionUnderCursor(mousePosition);
                Undo.RegisterCreatedObjectUndo(item.gameObject, "Create Tour Item");
                tour.items.Add(item);
                if (tour.items.Count == 1) tour.startItem = item;
                TourItemDrawer drawer = CreateItemDrawer(item);
                OnItemSelect(drawer);
            });
            genericMenu.ShowAsContext();
        }

        private void ProcessEvents()
        {
            Event e = Event.current;

            if (e.type == EventType.MouseDown)
            {
                if (e.button == 0 || e.button == 2)
                {
                    isDragged = false;
                }
                if (e.button == 1)
                {
                    ProcessContextMenu(e.mousePosition);
                }
            }
            else if (e.type == EventType.MouseUp)
            {
                if (e.button == 0)
                {
                    if (!isDragged && selectedDrawer != null && focusedWindow == this)
                    {
                        selectedDrawer = null;
                        Selection.activeGameObject = tour.gameObject;
                        Repaint();
                    }
                }
                else if (e.button == 1)
                {
                    startConnectionItem = null;
                }
            }
            else if (e.type == EventType.MouseDrag)
            {
                if (e.button == 0 || e.button == 2) OnDrag(e.delta);
            }
            else if (e.type == EventType.ScrollWheel) OnScrollWheel(e);
            else if (e.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                TourItemDrawer lastDrawer = null;

                bool createNewItems = DragAndDrop.objectReferences.Length > 1 && !(DragAndDrop.objectReferences[0] is Texture);

                if (!createNewItems)
                {
                    bool hasDrawer = false;
                    foreach (TourItemDrawer drawer in drawers)
                    {
                        if (drawer.viewRect.Contains(e.mousePosition))
                        {
                            Undo.RecordObject(drawer.item.panoRenderer, "Set Item Texture");
                            drawer.item.texture = DragAndDrop.objectReferences[0] as Texture;
                            hasDrawer = true;
                            break;
                        }
                    }

                    if (!hasDrawer) createNewItems = true;
                }
                
                if (createNewItems)
                {
                    List<TourItemDrawer> createdItems = new List<TourItemDrawer>();
                    foreach (Object obj in DragAndDrop.objectReferences)
                    {
                        if (obj is Texture == false) continue;

                        TourItem item = TourItem.Create(tour, obj as Texture);
                        item.position = GetPositionUnderCursor(e.mousePosition);
                        Undo.RegisterCreatedObjectUndo(item.gameObject, "Create Tour Item");
                        tour.items.Add(item);
                        if (tour.items.Count == 1) tour.startItem = item;
                        lastDrawer = CreateItemDrawer(item);
                        createdItems.Add(lastDrawer);
                    }

                    if (createdItems.Count > 1)
                    {
                        float a = 360f / createdItems.Count;
                        float r = 150;

                        for (int i = 0; i < createdItems.Count; i++)
                        {
                            float ca = a * i * Mathf.Deg2Rad;
                            Vector2 offset = new Vector2(Mathf.Cos(ca) * r, Mathf.Sin(ca) * r);
                            createdItems[i].item.position += offset;
                            createdItems[i].rect.position += offset;
                        }
                    }
                    OnItemSelect(lastDrawer);
                }

                e.Use();
                Repaint();
            }
            else if (e.type == EventType.DragUpdated)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            }
            else if (e.type == EventType.KeyUp)
            {
                if (e.keyCode == KeyCode.Delete || e.keyCode == KeyCode.Backspace)
                {
                    if (selectedDrawer != null)
                    {
                        OnItemRemove(selectedDrawer);
                        GUI.changed = true;
                    }
                }
            }
        }

        private void ProcessItemsEvents()
        {
            foreach (TourItemDrawer item in drawers)
            {
                item.ProcessEvents(tour.scale);
            }
        }

        private void UpdateSelectedItem()
        {
            if (Selection.activeGameObject == null) return;

            TourItem tourItem = Selection.activeGameObject.GetComponent<TourItem>();
            if (tourItem == null) return;

            selectedDrawer = GetDrawer(tourItem);
        }
    }
}