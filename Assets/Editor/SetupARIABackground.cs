// ============================================================
//  SetupARIABackground.cs
//  ARIA: Último Protocolo — Setup completo del fondo
//
//  Menú → ARIA → Setup Background
//  Crea la grilla en el editor (no en runtime)
// ============================================================

#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

public static class SetupARIABackground
{
    [MenuItem("ARIA/Setup Background")]
    public static void Setup()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) { Debug.LogError("[ARIA] No Canvas found."); return; }

        Undo.RegisterFullObjectHierarchyUndo(canvas.gameObject, "Setup ARIA Background");

        // ── Paso 1: Limpiar objetos anteriores ─────────────
        Limpiar(canvas, "IMG_DarkBase");
        Limpiar(canvas, "GridBackground");
        Limpiar(canvas, "ParticleContainer");

        // ── Paso 2: Fondo sólido (capa 0) ─────────────────
        GameObject darkBase = CrearImagen(canvas.gameObject, "IMG_DarkBase",
            new Color32(10, 14, 26, 255));
        darkBase.transform.SetSiblingIndex(0);

        // ── Paso 3: Grilla animada (capa 1) ───────────────
        GameObject gridGO = new GameObject("GridBackground");
        gridGO.transform.SetParent(canvas.transform, false);

        RawImage ri = gridGO.AddComponent<RawImage>();
        ri.color   = Color.white;

        // Generar y guardar textura
        Texture2D tex = GenerarTextura(256, 256, 40, 0.04f);
        string texPath = "Assets/UI/Textures/GridTex.png";
        Directory.CreateDirectory("Assets/UI/Textures");
        File.WriteAllBytes(texPath, tex.EncodeToPNG());
        AssetDatabase.Refresh();

        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(texPath);
        if (importer != null)
        {
            importer.wrapMode = TextureWrapMode.Repeat;
            importer.filterMode = FilterMode.Bilinear;
            importer.textureType = TextureImporterType.Default;
            importer.SaveAndReimport();
        }
        ri.texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);

        RectTransform rtGrid = gridGO.GetComponent<RectTransform>();
        rtGrid.anchorMin = Vector2.zero;
        rtGrid.anchorMax = Vector2.one;
        rtGrid.offsetMin = Vector2.zero;
        rtGrid.offsetMax = Vector2.zero;

        gridGO.transform.SetSiblingIndex(1);

        // Añadir animador
        GridAnimator anim = gridGO.AddComponent<GridAnimator>();
        anim.speed = 0.008f;

        // ── Paso 4: Hacer paneles transparentes ────────────
        string[] paneles = { "Panel_MainMenu","Panel_Opciones","Panel_Creditos",
                              "Panel_Salir","Panel_loading","Panel_Lore","Panel_Stats" };
        foreach (string nombre in paneles)
        {
            GameObject p = GameObject.Find(nombre);
            if (p == null) continue;
            Image img = p.GetComponent<Image>();
            if (img != null) img.color = new Color32(0, 0, 0, 0);
        }

        // ── Paso 5: Reordenar todo encima de la grilla ─────
        int total = canvas.transform.childCount;
        string[] arriba = { "BarraInferior","Corner_TL","Corner_TR","Corner_BL","Corner_BR","FadeOverlay" };
        foreach (string n in arriba)
        {
            Transform t = canvas.transform.Find(n);
            if (t != null) t.SetSiblingIndex(total - 1);
        }

        EditorUtility.SetDirty(canvas.gameObject);
        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("ARIA Background",
            "✅ Fondo configurado.\n\n" +
            "La grilla se guarda en Assets/UI/Textures/GridTex.png\n" +
            "Dale Play para ver la animación.", "OK");
    }

    // ── Crear imagen de fondo stretch ─────────────────────
    static GameObject CrearImagen(GameObject parent, string nombre, Color32 color)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(parent.transform, false);
        Image img = go.AddComponent<Image>();
        img.color = color;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return go;
    }

    // ── Generar textura de grilla procedural ──────────────
    static Texture2D GenerarTextura(int w, int h, int cellSize, float lineAlpha)
    {
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.wrapMode  = TextureWrapMode.Repeat;

        Color bg   = new Color(0.039f, 0.055f, 0.102f, 1f);  // #0A0E1A
        Color line = new Color(0f, 0.776f, 1f, lineAlpha);    // #00C6FF

        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                tex.SetPixel(x, y, (x % cellSize == 0 || y % cellSize == 0) ? line : bg);

        tex.Apply();
        return tex;
    }

    static void Limpiar(Canvas c, string nombre)
    {
        Transform t = c.transform.Find(nombre);
        if (t != null) Object.DestroyImmediate(t.gameObject);
    }
}
#endif


