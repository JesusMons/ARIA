// PowerGenerator.cs — Assets/Scripts/GameSystems/
// ARIA: Último Protocolo — Generador de energía interactuable
//
// SETUP:
// Adjuntar a una consola/panel en el server_room
// El jugador se acerca y presiona E para activarlo

using UnityEngine;
using UnityEngine.InputSystem;

public class PowerGenerator : MonoBehaviour
{
    [Header("Interacción")]
    public float rangoInteraccion = 2.5f;

    [Header("Mensajes")]
    public string mensajeApagado  = "[ E ]  ACTIVAR GENERADOR";
    public string mensajeActivado = "[ GENERADOR ACTIVO ]";

    [Header("Efectos al activar")]
    public GameObject luzGenerador;
    public AudioClip  sonidoActivacion;

    // ── Internos ───────────────────────────────────────────
    bool        activado       = false;
    bool        jugadorEnRango = false;
    Transform   jugador;
    AudioSource audioSrc;

    void Start()
    {
        GameObject p = GameObject.FindWithTag("GameController");
        if (p != null) jugador = p.transform;

        audioSrc = GetComponent<AudioSource>();
        if (audioSrc == null) audioSrc = gameObject.AddComponent<AudioSource>();

        if (luzGenerador != null) luzGenerador.SetActive(false);
    }

    void Update()
    {
        if (activado || jugador == null) return;

        float distancia    = Vector3.Distance(transform.position, jugador.position);
        bool enRangoActual = distancia <= rangoInteraccion;

        // Mostrar/ocultar prompt al entrar o salir del rango
        if (enRangoActual && !jugadorEnRango)
        {
            jugadorEnRango = true;
            HUDManager.MostrarMensaje(mensajeApagado);
        }
        else if (!enRangoActual && jugadorEnRango)
        {
            jugadorEnRango = false;
            HUDManager.OcultarMensaje();
        }

        // Activar con E
        if (jugadorEnRango && Keyboard.current.eKey.wasPressedThisFrame)
            Activar();
    }

    void Activar()
    {
        activado = true;

        if (sonidoActivacion != null)
            audioSrc.PlayOneShot(sonidoActivacion);

        if (luzGenerador != null)
            luzGenerador.SetActive(true);

        HUDManager.MostrarMensajeTemporal(mensajeActivado, 3f);

        // Activar energía global
        PowerManager powerManager = FindFirstObjectByType<PowerManager>();
        if (powerManager != null)
            powerManager.ActivarEnergia();
        else
            Debug.LogWarning("[PowerGenerator] No se encontró PowerManager en la escena.");
    }
}