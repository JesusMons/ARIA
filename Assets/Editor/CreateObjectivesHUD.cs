// CreateObjectivesHUD.cs — Assets/Editor/
// ARIA: Último Protocolo — Crea el HUD de objetivos
// Menú → ARIA → Create Objectives HUD

#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public static class CreateObjectivesHUD
{
    [MenuItem("ARIA/Create Objectives HUD")]
    public static void Crear()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("ARIA", "No se encontró Canvas.", "OK");
            return;
        }

        // Limpiar anterior
        Transform viejo = canvas.transform.Find("HUD_Objetivos");
        if (viejo != null) Object.DestroyImmediate(viejo.gameObject);

        // ── Panel principal ────────────────────────────
        GameObject panelGO = CrearUIElement("HUD_Objetivos", canvas.transform,
            new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f),
            new Vector2(30f, -30f), new Vector2(360f, 180f));

        panelGO.AddComponent<CanvasGroup>().alpha = 1f;
        panelGO.AddComponent<Image>().color = new Color32(10, 14, 26, 180);

        // ── Borde izquierdo azul ───────────────────────
        GameObject borde = CrearUIElement("Borde", panelGO.transform,
            new Vector2(0,0), new Vector2(0,1), new Vector2(0,0.5f),
            Vector2.zero, new Vector2(2f, 0f));
        borde.AddComponent<Image>().color = new Color32(0, 198, 255, 200);

        // ── Etiqueta sección ───────────────────────────
        GameObject seccionGO = CrearUIElement("TXT_Seccion", panelGO.transform,
            new Vector2(0,1), new Vector2(1,1), new Vector2(0,1),
            new Vector2(12f, -10f), new Vector2(-12f, 16f));
        TextMeshProUGUI txtSec = seccionGO.AddComponent<TextMeshProUGUI>();
        txtSec.text      = "// OBJETIVO ACTUAL";
        txtSec.fontSize  = 9f;
        txtSec.color     = new Color32(0, 198, 255, 140);
        txtSec.characterSpacing = 2f;

        // ── Objetivo actual ────────────────────────────
        GameObject objGO = CrearUIElement("TXT_ObjetivoActual", panelGO.transform,
            new Vector2(0,1), new Vector2(1,1), new Vector2(0,1),
            new Vector2(12f, -30f), new Vector2(-12f, 22f));
        TextMeshProUGUI txtObj = objGO.AddComponent<TextMeshProUGUI>();
        txtObj.text      = "EVALUANDO ENTORNO...";
        txtObj.fontSize  = 13f;
        txtObj.fontStyle = FontStyles.Bold;
        txtObj.color     = new Color32(240, 244, 248, 230);
        txtObj.characterSpacing = 1f;

        // ── Descripción del objetivo actual ────────────
        GameObject descGO = CrearUIElement("TXT_DescripcionObjetivo", panelGO.transform,
            new Vector2(0,1), new Vector2(1,1), new Vector2(0,1),
            new Vector2(12f, -54f), new Vector2(-12f, 34f));
        TextMeshProUGUI txtDesc = descGO.AddComponent<TextMeshProUGUI>();
        txtDesc.text        = "Algo no va bien. Explora el laboratorio.";
        txtDesc.fontSize    = 10f;
        txtDesc.color       = new Color32(200, 210, 220, 185);
        txtDesc.lineSpacing = 4f;
        txtDesc.enableWordWrapping = true;

        // ── Línea separadora ───────────────────────────
        GameObject lineaGO = CrearUIElement("Linea", panelGO.transform,
            new Vector2(0,1), new Vector2(1,1), new Vector2(0.5f,1f),
            new Vector2(0f, -94f), new Vector2(-12f, 1f));
        lineaGO.AddComponent<Image>().color = new Color32(0, 198, 255, 35);

        // ── Contenedor lista ───────────────────────────
        GameObject listaGO = CrearUIElement("Lista_Objetivos", panelGO.transform,
            Vector2.zero, Vector2.one, Vector2.zero,
            Vector2.zero, Vector2.zero);
        RectTransform rtLista = listaGO.GetComponent<RectTransform>();
        rtLista.offsetMin = new Vector2(12f, 8f);
        rtLista.offsetMax = new Vector2(-8f, -100f);

        VerticalLayoutGroup vlg = listaGO.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 5f;
        vlg.childControlHeight    = true;
        vlg.childControlWidth     = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        // ── Prefab item ────────────────────────────────
        GameObject prefabGO = CrearUIElement("Prefab_Item", panelGO.transform,
            Vector2.zero, Vector2.one, Vector2.zero,
            Vector2.zero, new Vector2(0f, 34f));
        prefabGO.SetActive(false);
        LayoutElement itemLayout = prefabGO.AddComponent<LayoutElement>();
        itemLayout.minHeight = 34f;
        itemLayout.preferredHeight = 34f;
        itemLayout.flexibleHeight = 0f;
        TextMeshProUGUI txtPrefab = prefabGO.AddComponent<TextMeshProUGUI>();
        txtPrefab.fontSize = 11f;
        txtPrefab.color    = new Color32(240, 244, 248, 140);
        txtPrefab.enableWordWrapping = true;

        // ── Asignar ObjectiveUI ────────────────────────
        ObjectiveUI ui = panelGO.AddComponent<ObjectiveUI>();
        ui.panelObjetivos     = panelGO;
        ui.txtObjetivoActual  = txtObj;
        ui.txtDescripcionObjetivo = txtDesc;
        ui.txtTituloSeccion   = txtSec;
        ui.contenedorLista    = listaGO.transform;
        ui.prefabItemObjetivo = prefabGO;

        // ── ObjectiveManager en GameManager ───────────
        GameObject gm = GameObject.Find("GameManager");
        if (gm != null && gm.GetComponent<ObjectiveManager>() == null)
            gm.AddComponent<ObjectiveManager>();

        EditorUtility.SetDirty(canvas.gameObject);
        EditorUtility.DisplayDialog("ARIA Objectives",
            "✅ HUD de objetivos creado.\n\nEsquina superior izquierda.", "OK");
    }

    // ── Helper: crea UI element con RectTransform correcto ──
    static GameObject CrearUIElement(string nombre, Transform padre,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
        Vector2 posicion, Vector2 size)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(padre, false);

        // RectTransform se agrega automáticamente al parentear a Canvas
        // pero lo forzamos por si acaso
        RectTransform rt = go.GetComponent<RectTransform>();
        if (rt == null) rt = go.AddComponent<RectTransform>();

        rt.anchorMin        = anchorMin;
        rt.anchorMax        = anchorMax;
        rt.pivot            = pivot;
        rt.anchoredPosition = posicion;
        rt.sizeDelta        = size;

        return go;
    }
}
#endif
