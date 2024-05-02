/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.InteractiveElements;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors.InteractiveElements
{
    public abstract class PrefabElementManagerEditor : InteractiveElementManagerEditor
    {
        protected SerializedProperty defaultPrefab;

        protected IPrefabElementList prefabManager
        {
            get { return manager as IPrefabElementList; }
        }

        protected override void CacheSerializedFields()
        {
            defaultPrefab = FindProperty("_defaultPrefab");
            base.CacheSerializedFields();
        }

        protected override void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            PropertyField(defaultPrefab);
            if (EditorGUI.EndChangeCheck())
            {
                if (Application.isPlaying)
                {
                    serializedObject.ApplyModifiedProperties();
                    for (int i = 0; i < manager.Count; i++)
                    {
                        PrefabElement el = prefabManager.GetItemAt(i);
                        if (el.prefab == null) el.Reinit();
                    }
                }
            }

            EditorGUILayout.Space();
            DrawListHeader();
            DrawItems();
        }
    }
}