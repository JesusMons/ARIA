// Door_02.cs — Assets/ScifiFacility/Scripts/
// ARIA: Último Protocolo — Puerta de un ala

using UnityEngine;

public class Door_02 : MonoBehaviour
{
    public Animation Wing;

    [Header("ARIA — Estado de la puerta")]
    public bool bloqueada       = false;
    public bool requiereEnergia = true;

    [Header("Detección por distancia")]
    public float rangoDeteccion = 3f;

    [Header("Mensajes")]
    public string mensajeBloqueada  = "[ ACCESO DENEGADO ]";
    public string mensajeSinEnergia = "[ SIN ENERGIA ]";

    Transform jugador;
    bool      estabaEnRango = false;
    bool      puertaAbierta = false;
    CodeDoor  codeDoor;

    void Start()
    {
        GameObject p = GameObject.FindWithTag("GameController");
        if (p != null) jugador = p.transform;
        codeDoor = GetComponent<CodeDoor>();
    }

    void Update()
    {
        if (jugador == null) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);
        bool enRango    = distancia <= rangoDeteccion;

        if (enRango && !estabaEnRango)
        {
            estabaEnRango = true;

            if (requiereEnergia && !PowerManager.EnergiaActiva)
            {
                HUDManager.MostrarWarning(mensajeSinEnergia);
                // Completar objetivo "despertar" al descubrir que no hay energía
                ObjectiveManager.Completar("despertar");
            }
            else if (bloqueada)
            {
                if (codeDoor == null)
                    HUDManager.MostrarError(mensajeBloqueada);
            }
            else
            {
                AbrirPuerta();
            }
        }
        else if (!enRango && estabaEnRango)
        {
            estabaEnRango = false;
            HUDManager.OcultarMensaje();
            if (puertaAbierta) CerrarPuerta();
        }
    }

    void AbrirPuerta()
    {
        puertaAbierta = true;
        GetComponent<AudioSource>().Play();
        Wing["door_02_wing"].speed = 1;
        Wing.Play();
    }

    void CerrarPuerta()
    {
        puertaAbierta = false;
        GetComponent<AudioSource>().Play();
        Wing["door_02_wing"].time  = Wing["door_02_wing"].length;
        Wing["door_02_wing"].speed = -1;
        Wing.Play();
    }

    public void Desbloquear()
    {
        bloqueada = false;
        if (estabaEnRango && (!requiereEnergia || PowerManager.EnergiaActiva))
            AbrirPuerta();
    }

    public void Bloquear() => bloqueada = true;
}
