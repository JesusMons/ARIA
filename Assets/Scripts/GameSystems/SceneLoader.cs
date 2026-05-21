// ============================================================
//  SceneLoader.cs
//  ARIA: Último Protocolo — Cargador de Escenas con Async
//
//  JERARQUÍA ESPERADA:
//  UI_Root
//    └── Panel_Loading          ← asignar en loadingPanel
//          ├── Slider_Progreso  ← asignar en barraProgreso
//          └── TXT_Porcentaje   ← asignar en textoProgreso (TMP)
//
//  Responsable: Cristian Ramírez
// ============================================================

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    // ── Singleton ──────────────────────────────────────────
    public static SceneLoader instancia;

    // ── UI de carga ────────────────────────────────────────
    [Header("Panel de Carga")]
    public GameObject loadingPanel;
    public Slider barraProgreso;
    public TextMeshProUGUI textoProgreso;

    [Header("Configuración")]
    [Tooltip("Tiempo mínimo de pantalla de carga (segundos). Evita parpadeos.")]
    public float tiempoMinimoEnPantallaCarga = 1.5f;

    [Tooltip("Escena usada si un save viejo apunta a una escena que ya no existe.")]
    public string escenaFallbackJuego = "level_1";

    // ──────────────────────────────────────────────────────
    void Awake()
    {
        // Singleton
        if (instancia != null)
        {
            Destroy(gameObject);
            return;
        }
        instancia = this;
        DontDestroyOnLoad(gameObject);
    }

    // ── API pública ────────────────────────────────────────

    /// <summary>Carga una escena por nombre con pantalla de carga.</summary>
    public void CargarEscena(string nombreEscena)
    {
        nombreEscena = NormalizarNombreEscena(nombreEscena);
        if (UIManager.instancia != null)
            UIManager.instancia.MostrarCanvasMenu(true);
        StartCoroutine(CargarEscenaAsync(nombreEscena));
    }

    /// <summary>Carga una escena por índice de Build Settings.</summary>
    public void CargarEscenaPorIndice(int indice)
    {
        StartCoroutine(CargarEscenaAsync(indice.ToString(), porIndice: true));
    }

    /// <summary>Recarga la escena activa (útil para reiniciar nivel).</summary>
    public void Reiniciar()
    {
        int indice = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(CargarEscenaAsync(indice.ToString(), porIndice: true));
    }

    /// <summary>Carga el menú principal.</summary>
    public void IrAlMenu()
    {
        CargarEscena("MainMenu");
    }

    // ── Carga asíncrona interna ────────────────────────────
    IEnumerator CargarEscenaAsync(string objetivo, bool porIndice = false)
    {
        Time.timeScale = 1f;
        bool cargandoEscenaJuego = !EsEscenaMenu(objetivo);

        if (cargandoEscenaJuego)
            OcultarPanelesMenuPersistentes();

        // Mostrar panel de carga
        if (loadingPanel != null)
            loadingPanel.SetActive(true);

        if (barraProgreso != null)
            barraProgreso.value = 0f;

        if (textoProgreso != null)
            textoProgreso.text = "0%";

        // Tiempo mínimo para que no parpadee
        float tiempoInicio = Time.realtimeSinceStartup;

        // Iniciar carga en segundo plano
        AsyncOperation operacion = porIndice
            ? SceneManager.LoadSceneAsync(int.Parse(objetivo))
            : SceneManager.LoadSceneAsync(objetivo);

        if (operacion == null)
        {
            string fallback = NormalizarNombreEscena(escenaFallbackJuego);
            if (!porIndice && objetivo != fallback)
            {
                Debug.LogWarning($"[SceneLoader] No se pudo cargar '{objetivo}'. Intentando fallback '{fallback}'.");
                operacion = SceneManager.LoadSceneAsync(fallback);
                SaveGameManager.ActualizarEscenaPartidaActual(fallback);
            }
        }

        if (operacion == null)
        {
            Debug.LogError($"[SceneLoader] No se pudo cargar la escena '{objetivo}'. Agregala a Build Profiles/Scenes.");
            if (loadingPanel != null)
                loadingPanel.SetActive(false);
            yield break;
        }

        // Unity no activa la escena hasta que se lo decimos (más control)
        operacion.allowSceneActivation = false;

        while (!operacion.isDone)
        {
            // Unity reporta 0–0.9 mientras carga, 1.0 al activar
            float progreso = Mathf.Clamp01(operacion.progress / 0.9f);

            if (barraProgreso != null)
                barraProgreso.value = progreso;

            if (textoProgreso != null)
                textoProgreso.text = $"{Mathf.RoundToInt(progreso * 100)}%";

            // Cuando está lista (90%) y pasó el tiempo mínimo → activar
            bool tiempoListo = (Time.realtimeSinceStartup - tiempoInicio) >= tiempoMinimoEnPantallaCarga;

            if (operacion.progress >= 0.9f && tiempoListo)
            {
                if (barraProgreso != null) barraProgreso.value = 1f;
                if (textoProgreso != null) textoProgreso.text = "100%";

                // Pequeño delay visual antes de activar
                yield return new WaitForSecondsRealtime(0.3f);
                operacion.allowSceneActivation = true;
            }

            yield return null;
        }

        // Ocultar panel al terminar (por si el objeto sobrevive)
        if (loadingPanel != null)
            loadingPanel.SetActive(false);

        if (cargandoEscenaJuego)
            OcultarPanelesMenuPersistentes();
    }

    void OcultarPanelesMenuPersistentes()
    {
        MenuSaveUI saveUI = FindFirstObjectByType<MenuSaveUI>();
        if (saveUI != null)
            saveUI.OcultarPanelesGuardado();

        if (UIManager.instancia != null)
        {
            UIManager.instancia.OcultarTodo();
            UIManager.instancia.MostrarCanvasMenu(false);
        }

        if (loadingPanel != null)
            loadingPanel.SetActive(false);
    }

    static bool EsEscenaMenu(string escena)
    {
        string normalizada = NormalizarNombreEscena(escena);
        return normalizada == "SampleScene" || normalizada == "SplashScreen";
    }

    public static string NormalizarNombreEscena(string nombreEscena)
    {
        if (string.IsNullOrWhiteSpace(nombreEscena)) return "level_1";

        switch (nombreEscena.Trim())
        {
            case "Level_01_Despertar":
            case "Level_01":
            case "Level01":
            case "Level_1":
                return "level_1";
            case "MainMenu":
                return "SampleScene";
            default:
                return nombreEscena.Trim();
        }
    }
}
