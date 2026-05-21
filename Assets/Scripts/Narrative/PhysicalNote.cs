// PhysicalNote.cs — Assets/Scripts/Narrative/
// ARIA: Último Protocolo — Nota física interactuable
// Responsable: Jesús Monsalvo

using UnityEngine;
using UnityEngine.InputSystem;

public class PhysicalNote : MonoBehaviour
{
    [Header("Datos de la nota")]
    public VossNoteData nota;

    [Header("Interacción")]
    public float  rangoInteraccion = 2f;
    public string mensajePrompt    = "[ E ]  LEER NOTA";
    public string mensajeYaLeida   = "[ NOTA LEIDA ]";
    public string mensajeSinEnergia= "[ SERVIDOR DESCONECTADO ]";

    [Header("Efectos al recoger")]
    public AudioClip sonidoRecoger;

    [Header("Visual — brillo al acercarse")]
    public bool  brillarAlEstar = true;
    public Color colorBrillo    = new Color32(255, 215, 0, 255);
    public float intensidadHalo = 2f;

    [Header("Visual — estado leído")]
    public Color colorLeida = new Color32(100, 100, 100, 180);

    // ── Internos ───────────────────────────────────────────
    Transform   jugador;
    AudioSource audioSrc;
    bool        jugadorEnRango = false;
    bool        yaLeida        = false;
    Renderer[]  renderers;
    Color[]     coloresOriginales;

    // ──────────────────────────────────────────────────────
    void Start()
    {
        GameObject p = GameObject.FindWithTag("GameController");
        if (p != null) jugador = p.transform;

        audioSrc  = GetComponent<AudioSource>();
        if (audioSrc == null) audioSrc = gameObject.AddComponent<AudioSource>();

        renderers = GetComponentsInChildren<Renderer>();
        GuardarColoresOriginales();

        if (nota != null && NoteManager.FueRecolectada(nota.id))
        {
            yaLeida = true;
            MarcarComoLeida();
        }
    }

    void Update()
    {
        if (jugador == null || nota == null) return;

        float distancia    = Vector3.Distance(transform.position, jugador.position);
        bool  enRangoActual = distancia <= rangoInteraccion;

        if (enRangoActual && !jugadorEnRango)
        {
            jugadorEnRango = true;
            if (yaLeida)
                HUDManager.MostrarMensaje(mensajeYaLeida);
            else if (EsDigitalSinEnergia())
                HUDManager.MostrarError(mensajeSinEnergia);
            else
            {
                HUDManager.MostrarMensaje(mensajePrompt);
                if (brillarAlEstar) AplicarBrillo(true);
            }
        }
        else if (!enRangoActual && jugadorEnRango)
        {
            jugadorEnRango = false;
            HUDManager.OcultarMensaje();
            if (brillarAlEstar && !yaLeida) AplicarBrillo(false);
        }

        if (!jugadorEnRango || !Keyboard.current.eKey.wasPressedThisFrame) return;

        if (yaLeida)              { NoteReaderUI.MostrarNota(nota); return; }
        if (EsDigitalSinEnergia()) { HUDManager.MostrarError(mensajeSinEnergia); return; }

        LeerNota();
    }

    // ── Sincronizar estado desde guardado ──────────────────
    /// <summary>Llamado por SaveGameManager al cargar una partida</summary>
    public void SincronizarEstadoGuardado()
    {
        if (nota == null) return;

        bool debeEstarLeida = NoteManager.FueRecolectada(nota.id);
        if (debeEstarLeida && !yaLeida)
        {
            yaLeida = true;
            MarcarComoLeida();
        }
        else if (!debeEstarLeida && yaLeida)
        {
            yaLeida = false;
            RestaurarColoresOriginales();
        }
    }

    bool EsDigitalSinEnergia()
    {
        if (nota == null || nota.tipo != TipoNota.Digital) return false;
        if (FindFirstObjectByType<PowerManager>() == null) return true;
        return !PowerManager.EnergiaActiva;
    }

    void LeerNota()
    {
        yaLeida = true;
        if (sonidoRecoger != null) audioSrc.PlayOneShot(sonidoRecoger);
        AplicarBrillo(false);
        HUDManager.OcultarMensaje();
        NoteManager.RegistrarNota(nota);
        NoteReaderUI.MostrarNota(nota);
        MarcarComoLeida();
    }

    void MarcarComoLeida()
    {
        foreach (Renderer r in renderers)
        {
            if (r.material.HasProperty("_BaseColor"))
                r.material.SetColor("_BaseColor", colorLeida);
            else if (r.material.HasProperty("_Color"))
                r.material.SetColor("_Color", colorLeida);
            if (r.material.HasProperty("_EmissionColor"))
            {
                r.material.SetColor("_EmissionColor", Color.black);
                r.material.DisableKeyword("_EMISSION");
            }
        }
    }

    void RestaurarColoresOriginales()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_BaseColor"))
                renderers[i].material.SetColor("_BaseColor", coloresOriginales[i]);
            else if (renderers[i].material.HasProperty("_Color"))
                renderers[i].material.SetColor("_Color", coloresOriginales[i]);
        }
    }

    void GuardarColoresOriginales()
    {
        coloresOriginales = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_BaseColor"))
                coloresOriginales[i] = renderers[i].material.GetColor("_BaseColor");
            else if (renderers[i].material.HasProperty("_Color"))
                coloresOriginales[i] = renderers[i].material.GetColor("_Color");
            else
                coloresOriginales[i] = Color.white;
        }
    }

    void AplicarBrillo(bool activar)
    {
        foreach (Renderer r in renderers)
        {
            if (!r.material.HasProperty("_EmissionColor")) continue;
            r.material.SetColor("_EmissionColor",
                activar ? colorBrillo * intensidadHalo : Color.black);
            if (activar) r.material.EnableKeyword("_EMISSION");
            else         r.material.DisableKeyword("_EMISSION");
        }
    }
}