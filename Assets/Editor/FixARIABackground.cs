// ============================================================
//  FixARIABackground.cs
//  ARIA: Último Protocolo — Corrección de fondo dinámico
//
//  Menú superior → ARIA → Fix Background
// ============================================================

#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public static class FixARIABackground
{
    [MenuItem("ARIA/Fix Background")]
    public static void FixBackground()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) { Debug.LogError("[ARIA] No Canvas found."); return; }

        Undo.RegisterFullObjectHierarchyUndo(canvas.gameObject, "Fix ARIA Background");

        // 1. Crear base oscura sólida (capa más baja)
        CrearBaseOscura(canvas.gameObject);

        // 2. El GridBackground ya existe — solo reordenarlo
        Transform grid = canvas.transform.Find("GridBackground");
        if (grid != null) grid.SetSiblingIndex(1); // índice 1 = sobre la base

        // 3. ParticleContainer sobre la grilla
        Transform particles = canvas.transform.Find("ParticleContainer");
        if (particles != null) particles.SetSiblingIndex(2);

        // 4. Hacer Panel_MainMenu y demás paneles semi-transparentes
        // (sin fondo sólido que tape la grilla)
        HacerPanelTransparente("Panel_MainMenu");
        HacerPanelTransparente("Panel_Opciones");
        HacerPanelTransparente("Panel_Creditos");
        HacerPanelTransparente("Panel_Salir");
        HacerPanelTransparente("Panel_loading");

        // 5. Panel_Lore y Panel_Stats sin fondo
        HacerPanelTransparente("Panel_Lore");
        HacerPanelTransparente("Panel_Stats");

        // 6. Todos los demás elementos encima
        // BarraInferior, corners → al final
        ReordenarElemento(canvas, "BarraInferior",  canvas.transform.childCount - 1);
        ReordenarElemento(canvas, "Corner_TL",      canvas.transform.childCount - 1);
        ReordenarElemento(canvas, "Corner_TR",      canvas.transform.childCount - 1);
        ReordenarElemento(canvas, "Corner_BL",      canvas.transform.childCount - 1);
        ReordenarElemento(canvas, "Corner_BR",      canvas.transform.childCount - 1);

        EditorUtility.SetDirty(canvas.gameObject);
        EditorUtility.DisplayDialog("ARIA Background",
            "✅ Fondo corregido.\n\nLa grilla ahora es visible en todos los paneles.", "OK");
    }

    static void CrearBaseOscura(GameObject canvasGO)
    {
        Transform ex = canvasGO.transform.Find("IMG_DarkBase");
        if (ex != null) Object.DestroyImmediate(ex.gameObject);

        GameObject go = new GameObject("IMG_DarkBase");
        go.transform.SetParent(canvasGO.transform, false);
        go.transform.SetAsFirstSibling(); // capa más baja

        Image img = go.AddComponent<Image>();
        img.color = new Color32(10, 14, 26, 255); // #0A0E1A sólido

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    static void HacerPanelTransparente(string nombre)
    {
        GameObject go = GameObject.Find(nombre);
        if (go == null) return;

        Image img = go.GetComponent<Image>();
        if (img != null)
        {
            // Semi-transparente para que la grilla se vea sutilmente
            Color c = img.color;
            img.color = new Color(c.r, c.g, c.b, 0f); // completamente transparente
        }
    }

    static void ReordenarElemento(Canvas canvas, string nombre, int index)
    {
        Transform t = canvas.transform.Find(nombre);
        if (t != null) t.SetSiblingIndex(index);
    }
}
#endif
