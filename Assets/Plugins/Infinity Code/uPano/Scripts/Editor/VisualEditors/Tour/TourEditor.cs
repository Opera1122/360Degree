/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.Controls;
using InfinityCode.uPano.Editors.Utils;
using InfinityCode.uPano.Enums;
using InfinityCode.uPano.Plugins;
using InfinityCode.uPano.Tours;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace InfinityCode.uPano.Editors.VisualEditors.Tours
{
    [CustomEditor(typeof(Tour))]
    public class TourEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Tour Maker")) TourMaker.OpenWindow();

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            SerializedProperty typeProp = serializedObject.FindProperty("preset");
            EditorGUILayout.PropertyField(typeProp);
            if (EditorGUI.EndChangeCheck())
            {
                if (typeProp.enumValueIndex == (int)TourPreset.standard) SetStandard();
                else SetGoogleVR();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void SetStandard()
        {
            Tour tour = target as Tour;
            TourItem[] items = tour.GetComponentsInChildren<TourItem>(true);
            foreach (TourItem item in items)
            {
                Pano pano = item.GetComponent<Pano>();
                pano.cameraType = Pano.CameraType.createNew;
                pano.existingCamera = null;
                pano.addPhysicsRaycaster = true;
                pano.rotationMode = Pano.RotationMode.rotateCamera;
            }

            KeyboardControl keyboardControl = tour.GetComponent<KeyboardControl>();
            if (keyboardControl == null) tour.gameObject.AddComponent<KeyboardControl>();

            MouseControl mouseControl = tour.GetComponent<MouseControl>();
            if (mouseControl == null) tour.gameObject.AddComponent<MouseControl>();

            TimedGaze gaze = tour.GetComponent<TimedGaze>();
            if (gaze != null)
            {
                if (gaze.gazeCircle != null)
                {
                    if (gaze.gazeCircle.transform.parent.childCount == 1) DestroyImmediate(gaze.gazeCircle.transform.parent.gameObject);
                    else DestroyImmediate(gaze.gazeCircle);
                }
                DestroyImmediate(gaze);
            }
        }

        private void SetGoogleVR()
        {
            Tour tour = target as Tour;
            TourItem[] items = tour.GetComponentsInChildren<TourItem>(true);
            foreach (TourItem item in items)
            {
                Pano pano = item.GetComponent<Pano>();
                pano.cameraType = Pano.CameraType.existing;
                pano.existingCamera = Camera.main;
                pano.addPhysicsRaycaster = false;
                pano.rotationMode = Pano.RotationMode.rotateGameObject;
            }

            KeyboardControl keyboardControl = tour.GetComponent<KeyboardControl>();
            if (keyboardControl != null) DestroyImmediate(keyboardControl);

            MouseControl mouseControl = tour.GetComponent<MouseControl>();
            if (mouseControl != null) DestroyImmediate(mouseControl);

            TimedGaze gaze = tour.GetComponent<TimedGaze>();
            if (gaze == null)
            {
                gaze = tour.gameObject.AddComponent<TimedGaze>();
                GameObject canvasGO = new GameObject("World Canvas");
                canvasGO.transform.SetSiblingIndex(0);
                canvasGO.layer = LayerMask.NameToLayer("UI");
                Canvas worldCanvas = canvasGO.AddComponent<Canvas>();
                worldCanvas.renderMode = RenderMode.WorldSpace;

                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(EditorUtils.assetPath + "\\Prefabs\\Gaze Circle.prefab");
                gaze.gazeCircle = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                gaze.gazeCircle.transform.SetParent(worldCanvas.transform);
                gaze.frontCircleImage = gaze.gazeCircle.transform.Find("Front Circle").GetComponent<Image>();

                Type type = Type.GetType("GvrReticlePointer, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
                if (type != null)
                {
                    Component obj = FindObjectOfType(type) as Component;
                    if (obj != null)
                    {
                        gaze.disableComponent = obj.GetComponent<MeshRenderer>();
                    }
                }
            }
        }
    }
}