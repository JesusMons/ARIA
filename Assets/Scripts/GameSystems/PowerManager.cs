// PowerManager.cs — Assets/Scripts/GameSystems/
// ARIA: Último Protocolo — Estado global de energía

using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class PowerManager : MonoBehaviour
{
    public static bool EnergiaActiva { get; private set; } = false;

    [Header("Referencias del asset")]
    public ThemeManager themeManager;
    public GameObject   objetoLuces;

    [Header("Transición")]
    [Range(5f, 10f)]
    public float duracionMinima      = 5f;
    [Range(5f, 10f)]
    public float duracionMaxima      = 10f;
    [Range(1f, 12f)]
    public float intensidadAmbiental = 10f;
    [Range(1f, 12f)]
    public float multiplicadorLuces  = 9f;

    [Header("Iluminacion restaurada")]
    public bool forzarRefuerzoIluminacion = true;
    public float minimoAmbientalRestaurado = 10f;
    public float minimoLuzRestaurada = 9f;
    public Color colorAmbienteEncendido = new Color(0.38f, 0.43f, 0.50f);
    [Range(0f, 2f)]
    public float intensidadReflejos = 1.2f;
    public bool aumentarAlcanceLuces = true;
    [Range(1f, 3f)]
    public float multiplicadorAlcance = 1.45f;
    [Range(8f, 40f)]
    public float alcanceMinimoLuz = 18f;

    [Header("Eventos")]
    public UnityEvent onEnergiaActivada;
    public UnityEvent onEnergiaDesactivada;

    [Header("Config")]
    public bool energiaAlInicio = false;

    readonly Dictionary<Light, float> rangosOriginales = new Dictionary<Light, float>();

    void Start()
    {
        EnergiaActiva = energiaAlInicio;

        if (themeManager == null)
            themeManager = FindFirstObjectByType<ThemeManager>();

        if (energiaAlInicio)
            AplicarEstadoIluminado();
    }

    public void ActivarEnergia()
    {
        if (EnergiaActiva) return;
        EnergiaActiva = true;
        onEnergiaActivada?.Invoke();
        SaveGameManager.GuardarPartidaActual();
        StartCoroutine(TransicionEncendido());
    }

    public void DesactivarEnergia()
    {
        EnergiaActiva = false;
        onEnergiaDesactivada?.Invoke();
        SaveGameManager.GuardarPartidaActual();
    }

    public void MarcarEnergiaActiva()
    {
        EnergiaActiva = true;
    }

    IEnumerator TransicionEncendido()
    {
        // Duración aleatoria entre 5 y 10 segundos
        float duracionTotal = Random.Range(duracionMinima, duracionMaxima);

        // ── Fase 1: alarma encendida TODO el tiempo ──────
        if (themeManager != null)
        {
            themeManager.setWarningLights(true);
            themeManager.setTorch(true);   // linterna siempre encendida
        }

        // ── Fase 2: encender luces gradualmente ──────────
        // Activar objetos pero empezar con intensidad 0
        if (objetoLuces != null)
        {
            objetoLuces.SetActive(true);
            foreach (Light luz in objetoLuces
                .GetComponentsInChildren<Light>(true))
            {
                luz.gameObject.SetActive(true);
                luz.enabled   = true;
                RegistrarRangoOriginal(luz);
                luz.intensity = 0f;   // empezar apagadas
            }
        }

        // Luz ambiental empieza en 0
        RenderSettings.ambientIntensity = 0f;
        RenderSettings.reflectionIntensity = 0f;

        // Fade gradual durante toda la duración
        float tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < duracionTotal)
        {
            tiempoTranscurrido += Time.deltaTime;
            float progreso = Mathf.Clamp01(tiempoTranscurrido / duracionTotal);
            float curva    = Mathf.SmoothStep(0f, 1f, progreso);

            // Subir luz ambiental gradualmente
            RenderSettings.ambientIntensity = Mathf.Lerp(0f, IntensidadAmbientalFinal(), curva);
            RenderSettings.ambientLight     = Color.Lerp(
                new Color(0.02f, 0.02f, 0.03f),
                colorAmbienteEncendido,
                curva
            );
            RenderSettings.reflectionIntensity = Mathf.Lerp(0f, intensidadReflejos, curva);

            // Subir intensidad de cada luz gradualmente
            if (objetoLuces != null)
            {
                foreach (Light luz in objetoLuces
                    .GetComponentsInChildren<Light>(true))
                {
                    AplicarAlcanceRestaurado(luz);
                    luz.intensity = Mathf.Lerp(0f, IntensidadLuzFinal(), curva);
                }
            }

            yield return null;
        }

        // ── Fase 3: apagar alarma al terminar ────────────
        if (themeManager != null)
        {
            themeManager.setWarningLights(false);
            themeManager.setTorch(true);   // linterna sigue encendida
        }

        // Valores finales exactos
        RenderSettings.ambientIntensity = IntensidadAmbientalFinal();
        RenderSettings.ambientLight     = colorAmbienteEncendido;
        RenderSettings.reflectionIntensity = intensidadReflejos;

        // Tema visual
        if (themeManager != null)
        {
            try
            {
                themeManager.setTheme(0);
                themeManager.setDirtAmount(2);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("[PowerManager] Error en tema: " + e.Message);
            }
        }

        Debug.Log($"[PowerManager] Luces encendidas en {duracionTotal:F1}s.");
    }

    void AplicarEstadoIluminado()
    {
        if (themeManager != null)
        {
            themeManager.setWarningLights(false);
            themeManager.setTorch(true);
            try { themeManager.setTheme(0); } catch { }
        }

        if (objetoLuces != null)
        {
            objetoLuces.SetActive(true);
            foreach (Light luz in objetoLuces
                .GetComponentsInChildren<Light>(true))
            {
                luz.gameObject.SetActive(true);
                luz.enabled   = true;
                RegistrarRangoOriginal(luz);
                AplicarAlcanceRestaurado(luz);
                luz.intensity = IntensidadLuzFinal();
            }
        }

        RenderSettings.ambientIntensity = IntensidadAmbientalFinal();
        RenderSettings.ambientLight     = colorAmbienteEncendido;
        RenderSettings.reflectionIntensity = intensidadReflejos;
    }

    void RegistrarRangoOriginal(Light luz)
    {
        if (luz == null || rangosOriginales.ContainsKey(luz)) return;
        rangosOriginales.Add(luz, luz.range);
    }

    void AplicarAlcanceRestaurado(Light luz)
    {
        if (!aumentarAlcanceLuces || luz == null) return;

        RegistrarRangoOriginal(luz);
        float rangoOriginal = rangosOriginales[luz];
        luz.range = Mathf.Max(rangoOriginal * multiplicadorAlcance, alcanceMinimoLuz);
    }

    float IntensidadAmbientalFinal()
    {
        return forzarRefuerzoIluminacion
            ? Mathf.Max(intensidadAmbiental, minimoAmbientalRestaurado)
            : intensidadAmbiental;
    }

    float IntensidadLuzFinal()
    {
        return forzarRefuerzoIluminacion
            ? Mathf.Max(multiplicadorLuces, minimoLuzRestaurada)
            : multiplicadorLuces;
    }

    void OnDestroy() => EnergiaActiva = false;
}
