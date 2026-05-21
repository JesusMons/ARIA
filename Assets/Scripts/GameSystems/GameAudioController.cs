// GameAudioController.cs — Assets/Scripts/GameSystems/
// ARIA: Último Protocolo — Controlador de audio del juego
//
// Maneja música, ambiente y SFX durante el gameplay.
// Trabaja junto al AudioManager (que maneja el menú).
//
// Responsable: Cristian Ramírez

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameAudioController : MonoBehaviour
{
    public static GameAudioController instancia;

    // ── Estados de música ──────────────────────────────────
    public enum EstadoMusica
    {
        Silencio,
        Exploracion,    // calma tensa — ARIA recorre la estación
        Tension,        // sospecha — ruidos, puertas bloqueadas
        Peligro,        // enemigos cerca
        Descubrimiento  // nota importante o evento narrativo
    }

    // ── Clips de música ────────────────────────────────────
    [Header("Música por estado")]
    public AudioClip musicaExploracion;
    public AudioClip musicaTension;
    public AudioClip musicaPeligro;
    public AudioClip musicaDescubrimiento;

    // ── Ambiente por zona ──────────────────────────────────
    [Header("Ambiente")]
    public AudioClip ambienteLaboratorio;
    public AudioClip ambienteServidor;
    public AudioClip ambienteCorredores;
    public AudioClip ambienteAlarma;        // al activar energía
    public AudioClip ambienteEstacionBase;  // zumbido general

    // ── SFX del juego ──────────────────────────────────────
    [Header("Efectos de juego")]
    public AudioClip sfxNotaEncontrada;
    public AudioClip sfxObjetivoCompletado;
    public AudioClip sfxEnergiaActivada;
    public AudioClip sfxPuertaBloqueada;
    public AudioClip sfxPuertaAbierta;
    public AudioClip sfxCodigoIncorrecto;
    public AudioClip sfxCodigoCorrecto;
    public AudioClip sfxPeligroCerca;

    // ── Pasos del jugador ──────────────────────────────────
    [Header("Pasos")]
    public AudioClip[] sfxPasosMetal;
    public AudioClip[] sfxPasosCemento;
    public float       intervaloEntrePasos = 0.45f;

    // ── Configuración ──────────────────────────────────────
    [Header("Configuración")]
    [Range(0f, 1f)] public float volumenMusica     = 0.35f;
    [Range(0f, 1f)] public float volumenAmbiente   = 0.25f;
    [Range(0f, 1f)] public float volumenSFX        = 0.8f;
    [Range(0f, 1f)] public float volumenPasos      = 0.4f;
    public float duracionTransicionMusica           = 2f;

    // ── Fuentes de audio ───────────────────────────────────
    AudioSource fuenteMusica;
    AudioSource fuenteAmbiente;
    AudioSource fuenteSFX;
    AudioSource fuentePasos;

    // ── Estado interno ─────────────────────────────────────
    EstadoMusica estadoActual   = EstadoMusica.Silencio;
    Coroutine    transicionActiva;
    float        proximoPaso    = 0f;
    bool         jugadorMoviendo= false;

    // ──────────────────────────────────────────────────────
    void Awake()
    {
        if (instancia != null) { Destroy(gameObject); return; }
        instancia = this;
        DontDestroyOnLoad(gameObject);
        CrearFuentes();
    }

    void Start()
    {
        SuscribirEventos();
        ReproducirAmbiente(ambienteEstacionBase);
        CambiarEstadoMusica(EstadoMusica.Exploracion);
    }

    void OnDestroy()
    {
        DesuscribirEventos();
    }

    // ── Crear fuentes ──────────────────────────────────────
    void CrearFuentes()
    {
        fuenteMusica  = CrearFuente("Musica",   volumenMusica,   true);
        fuenteAmbiente= CrearFuente("Ambiente", volumenAmbiente, true);
        fuenteSFX     = CrearFuente("SFX",      volumenSFX,      false);
        fuentePasos   = CrearFuente("Pasos",    volumenPasos,    false);
    }

    AudioSource CrearFuente(string nombre, float vol, bool loop)
    {
        GameObject go = new GameObject($"SRC_{nombre}");
        go.transform.SetParent(transform);
        AudioSource src = go.AddComponent<AudioSource>();
        src.volume     = vol;
        src.loop       = loop;
        src.playOnAwake= false;
        src.spatialBlend = 0f; // 2D
        return src;
    }

    // ── Eventos del juego ──────────────────────────────────
    void SuscribirEventos()
    {
        PowerManager pm = FindFirstObjectByType<PowerManager>();
        if (pm != null)
        {
            pm.onEnergiaActivada.AddListener(OnEnergiaActivada);
            pm.onEnergiaDesactivada.AddListener(OnEnergiaDesactivada);
        }

        NoteManager nm = FindFirstObjectByType<NoteManager>();
        if (nm != null)
            nm.onNotaEncontrada.AddListener(OnNotaEncontrada);

        ObjectiveManager om = FindFirstObjectByType<ObjectiveManager>();
        if (om != null)
            om.onObjetivoCompletado.AddListener(OnObjetivoCompletado);
    }

    void DesuscribirEventos()
    {
        PowerManager pm = FindFirstObjectByType<PowerManager>();
        if (pm != null)
        {
            pm.onEnergiaActivada.RemoveListener(OnEnergiaActivada);
            pm.onEnergiaDesactivada.RemoveListener(OnEnergiaDesactivada);
        }
    }

    // ── Callbacks de eventos ───────────────────────────────
    void OnEnergiaActivada()
    {
        PlaySFX(sfxEnergiaActivada);
        StartCoroutine(SecuenciaEnergiaActivada());
    }

    IEnumerator SecuenciaEnergiaActivada()
    {
        // Alarma roja → exploración
        ReproducirAmbiente(ambienteAlarma);
        CambiarEstadoMusica(EstadoMusica.Tension);
        yield return new WaitForSeconds(Random.Range(5f, 10f));
        ReproducirAmbiente(ambienteEstacionBase);
        CambiarEstadoMusica(EstadoMusica.Exploracion);
    }

    void OnEnergiaDesactivada()
    {
        ReproducirAmbiente(ambienteEstacionBase);
        CambiarEstadoMusica(EstadoMusica.Exploracion);
    }

    void OnNotaEncontrada(VossNoteData nota)
    {
        PlaySFX(sfxNotaEncontrada);
        StartCoroutine(MomentoDescubrimiento());
    }

    IEnumerator MomentoDescubrimiento()
    {
        CambiarEstadoMusica(EstadoMusica.Descubrimiento);
        yield return new WaitForSeconds(4f);
        CambiarEstadoMusica(EstadoMusica.Exploracion);
    }

    void OnObjetivoCompletado(Objetivo obj)
    {
        PlaySFX(sfxObjetivoCompletado);
    }

    // ── API pública ────────────────────────────────────────

    /// <summary>Cambiar estado musical (ej. al ver un enemigo)</summary>
    public static void SetEstado(EstadoMusica estado)
        => instancia?.CambiarEstadoMusica(estado);

    /// <summary>Cambiar ambiente de la zona actual</summary>
    public static void SetAmbiente(AudioClip clip)
        => instancia?.ReproducirAmbiente(clip);

    // SFX estáticos para llamar desde cualquier script
    public static void PlayPuertaBloqueada()   => instancia?.PlaySFX(instancia.sfxPuertaBloqueada);
    public static void PlayPuertaAbierta()     => instancia?.PlaySFX(instancia.sfxPuertaAbierta);
    public static void PlayCodigoIncorrecto()  => instancia?.PlaySFX(instancia.sfxCodigoIncorrecto);
    public static void PlayCodigoCorrecto()    => instancia?.PlaySFX(instancia.sfxCodigoCorrecto);
    public static void PlayPeligroCerca()      => instancia?.PlaySFX(instancia.sfxPeligroCerca);

    /// <summary>Llamar desde el FPController cuando el jugador se mueve</summary>
    public static void NotificarMovimiento(bool moviendose, bool enSuperficieMetal = true)
    {
        if (instancia == null) return;
        instancia.jugadorMoviendo = moviendose;
        instancia.ActualizarPasos(enSuperficieMetal);
    }

    // ── Música ─────────────────────────────────────────────
    void CambiarEstadoMusica(EstadoMusica nuevoEstado)
    {
        if (estadoActual == nuevoEstado) return;
        estadoActual = nuevoEstado;

        AudioClip clip = ObtenerClipMusica(nuevoEstado);
        if (transicionActiva != null) StopCoroutine(transicionActiva);
        transicionActiva = StartCoroutine(TransicionMusica(clip));
    }

    AudioClip ObtenerClipMusica(EstadoMusica estado)
    {
        return estado switch
        {
            EstadoMusica.Exploracion    => musicaExploracion,
            EstadoMusica.Tension        => musicaTension,
            EstadoMusica.Peligro        => musicaPeligro,
            EstadoMusica.Descubrimiento => musicaDescubrimiento,
            _                           => null
        };
    }

    IEnumerator TransicionMusica(AudioClip nuevoClip)
    {
        // Fade out
        float vol = fuenteMusica.volume;
        float t   = 0f;
        while (t < duracionTransicionMusica * 0.5f)
        {
            t += Time.unscaledDeltaTime;
            fuenteMusica.volume = Mathf.Lerp(vol, 0f, t / (duracionTransicionMusica * 0.5f));
            yield return null;
        }

        fuenteMusica.Stop();
        if (nuevoClip == null) yield break;

        fuenteMusica.clip   = nuevoClip;
        fuenteMusica.volume = 0f;
        fuenteMusica.Play();

        // Fade in
        t = 0f;
        while (t < duracionTransicionMusica * 0.5f)
        {
            t += Time.unscaledDeltaTime;
            fuenteMusica.volume = Mathf.Lerp(0f, volumenMusica, t / (duracionTransicionMusica * 0.5f));
            yield return null;
        }
        fuenteMusica.volume = volumenMusica;
    }

    // ── Ambiente ───────────────────────────────────────────
    void ReproducirAmbiente(AudioClip clip)
    {
        if (clip == null || fuenteAmbiente.clip == clip) return;
        fuenteAmbiente.clip   = clip;
        fuenteAmbiente.volume = volumenAmbiente;
        fuenteAmbiente.Play();
    }

    // ── SFX ────────────────────────────────────────────────
    void PlaySFX(AudioClip clip, float volOverride = -1f)
    {
        if (clip == null || fuenteSFX == null) return;
        fuenteSFX.PlayOneShot(clip, volOverride < 0 ? volumenSFX : volOverride);
    }

    // ── Pasos ──────────────────────────────────────────────
    void ActualizarPasos(bool esMetal)
    {
        if (!jugadorMoviendo || Time.time < proximoPaso) return;

        AudioClip[] clips = esMetal ? sfxPasosMetal : sfxPasosCemento;
        if (clips == null || clips.Length == 0) return;

        AudioClip paso = clips[Random.Range(0, clips.Length)];
        fuentePasos.PlayOneShot(paso, volumenPasos);
        proximoPaso = Time.time + intervaloEntrePasos;
    }

    // ── Control de volumen ─────────────────────────────────
    public void SetVolumenMusica(float vol)
    {
        volumenMusica = vol;
        fuenteMusica.volume = vol;
    }

    public void SetVolumenAmbiente(float vol)
    {
        volumenAmbiente = vol;
        fuenteAmbiente.volume = vol;
    }

    public void SetVolumenSFX(float vol)
    {
        volumenSFX = vol;
    }
}
