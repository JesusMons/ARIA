// ============================================================
//  StyleARIAMenu_Panels.cs
//  ARIA: Último Protocolo — Paneles laterales + detalles
//
//  Ejecutar DESPUÉS de StyleARIAMenu.cs
//  Menú superior → ARIA → Add Side Panels
// ============================================================

#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public static class StyleARIAMenuPanels
{
    static Color32 C_VOID     = Hex("0A0E1A");
    static Color32 C_BLUE     = Hex("00C6FF");
    static Color32 C_WHITE    = Hex("F0F4F8");
    static Color32 C_GOLD     = Hex("FFD700");
    static Color32 C_RED      = Hex("FF4757");
    static Color32 C_PANEL_BG = new Color32(0, 198, 255, 8);
    static Color32 C_DIM      = new Color32(240, 244, 248, 100);
    static Color32 C_DIMMER   = new Color32(240, 244, 248, 55);

    [MenuItem("ARIA/Add Side Panels")]
    public static void AgregarPaneles()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) { Debug.LogError("[ARIA] No se encontró Canvas."); return; }

        GameObject panel = GameObject.Find("Panel_MainMenu");
        if (panel == null) { Debug.LogError("[ARIA] No se encontró Panel_MainMenu."); return; }

        Undo.RegisterFullObjectHierarchyUndo(canvas.gameObject, "Add ARIA Side Panels");

        AjustarBotones(panel);
        AgregarEyebrow(panel);
        CrearPanelIzquierdo(canvas.gameObject);
        CrearPanelDerecho(canvas.gameObject);
        AgregarDivisor(panel);

        EditorUtility.SetDirty(canvas.gameObject);
        EditorUtility.DisplayDialog("ARIA Panels",
            "✅ Paneles laterales agregados.\n\nUsa Ctrl+Z si algo no quedó bien.",
            "OK");
    }

    // ── Ajustar padding y tamaño de botones ───────────────
    static void AjustarBotones(GameObject panel)
    {
        VerticalLayoutGroup vlg = panel.GetComponent<VerticalLayoutGroup>();
        if (vlg != null)
        {
            vlg.padding  = new RectOffset(540, 540, 320, 0);
            vlg.spacing  = 6f;
        }

        string[] btns = { "BTN_NuevaPartida","BTN_Continuar","BTN_Opciones","BTN_Creditos","BTN_Salir" };
        foreach (string name in btns)
        {
            Transform t = panel.transform.Find(name);
            if (t == null) continue;
            LayoutElement le = t.GetComponent<LayoutElement>();
            if (le != null) { le.preferredHeight = 56f; le.preferredWidth = 420f; }
        }
    }

    // ── Eyebrow encima del logo ────────────────────────────
    static void AgregarEyebrow(GameObject panel)
    {
        Transform ex = panel.transform.Find("TXT_Eyebrow");
        if (ex != null) Object.DestroyImmediate(ex.gameObject);

        GameObject go = new GameObject("TXT_Eyebrow");
        go.transform.SetParent(panel.transform, false);

        LayoutElement le = go.AddComponent<LayoutElement>();
        le.ignoreLayout = true;

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot     = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0f, -38f);
        rt.sizeDelta = new Vector2(700f, 28f);

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text      = "// PROYECTO ARIA — CLASIFICADO";
        tmp.fontSize  = 11f;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color     = new Color32(0, 198, 255, 140);
        tmp.characterSpacing = 4f;
    }

    // ── Divisor bajo subtítulo ─────────────────────────────
    static void AgregarDivisor(GameObject panel)
    {
        Transform ex = panel.transform.Find("IMG_Divisor");
        if (ex != null) Object.DestroyImmediate(ex.gameObject);

        GameObject go = new GameObject("IMG_Divisor");
        go.transform.SetParent(panel.transform, false);

        LayoutElement le = go.AddComponent<LayoutElement>();
        le.ignoreLayout = true;

        Image img = go.AddComponent<Image>();
        img.color = new Color32(0, 198, 255, 50);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot     = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0f, -240f);
        rt.sizeDelta = new Vector2(240f, 1f);
    }

    // ── Panel izquierdo — Lore ─────────────────────────────
    static void CrearPanelIzquierdo(GameObject canvasGO)
    {
        Transform ex = canvasGO.transform.Find("Panel_Lore");
        if (ex != null) Object.DestroyImmediate(ex.gameObject);

        GameObject panel = new GameObject("Panel_Lore");
        panel.transform.SetParent(canvasGO.transform, false);

        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0.5f);
        rt.anchorMax = new Vector2(0, 0.5f);
        rt.pivot     = new Vector2(0, 0.5f);
        rt.anchoredPosition = new Vector2(48f, 0f);
        rt.sizeDelta = new Vector2(260f, 480f);

        float yOffset = 0f;

        // Bloque 1 — Registro
        yOffset = CrearBloqueInfo(panel,
            "// REGISTRO DE SISTEMA",
            "Año 2157. Estación Orbital VOSS-7.\nTodos los sistemas nominales.\nProtocolo de consciencia: ACTIVO",
            yOffset, C_BLUE, C_DIM);

        yOffset -= 24f;

        // Bloque 2 — Última transmisión
        yOffset = CrearBloqueInfo(panel,
            "// ÚLTIMA TRANSMISIÓN",
            "\"Lena, si estás leyendo esto,\nel experimento funcionó.\nLo siento, y lo hice por ti.\"",
            yOffset, C_BLUE, C_DIM, cita: true);

        yOffset -= 24f;

        // Items de estado
        CrearStatusItem(panel, "INTEGRIDAD NEURONAL: 98.4%", ref yOffset, C_BLUE);
        CrearStatusItem(panel, "ARCHIVO VOSS: FRAGMENTADO",  ref yOffset, C_GOLD);
        CrearStatusItem(panel, "PROTOCOLO ARIA: EN LÍNEA",   ref yOffset, C_BLUE);
    }

    static float CrearBloqueInfo(GameObject parent, string titulo, string cuerpo,
        float yStart, Color32 colTitulo, Color32 colCuerpo, bool cita = false)
    {
        // Línea izquierda
        GameObject borde = new GameObject("Borde");
        borde.transform.SetParent(parent.transform, false);
        Image imgB = borde.AddComponent<Image>();
        imgB.color = new Color32(0, 198, 255, 80);
        RectTransform rtB = borde.GetComponent<RectTransform>();
        rtB.anchorMin = new Vector2(0, 1f);
        rtB.anchorMax = new Vector2(0, 1f);
        rtB.pivot     = new Vector2(0, 1f);
        rtB.anchoredPosition = new Vector2(0f, yStart);
        rtB.sizeDelta = new Vector2(2f, 80f);

        // Título
        GameObject goT = new GameObject("Titulo");
        goT.transform.SetParent(parent.transform, false);
        TextMeshProUGUI tmpT = goT.AddComponent<TextMeshProUGUI>();
        tmpT.text      = titulo;
        tmpT.fontSize  = 10f;
        tmpT.color     = colTitulo;
        tmpT.characterSpacing = 2f;
        tmpT.fontStyle = FontStyles.Bold;
        RectTransform rtT = goT.GetComponent<RectTransform>();
        rtT.anchorMin = new Vector2(0, 1f);
        rtT.anchorMax = new Vector2(1, 1f);
        rtT.pivot     = new Vector2(0, 1f);
        rtT.anchoredPosition = new Vector2(14f, yStart);
        rtT.sizeDelta = new Vector2(-14f, 20f);

        // Cuerpo
        GameObject goC = new GameObject("Cuerpo");
        goC.transform.SetParent(parent.transform, false);
        TextMeshProUGUI tmpC = goC.AddComponent<TextMeshProUGUI>();
        tmpC.text     = cuerpo;
        tmpC.fontSize = 12f;
        tmpC.color    = cita ? new Color32(240, 244, 248, 160) : colCuerpo;
        tmpC.lineSpacing = 8f;
        RectTransform rtC = goC.GetComponent<RectTransform>();
        rtC.anchorMin = new Vector2(0, 1f);
        rtC.anchorMax = new Vector2(1, 1f);
        rtC.pivot     = new Vector2(0, 1f);
        rtC.anchoredPosition = new Vector2(14f, yStart - 22f);
        rtC.sizeDelta = new Vector2(-14f, 60f);

        if (cita)
        {
            // Autor
            GameObject goA = new GameObject("Autor");
            goA.transform.SetParent(parent.transform, false);
            TextMeshProUGUI tmpA = goA.AddComponent<TextMeshProUGUI>();
            tmpA.text     = "— Dr. Elias Voss";
            tmpA.fontSize = 11f;
            tmpA.color    = C_GOLD;
            tmpA.fontStyle = FontStyles.Italic;
            RectTransform rtA = goA.GetComponent<RectTransform>();
            rtA.anchorMin = new Vector2(0, 1f);
            rtA.anchorMax = new Vector2(1, 1f);
            rtA.pivot     = new Vector2(0, 1f);
            rtA.anchoredPosition = new Vector2(14f, yStart - 82f);
            rtA.sizeDelta = new Vector2(-14f, 20f);
        }

        return yStart - 100f;
    }

    static void CrearStatusItem(GameObject parent, string texto, ref float yOffset, Color32 color)
    {
        // Punto indicador
        GameObject dot = new GameObject("Dot");
        dot.transform.SetParent(parent.transform, false);
        Image imgD = dot.AddComponent<Image>();
        imgD.color = color;
        RectTransform rtD = dot.GetComponent<RectTransform>();
        rtD.anchorMin = new Vector2(0, 1f);
        rtD.anchorMax = new Vector2(0, 1f);
        rtD.pivot     = new Vector2(0, 1f);
        rtD.anchoredPosition = new Vector2(0f, yOffset - 6f);
        rtD.sizeDelta = new Vector2(6f, 6f);

        // Texto
        GameObject go = new GameObject("Status_" + texto.Substring(0, 5));
        go.transform.SetParent(parent.transform, false);
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text      = texto;
        tmp.fontSize  = 11f;
        tmp.color     = new Color32(240, 244, 248, 120);
        tmp.characterSpacing = 1f;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1f);
        rt.anchorMax = new Vector2(1, 1f);
        rt.pivot     = new Vector2(0, 1f);
        rt.anchoredPosition = new Vector2(16f, yOffset);
        rt.sizeDelta = new Vector2(-16f, 20f);

        yOffset -= 22f;
    }

    // ── Panel derecho — Stats ──────────────────────────────
    static void CrearPanelDerecho(GameObject canvasGO)
    {
        Transform ex = canvasGO.transform.Find("Panel_Stats");
        if (ex != null) Object.DestroyImmediate(ex.gameObject);

        GameObject panel = new GameObject("Panel_Stats");
        panel.transform.SetParent(canvasGO.transform, false);

        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(1, 0.5f);
        rt.anchorMax = new Vector2(1, 0.5f);
        rt.pivot     = new Vector2(1, 0.5f);
        rt.anchoredPosition = new Vector2(-48f, 0f);
        rt.sizeDelta = new Vector2(240f, 480f);

        float y = 0f;

        CrearTarjeta(panel, "// TIEMPO EN ÓRBITA", "00:00:00", "CICLO ACTIVO", ref y, C_BLUE);
        y -= 12f;
        CrearTarjeta(panel, "// AMENAZAS NEUTRALIZADAS", "—", "SIN DATOS PREVIOS", ref y, C_BLUE);
        y -= 12f;

        // Niveles operativos
        CrearTituloStats(panel, "// NIVELES OPERATIVOS", ref y);
        CrearFilaNivel(panel, "ACTO I — EL DESPERTAR",  "DISPONIBLE", ref y, C_BLUE);
        CrearFilaNivel(panel, "ACTO II — LA FRACTURA",  "BLOQUEADO",  ref y, new Color32(240, 244, 248, 40));
        CrearFilaNivel(panel, "ACTO III — EL NÚCLEO",   "BLOQUEADO",  ref y, new Color32(240, 244, 248, 40));
        y -= 16f;

        // Versión
        GameObject goV = new GameObject("TXT_Version");
        goV.transform.SetParent(panel.transform, false);
        TextMeshProUGUI tmpV = goV.AddComponent<TextMeshProUGUI>();
        tmpV.text      = "v0.1.0 — BUILD ALFA";
        tmpV.fontSize  = 10f;
        tmpV.color     = new Color32(240, 244, 248, 60);
        tmpV.alignment = TextAlignmentOptions.TopRight;
        tmpV.characterSpacing = 2f;
        RectTransform rtV = goV.GetComponent<RectTransform>();
        rtV.anchorMin = new Vector2(0, 1f);
        rtV.anchorMax = new Vector2(1, 1f);
        rtV.pivot     = new Vector2(1, 1f);
        rtV.anchoredPosition = new Vector2(0f, y);
        rtV.sizeDelta = new Vector2(0f, 20f);
    }

    static void CrearTarjeta(GameObject parent, string titulo, string valor, string sub,
        ref float y, Color32 color)
    {
        GameObject card = new GameObject("Card");
        card.transform.SetParent(parent.transform, false);
        Image img = card.AddComponent<Image>();
        img.color = C_PANEL_BG;
        RectTransform rtCard = card.GetComponent<RectTransform>();
        rtCard.anchorMin = new Vector2(0, 1f);
        rtCard.anchorMax = new Vector2(1, 1f);
        rtCard.pivot     = new Vector2(0, 1f);
        rtCard.anchoredPosition = new Vector2(0f, y);
        rtCard.sizeDelta = new Vector2(0f, 72f);

        // Borde derecho
        GameObject borde = new GameObject("Borde");
        borde.transform.SetParent(card.transform, false);
        Image imgB = borde.AddComponent<Image>();
        imgB.color = new Color32(0, 198, 255, 100);
        RectTransform rtB = borde.GetComponent<RectTransform>();
        rtB.anchorMin = new Vector2(1, 0f);
        rtB.anchorMax = new Vector2(1, 1f);
        rtB.pivot     = new Vector2(1, 0.5f);
        rtB.anchoredPosition = Vector2.zero;
        rtB.sizeDelta = new Vector2(2f, 0f);

        Texto(card, titulo, 9f, new Color32(0,198,255,160), TextAlignmentOptions.TopRight, 10f,  -10f, 20f, 3f);
        Texto(card, valor,  24f, C_WHITE,                   TextAlignmentOptions.TopRight, 10f, -32f, 28f);
        Texto(card, sub,    10f, C_DIMMER,                   TextAlignmentOptions.TopRight, 10f, -58f, 16f, 1f);

        y -= 72f;
    }

    static void CrearTituloStats(GameObject parent, string titulo, ref float y)
    {
        Texto(parent, titulo, 9f, new Color32(0,198,255,140), TextAlignmentOptions.TopRight, 0f, y, 18f, 3f);
        y -= 22f;
    }

    static void CrearFilaNivel(GameObject parent, string nombre, string estado,
        ref float y, Color32 colEstado)
    {
        GameObject row = new GameObject("Row");
        row.transform.SetParent(parent.transform, false);
        RectTransform rt = row.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1f);
        rt.anchorMax = new Vector2(1, 1f);
        rt.pivot     = new Vector2(0, 1f);
        rt.anchoredPosition = new Vector2(0f, y);
        rt.sizeDelta = new Vector2(0f, 18f);

        Texto(row, nombre, 9f, C_DIMMER, TextAlignmentOptions.MidlineLeft,  0f, 0f, 18f, 1f);
        Texto(row, estado, 9f, colEstado, TextAlignmentOptions.MidlineRight, 0f, 0f, 18f, 1f);

        y -= 20f;
    }

    static void Texto(GameObject parent, string text, float size, Color32 color,
        TextAlignmentOptions align, float x, float posY, float h, float spacing = 0f)
    {
        GameObject go = new GameObject("T_" + text.Substring(0, Mathf.Min(8, text.Length)));
        go.transform.SetParent(parent.transform, false);
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text     = text;
        tmp.fontSize = size;
        tmp.color    = color;
        tmp.alignment = align;
        tmp.characterSpacing = spacing;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1f);
        rt.anchorMax = new Vector2(1, 1f);
        rt.pivot     = new Vector2(0, 1f);
        rt.anchoredPosition = new Vector2(x, posY);
        rt.sizeDelta = new Vector2(0f, h);
    }

    static Color32 Hex(string hex)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out Color c);
        return (Color32)c;
    }
}
#endif
