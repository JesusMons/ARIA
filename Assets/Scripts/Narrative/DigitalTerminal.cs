// DigitalTerminal.cs — Assets/Scripts/Narrative/
// ARIA: Último Protocolo — Terminal digital con archivos del Dr. Voss
// Responsable: Jesús Monsalvo

using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class DigitalTerminal : MonoBehaviour
{
    [Header("Archivos almacenados")]
    public List<VossNoteData> archivos;

    [Header("Interacción")]
    public float  rangoInteraccion  = 2.5f;
    public string mensajePrompt     = "[ E ]  ACCEDER AL TERMINAL";
    public string mensajeSinEnergia = "[ SERVIDOR DESCONECTADO ]";
    public string mensajeTodosLeidos= "[ SIN ARCHIVOS NUEVOS ]";

    [Header("Efectos")]
    public AudioClip  sonidoEncender;
    public AudioClip  sonidoAcceder;
    public AudioClip  sonidoError;
    public GameObject pantallaApagada;
    public GameObject pantallaEncendida;

    // ── Internos ───────────────────────────────────────────
    Transform   jugador;
    AudioSource audioSrc;
    bool        jugadorEnRango  = false;
    bool        energiaAnterior = false;

    // ──────────────────────────────────────────────────────
    void Start()
    {
        GameObject p = GameObject.FindWithTag("GameController");
        if (p != null) jugador = p.transform;

        audioSrc = GetComponent<AudioSource>();
        if (audioSrc == null) audioSrc = gameObject.AddComponent<AudioSource>();

        energiaAnterior = ObtenerEstadoEnergia();
        ActualizarPantalla(energiaAnterior);
    }

    void Update()
    {
        if (jugador == null) return;

        // Detectar cambio de energía
        bool energiaActual = ObtenerEstadoEnergia();
        if (energiaActual != energiaAnterior)
        {
            energiaAnterior = energiaActual;
            ActualizarPantalla(energiaActual);

            if (energiaActual && sonidoEncender != null)
                audioSrc.PlayOneShot(sonidoEncender);

            if (jugadorEnRango)
                MostrarPromptContextual();
        }

        float distancia    = Vector3.Distance(transform.position, jugador.position);
        bool  enRangoActual = distancia <= rangoInteraccion;

        if (enRangoActual && !jugadorEnRango)
        {
            jugadorEnRango = true;
            MostrarPromptContextual();
        }
        else if (!enRangoActual && jugadorEnRango)
        {
            jugadorEnRango = false;
            HUDManager.OcultarMensaje();
        }

        // Solo interactuar con E si hay energía Y está en rango
        if (jugadorEnRango                               &&
            ObtenerEstadoEnergia()                       &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            Interactuar();
        }
        else if (jugadorEnRango                          &&
                 !ObtenerEstadoEnergia()                 &&
                 Keyboard.current.eKey.wasPressedThisFrame)
        {
            // Sin energía — error visual y sonido, SIN registrar nota
            if (sonidoError != null) audioSrc.PlayOneShot(sonidoError);
            HUDManager.MostrarError(mensajeSinEnergia);
        }
    }

    void Interactuar()
    {
        // Doble verificación de energía — nunca registrar sin ella
        if (!ObtenerEstadoEnergia())
        {
            if (sonidoError != null) audioSrc.PlayOneShot(sonidoError);
            HUDManager.MostrarError(mensajeSinEnergia);
            return;
        }

        if (archivos == null || archivos.Count == 0)
        {
            HUDManager.MostrarMensaje(mensajeTodosLeidos);
            return;
        }

        // Buscar primer archivo NO leído con ID válido
        VossNoteData archivo = null;
        foreach (VossNoteData a in archivos)
        {
            if (a == null) continue;
            if (string.IsNullOrEmpty(a.id)) continue;
            if (!NoteManager.FueRecolectada(a.id))
            {
                archivo = a;
                break;
            }
        }

        if (archivo != null)
        {
            if (sonidoAcceder != null) audioSrc.PlayOneShot(sonidoAcceder);
            NoteManager.RegistrarNota(archivo);
            NoteReaderUI.MostrarNota(archivo);
            HUDManager.OcultarMensaje();
        }
        else
        {
            // Todos los archivos ya fueron leídos
            HUDManager.MostrarMensaje(mensajeTodosLeidos);
        }
    }

    void MostrarPromptContextual()
    {
        if (!ObtenerEstadoEnergia())
            HUDManager.MostrarError(mensajeSinEnergia);
        else
            HUDManager.MostrarMensaje(mensajePrompt);
    }

    void ActualizarPantalla(bool encendida)
    {
        if (pantallaApagada   != null) pantallaApagada.SetActive(!encendida);
        if (pantallaEncendida != null) pantallaEncendida.SetActive(encendida);
    }

    // Método seguro para obtener estado de energía
    bool ObtenerEstadoEnergia()
    {
        // Si no hay PowerManager en escena, asumir sin energía
        if (FindFirstObjectByType<PowerManager>() == null) return false;
        return PowerManager.EnergiaActiva;
    }
}