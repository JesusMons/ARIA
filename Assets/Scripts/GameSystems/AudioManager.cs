// AudioManager.cs — Assets/Scripts/GameSystems/
// ARIA: Último Protocolo — Gestor de audio global
// Responsable: Cristian Ramírez

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instancia;

    [Header("Fuentes de audio")]
    public AudioSource fuenteMusica;
    public AudioSource fuenteUI;
    public AudioSource fuenteAmbiente;

    [Header("Música")]
    public AudioClip musicaMenu;
    public AudioClip musicaJuego;

    [Header("Efectos UI")]
    public AudioClip sonidoHover;
    public AudioClip sonidoClic;
    public AudioClip sonidoVolver;
    public AudioClip sonidoError;

    [Header("Transiciones de panel")]
    public AudioClip sonidoAbrirPanel;
    public AudioClip sonidoCerrarPanel;
    public AudioClip sonidoConfirmar;

    [Header("Ambiente")]
    public AudioClip ambienciaEstacion;

    [Header("Volúmenes")]
    [Range(0f, 1f)] public float volumenMusica  = 0.4f;
    [Range(0f, 1f)] public float volumenUI       = 0.7f;
    [Range(0f, 1f)] public float volumenAmbiente = 0.2f;

    [Header("Escenas de menú (sin música de juego)")]
    public string[] escenasMenu = { "SampleScene", "MainMenu", "SplashScreen" };

    // ──────────────────────────────────────────────────────
    void Awake()
    {
        if (instancia != null) { Destroy(gameObject); return; }
        instancia = this;
        DontDestroyOnLoad(gameObject);
        ConfigurarFuentes();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        ReproducirMusicaSegunEscena(SceneManager.GetActiveScene().name);
        ReproducirAmbiente();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ReproducirMusicaSegunEscena(scene.name);
    }

    void ReproducirMusicaSegunEscena(string nombreEscena)
    {
        bool esMenu = System.Array.Exists(escenasMenu,
            e => string.Equals(e, nombreEscena, System.StringComparison.OrdinalIgnoreCase));

        if (esMenu)
            ReproducirMusicaMenu();
        else
            ReproducirMusicaJuego();
    }

    // ── Configuración ──────────────────────────────────────
    void ConfigurarFuentes()
    {
        if (fuenteMusica   == null) fuenteMusica   = CrearFuente("Musica",   volumenMusica,  true);
        if (fuenteUI       == null) fuenteUI       = CrearFuente("UI",       volumenUI,      false);
        if (fuenteAmbiente == null) fuenteAmbiente = CrearFuente("Ambiente", volumenAmbiente,true);

        fuenteMusica.loop   = true;
        fuenteAmbiente.loop = true;
    }

    AudioSource CrearFuente(string nombre, float volumen, bool loop)
    {
        GameObject go = new GameObject($"AudioSource_{nombre}");
        go.transform.SetParent(transform);
        AudioSource src = go.AddComponent<AudioSource>();
        src.volume = volumen;
        src.loop   = loop;
        src.playOnAwake = false;
        return src;
    }

    // ── Música ─────────────────────────────────────────────
    public void ReproducirMusicaMenu()
    {
        if (musicaMenu == null) return;
        if (fuenteMusica.clip == musicaMenu && fuenteMusica.isPlaying) return;
        StartCoroutine(TransicionMusica(musicaMenu, volumenMusica, 1.5f));
    }

    public void ReproducirMusicaJuego()
    {
        if (musicaJuego == null) return;
        if (fuenteMusica.clip == musicaJuego && fuenteMusica.isPlaying) return;
        StartCoroutine(TransicionMusica(musicaJuego, volumenMusica, 1.5f));
    }

    IEnumerator FadeInMusica(AudioClip clip, float volObjetivo, float duracion)
    {
        fuenteMusica.clip   = clip;
        fuenteMusica.volume = 0f;
        fuenteMusica.Play();

        float t = 0f;
        while (t < duracion)
        {
            t += Time.unscaledDeltaTime;
            fuenteMusica.volume = Mathf.Lerp(0f, volObjetivo, t / duracion);
            yield return null;
        }
        fuenteMusica.volume = volObjetivo;
    }

    IEnumerator TransicionMusica(AudioClip nuevoClip, float volObjetivo, float duracion)
    {
        if (fuenteMusica.isPlaying)
        {
            float volInicio = fuenteMusica.volume;
            float t = 0f;
            while (t < duracion * 0.5f)
            {
                t += Time.unscaledDeltaTime;
                fuenteMusica.volume = Mathf.Lerp(volInicio, 0f, t / (duracion * 0.5f));
                yield return null;
            }
            fuenteMusica.Stop();
        }
        yield return StartCoroutine(FadeInMusica(nuevoClip, volObjetivo, duracion * 0.5f));
    }

    // ── Ambiente ───────────────────────────────────────────
    void ReproducirAmbiente()
    {
        if (ambienciaEstacion == null) return;
        fuenteAmbiente.clip   = ambienciaEstacion;
        fuenteAmbiente.volume = volumenAmbiente;
        fuenteAmbiente.Play();
    }

    // ── Efectos UI ─────────────────────────────────────────
    public void PlayHover()      => PlayUI(sonidoHover,        volumenUI * 0.6f);
    public void PlayClic()       => PlayUI(sonidoClic,         volumenUI);
    public void PlayVolver()     => PlayUI(sonidoVolver,       volumenUI * 0.8f);
    public void PlayError()      => PlayUI(sonidoError,        volumenUI * 0.5f);
    public void PlayAbrirPanel() => PlayUI(sonidoAbrirPanel,   volumenUI * 0.7f);
    public void PlayCerrarPanel()=> PlayUI(sonidoCerrarPanel,  volumenUI * 0.7f);
    public void PlayConfirmar()  => PlayUI(sonidoConfirmar,    volumenUI);

    void PlayUI(AudioClip clip, float vol = -1f)
    {
        if (clip == null || fuenteUI == null) return;
        fuenteUI.PlayOneShot(clip, vol < 0 ? volumenUI : vol);
    }

    // ── Volumen ────────────────────────────────────────────
    public void SetVolumenMusica(float vol)
    {
        volumenMusica = vol;
        if (fuenteMusica != null) fuenteMusica.volume = vol;
    }

    public void SetVolumenUI(float vol)       { volumenUI = vol; }

    public void SetVolumenAmbiente(float vol)
    {
        volumenAmbiente = vol;
        if (fuenteAmbiente != null) fuenteAmbiente.volume = vol;
    }
}