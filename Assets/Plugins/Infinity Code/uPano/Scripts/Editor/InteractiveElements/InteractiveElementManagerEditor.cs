/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Editors.VisualEditors.InteractiveElements;
using InfinityCode.uPano.InteractiveElements;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors.InteractiveElements
{
    public abstract class InteractiveElementManagerEditor : SerializedEditor
    {
        protected SerializedProperty items;
        protected GUIStyle itemStyle;
        protected int removeItemAt = -1;
        private GUIStyle foldoutStyle;

        protected abstract IInteractiveElementList manager { get; }

        protected override void CacheSerializedFields()
        {
            items = FindProperty("_items");

            itemStyle = new GUIStyle();
            Texture2D t = itemStyle.normal.background = new Texture2D(1, 4);
            t.SetPixel(0, 0, new Color(0, 0, 0, 0.2f));
            t.SetPixel(0, 1, new Color(0, 0, 0, 0.1f));
            t.SetPixel(0, 2, new Color(1, 1, 1, 0f));
            t.SetPixel(0, 3, new Color(1, 1, 1, 0f));
            t.Apply();
            itemStyle.border = new RectOffset(0, 0, 1, 2);
        }

        protected abstract void CreateItem();

        protected void DrawItem(SerializedProperty item, int index)
        {
            SerializedProperty expandedProperty = item.FindPropertyRelative("_expanded");

            bool isExpanded = expandedProperty.boolValue;

            int titleHeight = isExpanded? 20: 24;
            EditorGUILayout.BeginHorizontal(GUILayout.Height(titleHeight));

            foldoutStyle.fixedHeight = 20;

            expandedProperty.boolValue = GUILayout.Toggle(expandedProperty.boolValue, "", foldoutStyle, GUILayout.ExpandWidth(false));

            if (isExpanded)
            {
                float oldLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth -= 15;
                PropertyField(item.FindPropertyRelative("_title"));
                EditorGUIUtility.labelWidth = oldLabelWidth;
            }
            else
            {
                string title = item.FindPropertyRelative("_title").stringValue;
                EditorGUILayout.LabelField(!string.IsNullOrEmpty(title) ? title : (index + 1).ToString());
            }

            if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
            {
                removeItemAt = index;
            }

            EditorGUILayout.EndHorizontal();

            if (!expandedProperty.boolValue) return;

            EditorGUI.indentLevel++;
            DrawItemContent(item, index);

            DrawQuickActions(item);
            DrawItemEvents(item);

            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
        }

        private void DrawQuickActions(SerializedProperty item)
        {
            SerializedProperty expandedQuickActions = item.FindPropertyRelative("_expandedQuickActions");
            expandedQuickActions.boolValue = EditorGUILayout.Foldout(expandedQuickActions.boolValue, "Quick Actions");
            if (!expandedQuickActions.boolValue) return;

            EditorGUI.indentLevel++;
            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 180;
            DrawQuickActionsContent(item);
            EditorGUIUtility.labelWidth = oldLabelWidth;
            EditorGUI.indentLevel--;
        }

        protected virtual void DrawQuickActionsContent(SerializedProperty item)
        {
            SerializedProperty loadPanoPrefab = item.FindPropertyRelative("loadPanoramaPrefab");
            PropertyField(loadPanoPrefab);

            SerializedProperty targetPanorama = item.FindPropertyRelative("switchToPanorama");
            PropertyField(targetPanorama);

            if (loadPanoPrefab.objectReferenceValue == null && targetPanorama.objectReferenceValue == null) return;

            if (targetPanorama.objectReferenceValue != null) PropertyField(item.FindPropertyRelative("copyPanTilt"));

            PropertyField(item.FindPropertyRelative("beforeTransitionPrefab"));
            PropertyField(item.FindPropertyRelative("afterTransitionPrefab"));
        }

        protected abstract void DrawItemContent(SerializedProperty item, int index);

        private void DrawItemEvents(SerializedProperty item)
        {
            SerializedProperty expandedEvents = item.FindPropertyRelative("_expandedEvents");
            expandedEvents.boolValue = EditorGUILayout.Foldout(expandedEvents.boolValue, "Events");
            if (!expandedEvents.boolValue) return;

            EditorGUIUtility.labelWidth += 30;
            PropertyField(item.FindPropertyRelative("ignoreGlobalActions"));
            EditorGUIUtility.labelWidth -= 30;

            PropertyField(item.FindPropertyRelative("OnClick"));
            PropertyField(item.FindPropertyRelative("OnPointerDown"));
            PropertyField(item.FindPropertyRelative("OnPointerUp"));
            PropertyField(item.FindPropertyRelative("OnPointerEnter"));
            PropertyField(item.FindPropertyRelative("OnPointerExit"));
        }

        protected void DrawItems()
        {
            if (foldoutStyle == null) foldoutStyle = new GUIStyle(EditorStyles.foldout);

            if (items.arraySize == 0)
            {
                EditorGUILayout.LabelField("No items.");
                return;
            }

            removeItemAt = -1;
            for (int i = 0; i < items.arraySize; i++)
            {
                EditorGUILayout.BeginVertical(itemStyle);
                DrawItem(items.GetArrayElementAtIndex(i), i);
                EditorGUILayout.EndVertical();
            }

            if (removeItemAt != -1) RemoveItemAt(removeItemAt);
        }

        protected void DrawListHeader()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
                CreateItem();
                serializedObject.Update();
            }

            EditorGUILayout.LabelField(" Total: " + items.arraySize, GUILayout.MaxWidth(80));

            EditorGUILayout.Space();
            DrawListHeaderCenter();

            if (GUILayout.Button("Collapse all", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
                for (int i = 0; i < items.arraySize; i++)
                {
                    SerializedProperty item = items.GetArrayElementAtIndex(i);
                    item.FindPropertyRelative("_expanded").boolValue = false;
                    item.FindPropertyRelative("_expandedEvents").boolValue = false;
                }
            }

            

            EditorGUILayout.EndHorizontal();
        }

        protected virtual void DrawListHeaderCenter()
        {
            if (GUILayout.Button("Visual Editor", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
                VisualInteractiveElementEditor.OpenWindow();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            manager.GetItems();
        }

        protected override void OnGUI()
        {
            DrawListHeader();
            DrawItems();
        }

        protected abstract void RemoveItemAt(int index);
    }
}