// CreateNoteReaderUI.cs — Assets/Editor/
// ARIA: Último Protocolo — Crea la UI de lectura de notas
// Menú → ARIA → Create Note Reader UI

#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public static class CreateNoteReaderUI
{
    [MenuItem("ARIA/Create Note Reader UI")]
    public static void Crear()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("ARIA", "No se encontró Canvas.", "OK");
            return;
        }

        Transform viejo = canvas.transform.Find("Panel_NoteReader");
        if (viejo != null) Object.DestroyImmediate(viejo.gameObject);

        // ── Panel fondo (pantalla completa) ────────────
        GameObject panelGO = Crear("Panel_NoteReader", canvas.transform,
            Vector2.zero, Vector2.one, new Vector2(0.5f,0.5f),
            Vector2.zero, Vector2.zero);
        panelGO.AddComponent<Image>().color = new Color32(5, 8, 15, 220);
        panelGO.SetActive(false);

        // ── Carta central ──────────────────────────────
        GameObject carta = Crear("Carta", panelGO.transform,
            new Vector2(0.5f,0.5f), new Vector2(0.5f,0.5f),
            new Vector2(0.5f,0.5f), Vector2.zero, new Vector2(700f, 500f));
        carta.AddComponent<Image>().color = new Color32(10, 14, 26, 245);

        // Borde izquierdo
        GameObject borde = Crear("Borde", carta.transform,
            new Vector2(0,0), new Vector2(0,1), new Vector2(0,0.5f),
            Vector2.zero, new Vector2(3f, 0f));
        borde.AddComponent<Image>().color = new Color32(0, 198, 255, 200);

        // Línea superior
        GameObject lineaSup = Crear("LineaSup", carta.transform,
            new Vector2(0,1), new Vector2(1,1), new Vector2(0.5f,1f),
            Vector2.zero, new Vector2(0f, 1f));
        lineaSup.AddComponent<Image>().color = new Color32(0, 198, 255, 60);

        // ── Header ────────────────────────────────────
        GameObject header = Crear("Header", carta.transform,
            new Vector2(0,1), new Vector2(1,1), new Vector2(0,1),
            new Vector2(20f, -20f), new Vector2(-20f, 80f));

        GameObject lblTipo = Crear("LBL_Tipo", header.transform,
            Vector2.zero, Vector2.one, new Vector2(0,1),
            new Vector2(0f, -4f), new Vector2(0f, 16f));
        TextMeshProUGUI txtTipo = lblTipo.AddComponent<TextMeshProUGUI>();
        txtTipo.text      = "// ARCHIVO CLASIFICADO - PROYECTO ARIA";
        txtTipo.fontSize  = 9f;
        txtTipo.color     = new Color32(0, 198, 255, 140);
        txtTipo.characterSpacing = 2f;

        GameObject lblTitulo = Crear("TXT_Titulo", header.transform,
            Vector2.zero, Vector2.one, new Vector2(0,1),
            new Vector2(0f, -22f), new Vector2(0f, 30f));
        TextMeshProUGUI txtTitulo = lblTitulo.AddComponent<TextMeshProUGUI>();
        txtTitulo.text      = "TITULO DE LA NOTA";
        txtTitulo.fontSize  = 20f;
        txtTitulo.fontStyle = FontStyles.Bold;
        txtTitulo.color     = new Color32(240, 244, 248, 240);
        txtTitulo.characterSpacing = 2f;

        GameObject lblMeta = Crear("TXT_Meta", header.transform,
            Vector2.zero, Vector2.one, new Vector2(0,1),
            new Vector2(0f, -56f), new Vector2(0f, 18f));
        TextMeshProUGUI txtMeta = lblMeta.AddComponent<TextMeshProUGUI>();
        txtMeta.text     = "2157.01.08  |  Laboratorio Principal";
        txtMeta.fontSize = 11f;
        txtMeta.color    = new Color32(0, 198, 255, 160);

        // ── Línea divisora ─────────────────────────────
        GameObject lineaDiv = Crear("LineaDiv", carta.transform,
            new Vector2(0,1), new Vector2(1,1), new Vector2(0.5f,1f),
            new Vector2(0f, -110f), new Vector2(-40f, 1f));
        lineaDiv.AddComponent<Image>().color = new Color32(0, 198, 255, 40);

        // ── Contenido ──────────────────────────────────
        GameObject contenidoGO = Crear("TXT_Contenido", carta.transform,
            new Vector2(0,0), new Vector2(1,1), new Vector2(0,1),
            new Vector2(20f, 60f), new Vector2(-20f, -120f));
        TextMeshProUGUI txtContenido = contenidoGO.AddComponent<TextMeshProUGUI>();
        txtContenido.text        = "";
        txtContenido.fontSize    = 14f;
        txtContenido.color       = new Color32(200, 210, 220, 220);
        txtContenido.lineSpacing = 8f;

        // ── Código de acceso ───────────────────────────
        GameObject codigoGO = Crear("TXT_Codigo", carta.transform,
            new Vector2(0,0), new Vector2(1,0), new Vector2(0,0),
            new Vector2(20f, 20f), new Vector2(-20f, 32f));
        TextMeshProUGUI txtCodigo = codigoGO.AddComponent<TextMeshProUGUI>();
        txtCodigo.text      = "";
        txtCodigo.fontSize  = 13f;
        txtCodigo.color     = new Color32(255, 215, 0, 220);
        txtCodigo.fontStyle = FontStyles.Bold;
        codigoGO.SetActive(false);

        // ── Instrucción cerrar ─────────────────────────
        GameObject cerrarGO = Crear("TXT_Cerrar", panelGO.transform,
            new Vector2(0.5f,0), new Vector2(0.5f,0), new Vector2(0.5f,0),
            new Vector2(0f, 30f), new Vector2(400f, 22f));
        TextMeshProUGUI txtCerrar = cerrarGO.AddComponent<TextMeshProUGUI>();
        txtCerrar.text      = "[ E ] CERRAR  |  [ ESC ] CERRAR";
        txtCerrar.fontSize  = 11f;
        txtCerrar.alignment = TextAlignmentOptions.Center;
        txtCerrar.color     = new Color32(0, 198, 255, 100);
        txtCerrar.characterSpacing = 2f;

        // ── NoteReaderUI en Canvas ─────────────────────
        NoteReaderUI reader = canvas.GetComponent<NoteReaderUI>();
        if (reader == null) reader = canvas.gameObject.AddComponent<NoteReaderUI>();
        reader.panelNota    = panelGO;
        reader.txtTitulo    = txtTitulo;
        reader.txtFecha     = txtMeta;
        reader.txtUbicacion = txtMeta;
        reader.txtContenido = txtContenido;
        reader.txtCodigo    = txtCodigo;

        EditorUtility.SetDirty(canvas.gameObject);
        EditorUtility.DisplayDialog("ARIA Note Reader",
            "Panel de lectura creado.\nSe abre con E al leer una nota.", "OK");
    }

    // ── Helper: crea UI element con RectTransform ──────────
    static GameObject Crear(string nombre, Transform padre,
        Vector2 ancMin, Vector2 ancMax, Vector2 pivot,
        Vector2 pos, Vector2 size)
    {
        // typeof(RectTransform) en el constructor evita el error
        GameObject go = new GameObject(nombre, typeof(RectTransform));
        go.transform.SetParent(padre, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin        = ancMin;
        rt.anchorMax        = ancMax;
        rt.pivot            = pivot;
        rt.anchoredPosition = pos;
        rt.sizeDelta        = size;
        return go;
    }
}
#endif