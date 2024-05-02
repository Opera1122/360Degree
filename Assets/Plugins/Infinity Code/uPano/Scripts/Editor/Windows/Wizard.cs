/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InfinityCode.uPano.Attributes;
using InfinityCode.uPano.Controls;
using InfinityCode.uPano.Editors.Utils;
using InfinityCode.uPano.Plugins;
using InfinityCode.uPano.Renderers;
using InfinityCode.uPano.Renderers.Base;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors.Windows
{
    public class Wizard: EditorWindow
    {
        /* TYPES */
        private List<PanoRendererType> types;
        private List<PluginWrapper> controls;
        private List<PluginWrapper> plugins;
        private PanoRendererType activeType;
        private PanoRendererType<SphericalPanoRenderer> spherical;
        private PanoRendererType<SingleTextureCubeFacesPanoRenderer> singleTexture;
        private PanoRendererType<CubeFacesPanoRenderer> cubeFaces;
        private PanoRendererType<CubemapPanoRenderer> cubemapRenderer;
        private PanoRendererType<CylindricalPanoRenderer> cylindrical;

        /* WINDOW PROPS */
        private Vector2 scrollPosition;

        /* PANO PROPS */
        private Texture texture;
        private Shader shader;
        private Shader cubemapShader;
        private Material material;
        private Cubemap cubemap;
        private Texture2D[] textures;
        private RectUV uv = RectUV.full;
        private CubeUV singleTextureUV = new CubeUV(RotatableRectUV.full, RotatableRectUV.full, RotatableRectUV.full, RotatableRectUV.full, RotatableRectUV.full, RotatableRectUV.full);
        private bool[] showSides = new bool[6];
        private Vector3 size = new Vector3(20, 20, 20);
        private SphericalPanoRenderer.MeshType meshType = SphericalPanoRenderer.MeshType.sphere;
        private float radius = 10;
        private int segments = 32;
        private int quality = 2;
        private float height = 10;
        private int sides = 32;

        private void CreatePano()
        {
            PanoRenderer renderer = activeType.CreatePano();
            renderer.shader = shader;
            renderer.defaultMaterial = material;

            if (activeType == spherical)
            {
                SphericalPanoRenderer r = renderer as SphericalPanoRenderer;
                r.texture = texture;
                r.radius = radius;
                r.meshType = meshType;
                if (meshType == SphericalPanoRenderer.MeshType.sphere) r.segments = segments;
                else r.quality = quality;
                r.uv = uv;
            }
            else if (activeType == singleTexture)
            {
                SingleTextureCubeFacesPanoRenderer r = renderer as SingleTextureCubeFacesPanoRenderer;
                r.texture = texture;
                r.cubeUV = singleTextureUV;
                r.size = size;
            }
            else if (activeType == cubeFaces)
            {
                CubeFacesPanoRenderer r = renderer as CubeFacesPanoRenderer;
                r.textures = textures;
                r.size = size;
            }
            else if (activeType == cubemapRenderer)
            {
                CubemapPanoRenderer r = renderer as CubemapPanoRenderer;
                r.cubemap = cubemap;
                r.size = size;
            }
            else if (activeType == cylindrical)
            {
                CylindricalPanoRenderer r = renderer as CylindricalPanoRenderer;
                r.texture = texture;
                r.height = height;
                r.radius = radius;
                r.sides = sides;
                r.uv = uv;
            }
            else throw new Exception("Wrong pano type");

            CreatePlugins(controls, renderer);
            CreatePlugins(plugins, renderer);

            Undo.RegisterCreatedObjectUndo(renderer.gameObject, "Create Panorama");

            Selection.activeGameObject = renderer.gameObject;

            Close();
        }

        private void CreatePlugins(List<PluginWrapper> plugins, PanoRenderer renderer)
        {
            foreach (PluginWrapper plugin in plugins)
            {
                if (plugin.requirePanoRenderer != null && plugin.requirePanoRenderer != activeType.GetPanoType()) continue;
                if (!plugin.enabled) continue;

                if (string.IsNullOrEmpty(plugin.createMethod)) renderer.gameObject.AddComponent(plugin.type);
                else InvokeMethod(plugin.createMethod, renderer);
            }
        }

        public void CreateUIButtons(PanoRenderer renderer)
        {
            string[] assets = AssetDatabase.FindAssets("UI Buttons Control");
            if (assets.Length == 0) return;
            string assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);

            if (string.IsNullOrEmpty(assetPath)) return;

            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            GameObject prefab = PrefabUtility.InstantiatePrefab(asset) as GameObject;

            Canvas canvas = CanvasUtils.GetCanvas();

            prefab.transform.SetParent(canvas.transform, false);
            prefab.GetComponent<UIButtonsControl>().panoInstance = renderer.pano;
        }

        public void CreateUICompass(PanoRenderer renderer)
        {
            string[] assets = AssetDatabase.FindAssets("UI Compass");
            if (assets.Length == 0) return;
            string assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);
            if (string.IsNullOrEmpty(assetPath)) return;
            GameObject prefab = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(assetPath)) as GameObject;

            Canvas canvas = CanvasUtils.GetCanvas();

            prefab.transform.SetParent(canvas.transform, false);
            prefab.GetComponent<UICompassControl>().panoInstance = renderer.pano;
        }

        public void CreateUIJoystick(PanoRenderer renderer)
        {
            string[] assets = AssetDatabase.FindAssets("UI Joystick Control");
            if (assets.Length == 0) return;
            string assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);
            if (string.IsNullOrEmpty(assetPath)) return;
            GameObject prefab = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(assetPath)) as GameObject;

            Canvas canvas = CanvasUtils.GetCanvas();

            prefab.transform.SetParent(canvas.transform, false);
            prefab.GetComponent<JoystickControl>().panoInstance = renderer.pano;
        }

        private void DrawControls()
        {
            EditorUtils.GroupLabel("Controls");

            foreach (PluginWrapper control in controls)
            {
                control.enabled = EditorGUILayout.Toggle(control.title, control.enabled);
            }
        }

        private void DrawCubeFaces()
        {
            EditorUtils.GroupLabel("Texture Settings");
            if (textures == null || textures.Length != 6) textures = new Texture2D[6];
            for (int i = 0; i < 6; i++)
            {
                textures[i] = EditorGUILayout.ObjectField(CubeUV.sides[i], textures[i], typeof(Texture2D), false) as Texture2D;
            }
            DrawShaderAndMaterial();
        }

        private void DrawCubeMesh()
        {
            EditorUtils.GroupLabel("Mesh Settings");
            size = EditorGUILayout.Vector3Field("Size", size);
        }

        private void DrawCubemap()
        {
            EditorUtils.GroupLabel("Cubemap Settings");
            cubemap = EditorGUILayout.ObjectField("Cubemap", cubemap, typeof(Cubemap), false) as Cubemap;
            cubemapShader = EditorGUILayout.ObjectField("Shader", cubemapShader, typeof(Shader), false) as Shader;
            material = EditorGUILayout.ObjectField("Material", material, typeof(Material), false) as Material;
        }

        private void DrawMesh()
        {
            EditorUtils.GroupLabel("Mesh Settings");
            if (activeType == spherical) meshType = (SphericalPanoRenderer.MeshType) EditorGUILayout.EnumPopup("Mesh Type", meshType);
            radius = EditorGUILayout.FloatField("Radius", radius);
            if (activeType == spherical)
            {
                if (meshType == SphericalPanoRenderer.MeshType.sphere) segments = EditorGUILayout.IntField("Segments", segments);
                else quality = EditorGUILayout.IntSlider("Quality", quality, 0, SphericalPanoRenderer.maxQuality);
            }
            else if (activeType == cylindrical)
            {
                height = EditorGUILayout.FloatField("Height", height);
                sides = EditorGUILayout.IntField("Sides", sides);
            }
        }

        private void DrawPlugins()
        {
            EditorUtils.GroupLabel("Plugins");

            foreach (PluginWrapper plugin in plugins)
            {
                if (plugin.requirePanoRenderer != null)
                {
                    if (plugin.requirePanoRenderer != activeType.GetPanoType()) continue;
                }

                plugin.enabled = EditorGUILayout.Toggle(plugin.title, plugin.enabled);
            }
        }

        private void DrawShaderAndMaterial()
        {
            shader = EditorGUILayout.ObjectField("Shader", shader, typeof(Shader), false) as Shader;
            material = EditorGUILayout.ObjectField("Material", material, typeof(Material), false) as Material;
        }

        private void DrawSingleTexture()
        {
            EditorUtils.GroupLabel("Texture Settings");
            texture = EditorGUILayout.ObjectField("Texture", texture, typeof(Texture), false) as Texture;
            DrawShaderAndMaterial();
        }

        private void DrawSingleTextureUV()
        {
            EditorUtils.GroupLabel("UV");

            if (GUILayout.Button("Presets")) DrawSingleTextureUVPresets();

            for (int i = 0; i < 6; i++)
            {
                if (showSides[i] = EditorGUILayout.Foldout(showSides[i], CubeUV.sides[i]))
                {
                    RotatableRectUV uv = singleTextureUV[i];
                    EditorGUI.indentLevel++;
                    for (int j = 0; j < 4; j++)
                    {
                        uv[j] = EditorGUILayout.FloatField(RectUV.sides[j], uv[j]);
                    }

                    uv.rotation = (RotatableRectUV.Rotation) EditorGUILayout.EnumPopup("Rotation", uv.rotation);

                    EditorGUI.indentLevel--;
                    singleTextureUV[i] = uv;
                }
            }
        }

        private void DrawSingleTextureUVPresets()
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Horizontal Cross"), false, () => { singleTextureUV = CubeUVPresets.horizontalCrossPreset; });
            menu.AddItem(new GUIContent("Vertical Cross"), false, () => { singleTextureUV = CubeUVPresets.verticalCrossPreset; });
            menu.AddItem(new GUIContent("Youtube (3x2)"), false, () => { singleTextureUV = CubeUVPresets.youtubePreset; });
            menu.ShowAsContext();
        }

        private void DrawUV()
        {
            EditorUtils.GroupLabel("UV");
            uv.left = EditorGUILayout.FloatField("Left", uv.left);
            uv.right = EditorGUILayout.FloatField("Right", uv.right);
            uv.top = EditorGUILayout.FloatField("Top", uv.top);
            uv.bottom = EditorGUILayout.FloatField("Bottom", uv.bottom);
        }

        private void InvokeMethod(string methodName, PanoRenderer renderer)
        {
            MethodInfo methodInfo = GetType().GetMethod(methodName);
            if (methodInfo != null) methodInfo.Invoke(this, new object[]{renderer});
        }

        private void OnEnable()
        {
            spherical = new PanoRendererType<SphericalPanoRenderer>("Spherical");
            singleTexture = new PanoRendererType<SingleTextureCubeFacesPanoRenderer>("Single Texture Cube Faces");
            cubeFaces = new PanoRendererType<CubeFacesPanoRenderer>("Cube Faces");
            cubemapRenderer = new PanoRendererType<CubemapPanoRenderer>("Cubemap");
            cylindrical = new PanoRendererType<CylindricalPanoRenderer>("Cylindrical");

            spherical.steps += DrawSingleTexture;
            spherical.steps += DrawUV;
            spherical.steps += DrawMesh;

            singleTexture.steps += DrawSingleTexture;
            singleTexture.steps += DrawSingleTextureUV;
            singleTexture.steps += DrawCubeMesh;

            cubeFaces.steps += DrawCubeFaces;
            cubeFaces.steps += DrawCubeMesh;

            cubemapRenderer.steps += DrawCubemap;
            cubeFaces.steps += DrawCubeMesh;

            cylindrical.steps += DrawSingleTexture;
            cylindrical.steps += DrawUV;
            cylindrical.steps += DrawMesh;

            types = new List<PanoRendererType> {spherical, singleTexture, cubeFaces, cubemapRenderer, cylindrical};

            activeType = types[0];

            shader = Shader.Find("Unlit/Texture");
            cubemapShader = Shader.Find("uPano/Cubemap");

            controls = new List<PluginWrapper>();
            plugins = new List<PluginWrapper>();

            Type[] assemblyTypes = typeof(Pano).Assembly.GetTypes();
            foreach (Type type in assemblyTypes)
            {
                if (type.IsSubclassOf(typeof(Plugin)) && !type.IsAbstract)
                {
                    if (type.IsSubclassOf(typeof(PanoControl)))
                    {
                        controls.Add(new PluginWrapper(type));
                    }
                    else plugins.Add(new PluginWrapper(type));
                }
            }

            controls = controls.OrderBy(c => c.title).ToList();
            plugins = plugins.OrderBy(p => p.title).ToList();
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorUtils.GroupLabel("Type");

            foreach (PanoRendererType type in types)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.ToggleLeft(type.title, type == activeType);
                if (EditorGUI.EndChangeCheck()) activeType = type;
            }

            EditorGUILayout.Space();

            if (activeType.steps != null) activeType.steps();

            EditorGUIUtility.labelWidth += 40;

            DrawControls();
            DrawPlugins();

            EditorGUIUtility.labelWidth -= 40;

            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Create"))
            {
                CreatePano();
            }
        }

        [MenuItem(EditorUtils.MENU_PATH + "Wizard", false, 0)]
        [MenuItem("GameObject/3D Object/Panorama")]
        public static void OpenWindow()
        {
            Wizard wizard = GetWindow<Wizard>(true, "Pano Wizard");
            wizard.position = new Rect(100, 100, 600, 725);

        }

        internal abstract class PanoRendererType
        {
            public Action steps;
            public string title;

            internal PanoRendererType(string title)
            {
                this.title = title;
            }

            public abstract Type GetPanoType();

            public abstract PanoRenderer CreatePano();
        }

        internal class PanoRendererType<T>: PanoRendererType
            where T: PanoRenderer
        {
            public Type type;

            internal PanoRendererType(string title): base(title)
            {
                type = typeof(T);
            }

            public override Type GetPanoType()
            {
                return type;
            }

            public override PanoRenderer CreatePano()
            {
                GameObject go = new GameObject("Panorama");
                T renderer = go.AddComponent<T>();
                return renderer;
            }
        }

        internal class PluginWrapper
        {
            public string title;
            public Type type;
            public Type requirePanoRenderer;
            public bool enabled;
            public string createMethod;

            public PluginWrapper(Type type, bool enabled = false)
            {
                this.type = type;
                this.enabled = enabled;
                title = ObjectNames.NicifyVariableName(type.Name);

                object[] attributes = type.GetCustomAttributes(false);
                foreach (object attribute in attributes)
                {
                    Type aType = attribute.GetType();
                    if (aType == typeof(RequirePanoRendererAttribute))
                    {
                        RequirePanoRendererAttribute rpt = attribute as RequirePanoRendererAttribute;
                        requirePanoRenderer = rpt.type;
                    }
                    else if (aType == typeof(WizardEnabledAttribute))
                    {
                        this.enabled = (attribute as WizardEnabledAttribute).enabled;
                    }
                    else if (aType == typeof(WizardCreateMethodAttribute))
                    {
                        createMethod = (attribute as WizardCreateMethodAttribute).methodName;
                    }
                }
            }
        }
    }
}
