// Door_01.cs — Assets/ScifiFacility/Scripts/
// ARIA: Último Protocolo — Puerta de dos alas

using UnityEngine;

public class Door_01 : MonoBehaviour
{
    public Animation WingLeft;
    public Animation WingRight;

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
        WingLeft["door_01_wing_left"].speed   =  1;
        WingRight["door_01_wing_right"].speed =  1;
        WingLeft.Play();
        WingRight.Play();
    }

    void CerrarPuerta()
    {
        puertaAbierta = false;
        GetComponent<AudioSource>().Play();
        WingLeft["door_01_wing_left"].time    = WingLeft["door_01_wing_left"].length;
        WingRight["door_01_wing_right"].time  = WingRight["door_01_wing_right"].length;
        WingLeft["door_01_wing_left"].speed   = -1;
        WingRight["door_01_wing_right"].speed = -1;
        WingLeft.Play();
        WingRight.Play();
    }

    public void Desbloquear()
    {
        bloqueada = false;
        if (estabaEnRango && (!requiereEnergia || PowerManager.EnergiaActiva))
            AbrirPuerta();
    }

    public void Bloquear() => bloqueada = true;
}
