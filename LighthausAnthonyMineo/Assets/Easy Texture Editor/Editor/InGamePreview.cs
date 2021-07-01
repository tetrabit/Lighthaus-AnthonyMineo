using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using EasyTextureEditor;

namespace EasyTextureEditor {


    public class InGamePreview : EditorWindow {

        public Texture2D tex {
            get {
                return _tex;
            }
            set {
                if (value != _tex) {
                    _tex = value;
                    LoadImage();
                }
            }
        }
        private Texture2D _tex = null;
        private RenderTexture preview;
        private RenderTexture befPreview;


        static public List<Renderer> renderers = new List<Renderer>();
        static public List<Material> materials = new List<Material>();
        static public List<Texture> textures = new List<Texture>();
        static public List<string> properties = new List<string>();


        private Texture2D fullTexture = null;



        //public Texture2D preview = null;

        private byte[] texData;
        private Color[] texClrs;

        private Color[] aftClrs;

        private static bool init = false;

        private static List<ImageEffect> effects;

        bool enablePreview = true;

        static int rendererSelection = -1;
        static int materialSelection = -1;
        static int textureSelection = -1;


        private PreviewSize _previewSize = PreviewSize._1024pxPreview;

        private static ComputeShader compute;

        private PreviewSize previewSize {
            get { return _previewSize; }
            set {
                if (_previewSize != value) {
                    _previewSize = value;
                    LoadImage();
                    PreviewUpdate();
                }
            }
        }
        private SaveFormat saveFormat;
        enum PreviewSize {
            _128pxPreview = 128, _256pxPreview = 256, _512pxPreview = 512, _1024pxPreview = 1024, _2048pxPreview = 2048
        }
        private enum SaveFormat {
            _JPEG, _PNG
        }

        public static void Init() {
            effects = new List<ImageEffect>();
            effects.Add(new IEBrightness());
            effects.Add(new IEContrast());
            effects.Add(new IEGAmme());
            effects.Add(new IEExposure());
            effects.Add(new IESaturation());
            effects.Add(new IEHue());
            effects.Add(new IEInvert());
            effects.Add(new IEGrayscale());
            effects.Add(new IEBlending());
            effects.Add(new IEReplace());

            compute = (ComputeShader)Resources.Load("ETEComputeShader");

            materialSelection = -1;
            rendererSelection = -1;
            textureSelection = -1;
            materials.Clear();
            renderers.Clear();
            textures.Clear();




            init = true;
            

        }

        private void SetupMaterials(int r) {
            materials.Clear();

            materials.AddRange(renderers[r].sharedMaterials);

        }

        private void SetupTextures(int m) {
            textures.Clear();
            properties.Clear();
            if (materials[m] == null)
                return;

            Shader shader = materials[m].shader;
            for (int i = 0; i < ShaderUtil.GetPropertyCount(shader); i++) {
                if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv) {
                    Texture t = materials[m].GetTexture(ShaderUtil.GetPropertyName(shader, i));
                    if (t != null) {
                        textures.Add(t);
                        string s = ShaderUtil.GetPropertyName(shader, i);
                        properties.Add(s);
                    }
                }
            }

            CleanUp();
            textureSelection = 0;
            Setup();

        }




        private void SetupRenderers() {
            renderers.Clear();


            RecAdd(ref renderers, go.transform);
        }


        void RecAdd(ref List<Renderer> l, Transform t) {

            Renderer r = t.gameObject.GetComponent<MeshRenderer>();

            if (r != null && r.enabled)
                l.Add(r);
            else {
                r = t.gameObject.GetComponent<SkinnedMeshRenderer>();
                if (r != null)
                    l.Add(r);
                else {
                    r = t.gameObject.GetComponent<ParticleSystemRenderer>();
                    if (r != null)
                        l.Add(r);
                    else {
                        r = t.gameObject.GetComponent<SpriteRenderer>();
                        if (r != null)
                            l.Add(r);
                    }

                }

            }
            for (int i = 0; i < t.childCount; i++) {
                RecAdd(ref l, t.GetChild(i));
            }

        }

        void OnDestroy() {
            CleanUp();
        }

