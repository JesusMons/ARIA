// CreateARIAHUD.cs — Assets/Editor/
// ARIA: Último Protocolo — Crea el HUD de alertas estilizado
// Menú → ARIA → Create HUD Alert

#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public static class CreateARIAHUD
{
    [MenuItem("ARIA/Create HUD Alert")]
    public static void CrearHUD()
    {
        // Buscar Canvas en la escena
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("ARIA HUD",
                "No se encontró Canvas en la escena.", "OK");
            return;
        }

        // Eliminar HUD anterior si existe
        Transform viejo = canvas.transform.Find("HUD_Alert");
        if (viejo != null) Object.DestroyImmediate(viejo.gameObject);

        // ── Contenedor principal ───────────────────────
        GameObject hudGO = new GameObject("HUD_Alert");
        hudGO.transform.SetParent(canvas.transform, false);

        RectTransform rtHud = hudGO.AddComponent<RectTransform>();
        rtHud.anchorMin        = new Vector2(0f, 0f);
        rtHud.anchorMax        = new Vector2(0f, 0f);
        rtHud.pivot            = new Vector2(0f, 0f);
        rtHud.anchoredPosition = new Vector2(40f, 80f);
        rtHud.sizeDelta        = new Vector2(320f, 52f);

        // ── Fondo oscuro ───────────────────────────────
        Image fondo = hudGO.AddComponent<Image>();
        fondo.color = new Color32(10, 14, 26, 210);

        // ── Borde izquierdo azul ───────────────────────
        GameObject borde = new GameObject("Borde");
        borde.transform.SetParent(hudGO.transform, false);
        Image imgBorde = borde.AddComponent<Image>();
        imgBorde.color = new Color32(0, 198, 255, 255);
        RectTransform rtBorde = borde.GetComponent<RectTransform>();
        rtBorde.anchorMin        = new Vector2(0f, 0f);
        rtBorde.anchorMax        = new Vector2(0f, 1f);
        rtBorde.pivot            = new Vector2(0f, 0.5f);
        rtBorde.anchoredPosition = Vector2.zero;
        rtBorde.sizeDelta        = new Vector2(2f, 0f);

        // ── Icono indicador ────────────────────────────
        GameObject iconoGO = new GameObject("TXT_Icono");
        iconoGO.transform.SetParent(hudGO.transform, false);
        TextMeshProUGUI icono = iconoGO.AddComponent<TextMeshProUGUI>();
        icono.text      = "▶";
        icono.fontSize  = 11f;
        icono.color     = new Color32(0, 198, 255, 180);
        icono.alignment = TextAlignmentOptions.MidlineLeft;
        RectTransform rtIcono = iconoGO.GetComponent<RectTransform>();
        rtIcono.anchorMin        = new Vector2(0f, 0f);
        rtIcono.anchorMax        = new Vector2(0f, 1f);
        rtIcono.pivot            = new Vector2(0f, 0.5f);
        rtIcono.anchoredPosition = new Vector2(12f, 0f);
        rtIcono.sizeDelta        = new Vector2(20f, 0f);

        // ── Texto del mensaje ──────────────────────────
        GameObject txtGO = new GameObject("TXT_Mensaje");
        txtGO.transform.SetParent(hudGO.transform, false);
        TextMeshProUGUI txt = txtGO.AddComponent<TextMeshProUGUI>();
        txt.text             = "";
        txt.fontSize         = 13f;
        txt.color            = new Color32(240, 244, 248, 230);
        txt.alignment        = TextAlignmentOptions.MidlineLeft;
        txt.characterSpacing = 2f;
        txt.fontStyle        = FontStyles.Bold;
        RectTransform rtTxt = txtGO.GetComponent<RectTransform>();
        rtTxt.anchorMin        = new Vector2(0f, 0f);
        rtTxt.anchorMax        = new Vector2(1f, 1f);
        rtTxt.pivot            = new Vector2(0f, 0.5f);
        rtTxt.offsetMin        = new Vector2(38f, 0f);
        rtTxt.offsetMax        = new Vector2(-12f, 0f);

        // ── Línea inferior decorativa ──────────────────
        GameObject linea = new GameObject("Linea");
        linea.transform.SetParent(hudGO.transform, false);
        Image imgLinea = linea.AddComponent<Image>();
        imgLinea.color = new Color32(0, 198, 255, 40);
        RectTransform rtLinea = linea.GetComponent<RectTransform>();
        rtLinea.anchorMin        = new Vector2(0f, 0f);
        rtLinea.anchorMax        = new Vector2(1f, 0f);
        rtLinea.pivot            = new Vector2(0.5f, 0f);
        rtLinea.anchoredPosition = Vector2.zero;
        rtLinea.sizeDelta        = new Vector2(0f, 1f);

        // ── HUDManager actualizado ─────────────────────
        // Eliminar HUDManager viejo del Canvas
        HUDManager hudViejo = canvas.GetComponent<HUDManager>();
        if (hudViejo != null) Object.DestroyImmediate(hudViejo);

        // Agregar HUDManager al Canvas
        HUDManager hud = canvas.gameObject.AddComponent<HUDManager>();
        hud.txtMensaje = txt;
        hud.txtIcono   = icono;

        EditorUtility.SetDirty(canvas.gameObject);
        EditorUtility.DisplayDialog("ARIA HUD",
            "HUD creado en la esquina inferior izquierda.\n\n" +
            "El HUDManager fue actualizado con referencia al ícono.", "OK");
    }
}
#endif
