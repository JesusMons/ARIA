// UIManager.cs — Assets/Scripts/UI/
// ARIA: Último Protocolo — Gestor global de UI
// Responsable: Cristian Ramírez

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager instancia;

    [Header("Todos los paneles de la UI")]
    public GameObject[] canvases;

    [Header("Raiz visual del menu")]
    public GameObject canvasMenu;

    [Header("Transición")]
    public CanvasGroup fadeOverlay;
    public float duracionFade = 0.15f;

    [Header("Escenas de menú")]
    public string[] escenasMenu = { "SampleScene", "MainMenu" };

    private GameObject panelActual;
    private Coroutine  transicionActiva;

    // ──────────────────────────────────────────────────────
    void Awake()
    {
        if (instancia != null) { Destroy(gameObject); return; }
        instancia = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (canvasMenu == null)
        {
            Canvas canvas = GetComponentInChildren<Canvas>(true);
            if (canvas != null) canvasMenu = canvas.gameObject;
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool esMenu = System.Array.Exists(escenasMenu,
            e => string.Equals(e, scene.name, System.StringComparison.OrdinalIgnoreCase));

        if (esMenu)
            StartCoroutine(MostrarMenuConDelay());
    }

    IEnumerator MostrarMenuConDelay()
    {
        yield return new WaitForSecondsRealtime(0.15f);
        panelActual = null;
        MostrarCanvasMenu(true);
        MostrarPanelPrincipal();
    }

    void MostrarPanelPrincipal()
    {
        if (canvases == null || canvases.Length == 0) return;

        foreach (GameObject panel in canvases)
        {
            if (panel != null &&
                panel.name.IndexOf("MainMenu", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                CambiarPanel(panel);
                return;
            }
        }

        foreach (GameObject panel in canvases)
        {
            if (panel != null) { CambiarPanel(panel); return; }
        }
    }

    // ── API pública ────────────────────────────────────────
    public void Mostrar(GameObject objetivo)
    {
        if (objetivo == null) return;
        if (panelActual == objetivo) return;

        if (transicionActiva != null) StopCoroutine(transicionActiva);

        if (fadeOverlay != null)
            transicionActiva = StartCoroutine(TransicionConFade(objetivo));
        else
            CambiarPanel(objetivo);
    }

    public void OcultarTodo()
    {
        foreach (GameObject panel in canvases)
            if (panel != null) panel.SetActive(false);
        panelActual = null;
    }

    public void MostrarCanvasMenu(bool mostrar)
    {
        if (canvasMenu == null)
        {
            Canvas canvas = GetComponentInChildren<Canvas>(true);
            if (canvas != null) canvasMenu = canvas.gameObject;
        }
        if (canvasMenu != null) canvasMenu.SetActive(mostrar);
    }

    /// <summary>
    /// Resetea el panel actual para que la próxima llamada
    /// a Mostrar() funcione aunque sea el mismo panel.
    /// Usar cuando paneles externos fueron ocultados
    /// directamente con SetActive(false).
    /// </summary>
    public void ResetearPanelActual()
    {
        panelActual = null;
    }

    public bool EstaVisible(GameObject panel)
        => panel != null && panel.activeSelf;

    // ── Internos ───────────────────────────────────────────
    void CambiarPanel(GameObject objetivo)
    {
        foreach (GameObject panel in canvases)
            if (panel != null) panel.SetActive(panel == objetivo);
        panelActual = objetivo;
    }

    IEnumerator TransicionConFade(GameObject objetivo)
    {
        yield return StartCoroutine(FadeA(1f));
        CambiarPanel(objetivo);
        yield return StartCoroutine(FadeA(0f));
        transicionActiva = null;
    }

    IEnumerator FadeA(float alphaObjetivo)
    {
        float alphaInicio = fadeOverlay.alpha;
        float tiempo = 0f;
        while (tiempo < duracionFade)
        {
            tiempo += Time.unscaledDeltaTime;
            fadeOverlay.alpha = Mathf.Lerp(alphaInicio, alphaObjetivo, tiempo / duracionFade);
            yield return null;
        }
        fadeOverlay.alpha = alphaObjetivo;
    }
}