        private void PreviewUpdate() {



            if (tex == null)
                return;

            RenderTexture.active = null;
            preview.Release();
            preview = new RenderTexture(befPreview.width, befPreview.height, 32);
            preview.wrapMode = TextureWrapMode.Repeat;
            preview.enableRandomWrite = true;
            preview.depth = 0;
            // preview.Create();
            RenderTexture.active = preview;

            Graphics.Blit(befPreview, preview);


            if (enablePreview)
                foreach (var effect in effects) {
                    effect.ApplyGPU(ref preview, compute);
                }

            if (run)
                materials[materialSelection].SetTexture(properties[textureSelection], preview);

            RenderTexture.active = null;
        }

        void LoadImage() {
            if (tex == null)
                return;

            string path = AssetDatabase.GetAssetPath(tex);

            if (Path.GetExtension(path) == ".tga") {
                fullTexture = TGALoader.LoadTGA(path);
            } else {
                texData = File.ReadAllBytes(path);
                fullTexture = new Texture2D(2, 2);
                ImageConversion.LoadImage(fullTexture, texData);
            }


            texClrs = fullTexture.GetPixels(0);

            float ratio = (float)(fullTexture.height) / fullTexture.width;
            Texture2D t2dPreview = QuickEditor.ScaleTexture(fullTexture, (int)previewSize, (int)((int)previewSize * ratio));
            if (preview != null)
                preview.Release();
            preview = new RenderTexture((int)previewSize, (int)((int)previewSize * ratio), 32);
            preview.enableRandomWrite = true;
            preview.Create();
            RenderTexture.active = preview;
            Graphics.Blit(t2dPreview, preview);
            if (befPreview != null)
                befPreview.Release();
            befPreview = new RenderTexture((int)previewSize, (int)((int)previewSize * ratio), 32);
            befPreview.enableRandomWrite = true;
            befPreview.Create();
            RenderTexture.active = befPreview;
            Graphics.Blit(preview, befPreview);
            RenderTexture.active = null;

        }
        bool run = false;
        void Setup() {
            run = true;
            // materials[materialSelection].SetTexture(properties[textureSelection], preview);
        }

        void CleanUp() {

            run = false;
            if (materials.Count > 0 && textures.Count > 0 && properties.Count > 0)
                materials[materialSelection].SetTexture(properties[textureSelection], textures[textureSelection]);
        }




        Texture2D CreateOutput() {
            RenderTexture outputRender = new RenderTexture(fullTexture.width, fullTexture.height, 32);
            outputRender.enableRandomWrite = true;
            outputRender.Create();
            RenderTexture.active = outputRender;
            Graphics.Blit(fullTexture, outputRender);

            foreach (var effect in effects) {
                effect.ApplyGPU(ref outputRender, compute);
            }

            Texture2D t = new Texture2D(outputRender.width, outputRender.height, TextureFormat.RGBA32, false);
            RenderTexture.active = outputRender;
            t.ReadPixels(new Rect(0, 0, outputRender.width, outputRender.height), 0, 0);

            t.Apply();

            RenderTexture.active = null;
            outputRender.Release();
            return t;


        }

        public GameObject _go;
        public GameObject go {
            get { return _go; }
            set {
                if (value != _go) {
                    /*
                    if (_go != null)
                        CleanUp();
                        */
                    CleanUp();//
                    Init();

                    PreviewUpdate();
                }
                _go = value;
            }
        }


