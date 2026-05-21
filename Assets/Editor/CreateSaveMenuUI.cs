// CreateSaveMenuUI.cs - Assets/Editor/
// ARIA: crea paneles de Nueva Partida y Continuar.

#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public static class CreateSaveMenuUI
{
    [MenuItem("ARIA/Create Save Menu Panels")]
    public static void Crear()
    {
        MenuManager menu = Object.FindFirstObjectByType<MenuManager>();
        if (menu == null)
        {
            EditorUtility.DisplayDialog("ARIA Saves", "No se encontro MenuManager en la escena.", "OK");
            return;
        }

        Transform padre = menu.menuPanel != null ? menu.menuPanel.transform.parent : menu.transform;

        Transform viejoNueva = padre.Find("Panel_NuevaPartida");
        if (viejoNueva != null) Object.DestroyImmediate(viejoNueva.gameObject);

        Transform viejoContinuar = padre.Find("Panel_ContinuarPartidas");
        if (viejoContinuar != null) Object.DestroyImmediate(viejoContinuar.gameObject);

        GameObject panelNueva = CrearPanelBase("Panel_NuevaPartida", padre);
        Transform nuevaCard = panelNueva.transform.Find("Card");
        TextMeshProUGUI tituloNueva = TMP(nuevaCard, "TXT_Titulo", "// NUEVA PARTIDA", 18f,
            new Color32(240, 244, 248, 245), FontStyles.Bold,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0f, -72f), new Vector2(420f, 30f));
        tituloNueva.alignment = TextAlignmentOptions.Center;

        TMP(nuevaCard, "TXT_Label", "NOMBRE DEL REGISTRO", 10f,
            new Color32(0, 198, 255, 160), FontStyles.Normal,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0f, -122f), new Vector2(420f, 20f)).alignment = TextAlignmentOptions.Center;

        TMP_InputField input = CrearInput(nuevaCard, "INPUT_NombrePartida",
            new Vector2(0f, -166f), new Vector2(420f, 44f));

        TextMeshProUGUI error = TMP(nuevaCard, "TXT_Error", "", 10f,
            new Color32(255, 71, 87, 230), FontStyles.Bold,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0f, -204f), new Vector2(420f, 20f));
        error.alignment = TextAlignmentOptions.Center;

        Button crear = CrearBoton(nuevaCard, "BTN_CrearPartida", "INICIAR SESION",
            new Vector2(0f, -254f), new Vector2(260f, 40f), new Color32(0, 198, 255, 210));
        Button cancelarNueva = CrearBoton(nuevaCard, "BTN_CancelarNueva", "VOLVER",
            new Vector2(0f, -306f), new Vector2(180f, 34f), new Color32(80, 95, 110, 190));

        GameObject panelContinuar = CrearPanelBase("Panel_ContinuarPartidas", padre);
        Transform continuarCard = panelContinuar.transform.Find("Card");
        TextMeshProUGUI tituloContinuar = TMP(continuarCard, "TXT_Titulo", "// CONTINUAR PARTIDA", 18f,
            new Color32(240, 244, 248, 245), FontStyles.Bold,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0f, -56f), new Vector2(480f, 30f));
        tituloContinuar.alignment = TextAlignmentOptions.Center;

        GameObject lista = UI("Lista_Partidas", continuarCard,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0f, -220f), new Vector2(520f, 250f));
        VerticalLayoutGroup layout = lista.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 8f;
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;

        GameObject prefab = UI("Prefab_Partida", continuarCard,
            new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f),
            Vector2.zero, new Vector2(0f, 54f));
        prefab.AddComponent<Image>().color = new Color32(7, 18, 32, 225);
        prefab.AddComponent<Button>();
        LayoutElement prefabLayout = prefab.AddComponent<LayoutElement>();
        prefabLayout.minHeight = 64f;
        prefabLayout.preferredHeight = 64f;
        prefabLayout.flexibleHeight = 0f;
        TextMeshProUGUI prefabTxt = TMP(prefab.transform, "TXT_Partida", "PARTIDA\n<size=75%>ESCENA | FECHA</size>", 12f,
            new Color32(230, 238, 245, 235), FontStyles.Bold,
            Vector2.zero, Vector2.one, new Vector2(0f, 0.5f),
            new Vector2(14f, 0f), new Vector2(-28f, -8f));
        prefabTxt.fontSize = 15f;
        prefabTxt.alignment = TextAlignmentOptions.MidlineLeft;
        prefab.SetActive(false);

        TextMeshProUGUI sinPartidas = TMP(continuarCard, "TXT_SinPartidas", "NO HAY PARTIDAS GUARDADAS", 12f,
            new Color32(255, 215, 0, 210), FontStyles.Bold,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0f, -176f), new Vector2(420f, 24f));
        sinPartidas.alignment = TextAlignmentOptions.Center;

        Button volverContinuar = CrearBoton(continuarCard, "BTN_VolverContinuar", "VOLVER",
            new Vector2(0f, -430f), new Vector2(180f, 34f), new Color32(80, 95, 110, 190));

        panelNueva.SetActive(false);
        panelContinuar.SetActive(false);

        MenuSaveUI saveUI = menu.GetComponent<MenuSaveUI>();
        if (saveUI == null) saveUI = menu.gameObject.AddComponent<MenuSaveUI>();

        saveUI.menuManager = menu;
        saveUI.panelNuevaPartida = panelNueva;
        saveUI.inputNombrePartida = input;
        saveUI.txtErrorNuevaPartida = error;
        saveUI.btnCrearPartida = crear;
        saveUI.btnCancelarNueva = cancelarNueva;
        saveUI.panelContinuar = panelContinuar;
        saveUI.contenedorPartidas = lista.transform;
        saveUI.prefabPartida = prefab;
        saveUI.txtSinPartidas = sinPartidas;
        saveUI.btnVolverContinuar = volverContinuar;

        menu.saveUI = saveUI;
        EditorUtility.SetDirty(menu.gameObject);

        EditorUtility.DisplayDialog("ARIA Saves",
            "Paneles de Nueva Partida y Continuar creados.\nConectados al MenuManager.", "OK");
    }

    static GameObject CrearPanelBase(string nombre, Transform padre)
    {
        GameObject panel = UI(nombre, padre, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        panel.AddComponent<Image>().color = new Color32(5, 8, 15, 232);

        GameObject card = UI("Card", panel.transform,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            Vector2.zero, new Vector2(620f, 520f));
        card.AddComponent<Image>().color = new Color32(10, 14, 26, 245);

        GameObject borde = UI("Borde", card.transform,
            new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, 0.5f),
            Vector2.zero, new Vector2(3f, 0f));
        borde.AddComponent<Image>().color = new Color32(0, 198, 255, 210);

        return panel;
    }

    static TMP_InputField CrearInput(Transform padre, string nombre, Vector2 pos, Vector2 size)
    {
        GameObject go = UI(nombre, padre,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), pos, size);
        go.AddComponent<Image>().color = new Color32(7, 18, 32, 245);

        TMP_InputField input = go.AddComponent<TMP_InputField>();
        TextMeshProUGUI text = TMP(go.transform, "Text", "", 15f,
            new Color32(240, 244, 248, 240), FontStyles.Bold,
            Vector2.zero, Vector2.one, new Vector2(0f, 0.5f),
            new Vector2(14f, 0f), new Vector2(-28f, -8f));
        TextMeshProUGUI placeholder = TMP(go.transform, "Placeholder", "NOMBRE DE LA PARTIDA", 13f,
            new Color32(120, 145, 160, 150), FontStyles.Normal,
            Vector2.zero, Vector2.one, new Vector2(0f, 0.5f),
            new Vector2(14f, 0f), new Vector2(-28f, -8f));

        input.textComponent = text;
        input.placeholder = placeholder;
        input.characterLimit = 32;
        return input;
    }

    static Button CrearBoton(Transform padre, string nombre, string texto, Vector2 pos, Vector2 size, Color color)
    {
        GameObject go = UI(nombre, padre,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), pos, size);
        go.AddComponent<Image>().color = color;
        Button boton = go.AddComponent<Button>();
        TextMeshProUGUI label = TMP(go.transform, "TXT", texto, 12f,
            new Color32(240, 244, 248, 245), FontStyles.Bold,
            Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        label.alignment = TextAlignmentOptions.Center;
        label.characterSpacing = 2f;
        return boton;
    }

    static TextMeshProUGUI TMP(Transform padre, string nombre, string texto, float size, Color color,
        FontStyles style, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 pos, Vector2 rectSize)
    {
        GameObject go = UI(nombre, padre, anchorMin, anchorMax, pivot, pos, rectSize);
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = texto;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.fontStyle = style;
        tmp.enableWordWrapping = true;
        return tmp;
    }

    static GameObject UI(string nombre, Transform padre,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject(nombre, typeof(RectTransform));
        go.transform.SetParent(padre, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return go;
    }
}
#endif
