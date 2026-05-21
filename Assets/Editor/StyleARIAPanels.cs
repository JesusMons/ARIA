// ============================================================
//  StyleARIAPanels.cs  →  Assets/Editor/
//  ARIA: Último Protocolo — Estilizar paneles secundarios
//  Menú → ARIA → Style Secondary Panels
// ============================================================

#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public static class StyleARIAPanels
{
    static Color32 C_BLUE     = Hex("00C6FF");
    static Color32 C_BLUE_DIM = new Color32(0, 198, 255, 40);
    static Color32 C_WHITE    = Hex("F0F4F8");
    static Color32 C_DIM      = new Color32(240, 244, 248, 130);
    static Color32 C_DIMMER   = new Color32(240, 244, 248, 70);
    static Color32 C_RED      = Hex("FF4757");
    static Color32 C_GOLD     = Hex("FFD700");
    static Color32 C_PANEL    = new Color32(0, 0, 0, 0);

    [MenuItem("ARIA/Style Secondary Panels")]
    public static void EstilizarPaneles()
    {
        EstilizarOpciones();
        EstilizarCreditos();
        EstilizarSalir();
        EditorUtility.DisplayDialog("ARIA Panels",
            "Paneles estilizados:\n- Panel_Opciones\n- Panel_Creditos\n- Panel_Salir", "OK");
    }

    // ── OPCIONES ───────────────────────────────────────────
    static void EstilizarOpciones()
    {
        GameObject panel = FindInactivo("Panel_Opciones");
        if (panel == null) { Debug.LogWarning("[ARIA] Panel_Opciones no encontrado."); return; }
        LimpiarHijos(panel);
        ConfigurarPanel(panel);
        CrearTitulo(panel, "// OPCIONES DE SISTEMA", -80f);
        CrearDivisor(panel, -140f);
        CrearSeccion(panel, "// AUDIO", -170f);
        CrearSliderRow(panel, "VOLUMEN GENERAL",   -210f, "SliderVolumen");
        CrearSliderRow(panel, "EFECTOS DE SONIDO", -255f, "SliderSFX");
        CrearSliderRow(panel, "MUSICA",             -300f, "SliderMusica");
        CrearSeccion(panel, "// GRAFICOS", -355f);
        CrearToggleRow(panel, "PANTALLA COMPLETA", -395f, "ToggleFullscreen");
        CrearToggleRow(panel, "VSYNC",              -435f, "ToggleVSync");
        CrearBotonVolver(panel, "BTN_VolverOpciones", -520f);
        EditorUtility.SetDirty(panel);
    }

    // ── CREDITOS ───────────────────────────────────────────
    static void EstilizarCreditos()
    {
        GameObject panel = FindInactivo("Panel_Creditos");
        if (panel == null) { Debug.LogWarning("[ARIA] Panel_Creditos no encontrado."); return; }
        LimpiarHijos(panel);
        ConfigurarPanel(panel);
        CrearTitulo(panel, "// EQUIPO DE DESARROLLO", -70f);
        CrearDivisor(panel, -120f);
        CrearMiembroEquipo(panel, "SEBASTIAN ACOSTA",  "Programador Senior",      -160f);
        CrearMiembroEquipo(panel, "CRISTIAN RAMIREZ",  "Programador Junior - UI", -220f);
        CrearMiembroEquipo(panel, "KEVIN ESTRADA",     "Arte - Diseno - Shaders", -280f);
        CrearMiembroEquipo(panel, "JESUS MONSALVO",    "Game Designer - QA",      -340f);
        CrearDivisor(panel, -390f);
        CrearInfoJuego(panel, -430f);
        CrearBotonVolver(panel, "BTN_VolverCreditos", -530f);
        EditorUtility.SetDirty(panel);
    }

    // ── SALIR ──────────────────────────────────────────────
    static void EstilizarSalir()
    {
        GameObject panel = FindInactivo("Panel_Salir");
        if (panel == null) { Debug.LogWarning("[ARIA] Panel_Salir no encontrado."); return; }
        LimpiarHijos(panel);
        ConfigurarPanel(panel);

        TMP(panel, "TXT_Icono", "[ ! ]", 28f, C_RED, TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, 60f), new Vector2(80f, 50f));

        TMP(panel, "TXT_Titulo", "// TERMINAR PROTOCOLO", 18f,
            new Color32(255, 71, 87, 220), TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, 10f), new Vector2(600f, 40f), spacing: 4f);

        TMP(panel, "TXT_Mensaje",
            "Confirmar cierre de sesion?\nLos datos no guardados se perderan.",
            14f, C_DIM, TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, -45f), new Vector2(500f, 50f));

        CrearBotonSalir(panel, "BTN_CancelarSalir",   "CANCELAR",         -120f, false);
        CrearBotonSalir(panel, "BTN_ConfirmarSalir",  "CONFIRMAR SALIDA", -175f, true);
        EditorUtility.SetDirty(panel);
    }

    // ── HELPERS ────────────────────────────────────────────
    static void ConfigurarPanel(GameObject panel)
    {
        Image img = panel.GetComponent<Image>() ?? panel.AddComponent<Image>();
        img.color = C_PANEL;
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
    }

    static void CrearTitulo(GameObject parent, string texto, float posY)
    {
        TMP(parent, "TXT_Titulo", texto, 13f, C_BLUE, TextAlignmentOptions.Center,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0, posY), new Vector2(700f, 30f), spacing: 4f);
    }

    static void CrearDivisor(GameObject parent, float posY)
    {
        GameObject go = new GameObject("Divisor");
        go.transform.SetParent(parent.transform, false);
        go.AddComponent<Image>().color = new Color32(0, 198, 255, 50);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f); rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0, posY);
        rt.sizeDelta = new Vector2(400f, 1f);
    }

    static void CrearSeccion(GameObject parent, string texto, float posY)
    {
        TMP(parent, "SEC_" + texto.GetHashCode(), texto, 10f,
            new Color32(0, 198, 255, 160), TextAlignmentOptions.Center,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0, posY), new Vector2(400f, 22f), spacing: 3f);
    }

    static void CrearSliderRow(GameObject parent, string label, float posY, string nombre)
    {
        GameObject row = new GameObject(nombre + "_Row");
        row.transform.SetParent(parent.transform, false);
        RectTransform rtR = row.AddComponent<RectTransform>();
        rtR.anchorMin = new Vector2(0.5f, 1f); rtR.anchorMax = new Vector2(0.5f, 1f);
        rtR.pivot = new Vector2(0.5f, 1f);
        rtR.anchoredPosition = new Vector2(0, posY);
        rtR.sizeDelta = new Vector2(500f, 36f);

        GameObject borde = new GameObject("Borde");
        borde.transform.SetParent(row.transform, false);
        borde.AddComponent<Image>().color = C_BLUE_DIM;
        RectTransform rtB = borde.GetComponent<RectTransform>();
        rtB.anchorMin = new Vector2(0, 0); rtB.anchorMax = new Vector2(0, 1);
        rtB.pivot = new Vector2(0, 0.5f);
        rtB.anchoredPosition = Vector2.zero; rtB.sizeDelta = new Vector2(2f, 0f);

        TMP(row, "Lbl", label, 12f, C_DIM, TextAlignmentOptions.MidlineLeft,
            new Vector2(0, 0), new Vector2(0.4f, 1f), new Vector2(12f, 0), Vector2.zero);

        GameObject sliderGO = new GameObject(nombre);
        sliderGO.transform.SetParent(row.transform, false);
        Slider slider = sliderGO.AddComponent<Slider>();
        slider.value = 0.8f;
        RectTransform rtS = sliderGO.GetComponent<RectTransform>();
        rtS.anchorMin = new Vector2(0.45f, 0.2f); rtS.anchorMax = new Vector2(1f, 0.8f);
        rtS.offsetMin = Vector2.zero; rtS.offsetMax = Vector2.zero;

        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(sliderGO.transform, false);
        Image imgBg = bg.AddComponent<Image>();
        imgBg.color = new Color32(0, 198, 255, 20);
        RectTransform rtBg = bg.GetComponent<RectTransform>();
        rtBg.anchorMin = Vector2.zero; rtBg.anchorMax = Vector2.one;
        rtBg.offsetMin = Vector2.zero; rtBg.offsetMax = Vector2.zero;
        slider.targetGraphic = imgBg;

        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderGO.transform, false);
        RectTransform rtFA = fillArea.AddComponent<RectTransform>();
        rtFA.anchorMin = Vector2.zero; rtFA.anchorMax = Vector2.one;
        rtFA.offsetMin = Vector2.zero; rtFA.offsetMax = Vector2.zero;

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        Image imgFill = fill.AddComponent<Image>();
        imgFill.color = C_BLUE;
        RectTransform rtFill = fill.GetComponent<RectTransform>();
        rtFill.anchorMin = Vector2.zero; rtFill.anchorMax = Vector2.one;
        rtFill.offsetMin = Vector2.zero; rtFill.offsetMax = Vector2.zero;
        slider.fillRect = rtFill;
    }

    static void CrearToggleRow(GameObject parent, string label, float posY, string nombre)
    {
        GameObject row = new GameObject(nombre + "_Row");
        row.transform.SetParent(parent.transform, false);
        RectTransform rtR = row.AddComponent<RectTransform>();
        rtR.anchorMin = new Vector2(0.5f, 1f); rtR.anchorMax = new Vector2(0.5f, 1f);
        rtR.pivot = new Vector2(0.5f, 1f);
        rtR.anchoredPosition = new Vector2(0, posY);
        rtR.sizeDelta = new Vector2(500f, 36f);

        GameObject borde = new GameObject("Borde");
        borde.transform.SetParent(row.transform, false);
        borde.AddComponent<Image>().color = C_BLUE_DIM;
        RectTransform rtB = borde.GetComponent<RectTransform>();
        rtB.anchorMin = new Vector2(0, 0); rtB.anchorMax = new Vector2(0, 1);
        rtB.pivot = new Vector2(0, 0.5f);
        rtB.anchoredPosition = Vector2.zero; rtB.sizeDelta = new Vector2(2f, 0f);

        TMP(row, "Lbl", label, 12f, C_DIM, TextAlignmentOptions.MidlineLeft,
            new Vector2(0, 0), new Vector2(0.75f, 1f), new Vector2(12f, 0), Vector2.zero);

        GameObject toggleBox = new GameObject(nombre);
        toggleBox.transform.SetParent(row.transform, false);
        Image imgBox = toggleBox.AddComponent<Image>();
        imgBox.color = new Color32(0, 198, 255, 30);
        RectTransform rtTB = toggleBox.GetComponent<RectTransform>();
        rtTB.anchorMin = new Vector2(0.8f, 0.2f); rtTB.anchorMax = new Vector2(0.9f, 0.8f);
        rtTB.offsetMin = Vector2.zero; rtTB.offsetMax = Vector2.zero;
        Toggle toggle = toggleBox.AddComponent<Toggle>();
        toggle.isOn = true;
        toggle.targetGraphic = imgBox;
    }

    static void CrearMiembroEquipo(GameObject parent, string nombre, string rol, float posY)
    {
        GameObject row = new GameObject("M_" + nombre.Split(' ')[0]);
        row.transform.SetParent(parent.transform, false);
        RectTransform rt = row.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f); rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0, posY);
        rt.sizeDelta = new Vector2(500f, 50f);

        GameObject borde = new GameObject("Borde");
        borde.transform.SetParent(row.transform, false);
        borde.AddComponent<Image>().color = C_BLUE;
        RectTransform rtB = borde.GetComponent<RectTransform>();
        rtB.anchorMin = new Vector2(0,0); rtB.anchorMax = new Vector2(0,1);
        rtB.pivot = new Vector2(0, 0.5f);
        rtB.anchoredPosition = Vector2.zero; rtB.sizeDelta = new Vector2(2f, 0f);

        TMP(row, "N", nombre, 15f, C_WHITE, TextAlignmentOptions.MidlineLeft,
            new Vector2(0, 0.5f), new Vector2(1f, 1f),
            new Vector2(14f, 0), Vector2.zero, spacing: 2f, bold: true);

        TMP(row, "R", rol, 11f, C_BLUE, TextAlignmentOptions.MidlineLeft,
            new Vector2(0, 0), new Vector2(1f, 0.5f),
            new Vector2(14f, 0), Vector2.zero, spacing: 1f);
    }

    static void CrearInfoJuego(GameObject parent, float posY)
    {
        TMP(parent, "TXT_Info",
            "ARIA: ULTIMO PROTOCOLO  v0.1.0 BUILD ALFA\nUnity 6.3 LTS  Universidad de La Guajira  2026",
            11f, C_DIMMER, TextAlignmentOptions.Center,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0, posY), new Vector2(600f, 40f));
    }

    static void CrearBotonVolver(GameObject parent, string nombre, float posY)
    {
        GameObject btn = new GameObject(nombre);
        btn.transform.SetParent(parent.transform, false);
        btn.AddComponent<Image>().color = new Color32(13, 26, 46, 180);
        Button button = btn.AddComponent<Button>();
        ColorBlock cb = button.colors;
        cb.normalColor = new Color32(13, 26, 46, 180);
        cb.highlightedColor = new Color32(0, 198, 255, 60);
        cb.pressedColor = new Color32(0, 198, 255, 120);
        cb.fadeDuration = 0.1f;
        button.colors = cb;
        RectTransform rt = btn.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f); rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0, posY);
        rt.sizeDelta = new Vector2(280f, 52f);

        GameObject borde = new GameObject("Borde");
        borde.transform.SetParent(btn.transform, false);
        borde.AddComponent<Image>().color = C_BLUE;
        RectTransform rtB = borde.GetComponent<RectTransform>();
        rtB.anchorMin = new Vector2(0,0); rtB.anchorMax = new Vector2(0,1);
        rtB.pivot = new Vector2(0, 0.5f);
        rtB.anchoredPosition = Vector2.zero; rtB.sizeDelta = new Vector2(2f, 0f);

        TMP(btn, "TXT", "<- VOLVER", 14f, C_WHITE, TextAlignmentOptions.Center,
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, spacing: 3f, bold: true);
    }

    static void CrearBotonSalir(GameObject parent, string nombre, string label, float posY, bool esRojo)
    {
        Color32 color = esRojo ? C_RED : C_BLUE;
        Color32 bg    = esRojo ? new Color32(40,10,15,200) : new Color32(13,26,46,180);

        GameObject btn = new GameObject(nombre);
        btn.transform.SetParent(parent.transform, false);
        btn.AddComponent<Image>().color = bg;
        Button button = btn.AddComponent<Button>();
        ColorBlock cb = button.colors;
        cb.normalColor = bg;
        cb.highlightedColor = esRojo ? new Color32(255,71,87,80) : new Color32(0,198,255,60);
        cb.pressedColor     = esRojo ? new Color32(255,71,87,140): new Color32(0,198,255,120);
        cb.fadeDuration = 0.1f;
        button.colors = cb;
        RectTransform rt = btn.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f); rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(0, posY);
        rt.sizeDelta = new Vector2(320f, 54f);

        GameObject borde = new GameObject("Borde");
        borde.transform.SetParent(btn.transform, false);
        borde.AddComponent<Image>().color = color;
        RectTransform rtB = borde.GetComponent<RectTransform>();
        rtB.anchorMin = new Vector2(0,0); rtB.anchorMax = new Vector2(0,1);
        rtB.pivot = new Vector2(0, 0.5f);
        rtB.anchoredPosition = Vector2.zero; rtB.sizeDelta = new Vector2(2f, 0f);

        TMP(btn, "TXT", label, 15f, new Color32(color.r, color.g, color.b, 220),
            TextAlignmentOptions.Center, Vector2.zero, Vector2.one,
            Vector2.zero, Vector2.zero, spacing: 4f, bold: true);
    }

    static void TMP(GameObject parent, string nombre, string texto,
        float size, Color32 color, TextAlignmentOptions align,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 sizeDelta,
        float spacing = 0f, bool bold = false)
    {
        Transform ex = parent.transform.Find(nombre);
        if (ex != null) Object.DestroyImmediate(ex.gameObject);
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(parent.transform, false);
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = texto;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.alignment = align;
        tmp.characterSpacing = spacing;
        tmp.fontStyle = bold ? FontStyles.Bold : FontStyles.Normal;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
        rt.pivot = anchorMin;
        rt.anchoredPosition = pos;
        rt.sizeDelta = sizeDelta;
    }

    static void LimpiarHijos(GameObject go)
    {
        while (go.transform.childCount > 0)
            Object.DestroyImmediate(go.transform.GetChild(0).gameObject);
    }

    static GameObject FindInactivo(string nombre)
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>(FindObjectsInactive.Include);
        if (canvas == null) return null;
        Transform t = canvas.transform.Find(nombre);
        return t != null ? t.gameObject : null;
    }

    static Color32 Hex(string hex)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out Color c);
        return c;
    }
}
#endif