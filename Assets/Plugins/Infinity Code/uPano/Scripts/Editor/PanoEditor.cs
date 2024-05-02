/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using System.Collections.Generic;
using System.Linq;
using InfinityCode.uPano.Editors.Utils;
using InfinityCode.uPano.Editors.VisualEditors.InteractiveElements;
using InfinityCode.uPano.Editors.Windows;
using InfinityCode.uPano.HotSpots;
using InfinityCode.uPano.Renderers.Base;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors
{
    [CustomEditor(typeof(Pano))]
    public class PanoEditor : SerializedEditor
    {
        private Pano pano;

        private SerializedProperty pCameraCullingMask;
        private SerializedProperty pNewCameraProjection;
        private SerializedProperty pNewCameraDepth;
        private SerializedProperty pNewCameraDepthType;
        private SerializedProperty pCameraType;
        private SerializedProperty pCamera;
        private SerializedProperty pRotationMode;
        private GUIContent updateAvailableContent;
        private SerializedProperty pRotateGameObject;
        private SerializedProperty pAddPhysicsRaycaster;

        public static void AddCompilerDirective(string directive)
        {
            BuildTargetGroup[] targetGroups = (BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup));
            foreach (BuildTargetGroup g in targetGroups)
            {
                if (g == BuildTargetGroup.Unknown) continue;
                int ig = (int)g;
                if (ig == 2 ||
                    ig == 5 ||
                    ig == 6 ||
                    ig >= 15 && ig <= 18 ||
                    ig == 20 ||
                    ig >= 22 && ig <= 24 ||
                    ig == 26) continue;

                string currentDefinitions = PlayerSettings.GetScriptingDefineSymbolsForGroup(g);
                List<string> directives = currentDefinitions.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
                if (!directives.Contains(directive))
                {
                    directives.Add(directive);
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(g, string.Join(";", directives.ToArray()));
                }
            }
        }

        protected override void CacheSerializedFields()
        {
            pCameraCullingMask = FindProperty("_cameraCullingMask");
            pNewCameraProjection = FindProperty("_newCameraProjection");
            pNewCameraDepth = FindProperty("_newCameraDepth");
            pNewCameraDepthType = FindProperty("_newCameraDepthType");
            pCameraType = FindProperty("_cameraType");
            pCamera = FindProperty("_existingCamera");
            pRotationMode = FindProperty("_rotationMode");
            pRotateGameObject = FindProperty("_rotateGameObject");
            pAddPhysicsRaycaster = FindProperty("_addPhysicsRaycaster");
        }

        private void CameraGUI()
        {
            EditorUtils.GroupLabel("Camera Settings");

            PropertyField(pCameraType, "Camera");

            if (pCameraType.enumValueIndex == (int) Pano.CameraType.createNew)
            {
                PropertyField(pNewCameraDepthType, "Depth");
                if (pNewCameraDepthType.enumValueIndex == (int) Pano.NewCameraDepthType.manual) PropertyField(pNewCameraDepth, GUIContent.none);
                PropertyField(pCameraCullingMask, "Culling Mask");
                PropertyField(pNewCameraProjection, "Projection");
            }
            else
            {
                PropertyField(pCamera, GUIContent.none);
                EditorGUIUtility.labelWidth += 10;
                PropertyField(pAddPhysicsRaycaster);
                EditorGUIUtility.labelWidth -= 10;
                PropertyField(pRotationMode);
                if (pRotationMode.enumValueIndex == (int) Pano.RotationMode.rotateGameObject) PropertyField(pRotateGameObject);
            }

            EditorGUILayout.Space();
        }

        private void OnDestroy()
        {
            Pano.isPreview = false;

            if (!Application.isPlaying && pano != null)
            {
                PanoRenderer renderer = pano.GetComponent<PanoRenderer>();
                if (renderer != null) renderer.DestroyMesh();
                HotSpotManager hotSpotManager = pano.GetComponent<HotSpotManager>();
                if (hotSpotManager != null) hotSpotManager.StopPreview();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            pano = target as Pano;

            updateAvailableContent = new GUIContent("Update Available", EditorUtils.LoadAsset<Texture2D>("Icons\\update_available.png"), "Update Available");
            Updater.CheckNewVersionAvailable();

            Pano.isPreview = false;
            if (!Application.isPlaying && pano != null)
            {
                PanoRenderer renderer = pano.GetComponent<PanoRenderer>();
                if (renderer != null) renderer.DestroyMesh();

                HotSpotManager hotSpotManager = pano.GetComponent<HotSpotManager>();
                if (hotSpotManager != null) hotSpotManager.StopPreview();
            }

            VisualInteractiveElementEditor.Redraw();
        }

        protected override void OnGUI()
        {
            ToolbarGUI();
            CameraGUI();
            ViewingParamsGUI();
            PreviewGUI();
        }

        private void PlaymodeStateChanged(PlayModeStateChange state)
        {
            EditorApplication.playModeStateChanged -= PlaymodeStateChanged;
            PanoRenderer panoType = pano.GetComponent<PanoRenderer>();
            panoType.DestroyMesh();

            HotSpotManager hotSpotManager = pano.GetComponent<HotSpotManager>();
            if (hotSpotManager != null) hotSpotManager.StopPreview();

            Pano.isPreview = false;
        }

        private void PreviewGUI()
        {
            if (EditorApplication.isPlaying) return;

            GUIStyle previewStyle = new GUIStyle(GUI.skin.button);
            if (Pano.isPreview) previewStyle.normal = previewStyle.active;
            if (!GUILayout.Button("Preview", previewStyle)) return;

            Pano.isPreview = !Pano.isPreview;
            PanoRenderer renderer = pano.GetComponent<PanoRenderer>();
            if (renderer == null) return;

            HotSpotManager hotSpotManager = renderer.GetComponent<HotSpotManager>();
            if (Pano.isPreview)
            {
                renderer.CreateMesh();

                if (hotSpotManager != null) hotSpotManager.StartPreview(renderer);
                EditorApplication.playModeStateChanged += PlaymodeStateChanged;
            }
            else
            {
                renderer.DestroyMesh();
                if (hotSpotManager != null) hotSpotManager.StopPreview();
                EditorApplication.playModeStateChanged -= PlaymodeStateChanged;
            }
        }

        private void ToolbarGUI()
        {
            EditorGUILayout.Space();

            GUIStyle buttonStyle = new GUIStyle(EditorStyles.toolbarButton);

            GUILayout.BeginHorizontal();

            if (Updater.hasNewVersion)
            {
                Color defBackgroundColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(1, 0.5f, 0.5f);
                if (GUILayout.Button(updateAvailableContent, EditorStyles.toolbarButton))
                {
                    Updater.OpenWindow();
                }
                GUI.backgroundColor = defBackgroundColor;
            }
            else
            {
                if (GUILayout.Button("uPano v" + Pano.version, buttonStyle, GUILayout.ExpandWidth(true)))
                {
                    Updater.OpenWindow();
                }
            }

            if (GUILayout.Button("Help", buttonStyle, GUILayout.ExpandWidth(false)))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Product Page"), false, Links.OpenHomepage);
                menu.AddItem(new GUIContent("Documentation"), false, Links.OpenDocumentation);
                menu.AddItem(new GUIContent("API Reference"), false, Links.OpenAPIReference);
                menu.AddItem(new GUIContent("Support"), false, Links.OpenSupport);
                menu.AddItem(new GUIContent("Forum"), false, Links.OpenForum);
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Check Updates"), false, Updater.OpenWindow);
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("About"), false, About.OpenWindow);
                menu.ShowAsContext();
            }

            GUILayout.EndHorizontal();
        }

        private void ViewingParamsGUI()
        {
            EditorUtils.GroupLabel("Viewing Parameters");

            pano.northPan = EditorGUILayout.FloatField("North Pan", pano.northPan);
            pano.pan = EditorGUILayout.FloatField("Pan", pano.pan);
            pano.tilt = EditorGUILayout.FloatField("Tilt", pano.tilt);
            pano.fov = EditorGUILayout.FloatField("FOV", pano.fov);

            EditorGUILayout.Space();
        }
    }
}