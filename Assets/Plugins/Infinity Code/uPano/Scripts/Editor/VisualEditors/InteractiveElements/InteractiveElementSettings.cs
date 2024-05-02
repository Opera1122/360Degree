/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Directions;
using InfinityCode.uPano.Editors.Utils;
using InfinityCode.uPano.Editors.VisualEditors.Tours;
using InfinityCode.uPano.HotAreas;
using InfinityCode.uPano.HotSpots;
using InfinityCode.uPano.InteractiveElements;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors.VisualEditors.InteractiveElements
{
    public class InteractiveElementSettings : EditorWindow
    {
        internal static InteractiveElementSettings wnd;
        private Vector2 scrollPosition;
        private bool focusOnRepaint;

        public static void CloseWindow()
        {
            if (wnd != null) wnd.Close();
        }

        private void OnDestroy()
        {
            wnd = null;
        }

        private void OnEnable()
        {
            wnd = this;
        }

        private void OnGUI()
        {
            InteractiveElement element = VisualInteractiveElementEditor.activeElement;
            if (element == null) return;

            if (Event.current.type == EventType.Repaint && focusOnRepaint)
            {
                Focus();
                focusOnRepaint = false;
            }

            EditorGUI.BeginChangeCheck();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            if (element is HotSpot) DrawHotSpotSettings(element);
            else if (element is HotArea) DrawHotAreaSettings(element);
            else if (element is Direction) DrawDirectionSettings(element);

            EditorGUILayout.EndScrollView();

            if (EditorGUI.EndChangeCheck()) VisualInteractiveElementEditor.Redraw();
        }

        private void DrawDirectionSettings(InteractiveElement element)
        {
            Direction direction = element as Direction;
            int index = VisualInteractiveElementEditor.directionManager.IndexOf(direction);

            if (index == -1) return;

            VisualInteractiveElementEditor.serializedDirectionManager.Update();
            SerializedProperty item = VisualInteractiveElementEditor.serializedDirectionItems.GetArrayElementAtIndex(index);

            EditorGUILayout.PropertyField(item.FindPropertyRelative("_title"));

            EditorGUI.BeginChangeCheck();
            SerializedProperty prefabProperty = item.FindPropertyRelative("_prefab");
            EditorGUILayout.PropertyField(prefabProperty);
            if (EditorGUI.EndChangeCheck() && Application.isPlaying) direction.prefab = prefabProperty.objectReferenceValue as GameObject;

            EditorGUILayout.PropertyField(item.FindPropertyRelative("_pan"));

            EditorGUILayout.PropertyField(item.FindPropertyRelative("_scale"));

            DrawQuickActions(element, item);
            DrawEvents(element, item);

            VisualInteractiveElementEditor.serializedDirectionManager.ApplyModifiedProperties();
        }

        private void DrawHotAreaSettings(InteractiveElement element)
        {
            HotArea area = element as HotArea;
            int index = VisualInteractiveElementEditor.hotAreaManager.IndexOf(area);

            if (index == -1) return;

            VisualInteractiveElementEditor.serializedHotAreaManager.Update();
            SerializedProperty item = VisualInteractiveElementEditor.serializedHotAreaItems.GetArrayElementAtIndex(index);

            EditorGUILayout.PropertyField(item.FindPropertyRelative("_title"));

            EditorGUILayout.PropertyField(item.FindPropertyRelative("points"));
            EditorGUILayout.PropertyField(item.FindPropertyRelative("color"));

            DrawQuickActions(element, item);
            DrawEvents(element, item);

            VisualInteractiveElementEditor.serializedHotAreaManager.ApplyModifiedProperties();
        }

        private void DrawHotSpotSettings(InteractiveElement element)
        {
            HotSpot hotSpot = element as HotSpot;
            int index = VisualInteractiveElementEditor.hotSpotManager.IndexOf(hotSpot);

            if (index == -1 || VisualInteractiveElementEditor.serializedHotSpotManager == null) return;

            VisualInteractiveElementEditor.serializedHotSpotManager.Update();
            SerializedProperty item = VisualInteractiveElementEditor.serializedHotSpotItems.GetArrayElementAtIndex(index);

            EditorGUILayout.PropertyField(item.FindPropertyRelative("_title"));

            EditorGUI.BeginChangeCheck();
            SerializedProperty prefabProperty = item.FindPropertyRelative("_prefab");
            EditorGUILayout.PropertyField(prefabProperty);
            if (EditorGUI.EndChangeCheck() && Application.isPlaying) hotSpot.prefab = prefabProperty.objectReferenceValue as GameObject;

            EditorGUILayout.PropertyField(item.FindPropertyRelative("_pan"));
            EditorGUILayout.PropertyField(item.FindPropertyRelative("_tilt"));
            EditorGUILayout.PropertyField(item.FindPropertyRelative("_lookToCenter"));

            EditorGUI.BeginChangeCheck();
            Quaternion rotation = Quaternion.Euler(EditorGUILayout.Vector3Field("Rotation", hotSpot.rotation.eulerAngles));
            if (EditorGUI.EndChangeCheck())
            {
                hotSpot.rotation = rotation;
                VisualInteractiveElementEditor.serializedHotSpotManager.Update();
            }

            EditorGUILayout.PropertyField(item.FindPropertyRelative("_scale"));
            EditorGUILayout.PropertyField(item.FindPropertyRelative("_distanceMultiplier"));

            DrawQuickActions(element, item);
            DrawEvents(element, item);

            VisualInteractiveElementEditor.serializedHotSpotManager.ApplyModifiedProperties();
        }

        private static void DrawQuickActions(InteractiveElement element, SerializedProperty prop)
        {
            EditorUtils.GroupLabel("Quick Actions");

            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 180;

            DrawQuickActionTargetPanorama(element, prop);
            DrawQuickActionTooltip(element, prop);

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }

        private static void DrawQuickActionTooltip(InteractiveElement element, SerializedProperty prop)
        {
            if (VisualInteractiveElementEditor.activeElement != null) return;

            SerializedProperty tooltip = prop.FindPropertyRelative("tooltip");
            EditorGUILayout.PropertyField(tooltip);
            if (string.IsNullOrEmpty(tooltip.stringValue)) return;

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("tooltipAction"));
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("tooltipPrefab"));
            EditorGUI.indentLevel--;
        }

        private static void DrawQuickActionTargetPanorama(InteractiveElement element, SerializedProperty prop)
        {
            SerializedProperty loadPanoPrefab = prop.FindPropertyRelative("loadPanoramaPrefab");
            EditorGUILayout.PropertyField(loadPanoPrefab);

            SerializedProperty targetPano = prop.FindPropertyRelative("switchToPanorama");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(targetPano);
            if (EditorGUI.EndChangeCheck())
            {
                TourMaker maker = TourMaker.instance;
                
                if (maker != null)
                {
                    TourMaker.selectedDrawer.item.SetOutLink(element, targetPano.objectReferenceValue as GameObject);
                    maker.Repaint();
                }
            }

            if (loadPanoPrefab.objectReferenceValue == null && targetPano.objectReferenceValue == null) return;

            if (targetPano.objectReferenceValue != null)
            {
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("copyPanTilt"));
            }

            EditorGUILayout.PropertyField(prop.FindPropertyRelative("beforeTransitionPrefab"));
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("afterTransitionPrefab"));
        }

        private static void DrawEvents(InteractiveElement element, SerializedProperty prop)
        {
            EditorUtils.GroupLabel("Events");

            EditorGUILayout.PropertyField(prop.FindPropertyRelative("ignoreGlobalActions"));
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("OnClick"));
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("OnPointerDown"));
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("OnPointerUp"));
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("OnPointerEnter"));
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("OnPointerExit"));
        }

        internal static void OpenWindow()
        {
            InteractiveElementSettings window = GetWindow<InteractiveElementSettings>(true, "Interactive Element Settings", false);
            window.focusOnRepaint = true;
            window.Repaint();
        }

        internal static void Redraw()
        {
            if (wnd == null) return;

            if (VisualInteractiveElementEditor.activeElement == null) CloseWindow();
            else wnd.Repaint();
        }
    }
}