// ============================================================
//  StyleARIAMenu.cs
//  ARIA: Último Protocolo — Auto-estilizador del Menú Principal
//
//  USO:
//  1. Colocar en Assets/Editor/StyleARIAMenu.cs
//  2. Abrir la escena MainMenu en Unity
//  3. Menú superior → ARIA → Style Main Menu
//
//  Responsable: Cristian Ramírez
// ============================================================

#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using System.Collections.Generic;

public static class StyleARIAMenu
{
    // ── Paleta de colores del mockup ───────────────────────
    static Color32 C_VOID       = Hex("0A0E1A"); // fondo principal
    static Color32 C_DARK_PANEL = Hex("0D1220"); // fondo paneles
    static Color32 C_BLUE       = Hex("00C6FF"); // azul protocolo
    static Color32 C_BLUE_DIM   = new Color32(0, 198, 255, 60);
    static Color32 C_BLUE_MID   = new Color32(0, 198, 255, 120);
    static Color32 C_WHITE      = Hex("F0F4F8"); // texto principal
    static Color32 C_WHITE_DIM  = new Color32(240, 244, 248, 140);
    static Color32 C_RED        = Hex("FF4757"); // salir / peligro
    static Color32 C_TRANSPARENT = new Color32(0, 0, 0, 0);

    // ── Datos de los botones ───────────────────────────────
    static readonly string[] BTN_NAMES   = { "BTN_NuevaPartida", "BTN_Continuar", "BTN_Opciones", "BTN_Creditos", "BTN_Salir" };
    static readonly string[] BTN_LABELS  = { "NUEVA PARTIDA", "CONTINUAR", "OPCIONES", "CRÉDITOS", "SALIR" };
    static readonly string[] BTN_INDEX   = { "01", "02", "03", "04", "05" };

    // ──────────────────────────────────────────────────────
    [MenuItem("ARIA/Style Main Menu")]
    public static void EstilizarMenu()
    {
        GameObject panel = GameObject.Find("Panel_MainMenu");
        if (panel == null)
        {
            EditorUtility.DisplayDialog("ARIA Style", "No se encontró 'Panel_MainMenu' en la escena.\nAsegúrate de tener la escena del menú abierta.", "OK");
            return;
        }

        Undo.RegisterFullObjectHierarchyUndo(panel, "Style ARIA Menu");

        EstilizarPanel(panel);
        EstilizarLogo(panel);
        CrearSubtitulo(panel);
        CrearLineaAno(panel);
        EstilizarBotones(panel);
        CrearBarraInferior();
        CrearEsquinas();

        EditorUtility.SetDirty(panel);
        EditorUtility.DisplayDialog("ARIA Style", "✅ Menú estilizado correctamente.\n\nRevisa el Game View para ver el resultado.\nSi algo no quedó bien, usa Ctrl+Z para deshacer.", "OK");
    }

    // ── 1. Panel principal ─────────────────────────────────
    static void EstilizarPanel(GameObject panel)
    {
        // Fondo oscuro
        Image img = panel.GetComponent<Image>();
        if (img == null) img = panel.AddComponent<Image>();
        img.color = C_VOID;

        // Stretch completo
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        // VerticalLayoutGroup centrado
        VerticalLayoutGroup vlg = panel.GetComponent<VerticalLayoutGroup>();
        if (vlg == null) vlg = panel.AddComponent<VerticalLayoutGroup>();
        vlg.childAlignment        = TextAnchor.MiddleCenter;
        vlg.spacing               = 8;
        vlg.childControlWidth     = true;
        vlg.childControlHeight    = false;
        vlg.childForceExpandWidth = false;
        vlg.childForceExpandHeight = false;
        vlg.padding = new RectOffset(600, 600, 0, 0);
    }

    // ── 2. Logo ARIA ───────────────────────────────────────
    static void EstilizarLogo(GameObject panel)
    {
        Transform logoT = panel.transform.Find("TXT_Logo");
        if (logoT == null) return;

        GameObject logo = logoT.gameObject;

        // Ignorar layout para posicionarlo libremente
        LayoutElement le = logo.GetComponent<LayoutElement>();
        if (le == null) le = logo.AddComponent<LayoutElement>();
        le.ignoreLayout = true;

        // Posición
        RectTransform rt = logo.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot     = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0, -80f);
        rt.sizeDelta = new Vector2(600f, 120f);

