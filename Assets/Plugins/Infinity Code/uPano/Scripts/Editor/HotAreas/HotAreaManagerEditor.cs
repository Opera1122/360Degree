/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Editors.InteractiveElements;
using InfinityCode.uPano.HotAreas;
using InfinityCode.uPano.InteractiveElements;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors.HotAreas
{
    [CustomEditor(typeof(HotAreaManager))]
    public class HotAreaManagerEditor : InteractiveElementManagerEditor
    {
        private HotAreaManager _manager;

        protected override IInteractiveElementList manager
        {
            get { return _manager; }
        }

        protected override void CreateItem()
        {
            _manager.Create();
        }

        protected override void DrawQuickActionsContent(SerializedProperty item)
        {
            base.DrawQuickActionsContent(item);

            SerializedProperty tooltip = item.FindPropertyRelative("tooltip");
            if (tooltip != null)
            {
                PropertyField(tooltip);
                if (!string.IsNullOrEmpty(tooltip.stringValue))
                {
                    EditorGUI.indentLevel++;
                    PropertyField(item.FindPropertyRelative("tooltipAction"));
                    PropertyField(item.FindPropertyRelative("tooltipPrefab"));
                    EditorGUI.indentLevel--;
                }
            }
        }

        protected override void DrawItemContent(SerializedProperty item, int index)
        {
            SerializedProperty pointsProperty = item.FindPropertyRelative("points");
            PropertyField(pointsProperty);

            SerializedProperty colorProperty = item.FindPropertyRelative("color");
            PropertyField(colorProperty);
        }

        protected override void OnEnable()
        {
            _manager = (HotAreaManager)target;
            base.OnEnable();
        }

        protected override void RemoveItemAt(int index)
        {
            if (!Application.isPlaying) items.DeleteArrayElementAtIndex(index);
            _manager.RemoveAt(index);
        }
    }
}