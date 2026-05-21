// ObjectiveUI.cs — Assets/Scripts/UI/
// ARIA: Último Protocolo — HUD de objetivos (top-left)
// Responsable: Cristian Ramírez

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ObjectiveUI : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject        panelObjetivos;
    public TextMeshProUGUI   txtObjetivoActual;
    public TextMeshProUGUI   txtDescripcionObjetivo;
    public TextMeshProUGUI   txtTituloSeccion;
    public Transform         contenedorLista;
    public GameObject        prefabItemObjetivo;

    [Header("Configuración")]
    public float duracionFade      = 0.4f;
    public float tiempoDestello    = 3f;   // cuánto dura el destello al cambiar objetivo
    public float altoItemObjetivo  = 34f;

    [Header("Colores")]
    public Color colorActivo     = new Color32(240, 244, 248, 230);
    public Color colorCompletado = new Color32(0,   198, 255, 120);
    public Color colorDestello   = new Color32(0,   198, 255, 255);

    // ── Internos ───────────────────────────────────────────
    CanvasGroup   canvasGroup;
    Coroutine     destelloCoroutine;
    List<GameObject> itemsCreados = new List<GameObject>();

    // ──────────────────────────────────────────────────────
    void Start()
    {
        canvasGroup = panelObjetivos?.GetComponent<CanvasGroup>();
        if (canvasGroup == null && panelObjetivos != null)
            canvasGroup = panelObjetivos.AddComponent<CanvasGroup>();

        AjustarPanelExistente();
        PrepararDescripcion();
        ConfigurarLayoutLista();

        // Suscribir a eventos del ObjectiveManager
        if (ObjectiveManager.instancia != null)
        {
            ObjectiveManager.instancia.onObjetivoActualizado
                .AddListener(OnObjetivoActualizado);
            ObjectiveManager.instancia.onObjetivoActivado
                .AddListener(OnObjetivoActivado);
        }

        ActualizarUI();
    }

    // ── Eventos ────────────────────────────────────────────
    void OnObjetivoActualizado(Objetivo obj)
    {
        ActualizarUI();
    }

    void OnObjetivoActivado(Objetivo obj)
    {
        ActualizarUI();

        // Destello al aparecer nuevo objetivo
        if (destelloCoroutine != null) StopCoroutine(destelloCoroutine);
        destelloCoroutine = StartCoroutine(DestellarObjetivo());
    }

    // ── Actualizar UI ──────────────────────────────────────
    void ActualizarUI()
    {
        ConfigurarLayoutLista();

        // Objetivo actual en el texto principal
        Objetivo actual = ObjectiveManager.ObtenerActual();
        if (txtObjetivoActual != null)
        {
            if (actual != null)
            {
                txtObjetivoActual.text  = "▶  " + actual.titulo.ToUpper();
                txtObjetivoActual.color = colorActivo;
            }
            else
            {
                txtObjetivoActual.text  = "// TODOS LOS OBJETIVOS COMPLETADOS";
                txtObjetivoActual.color = colorCompletado;
            }
        }

        if (txtDescripcionObjetivo != null)
        {
            if (actual != null)
            {
                txtDescripcionObjetivo.text = actual.descripcion;
                txtDescripcionObjetivo.color = colorActivo;
            }
            else
            {
                txtDescripcionObjetivo.text = "No quedan instrucciones pendientes.";
                txtDescripcionObjetivo.color = colorCompletado;
            }
        }

        // Lista completa de objetivos visibles
        if (contenedorLista != null)
        {
            // Limpiar items anteriores
            foreach (var item in itemsCreados)
                if (item != null) Destroy(item);
            itemsCreados.Clear();

            List<Objetivo> visibles = ObjectiveManager.ObtenerVisibles();
            foreach (Objetivo obj in visibles)
            {
                if (prefabItemObjetivo == null) continue;

                GameObject go = Instantiate(prefabItemObjetivo, contenedorLista);
                ConfigurarItemObjetivo(go);
                TextMeshProUGUI txt = go.GetComponentInChildren<TextMeshProUGUI>();

                if (txt != null)
                {
                    string estado = obj.completado ? "✓  " : "○  ";
                    txt.text  = estado + obj.titulo +
                        $"\n<size=80%><color=#9FB6C8>{obj.descripcion}</color></size>";
                    txt.color = obj.completado ? colorCompletado : colorActivo;
                    txt.fontStyle = obj.completado
                        ? FontStyles.Strikethrough
                        : FontStyles.Normal;
                }
                itemsCreados.Add(go);
            }
        }
    }

    void PrepararDescripcion()
    {
        if (txtDescripcionObjetivo == null && panelObjetivos != null)
        {
            Transform desc = panelObjetivos.transform.Find("TXT_DescripcionObjetivo");
            if (desc != null)
                txtDescripcionObjetivo = desc.GetComponent<TextMeshProUGUI>();
        }

        if (txtDescripcionObjetivo != null || panelObjetivos == null)
            return;

        GameObject go = new GameObject("TXT_DescripcionObjetivo", typeof(RectTransform));
        go.transform.SetParent(panelObjetivos.transform, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = new Vector2(12f, -54f);
        rt.sizeDelta = new Vector2(-12f, 34f);

        txtDescripcionObjetivo = go.AddComponent<TextMeshProUGUI>();
        txtDescripcionObjetivo.fontSize = 10f;
        txtDescripcionObjetivo.color = new Color32(200, 210, 220, 185);
        txtDescripcionObjetivo.enableWordWrapping = true;
        txtDescripcionObjetivo.lineSpacing = 4f;
    }

    void AjustarPanelExistente()
    {
        if (panelObjetivos == null) return;

        RectTransform panelRt = panelObjetivos.GetComponent<RectTransform>();
        if (panelRt != null && panelRt.sizeDelta.y < 180f)
            panelRt.sizeDelta = new Vector2(Mathf.Max(panelRt.sizeDelta.x, 360f), 180f);

        Transform linea = panelObjetivos.transform.Find("Linea");
        if (linea != null)
        {
            RectTransform rt = linea.GetComponent<RectTransform>();
            if (rt != null)
                rt.anchoredPosition = new Vector2(0f, -94f);
        }

        if (contenedorLista != null)
        {
            RectTransform rtLista = contenedorLista.GetComponent<RectTransform>();
            if (rtLista != null)
            {
                rtLista.offsetMin = new Vector2(12f, 8f);
                rtLista.offsetMax = new Vector2(-8f, -100f);
            }
        }
    }

    void ConfigurarLayoutLista()
    {
        if (contenedorLista == null) return;

        VerticalLayoutGroup layout = contenedorLista.GetComponent<VerticalLayoutGroup>();
        if (layout == null) return;

        layout.spacing = 5f;
        layout.childControlHeight = true;
        layout.childForceExpandHeight = false;
        layout.childControlWidth = true;
        layout.childForceExpandWidth = true;
    }

    void ConfigurarItemObjetivo(GameObject item)
    {
        if (item == null) return;

        LayoutElement layoutElement = item.GetComponent<LayoutElement>();
        if (layoutElement == null) layoutElement = item.AddComponent<LayoutElement>();

        layoutElement.minHeight = altoItemObjetivo;
        layoutElement.preferredHeight = altoItemObjetivo;
        layoutElement.flexibleHeight = 0f;

        RectTransform rt = item.GetComponent<RectTransform>();
        if (rt != null)
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, altoItemObjetivo);
    }

    // ── Destello al nuevo objetivo ─────────────────────────
    IEnumerator DestellarObjetivo()
    {
        if (txtObjetivoActual == null) yield break;

        float t = 0f;
        Color original = txtObjetivoActual.color;

        while (t < tiempoDestello)
        {
            t += Time.deltaTime;
            float ping = Mathf.PingPong(t * 3f, 1f);
            txtObjetivoActual.color = Color.Lerp(original, colorDestello, ping);
            yield return null;
        }

        txtObjetivoActual.color = original;
    }

    // ── Mostrar/ocultar panel ──────────────────────────────
    public void MostrarPanel(bool mostrar)
    {
        if (canvasGroup == null) return;
        StartCoroutine(FadePanel(mostrar ? 1f : 0f));
    }

    IEnumerator FadePanel(float objetivo)
    {
        float inicio = canvasGroup.alpha;
        float t = 0f;
        while (t < duracionFade)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(inicio, objetivo, t / duracionFade);
            yield return null;
        }
        canvasGroup.alpha = objetivo;
    }
}
