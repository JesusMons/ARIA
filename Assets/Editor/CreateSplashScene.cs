// ============================================================
//  CreateSplashScene.cs  →  Assets/Editor/
//  ARIA: Último Protocolo — Crear escena de intro
//
//  Menú → ARIA → Create Splash Screen
// ============================================================

#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using TMPro;

public static class CreateSplashScene
{
    [MenuItem("ARIA/Create Splash Screen")]
    public static void CrearSplash()
    {
        // Guardar escena actual
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        // Crear nueva escena
        var scene = EditorSceneManager.NewScene(
            NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Canvas
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        // Fondo negro
        GameObject fondoGO = Crear("IMG_Fondo", canvasGO);
        Image fondoImg = fondoGO.AddComponent<Image>();
        fondoImg.color = new Color32(10, 14, 26, 255);
        Stretch(fondoGO);

        // FadeOverlay
        GameObject fadeGO = Crear("FadeOverlay", canvasGO);
        Image fadeImg = fadeGO.AddComponent<Image>();
        fadeImg.color = new Color32(0, 0, 0, 255);
        Stretch(fadeGO);

        // Logo ARIA
        GameObject logoGO = Crear("TXT_Logo", canvasGO);
        TextMeshProUGUI logo = logoGO.AddComponent<TextMeshProUGUI>();
        logo.text = "ARIA";
        logo.fontSize = 100f;
        logo.fontStyle = FontStyles.Bold;
        logo.alignment = TextAlignmentOptions.Center;
        logo.color = new Color32(240, 244, 248, 0);
        logo.characterSpacing = 18f;
        CentrarElemento(logoGO, new Vector2(800f, 130f), new Vector2(0f, 60f));

        // Subtítulo
        GameObject subGO = Crear("TXT_Subtitulo", canvasGO);
        TextMeshProUGUI sub = subGO.AddComponent<TextMeshProUGUI>();
        sub.text = "ÚLTIMO PROTOCOLO";
        sub.fontSize = 18f;
        sub.alignment = TextAlignmentOptions.Center;
        sub.color = new Color32(0, 198, 255, 0);
        sub.characterSpacing = 10f;
        CentrarElemento(subGO, new Vector2(600f, 40f), new Vector2(0f, -10f));

        // Frase
        GameObject fraseGO = Crear("TXT_Frase", canvasGO);
        TextMeshProUGUI frase = fraseGO.AddComponent<TextMeshProUGUI>();
        frase.text = "";
        frase.fontSize = 22f;
        frase.alignment = TextAlignmentOptions.Center;
        frase.color = new Color32(240, 244, 248, 0);
        frase.lineSpacing = 10f;
        CentrarElemento(fraseGO, new Vector2(900f, 120f), new Vector2(0f, 0f));

        // Texto continuar
        GameObject contGO = Crear("TXT_Continuar", canvasGO);
        TextMeshProUGUI cont = contGO.AddComponent<TextMeshProUGUI>();
        cont.text = "// PRESIONA CUALQUIER TECLA PARA CONTINUAR";
        cont.fontSize = 12f;
        cont.alignment = TextAlignmentOptions.Center;
        cont.color = new Color32(0, 198, 255, 0);
        cont.characterSpacing = 3f;
        RectTransform rtCont = contGO.GetComponent<RectTransform>();
        rtCont.anchorMin = new Vector2(0.5f, 0f);
        rtCont.anchorMax = new Vector2(0.5f, 0f);
        rtCont.pivot = new Vector2(0.5f, 0f);
        rtCont.anchoredPosition = new Vector2(0f, 50f);
        rtCont.sizeDelta = new Vector2(700f, 30f);

        // EventSystem
        GameObject esGO = new GameObject("EventSystem");
        esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
        esGO.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();


        // SplashScreenController
        SplashScreenController controller = canvasGO.AddComponent<SplashScreenController>();
        controller.txtLogo      = logo;
        controller.txtSubtitulo = sub;
        controller.txtFrase     = frase;
        controller.txtContinuar = cont;
        controller.imgFondo     = fondoImg;
        controller.fadeOverlay  = fadeImg;
        controller.escenaSiguiente = "MainMenu";

        // Guardar escena
        string path = "Assets/Scenes/SplashScreen.unity";
        EditorSceneManager.SaveScene(scene, path);
        AssetDatabase.Refresh();

        // Agregar a Build Settings
        AgregarABuildSettings(path);

        EditorUtility.DisplayDialog("ARIA Splash",
            "✅ Escena SplashScreen creada en Assets/Scenes/\n\n" +
            "Se agregó automáticamente al Build Settings como escena 0.\n\n" +
            "Asegúrate de que MainMenu sea la escena 1.",
            "OK");
    }

    static void AgregarABuildSettings(string splashPath)
    {
        var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>();

        // SplashScreen primero
        scenes.Add(new EditorBuildSettingsScene(splashPath, true));

        // Agregar escenas existentes
        foreach (var s in EditorBuildSettings.scenes)
        {
            if (s.path != splashPath)
                scenes.Add(s);
        }

        EditorBuildSettings.scenes = scenes.ToArray();
    }

    static GameObject Crear(string nombre, GameObject parent)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(parent.transform, false);
        go.AddComponent<RectTransform>();
        return go;
    }

    static void Stretch(GameObject go)
    {
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    static void CentrarElemento(GameObject go, Vector2 size, Vector2 offset)
    {
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = size;
        rt.anchoredPosition = offset;
    }
}
#endif
