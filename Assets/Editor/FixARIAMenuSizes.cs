// ============================================================
//  FixARIAMenuSizes.cs
//  ARIA: Último Protocolo — Ajuste de tamaños y centrado
//
//  Menú superior → ARIA → Fix Menu Sizes
// ============================================================

#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public static class FixARIAMenuSizes
{
    [MenuItem("ARIA/Fix Menu Sizes")]
    public static void FixSizes()
    {
        // ── Panel principal ──────────────────────────────
        GameObject panel = GameObject.Find("Panel_MainMenu");
        if (panel != null)
        {
            VerticalLayoutGroup vlg = panel.GetComponent<VerticalLayoutGroup>();
            if (vlg != null)
            {
                // Reducir padding lateral para que los botones sean más grandes
                vlg.padding = new RectOffset(420, 420, 310, 0);
                vlg.spacing = 10f;
            }

            // Logo más grande
            Transform logo = panel.transform.Find("TXT_Logo");
            if (logo != null)
            {
                TextMeshProUGUI tmp = logo.GetComponent<TextMeshProUGUI>();
                if (tmp != null) { tmp.fontSize = 100f; tmp.characterSpacing = 18f; }
                RectTransform rt = logo.GetComponent<RectTransform>();
                if (rt != null) rt.sizeDelta = new Vector2(700f, 130f);
            }

            // Subtítulo
            Transform sub = panel.transform.Find("TXT_Subtitulo");
            if (sub != null)
            {
                TextMeshProUGUI tmp = sub.GetComponent<TextMeshProUGUI>();
                if (tmp != null) tmp.fontSize = 17f;
                RectTransform rt = sub.GetComponent<RectTransform>();
                if (rt != null) { rt.sizeDelta = new Vector2(700f, 44f); rt.anchoredPosition = new Vector2(0, -195f); }
            }

            // Eyebrow
            Transform eye = panel.transform.Find("TXT_Eyebrow");
            if (eye != null)
            {
                RectTransform rt = eye.GetComponent<RectTransform>();
                if (rt != null) rt.anchoredPosition = new Vector2(0, -44f);
                TextMeshProUGUI tmp = eye.GetComponent<TextMeshProUGUI>();
                if (tmp != null) tmp.fontSize = 12f;
            }

            // Línea año
            Transform ano = panel.transform.Find("TXT_Ano");
            if (ano != null)
            {
                RectTransform rt = ano.GetComponent<RectTransform>();
                if (rt != null) rt.anchoredPosition = new Vector2(0, -248f);
                TextMeshProUGUI tmp = ano.GetComponent<TextMeshProUGUI>();
                if (tmp != null) tmp.fontSize = 12f;
            }

            // Divisor
            Transform div = panel.transform.Find("IMG_Divisor");
            if (div != null)
            {
                RectTransform rt = div.GetComponent<RectTransform>();
                if (rt != null) rt.anchoredPosition = new Vector2(0, -268f);
            }

            // Botones — aumentar tamaño
            string[] btns = { "BTN_NuevaPartida","BTN_Continuar","BTN_Opciones","BTN_Creditos","BTN_Salir" };
            foreach (string name in btns)
            {
                Transform t = panel.transform.Find(name);
                if (t == null) continue;
                LayoutElement le = t.GetComponent<LayoutElement>();
                if (le != null) { le.preferredHeight = 62f; le.preferredWidth = 460f; }

                // Texto más grande
                Transform lbl = t.Find("TXT_Label");
                if (lbl != null)
                {
                    TextMeshProUGUI tmp = lbl.GetComponent<TextMeshProUGUI>();
                    if (tmp != null) { tmp.fontSize = 17f; tmp.characterSpacing = 4f; }
                }
                Transform idx = t.Find("TXT_Index");
                if (idx != null)
                {
                    TextMeshProUGUI tmp = idx.GetComponent<TextMeshProUGUI>();
                    if (tmp != null) tmp.fontSize = 13f;
                }
            }
        }

        // ── Panel derecho — centrar el reloj ─────────────
        FixClock();

        // ── Paneles laterales — aumentar tamaño ──────────
        FixPanelLateral("Panel_Lore",  true);
        FixPanelLateral("Panel_Stats", false);

        EditorUtility.DisplayDialog("ARIA Fix Sizes", "✅ Tamaños corregidos.", "OK");
        Debug.Log("[ARIA] Fix de tamaños aplicado.");
    }

    static void FixClock()
    {
        // Buscar la tarjeta del reloj en Panel_Stats
        GameObject stats = GameObject.Find("Panel_Stats");
        if (stats == null) return;

        // Buscar el texto que tiene el valor del reloj (00:00:00)
        TextMeshProUGUI[] tmps = stats.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var tmp in tmps)
        {
            if (tmp.text.Contains("00:00:00") || tmp.text.Contains(":"))
            {
                // Centrar y agrandar
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.fontSize  = 28f;
                tmp.fontStyle = FontStyles.Bold;

                // Centrar su RectTransform
                RectTransform rt = tmp.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.anchorMin = new Vector2(0, rt.anchorMin.y);
                    rt.anchorMax = new Vector2(1, rt.anchorMax.y);
                    rt.offsetMin = new Vector2(8f, rt.offsetMin.y);
                    rt.offsetMax = new Vector2(-8f, rt.offsetMax.y);
                }

                // Agregar el script del reloj
                GameObject go = tmp.gameObject;
                if (go.GetComponent<ClockDisplay>() == null)
                    go.AddComponent<ClockDisplay>();

                break;
            }
        }
    }

    static void FixPanelLateral(string nombre, bool izquierdo)
    {
        GameObject panel = GameObject.Find(nombre);
        if (panel == null) return;

        RectTransform rt = panel.GetComponent<RectTransform>();
        if (rt == null) return;

        rt.sizeDelta = new Vector2(280f, 520f);

        // Ajustar posición
        float xPos = izquierdo ? 55f : -55f;
        rt.anchoredPosition = new Vector2(xPos, 0f);

        // Aumentar tamaño de textos internos
        TextMeshProUGUI[] tmps = panel.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var tmp in tmps)
        {
            if (tmp.fontSize <= 9f)  tmp.fontSize = 10f;
            if (tmp.fontSize == 12f) tmp.fontSize = 13f;
        }
    }
}
#endif