        bool scrollBarPresent = true;
        Vector2 scrollPos;
        private void OnGUI() {
            if (!init)
                Init();




            GUI.skin.button.wordWrap = true;
            bool updateFlag = false;
            scrollPos = GUILayout.BeginScrollView(scrollPos, false, false, GUIStyle.none, GUI.skin.verticalScrollbar);

            GUILayout.Space(10);
            go = EditorGUILayout.ObjectField("Game object", go, typeof(GameObject), true) as GameObject;
            if (go == null) {
                GUILayout.EndScrollView();
                return;
            }

            if (rendererSelection == -1) {
                rendererSelection = 0;
                materialSelection = 0;
                textureSelection = 0;
                SetupRenderers();
                if (renderers.Count > 0) {
                    SetupMaterials(0);
                    if (materials.Count > 0 && materials[0] != null)
                        SetupTextures(0);
                }
            }

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            if (renderers.Count == 0) {

                GUILayout.Label("This object has no renderer.");
                GUILayout.EndScrollView();
                return;
            }

            GUILayout.Label("Select a renderer:");

            GUILayout.BeginHorizontal();
            int buttonWidth = 50;
            int c = 0;

            for (int i = 0; i < renderers.Count; i++) {
                c += buttonWidth;

                if (c > position.width - buttonWidth) {
                    c = 0;
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
                string s = renderers[i].gameObject.name;
                var oldColor = GUI.backgroundColor;
                if (rendererSelection == i) {

                    GUI.backgroundColor = Color.cyan;
                }

                if (GUILayout.Button(s)) {
                    CleanUp();
                    rendererSelection = i;
                    SetupMaterials(i);
                    materialSelection = 0;
                    textureSelection = 0;
                    if (materials.Count > 0)
                        SetupTextures(0);

                }
                GUI.backgroundColor = oldColor;


            }
            GUILayout.EndHorizontal();


            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Select a material:");
            GUILayout.BeginHorizontal();

            c = 0;

            for (int i = 0; i < materials.Count; i++) {
                c += buttonWidth;

                if (c > position.width - buttonWidth) {
                    c = 0;
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
                if (materials[i] == null)
                    continue;
                string s = materials[i].name;
                var oldColor = GUI.backgroundColor;
                if (materialSelection == i) {

                    GUI.backgroundColor = Color.cyan;
                }
                if (GUILayout.Button(s)) {
                    CleanUp();
                    materialSelection = i;
                    SetupTextures(i);
                }
                GUI.backgroundColor = oldColor;


            }
            GUILayout.EndHorizontal();


            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Select a texture:");
            GUILayout.BeginHorizontal();

            c = 0;

            for (int i = 0; i < textures.Count; i++) {
                c += buttonWidth;

                if (c > position.width - buttonWidth) {
                    c = 0;
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
                string s = textures[i].name;
                var oldColor = GUI.backgroundColor;
                if (textureSelection == i) {

                    GUI.backgroundColor = Color.cyan;
                }
                if (GUILayout.Button(s)) {

                    CleanUp();
                    textureSelection = i;
                    Setup();

                }
                GUI.backgroundColor = oldColor;


            }
            GUILayout.EndHorizontal();

            {
                if (textureSelection == -1 || textures.Count == 0) {

                    GUILayout.EndScrollView();
                    return;
                }
                tex = textures[textureSelection] as Texture2D;
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                previewSize = (PreviewSize)EditorGUILayout.EnumPopup(previewSize);

                //Debug.Log(GUILayoutUtility.GetLastRect().position.y);

                if (preview != null) {
                    float ratio = (float)preview.width / (float)preview.height;
                    int ew = preview.width;
                    int eh = preview.height;
                    int padding = 10;
                    int verticalOffset = (int)GUILayoutUtility.GetLastRect().position.y + 25;
                    float windowRatio = position.width / preview.width;

                    if (ew > eh) {
                        ew = (int)(position.width - padding * 2);
                        eh = (int)(windowRatio * preview.height);
                    } else {
                        eh = (int)(position.width - padding * 2);
                        ew = (int)(eh * ratio);
                    }
                    float scrollOffset = 0;
                    if (scrollBarPresent)
                        scrollOffset = padding;

                    GUI.DrawTexture(new Rect(padding, verticalOffset, ew, eh), preview);
                    //GUI.DrawTexture(new Rect(padding, verticalOffset, position.width - padding * 2 - scrollOffset, windowRatio * (preview.height - scrollOffset)), preview);

                    if (ratio < 1.1f)
                        GUILayout.Space(position.width - padding * 2);
                    else
                        GUILayout.Space(windowRatio * preview.height);
                    /*
                    GUILayout.Space(windowRatio * preview.height / 2f);
                    GUILayout.Space(windowRatio * preview.height / 2f);*/
                    GUILayout.Space(padding);
                }



                if (enablePreview) {
                    if (GUILayout.Button("See the original image")) {
                        updateFlag = true;
                        enablePreview = !enablePreview;
                    }
                } else {
                    var b = GUI.backgroundColor;
                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button("See the modified image")) {
                        updateFlag = true;
                        enablePreview = !enablePreview;
                    }
                    GUI.backgroundColor = b;

                }
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);


                for (int i = 0; i < effects.Count; i++) {
                    if (effects[i].OnGUI(position, false, i == effects.Count - 1))
                        updateFlag = true;
                }


                string assetPath = AssetDatabase.GetAssetPath(tex);
                string[] parts = assetPath.Split('.');
                string newPath = parts[0];
                foreach (var item in effects) {
                    newPath += item.ToString();
                }
                GUILayout.Label("Quick save as: " + newPath, EditorStyles.wordWrappedLabel);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Reset", GUILayout.Width(50))) {
                    foreach (var item in effects) {
                        item.magnitude = 0f;
                        item.value = false;
                        updateFlag = true;

                    }
                }
                if (GUILayout.Button("Quick save as JPEG")) {
                    bool checkDiff = false;
                    foreach (var item in effects) {
                        if (item.Altered())
                            checkDiff = true;
                    }
                    if (checkDiff) {
                        Texture2D t = CreateOutput();
                        QuickEditor.SaveTextureAsJPG(t, newPath);
                        AssetDatabase.Refresh();


                        textures[textureSelection] = (Texture2D)AssetDatabase.LoadAssetAtPath(newPath + ".jpeg", typeof(Texture2D));
                        materials[materialSelection].SetTexture(properties[textureSelection], (Texture2D)AssetDatabase.LoadAssetAtPath(newPath + ".jpeg", typeof(Texture2D)));
                        GameObject g = go;
                        go = null;
                        go = g;
                        updateFlag = true;
                    } else {
                        Debug.Log("Nothing to save.");
                    }
                }
                if (GUILayout.Button("Quick save as PNG")) {
                    bool checkDiff = false;
                    foreach (var item in effects) {
                        if (item.Altered())
                            checkDiff = true;
                    }
                    if (checkDiff) {
                        Texture2D t = CreateOutput();
                        QuickEditor.SaveTextureAsPNG(t, newPath);
                        AssetDatabase.Refresh();
                        textures[textureSelection] = (Texture2D)AssetDatabase.LoadAssetAtPath(newPath + ".png", typeof(Texture2D));
                        materials[materialSelection].SetTexture(properties[textureSelection], (Texture2D)AssetDatabase.LoadAssetAtPath(newPath + ".png", typeof(Texture2D)));
                        GameObject g = go;
                        go = null;
                        go = g;
                        updateFlag = true;



                    } else {
                        Debug.Log("Nothing to save.");
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(15);
                GUILayout.BeginHorizontal();
                saveFormat = (SaveFormat)EditorGUILayout.EnumPopup(saveFormat, GUILayout.Width(50));

                if (GUILayout.Button("Save elsewhere ...")) {
                    string currentPath = AssetDatabase.GetAssetPath(tex);
                    string name = Path.GetFileName(currentPath);
                    currentPath = Path.GetDirectoryName(currentPath);
                    if (saveFormat == SaveFormat._JPEG) {
                        string path = EditorUtility.SaveFilePanel("Save image", currentPath, name, "jpeg");
                        if (path != "") {


                            Texture2D t = CreateOutput();
                            QuickEditor.SaveTextureAsJPG(t, path, true);
                            string p = Application.dataPath;
                            path = path.Remove(0, p.Length - 6);
                            AssetDatabase.Refresh();

                            textures[textureSelection] = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
                            materials[materialSelection].SetTexture(properties[textureSelection], (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)));
                            GameObject g = go;
                            go = null;
                            go = g;
                            updateFlag = true;


                        }
                    } else {
                        string path = EditorUtility.SaveFilePanel("Save image", currentPath, name, "png");
                        if (path != "") {


                            Texture2D t = CreateOutput();
                            QuickEditor.SaveTextureAsPNG(t, path, true);
                            string p = Application.dataPath;
                            path = path.Remove(0, p.Length - 6);
                            AssetDatabase.Refresh();

                            textures[textureSelection] = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
                            materials[materialSelection].SetTexture(properties[textureSelection], (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)));
                            GameObject g = go;
                            go = null;
                            go = g;
                            updateFlag = true;

                        }
                    }


                }
                if (GUILayout.Button("Open input folder")) {
                    string currentPath = AssetDatabase.GetAssetPath(tex);
                    System.Diagnostics.Process.Start("explorer.exe", "/select," + Path.GetFullPath(currentPath));
                }

                GUILayout.EndHorizontal();


                if (updateFlag) {
                    PreviewUpdate();
                }


                int h = (int)GUILayoutUtility.GetLastRect().y;
                if (h != 0) {
                    if (h > position.height - 20)
                        scrollBarPresent = true;
                    else scrollBarPresent = false;
                }

                EditorGUILayout.EndScrollView();
            }

        }




        private void OnInspectorUpdate() {

            if (tex != null && fullTexture == null) {
                LoadImage();
            }
        }


        [MenuItem("Window/Easy Texture Editor/Real time preview")]
        public static void ShowWindow() {
            if (!init) {
                Init();
            }
            GetWindow(typeof(InGamePreview),false,"Real time preview");
        }


    }
}