        // TextMeshPro
        TextMeshProUGUI tmp = logo.GetComponent<TextMeshProUGUI>();
        if (tmp == null) tmp = logo.AddComponent<TextMeshProUGUI>();
        tmp.text      = "ARIA";
        tmp.fontSize  = 90f;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color     = C_WHITE;
        tmp.characterSpacing = 15f;
    }

    // ── 3. Subtítulo ───────────────────────────────────────
    static void CrearSubtitulo(GameObject panel)
    {
        // Eliminar si ya existe
        Transform existente = panel.transform.Find("TXT_Subtitulo");
        if (existente != null) Object.DestroyImmediate(existente.gameObject);

        GameObject go = new GameObject("TXT_Subtitulo");
        go.transform.SetParent(panel.transform, false);

        LayoutElement le = go.AddComponent<LayoutElement>();
        le.ignoreLayout = true;

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot     = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0, -180f);
        rt.sizeDelta = new Vector2(600f, 40f);

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text      = "ÚLTIMO PROTOCOLO";
        tmp.fontSize  = 16f;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color     = new Color32(0, 198, 255, 180);
        tmp.characterSpacing = 10f;
    }

    // ── 4. Línea de año ────────────────────────────────────
    static void CrearLineaAno(GameObject panel)
    {
        Transform existente = panel.transform.Find("TXT_Ano");
        if (existente != null) Object.DestroyImmediate(existente.gameObject);

        GameObject go = new GameObject("TXT_Ano");
        go.transform.SetParent(panel.transform, false);

        LayoutElement le = go.AddComponent<LayoutElement>();
        le.ignoreLayout = true;

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot     = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0, -225f);
        rt.sizeDelta = new Vector2(700f, 30f);

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text      = "// 2157 — ESTACIÓN ORBITAL VOSS-7";
        tmp.fontSize  = 11f;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color     = new Color32(240, 244, 248, 80);
        tmp.characterSpacing = 3f;
    }

    // ── 5. Botones ─────────────────────────────────────────
    static void EstilizarBotones(GameObject panel)
    {
        for (int i = 0; i < BTN_NAMES.Length; i++)
        {
            Transform btnT = panel.transform.Find(BTN_NAMES[i]);
            if (btnT == null) { Debug.LogWarning($"[ARIA Style] No se encontró {BTN_NAMES[i]}"); continue; }

            GameObject btn = btnT.gameObject;
            bool esSalir = (i == 4);

            // ── Tamaño del botón ──
            LayoutElement le = btn.GetComponent<LayoutElement>();
            if (le == null) le = btn.AddComponent<LayoutElement>();
            le.preferredHeight = 52f;
            le.preferredWidth  = 380f;

            // ── Imagen de fondo ──
            Image imgBtn = btn.GetComponent<Image>();
            if (imgBtn == null) imgBtn = btn.AddComponent<Image>();
            imgBtn.color = new Color32(13, 26, 46, 180);

            // ── Button colors ──
            Button button = btn.GetComponent<Button>();
            if (button != null)
            {
                ColorBlock cb = button.colors;
                cb.normalColor      = new Color32(13, 26, 46, 180);
                cb.highlightedColor = esSalir
                    ? new Color32(255, 71, 87, 60)
                    : new Color32(0, 198, 255, 60);
                cb.pressedColor     = esSalir
                    ? new Color32(255, 71, 87, 120)
                    : new Color32(0, 198, 255, 120);
                cb.selectedColor    = new Color32(0, 198, 255, 30);
                cb.fadeDuration     = 0.1f;
                button.colors = cb;
            }

            // ── Limpiar hijos de texto anteriores ──
            List<Transform> aEliminar = new List<Transform>();
            foreach (Transform child in btn.transform)
                aEliminar.Add(child);
            foreach (Transform child in aEliminar)
                Object.DestroyImmediate(child.gameObject);

            // ── Borde izquierdo ──
            GameObject borde = new GameObject("IMG_Borde");
            borde.transform.SetParent(btn.transform, false);
            Image imgBorde = borde.AddComponent<Image>();
            imgBorde.color = esSalir ? C_RED : C_BLUE;
            RectTransform rtBorde = borde.GetComponent<RectTransform>();
            rtBorde.anchorMin = new Vector2(0, 0);
            rtBorde.anchorMax = new Vector2(0, 1);
            rtBorde.pivot     = new Vector2(0, 0.5f);
            rtBorde.anchoredPosition = Vector2.zero;
            rtBorde.sizeDelta = new Vector2(3f, 0f);

            // ── Índice (01, 02...) ──
            GameObject idx = new GameObject("TXT_Index");
            idx.transform.SetParent(btn.transform, false);
            TextMeshProUGUI tmpIdx = idx.AddComponent<TextMeshProUGUI>();
            tmpIdx.text      = BTN_INDEX[i];
            tmpIdx.fontSize  = 12f;
            tmpIdx.alignment = TextAlignmentOptions.MidlineLeft;
            tmpIdx.color     = esSalir
                ? new Color32(255, 71, 87, 120)
                : new Color32(0, 198, 255, 120);
            RectTransform rtIdx = idx.GetComponent<RectTransform>();
            rtIdx.anchorMin = new Vector2(0, 0);
            rtIdx.anchorMax = new Vector2(0, 1);
            rtIdx.pivot     = new Vector2(0, 0.5f);
            rtIdx.anchoredPosition = new Vector2(16f, 0f);
            rtIdx.sizeDelta = new Vector2(30f, 0f);

            // ── Label principal ──
            GameObject lbl = new GameObject("TXT_Label");
            lbl.transform.SetParent(btn.transform, false);
            TextMeshProUGUI tmpLbl = lbl.AddComponent<TextMeshProUGUI>();
            tmpLbl.text      = BTN_LABELS[i];
            tmpLbl.fontSize  = 15f;
            tmpLbl.fontStyle = FontStyles.Bold;
            tmpLbl.alignment = TextAlignmentOptions.Midline;
            tmpLbl.color     = esSalir ? new Color32(255, 71, 87, 220) : C_WHITE;
            tmpLbl.characterSpacing = 3f;
            RectTransform rtLbl = lbl.GetComponent<RectTransform>();
            rtLbl.anchorMin = Vector2.zero;
            rtLbl.anchorMax = Vector2.one;
            rtLbl.offsetMin = new Vector2(55f, 0f);
            rtLbl.offsetMax = Vector2.zero;
        }
    }

    // ── 6. Barra inferior ──────────────────────────────────
    static void CrearBarraInferior()
    {
        // Buscar el Canvas raíz
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) return;

        Transform existente = canvas.transform.Find("BarraInferior");
        if (existente != null) Object.DestroyImmediate(existente.gameObject);

        GameObject barra = new GameObject("BarraInferior");
        barra.transform.SetParent(canvas.transform, false);

        Image img = barra.AddComponent<Image>();
        img.color = new Color32(10, 14, 26, 200);

        RectTransform rt = barra.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 0);
        rt.pivot     = new Vector2(0.5f, 0f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(0, 44f);

        // Texto de hints
        GameObject hints = new GameObject("TXT_Hints");
        hints.transform.SetParent(barra.transform, false);
        TextMeshProUGUI tmp = hints.AddComponent<TextMeshProUGUI>();
        tmp.text     = "[W/S] Navegar    [ENTER] Seleccionar    [ESC] Atrás";
        tmp.fontSize = 11f;
        tmp.color    = new Color32(240, 244, 248, 80);
        tmp.alignment = TextAlignmentOptions.MidlineLeft;
        tmp.characterSpacing = 1f;
        RectTransform rtH = hints.GetComponent<RectTransform>();
        rtH.anchorMin = Vector2.zero;
        rtH.anchorMax = Vector2.one;
        rtH.offsetMin = new Vector2(40f, 0f);
        rtH.offsetMax = new Vector2(-40f, 0f);

        // Copyright
        GameObject copy = new GameObject("TXT_Copyright");
        copy.transform.SetParent(barra.transform, false);
        TextMeshProUGUI tmpC = copy.AddComponent<TextMeshProUGUI>();
        tmpC.text      = "© 2157 VOSS DYNAMICS";
        tmpC.fontSize  = 11f;
        tmpC.color     = new Color32(0, 198, 255, 50);
        tmpC.alignment = TextAlignmentOptions.MidlineRight;
        tmpC.characterSpacing = 2f;
        RectTransform rtC = copy.GetComponent<RectTransform>();
        rtC.anchorMin = Vector2.zero;
        rtC.anchorMax = Vector2.one;
        rtC.offsetMin = new Vector2(0f, 0f);
        rtC.offsetMax = new Vector2(-40f, 0f);
    }

    // ── 7. Esquinas decorativas ────────────────────────────
    static void CrearEsquinas()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) return;

        string[] nombres = { "Corner_TL", "Corner_TR", "Corner_BL", "Corner_BR" };
        Vector2[] anchMin = { new Vector2(0,1), new Vector2(1,1), new Vector2(0,0), new Vector2(1,0) };
        Vector2[] anchMax = { new Vector2(0,1), new Vector2(1,1), new Vector2(0,0), new Vector2(1,0) };
        Vector2[] pivots  = { new Vector2(0,1), new Vector2(1,1), new Vector2(0,0), new Vector2(1,0) };
        Vector2[] offsets = { new Vector2(20,-20), new Vector2(-20,-20), new Vector2(20,20), new Vector2(-20,20) };

        for (int i = 0; i < 4; i++)
        {
            Transform ex = canvas.transform.Find(nombres[i]);
            if (ex != null) Object.DestroyImmediate(ex.gameObject);

            GameObject corner = new GameObject(nombres[i]);
            corner.transform.SetParent(canvas.transform, false);

            Image img = corner.AddComponent<Image>();
            img.color = new Color32(0, 198, 255, 80);

            RectTransform rt = corner.GetComponent<RectTransform>();
            rt.anchorMin = anchMin[i];
            rt.anchorMax = anchMax[i];
            rt.pivot     = pivots[i];
            rt.anchoredPosition = offsets[i];
            rt.sizeDelta = new Vector2(24f, 24f);
        }
    }

    // ── Utilidad: hex a Color32 ────────────────────────────
    static Color32 Hex(string hex)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out Color c);
        return c;
    }
}
#endif
