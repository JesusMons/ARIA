// CreatePauseMenuUI.cs — Assets/Editor/
// ARIA: Último Protocolo — Crea el menú de pausa en la escena de juego
// Menú → ARIA → Create Pause Menu
//
// EJECUTAR DESDE LA ESCENA DE JUEGO (Level_1)

#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public static class CreatePauseMenuUI
{
    // ── Paleta ─────────────────────────────────────────────
    static readonly Color32 COL_OVERLAY     = new Color32(5,   8,  15, 215);
    static readonly Color32 COL_CARD        = new Color32(10, 14,  26, 245);
    static readonly Color32 COL_BORDE_CYAN  = new Color32(0, 198, 255, 210);
    static readonly Color32 COL_LINEA       = new Color32(0, 198, 255,  40);
    static readonly Color32 COL_TEXTO       = new Color32(240, 244, 248, 245);
    static readonly Color32 COL_SUBTITULO   = new Color32(0, 198, 255, 150);
    static readonly Color32 COL_BTN_PAUSA   = new Color32(10,  14,  26, 200);
    static readonly Color32 COL_BTN_RESUMIR = new Color32(0,  198, 255, 210);
    static readonly Color32 COL_BTN_GUARDAR = new Color32(18, 160,  70, 215);
    static readonly Color32 COL_BTN_SALIR   = new Color32(150,  25,  25, 215);
    static readonly Color32 COL_BTN_VOLVER  = new Color32(60,  75,  90, 190);

    [MenuItem("ARIA/Create Pause Menu")]
    public static void Crear()
    {
        // Buscar canvas del juego (preferencia: el que tiene HUDManager)
        Canvas canvas = null;
        HUDManager hud = Object.FindFirstObjectByType<HUDManager>();
        if (hud != null)
            canvas = hud.GetComponent<Canvas>() ?? hud.GetComponentInParent<Canvas>();
        if (canvas == null)
            canvas = Object.FindFirstObjectByType<Canvas>();

        if (canvas == null)
        {
            EditorUtility.DisplayDialog("ARIA Pause Menu",
                "No se encontro Canvas en la escena.\n\nAbre la escena de juego (Level_1) antes de ejecutar esto.",
                "OK");
            return;
        }

        // Eliminar instancia anterior si existe
        Transform viejo = canvas.transform.Find("PauseMenuRoot");
        if (viejo != null) Object.DestroyImmediate(viejo.gameObject);

        // ── Raíz contenedora ───────────────────────────────
        GameObject raiz = UI("PauseMenuRoot", canvas.transform,
            Vector2.zero, Vector2.one,
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        // ── BTN_Pausa (HUD, esquina superior derecha) ──────
        GameObject btnPausaGO = CrearBtnPausa(raiz.transform);

        // ── Panel_Pausa (overlay full-screen) ─────────────
        GameObject panelPausa = CrearPanelPausa(raiz.transform);

        // ── PauseMenuUI en la raíz ─────────────────────────
        PauseMenuUI pauseUI = raiz.GetComponent<PauseMenuUI>();
        if (pauseUI == null) pauseUI = raiz.AddComponent<PauseMenuUI>();

        pauseUI.btnPausa     = btnPausaGO.GetComponent<Button>();
        pauseUI.panelPausa   = panelPausa;
        pauseUI.btnReanudar  = panelPausa.transform.Find("Card/BTN_Reanudar")?.GetComponent<Button>();
        pauseUI.btnGuardar   = panelPausa.transform.Find("Card/BTN_Guardar")?.GetComponent<Button>();
        pauseUI.btnSalirMenu = panelPausa.transform.Find("Card/BTN_SalirMenu")?.GetComponent<Button>();

        EditorUtility.SetDirty(canvas.gameObject);
        Selection.activeGameObject = raiz;

        EditorUtility.DisplayDialog("ARIA Pause Menu",
            "Menu de pausa creado correctamente.\n\n" +
            "  BTN_Pausa     → esquina superior derecha\n" +
            "  Escape        → tambien activa/desactiva pausa\n" +
            "  Guardar       → captura posicion + notas\n" +
            "  Salir al Menu → guarda antes de salir\n\n" +
            "PauseMenuUI asignado en PauseMenuRoot.",
            "OK");
    }

    // ── BTN_Pausa ──────────────────────────────────────────
    static GameObject CrearBtnPausa(Transform padre)
    {
        GameObject go = UI("BTN_Pausa", padre,
            new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f),
            new Vector2(-20f, -20f), new Vector2(48f, 48f));

        go.AddComponent<Image>().color = COL_BTN_PAUSA;

        // Borde inferior decorativo
        GameObject borde = UI("Borde", go.transform,
            Vector2.zero, new Vector2(1f, 0f), new Vector2(0.5f, 0f),
            Vector2.zero, new Vector2(0f, 2f));
        borde.AddComponent<Image>().color = COL_BORDE_CYAN;

        // Borde izquierdo decorativo
        GameObject bordeIzq = UI("BordeIzq", go.transform,
            Vector2.zero, new Vector2(0f, 1f), new Vector2(0f, 0.5f),
            Vector2.zero, new Vector2(2f, 0f));
        bordeIzq.AddComponent<Image>().color = new Color32(0, 198, 255, 80);

        Button boton = go.AddComponent<Button>();
        ColorBlock cb = boton.colors;
        cb.normalColor      = Color.white;
        cb.highlightedColor = new Color32(30, 50, 80, 255);
        cb.pressedColor     = new Color32(0, 130, 180, 255);
        boton.colors = cb;

        // Icono pausa "II"
        TextMeshProUGUI icono = TMP(go.transform, "TXT_Icono", "II", 15f,
            new Color32(0, 198, 255, 240), FontStyles.Bold,
            Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
            Vector2.zero, Vector2.zero);
        icono.alignment = TextAlignmentOptions.Center;
        icono.characterSpacing = 4f;

        return go;
    }

    // ── Panel_Pausa ────────────────────────────────────────
    static GameObject CrearPanelPausa(Transform padre)
    {
        // Overlay oscuro full-screen
        GameObject panel = UI("Panel_Pausa", padre,
            Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
            Vector2.zero, Vector2.zero);
        panel.AddComponent<Image>().color = COL_OVERLAY;

        // Card centrada
        GameObject card = UI("Card", panel.transform,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            Vector2.zero, new Vector2(440f, 370f));
        card.AddComponent<Image>().color = COL_CARD;

        // Borde izquierdo cyan (estilo ARIA)
        GameObject borde = UI("Borde", card.transform,
            new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, 0.5f),
            Vector2.zero, new Vector2(3f, 0f));
        borde.AddComponent<Image>().color = COL_BORDE_CYAN;

        // Borde superior decorativo tenue
        GameObject bordeSup = UI("BordeSup", card.transform,
            new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f),
            Vector2.zero, new Vector2(0f, 1f));
        bordeSup.AddComponent<Image>().color = new Color32(0, 198, 255, 80);

        // Título
        TextMeshProUGUI titulo = TMP(card.transform, "TXT_Titulo", "// SISTEMA EN PAUSA", 18f,
            COL_TEXTO, FontStyles.Bold,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0f, -54f), new Vector2(400f, 28f));
        titulo.alignment        = TextAlignmentOptions.Center;
        titulo.characterSpacing = 2f;

        // Línea separadora superior
        GameObject linea1 = UI("Linea1", card.transform,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0f, -93f), new Vector2(380f, 1f));
        linea1.AddComponent<Image>().color = COL_LINEA;

        // Subtítulo de estado
        TextMeshProUGUI estado = TMP(card.transform, "TXT_Estado",
            "SESION ACTIVA  //  REGISTRO SEGURO", 9f,
            COL_SUBTITULO, FontStyles.Normal,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0f, -110f), new Vector2(400f, 16f));
        estado.alignment        = TextAlignmentOptions.Center;
        estado.characterSpacing = 1f;

        // BTN_Reanudar
        CrearBoton(card.transform, "BTN_Reanudar", "REANUDAR",
            new Vector2(0f, -162f), new Vector2(300f, 46f), COL_BTN_RESUMIR);

        // BTN_Guardar
        CrearBoton(card.transform, "BTN_Guardar", "GUARDAR PARTIDA",
            new Vector2(0f, -220f), new Vector2(300f, 46f), COL_BTN_GUARDAR);

        // Línea separadora inferior
        GameObject linea2 = UI("Linea2", card.transform,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0f, -282f), new Vector2(380f, 1f));
        linea2.AddComponent<Image>().color = COL_LINEA;

        // BTN_SalirMenu
        CrearBoton(card.transform, "BTN_SalirMenu", "SALIR AL MENU",
            new Vector2(0f, -317f), new Vector2(300f, 40f), COL_BTN_SALIR);

        panel.SetActive(false);
        return panel;
    }

    // ── Helpers ────────────────────────────────────────────

    static Button CrearBoton(Transform padre, string nombre, string texto,
        Vector2 pos, Vector2 size, Color32 color)
    {
        GameObject go = UI(nombre, padre,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            pos, size);
        go.AddComponent<Image>().color = color;

        Button boton = go.AddComponent<Button>();
        Color c = color;
        ColorBlock cb = boton.colors;
        cb.normalColor      = Color.white;
        cb.highlightedColor = new Color(
            Mathf.Clamp01(c.r + 0.15f),
            Mathf.Clamp01(c.g + 0.15f),
            Mathf.Clamp01(c.b + 0.15f), c.a);
        cb.pressedColor = new Color(c.r * 0.65f, c.g * 0.65f, c.b * 0.65f, c.a);
        boton.colors = cb;

        TextMeshProUGUI label = TMP(go.transform, "TXT", texto, 12f,
            COL_TEXTO, FontStyles.Bold,
            Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
            Vector2.zero, Vector2.zero);
        label.alignment        = TextAlignmentOptions.Center;
        label.characterSpacing = 2f;

        return boton;
    }

    static TextMeshProUGUI TMP(Transform padre, string nombre, string texto, float size, Color color,
        FontStyles style, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 pos, Vector2 rectSize)
    {
        GameObject go = UI(nombre, padre, anchorMin, anchorMax, pivot, pos, rectSize);
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text             = texto;
        tmp.fontSize         = size;
        tmp.color            = color;
        tmp.fontStyle        = style;
        tmp.enableWordWrapping = true;
        return tmp;
    }

    static GameObject UI(string nombre, Transform padre,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject(nombre, typeof(RectTransform));
        go.transform.SetParent(padre, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin        = anchorMin;
        rt.anchorMax        = anchorMax;
        rt.pivot            = pivot;
        rt.anchoredPosition = pos;
        rt.sizeDelta        = size;
        return go;
    }
}
#endif
