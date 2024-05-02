/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Directions;
using InfinityCode.uPano.Editors.Utils;
using InfinityCode.uPano.Editors.VisualEditors.Tours;
using InfinityCode.uPano.HotAreas;
using InfinityCode.uPano.HotSpots;
using InfinityCode.uPano.InteractiveElements;
using InfinityCode.uPano.Plugins;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors.VisualEditors.InteractiveElements
{
    public partial class VisualInteractiveElementEditor
    {
        internal static InteractiveElement activeElement;

        internal static DirectionManager directionManager;
        internal static HotAreaManager hotAreaManager;
        internal static HotSpotManager hotSpotManager;
        internal static SerializedObject serializedDirectionManager;
        internal static SerializedProperty serializedDirectionItems;
        internal static SerializedObject serializedHotAreaManager;
        internal static SerializedProperty serializedHotAreaItems;
        internal static SerializedObject serializedHotSpotManager;
        internal static SerializedProperty serializedHotSpotItems;

        internal static Texture hotSpotIcon;
        internal static Texture hotSpotSelectedIcon;

        private static GUIStyle _missedPrefabStyle;
        private static GUIStyle _normalPrefabStyle;

        internal InteractiveElement dragElement;
        internal int dragControlPoint = -1;

        public GUIStyle missedPrefabStyle
        {
            get
            {
                if (_missedPrefabStyle == null)
                {
                    _missedPrefabStyle = new GUIStyle(normalPrefabStyle)
                    {
                        normal =
                        {
                            textColor = Color.red
                        }
                    };
                }

                return _missedPrefabStyle;
            }
        }

        public GUIStyle normalPrefabStyle
        {
            get
            {
                if (_normalPrefabStyle == null)
                {
                    _normalPrefabStyle = new GUIStyle(TourMaker.helpStyle)
                    {
                        margin = new RectOffset(3, 3, 2, 2), 
                        padding = new RectOffset(4, 4, 3, 3), 
                        fontSize = 12, 
                        normal =
                        {
                            textColor = Color.white
                        }
                    };
                }

                return _normalPrefabStyle;
            }
        }

        private void AddActiveAreaPoint()
        {
            HotArea hotArea = activeElement as HotArea;
            if (hotArea == null) return;

            Vector2 mousePosition = Event.current.mousePosition;

            PanTilt panTilt = PointToPanTilt(mousePosition);
            hotArea.points.Add(panTilt);

            EditorUtils.SetDirty(hotAreaManager);
        }

        private void CreateDirectionFromContextMenu(Vector2 mousePosition)
        {
            float pan, tilt;
            PointToPanTilt(mousePosition, out pan, out tilt);

            Undo.RecordObject(directionManager, "Create Connection");

            Direction direction = directionManager.Create(pan, null);
            TrySetConnection(direction);
            EditorUtils.SetDirty(directionManager);

            SelectElement(direction);
        }

        private void CreateHotAreaFromContextMenu(Vector2 mousePosition)
        {
            Undo.RecordObject(hotAreaManager, "Create Hot Area");

            HotArea area = hotAreaManager.Create();
            TrySetConnection(area);
            EditorUtils.SetDirty(hotAreaManager);

            SelectElement(area);

            mode = Mode.createHotArea;
        }

        private void CreateHotSpotFromContextMenu(Vector2 mousePosition)
        {
            float pan, tilt;
            PointToPanTilt(mousePosition, out pan, out tilt);

            Undo.RecordObject(hotSpotManager, "Create Connection");

            HotSpot hotSpot = hotSpotManager.Create(pan, tilt, null);
            TrySetConnection(hotSpot);
            EditorUtils.SetDirty(hotSpotManager);

            SelectElement(hotSpot);
        }

        private void DrawConnectionHint()
        {
            DrawHint("Connection mode", "Right-click in the place where you want to create a Hot Spot,\nDirection or Hot Area.", 10);
        }

        private void DrawCreateHotAreaHint()
        {
            DrawHint("Create hot area mode", "Click to create a point");
        }

        private void DrawEditHotAreaHint()
        {
            DrawHint("Edit hot area mode", "Move the points. Hold CTRL to add a point. Hold ALT to delete a point.");
        }

        private void DrawElements()
        {
            if (Event.current.type != EventType.Repaint) return;

            foreach (HotArea area in hotAreaManager)
            {
                HotAreaNodeDrawer.Draw(this, area);
            }

            foreach (HotSpot spot in hotSpotManager)
            {
                PointNodeDrawer.Draw(this, spot);
            }

            foreach (Direction direction in directionManager)
            {
                PointNodeDrawer.Draw(this, direction);
            }
        }

        private void InitManager<T>(out T manager, out SerializedObject serializedObject, out SerializedProperty serializedProperty) where T: Plugin
        {
            manager = panoRenderer.GetComponent<T>();
            if (manager == null) manager = panoRenderer.gameObject.AddComponent<T>();

            serializedObject = new SerializedObject(manager);
            serializedProperty = serializedObject.FindProperty("_items");
        }

        private void InitManagers()
        {
            InitManager(out hotSpotManager, out serializedHotSpotManager, out serializedHotSpotItems);
            InitManager(out directionManager, out serializedDirectionManager, out serializedDirectionItems);
            InitManager(out hotAreaManager, out serializedHotAreaManager, out serializedHotAreaItems);
        }

        private void OnSelectionChanged()
        {
            InitPanoRenderer();
            Redraw();
        }

        public void ProcessElementContextMenu(InteractiveElement element)
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Remove"), false, RemoveElementFromContextMenu, element);
            genericMenu.ShowAsContext();
        }

        private void ProcessElementEvents()
        {
            Event e = Event.current;
            if (e.type == EventType.Used || e.type == EventType.Repaint || e.type == EventType.Layout) return;

            foreach (HotSpot spot in hotSpotManager)
            {
                PointNodeDrawer.ProcessEvents(this, spot);
            }

            if (e.type == EventType.Used) return;

            foreach (Direction direction in directionManager)
            {
                PointNodeDrawer.ProcessEvents(this, direction);
            }

            if (e.type == EventType.Used) return;

            HotArea hotArea = activeElement as HotArea;
            if (hotArea != null) HotAreaNodeDrawer.ProcessEvents(this, hotArea);

            foreach (HotArea area in hotAreaManager)
            {
                if (activeElement != area) HotAreaNodeDrawer.ProcessEvents(this, area);
            }
        }

        private void RemoveActiveElement()
        {
            HotSpot hotSpot = activeElement as HotSpot;
            Direction direction = activeElement as Direction;
            HotArea hotArea = activeElement as HotArea;

            if (hotSpot != null)
            {
                hotSpotManager.Remove(hotSpot);
                EditorUtils.SetDirty(hotSpotManager);
            }
            else if (direction != null)
            {
                directionManager.Remove(direction);
                EditorUtils.SetDirty(directionManager);
            }
            else if (hotArea != null)
            {
                hotAreaManager.Remove(hotArea);
                EditorUtils.SetDirty(hotAreaManager);
            }

            if (TourMaker.instance != null) TourMaker.instance.InitItems();
        }

        private void RemoveElementFromContextMenu(object userdata)
        {
            InteractiveElement element = userdata as InteractiveElement;

            if (activeElement == element) activeElement = null;

            HotSpot hotSpot = activeElement as HotSpot;
            Direction direction = activeElement as Direction;
            HotArea area = activeElement as HotArea;

            if (hotSpot != null)
            {
                hotSpotManager.Remove(hotSpot);
                EditorUtils.SetDirty(hotSpotManager);
            }
            else if (direction != null)
            {
                directionManager.Remove(direction);
                EditorUtils.SetDirty(directionManager);
            }
            else if (area != null)
            {
                hotAreaManager.Remove(area);
                mode = Mode.idle;
                EditorUtils.SetDirty(hotAreaManager);
            }

            if (TourMaker.instance != null) TourMaker.instance.InitItems();
        }

        public void SelectElement(InteractiveElement element)
        {
            if (activeElement == element) return;

            activeElement = element;
            InteractiveElementSettings.OpenWindow();
            wnd.Focus();
            GUI.changed = true;

            if (activeElement is HotArea) mode = Mode.editHotArea;
            else mode = Mode.idle;
        }
    }
}