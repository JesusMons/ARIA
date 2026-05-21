// CreateJournalAndCodeUI.cs - Assets/Editor/
// ARIA: Ultimo Protocolo - Crea Diario de Voss y Panel de Codigo

#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Events;
using TMPro;

public static class CreateJournalAndCodeUI
{
    [MenuItem("ARIA/Create Voss Journal UI")]
    public static void CrearDiario()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("ARIA", "No se encontro Canvas.", "OK");
            return;
        }

        Transform viejo = canvas.transform.Find("Panel_VossJournal");
        if (viejo != null) Object.DestroyImmediate(viejo.gameObject);
        Transform viejoHint = canvas.transform.Find("HUD_VossJournalHint");
        if (viejoHint != null) Object.DestroyImmediate(viejoHint.gameObject);

        GameObject panel = Crear("Panel_VossJournal", canvas.transform,
            Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        panel.AddComponent<Image>().color = new Color32(4, 7, 12, 232);
        CanvasGroup cg = panel.AddComponent<CanvasGroup>();
        panel.SetActive(false);

        GameObject marco = Crear("Marco", panel.transform,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            Vector2.zero, new Vector2(900f, 560f));
        marco.AddComponent<Image>().color = new Color32(10, 14, 26, 245);

        CrearBarra(marco.transform, "Borde", new Color32(0, 198, 255, 210),
            new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 0.5f), Vector2.zero, new Vector2(3f, 0f));

        TextMeshProUGUI titulo = TMP(marco.transform, "TXT_Header", "// REGISTRO DR. ELIAS VOSS", 18f,
            new Color32(240, 244, 248, 240), FontStyles.Bold,
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 1), new Vector2(24f, -22f), new Vector2(-24f, 28f));
        titulo.characterSpacing = 2f;

        TextMeshProUGUI contador = TMP(marco.transform, "TXT_Contador", "00 ARCHIVOS", 10f,
            new Color32(0, 198, 255, 150), FontStyles.Normal,
            new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-24f, -24f), new Vector2(140f, 18f));
        contador.alignment = TextAlignmentOptions.Right;

        GameObject listaPanel = Crear("ListaPanel", marco.transform,
            new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 0.5f),
            new Vector2(24f, -38f), new Vector2(280f, 450f));
        listaPanel.AddComponent<Image>().color = new Color32(0, 198, 255, 18);

        GameObject lista = Crear("Lista_Notas", listaPanel.transform,
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, Vector2.zero);
        RectTransform rtLista = lista.GetComponent<RectTransform>();
        rtLista.offsetMin = new Vector2(10f, 10f);
        rtLista.offsetMax = new Vector2(-10f, -10f);
        VerticalLayoutGroup vlg = lista.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 6f;
        vlg.childControlHeight = true;
        vlg.childControlWidth = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        GameObject prefab = Crear("Prefab_ItemNota", listaPanel.transform,
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1),
            new Vector2(0f, -10f), new Vector2(-20f, 48f));
        prefab.AddComponent<Image>().color = new Color32(7, 18, 32, 220);
        prefab.AddComponent<Button>();
        LayoutElement prefabLayout = prefab.AddComponent<LayoutElement>();
        prefabLayout.minHeight = 44f;
        prefabLayout.preferredHeight = 44f;
        prefabLayout.flexibleHeight = 0f;
        TextMeshProUGUI txtPrefab = TMP(prefab.transform, "TXT_Item", "[FIS] ARCHIVO\n<size=75%>UBICACION</size>", 10f,
            new Color32(220, 232, 240, 230), FontStyles.Normal,
            Vector2.zero, Vector2.one, new Vector2(0, 0.5f), new Vector2(10f, 0f), new Vector2(-18f, -8f));
        txtPrefab.alignment = TextAlignmentOptions.MidlineLeft;
        prefab.SetActive(false);

        TextMeshProUGUI estado = TMP(marco.transform, "TXT_Estado", "// SIN ARCHIVOS RECUPERADOS", 10f,
            new Color32(0, 198, 255, 150), FontStyles.Normal,
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 1), new Vector2(330f, -74f), new Vector2(-34f, 18f));

        TextMeshProUGUI detTitulo = TMP(marco.transform, "TXT_Titulo", "REGISTRO VACIO", 22f,
            new Color32(240, 244, 248, 240), FontStyles.Bold,
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 1), new Vector2(330f, -100f), new Vector2(-34f, 34f));

        TextMeshProUGUI meta = TMP(marco.transform, "TXT_Meta", "-- | SIN UBICACION | DR. VOSS", 11f,
            new Color32(0, 198, 255, 165), FontStyles.Normal,
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 1), new Vector2(330f, -138f), new Vector2(-34f, 20f));

        CrearBarra(marco.transform, "LineaDetalle", new Color32(0, 198, 255, 45),
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(154f, -166f), new Vector2(-340f, 1f));

        TextMeshProUGUI contenido = TMP(marco.transform, "TXT_Contenido", "Las notas encontradas quedaran almacenadas aqui.", 14f,
            new Color32(200, 210, 220, 225), FontStyles.Normal,
            new Vector2(0, 0), new Vector2(1, 1), new Vector2(0, 1), new Vector2(330f, 70f), new Vector2(-34f, -184f));
        contenido.lineSpacing = 8f;

        TextMeshProUGUI codigo = TMP(marco.transform, "TXT_Codigo", "", 13f,
            new Color32(255, 215, 0, 230), FontStyles.Bold,
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 0), new Vector2(330f, 24f), new Vector2(-34f, 28f));
        codigo.gameObject.SetActive(false);

        TextMeshProUGUI cerrar = TMP(panel.transform, "TXT_Cerrar", "[ J ] CERRAR  |  [ ESC ] CERRAR", 11f,
            new Color32(0, 198, 255, 120), FontStyles.Normal,
            new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0f, 30f), new Vector2(420f, 22f));
        cerrar.alignment = TextAlignmentOptions.Center;

        JournalUI journal = canvas.GetComponent<JournalUI>();
        if (journal == null) journal = canvas.gameObject.AddComponent<JournalUI>();
        journal.panelDiario = panel;
        journal.canvasGroup = cg;
        journal.contenedorNotas = lista.transform;
        journal.prefabItemNota = prefab;
        journal.txtContador = contador;
        journal.txtTitulo = detTitulo;
        journal.txtFecha = meta;
        journal.txtUbicacion = meta;
        journal.txtTipo = meta;
        journal.txtContenido = contenido;
        journal.txtCodigo = codigo;
        journal.txtEstado = estado;

        Button hint = CrearBoton(canvas.transform, "HUD_VossJournalHint", "[ J ] REGISTRO VOSS",
            new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(30f, -172f), new Vector2(220f, 34f));
        Image hintImg = hint.GetComponent<Image>();
        if (hintImg != null) hintImg.color = new Color32(10, 14, 26, 185);
        TextMeshProUGUI hintTxt = hint.GetComponentInChildren<TextMeshProUGUI>();
        if (hintTxt != null)
        {
            hintTxt.fontSize = 11f;
            hintTxt.color = new Color32(0, 198, 255, 180);
            hintTxt.characterSpacing = 1.5f;
        }
        UnityEventTools.AddPersistentListener(hint.onClick, journal.Abrir);

        EditorUtility.SetDirty(canvas.gameObject);
        EditorUtility.DisplayDialog("ARIA Journal", "Diario de Voss creado. Se abre con J.", "OK");
    }

    [MenuItem("ARIA/Create Code Panel UI")]
    public static void CrearPanelCodigo()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("ARIA", "No se encontro Canvas.", "OK");
            return;
        }

        Transform viejo = canvas.transform.Find("Panel_CodeAccess");
        if (viejo != null) Object.DestroyImmediate(viejo.gameObject);

        GameObject panel = Crear("Panel_CodeAccess", canvas.transform,
            Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        panel.AddComponent<Image>().color = new Color32(3, 6, 10, 205);
        CanvasGroup cg = panel.AddComponent<CanvasGroup>();
        panel.SetActive(false);

        GameObject consola = Crear("Consola", panel.transform,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            Vector2.zero, new Vector2(420f, 560f));
        consola.AddComponent<Image>().color = new Color32(9, 13, 22, 248);
        CrearBarra(consola.transform, "Borde", new Color32(255, 168, 0, 210),
            new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 0.5f), Vector2.zero, new Vector2(3f, 0f));

        TextMeshProUGUI titulo = TMP(consola.transform, "TXT_Titulo", "ACCESO RESTRINGIDO", 18f,
            new Color32(240, 244, 248, 245), FontStyles.Bold,
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(0f, -24f), new Vector2(-40f, 30f));
        titulo.alignment = TextAlignmentOptions.Center;

        TextMeshProUGUI estado = TMP(consola.transform, "TXT_Estado", "PANEL INDUSTRIAL // ARIA", 10f,
            new Color32(255, 168, 0, 150), FontStyles.Normal,
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(0f, -58f), new Vector2(-40f, 18f));
        estado.alignment = TextAlignmentOptions.Center;

        TextMeshProUGUI display = TMP(consola.transform, "TXT_Display", "____", 42f,
            new Color32(0, 255, 190, 245), FontStyles.Bold,
            new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0f, -116f), new Vector2(260f, 64f));
        display.alignment = TextAlignmentOptions.Center;
        display.characterSpacing = 12f;

        Image luzVerde = Luz(consola.transform, "LuzVerde", new Vector2(-44f, -180f), new Color32(0, 80, 60, 160));
        Image luzRoja = Luz(consola.transform, "LuzRoja", new Vector2(44f, -180f), new Color32(80, 20, 25, 160));

        GameObject keypad = Crear("Keypad", consola.transform,
            new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0f, -310f), new Vector2(240f, 230f));
        GridLayoutGroup grid = keypad.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(70f, 48f);
        grid.spacing = new Vector2(10f, 10f);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 3;

        CodePanelUI codeUI = canvas.GetComponent<CodePanelUI>();
        if (codeUI == null) codeUI = canvas.gameObject.AddComponent<CodePanelUI>();

        for (int i = 1; i <= 9; i++)
            CrearBotonNumero(keypad.transform, i.ToString(), codeUI);
        CrearBotonAccion(keypad.transform, "CLR", codeUI.Limpiar);
        CrearBotonNumero(keypad.transform, "0", codeUI);
        CrearBotonAccion(keypad.transform, "OK", codeUI.Confirmar);

        TextMeshProUGUI mensaje = TMP(consola.transform, "TXT_Mensaje", "ESPERANDO CODIGO", 13f,
            new Color32(240, 244, 248, 220), FontStyles.Bold,
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0), new Vector2(0f, 50f), new Vector2(-40f, 24f));
        mensaje.alignment = TextAlignmentOptions.Center;

        Button cerrarBtn = CrearBoton(consola.transform, "BTN_Cerrar", "CERRAR",
            new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0f, 20f), new Vector2(120f, 32f));
        UnityEventTools.AddPersistentListener(cerrarBtn.onClick, codeUI.Cerrar);

        codeUI.panelCodigo = panel;
        codeUI.canvasGroup = cg;
        codeUI.txtTitulo = titulo;
        codeUI.txtDisplay = display;
        codeUI.txtMensaje = mensaje;
        codeUI.txtEstado = estado;
        codeUI.luzVerde = luzVerde;
        codeUI.luzRoja = luzRoja;

        EditorUtility.SetDirty(canvas.gameObject);
        EditorUtility.DisplayDialog("ARIA Code Panel", "Panel de codigo creado. CodeDoor lo abre con E.", "OK");
    }

    static void CrearBotonNumero(Transform padre, string numero, CodePanelUI codeUI)
    {
        Button boton = CrearBoton(padre, "BTN_" + numero, numero,
            Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        UnityEventTools.AddStringPersistentListener(boton.onClick, codeUI.PulsarNumero, numero);
    }

    static void CrearBotonAccion(Transform padre, string texto, UnityEngine.Events.UnityAction accion)
    {
        Button boton = CrearBoton(padre, "BTN_" + texto, texto,
            Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        UnityEventTools.AddPersistentListener(boton.onClick, accion);
    }

    static Button CrearBoton(Transform padre, string nombre, string texto,
        Vector2 ancMin, Vector2 ancMax, Vector2 pivot, Vector2 pos, Vector2 size)
    {
        GameObject go = Crear(nombre, padre, ancMin, ancMax, pivot, pos, size);
        go.AddComponent<Image>().color = new Color32(15, 26, 38, 245);
        Button boton = go.AddComponent<Button>();
        TextMeshProUGUI label = TMP(go.transform, "TXT", texto, 15f,
            new Color32(240, 244, 248, 240), FontStyles.Bold,
            Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        label.alignment = TextAlignmentOptions.Center;
        return boton;
    }

    static Image Luz(Transform padre, string nombre, Vector2 pos, Color color)
    {
        GameObject go = Crear(nombre, padre,
            new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 0.5f), pos, new Vector2(22f, 22f));
        Image img = go.AddComponent<Image>();
        img.color = color;
        return img;
    }

    static void CrearBarra(Transform padre, string nombre, Color color,
        Vector2 ancMin, Vector2 ancMax, Vector2 pivot, Vector2 pos, Vector2 size)
    {
        GameObject go = Crear(nombre, padre, ancMin, ancMax, pivot, pos, size);
        go.AddComponent<Image>().color = color;
    }

    static TextMeshProUGUI TMP(Transform padre, string nombre, string texto, float size, Color color,
        FontStyles estilo, Vector2 ancMin, Vector2 ancMax, Vector2 pivot, Vector2 pos, Vector2 rectSize)
    {
        GameObject go = Crear(nombre, padre, ancMin, ancMax, pivot, pos, rectSize);
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = texto;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.fontStyle = estilo;
        tmp.enableWordWrapping = true;
        return tmp;
    }

    static GameObject Crear(string nombre, Transform padre,
        Vector2 ancMin, Vector2 ancMax, Vector2 pivot, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject(nombre, typeof(RectTransform));
        go.transform.SetParent(padre, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = ancMin;
        rt.anchorMax = ancMax;
        rt.pivot = pivot;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return go;
    }
}
#endif
