// CodeDoor.cs - Assets/Scripts/GameSystems/
// ARIA: Ultimo Protocolo - Puerta con codigo y condiciones adicionales

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class CodeDoor : MonoBehaviour
{
    [System.Serializable]
    public struct ResultadoAcceso
    {
        public bool autorizado;
        public string mensaje;

        public ResultadoAcceso(bool autorizado, string mensaje)
        {
            this.autorizado = autorizado;
            this.mensaje = mensaje;
        }
    }

    [Header("Identidad")]
    public string idPuerta = "server_room";
    public string nombrePanel = "Acceso restringido";

    [Header("Interaccion")]
    public float rangoInteraccion = 3f;
    public string mensajePrompt = "[ E ] INGRESAR CODIGO";

    [Header("Requisitos")]
    public bool requiereCodigo = true;
    public bool requiereNotaConCodigo = true;
    public bool requiereEnergia = true;
    public bool requiereServidorActivo = false;
    public bool servidorActivo = false;
    public bool requiereTarjetaAcceso = false;
    public bool tarjetaAccesoObtenida = false;
    public bool requiereEventoCompleto = false;
    public bool eventoCompleto = false;

    [Header("Mensajes")]
    public string mensajeCodigoIncorrecto = "CODIGO INCORRECTO";
    public string mensajeCodigoNoEncontrado = "CODIGO NO RECUPERADO";
    public string mensajeAutorizado = "ACCESO AUTORIZADO";
    public string mensajeSinEnergia = "SIN ENERGIA";
    public string mensajeServidor = "SERVIDOR DESCONECTADO";
    public string mensajeTarjeta = "NIVEL DE SEGURIDAD INSUFICIENTE";
    public string mensajeEvento = "ACCESO IMPOSIBLE";
    public string mensajeDesbloqueada = "PUERTA DESBLOQUEADA";

    [Header("Objetivos")]
    public string idObjetivoCompletar = "";
    public string idObjetivoActivar = "";

    [Header("Eventos")]
    public UnityEvent onAccesoAutorizado;
    public UnityEvent onAccesoDenegado;
    public UnityEvent onPuertaDesbloqueada;

    Transform jugador;
    bool jugadorEnRango = false;
    bool desbloqueada = false;
    Door_01 puertaDoble;
    Door_02 puertaSimple;

    public string NombrePanel => string.IsNullOrEmpty(nombrePanel) ? "Acceso restringido" : nombrePanel;

    void Start()
    {
        GameObject p = GameObject.FindWithTag("GameController");
        if (p != null) jugador = p.transform;

        puertaDoble = GetComponent<Door_01>();
        puertaSimple = GetComponent<Door_02>();

        if (puertaDoble != null) puertaDoble.Bloquear();
        if (puertaSimple != null) puertaSimple.Bloquear();

        if (SaveGameManager.PuertaEstaDesbloqueada(idPuerta))
            AplicarDesbloqueoGuardado();
    }

    void Update()
    {
        if (jugador == null || desbloqueada) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);
        bool enRango = distancia <= rangoInteraccion;

        if (enRango && !jugadorEnRango)
        {
            jugadorEnRango = true;
            MostrarPrompt();
        }
        else if (!enRango && jugadorEnRango)
        {
            jugadorEnRango = false;
            HUDManager.OcultarMensaje();
        }

        if (jugadorEnRango && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            IntentarAbrirPanel();
    }

    public ResultadoAcceso ValidarAcceso(string codigoIngresado)
    {
        ResultadoAcceso condicion = ValidarCondiciones();
        if (!condicion.autorizado)
        {
            onAccesoDenegado?.Invoke();
            return condicion;
        }

        if (requiereCodigo && requiereNotaConCodigo && !NoteManager.CodigoFueDescubierto(idPuerta))
        {
            onAccesoDenegado?.Invoke();
            return new ResultadoAcceso(false, mensajeCodigoNoEncontrado);
        }

        if (requiereCodigo && !NoteManager.VerificarCodigo(idPuerta, codigoIngresado))
        {
            onAccesoDenegado?.Invoke();
            return new ResultadoAcceso(false, mensajeCodigoIncorrecto);
        }

        onAccesoAutorizado?.Invoke();
        return new ResultadoAcceso(true, mensajeAutorizado);
    }

    public void DesbloquearDesdePanel()
    {
        desbloqueada = true;

        if (puertaDoble != null) puertaDoble.Desbloquear();
        if (puertaSimple != null) puertaSimple.Desbloquear();

        if (!string.IsNullOrEmpty(idObjetivoCompletar))
            ObjectiveManager.Completar(idObjetivoCompletar);
        if (!string.IsNullOrEmpty(idObjetivoActivar))
            ObjectiveManager.Activar(idObjetivoActivar);

        HUDManager.MostrarOK(mensajeDesbloqueada);
        SaveGameManager.MarcarPuertaDesbloqueada(idPuerta);
        onPuertaDesbloqueada?.Invoke();
    }

    void AplicarDesbloqueoGuardado()
    {
        desbloqueada = true;

        if (puertaDoble != null) puertaDoble.Desbloquear();
        if (puertaSimple != null) puertaSimple.Desbloquear();
    }

    public void SetServidorActivo(bool activo) => servidorActivo = activo;
    public void SetTarjetaAccesoObtenida(bool obtenida) => tarjetaAccesoObtenida = obtenida;
    public void SetEventoCompleto(bool completo) => eventoCompleto = completo;

    void IntentarAbrirPanel()
    {
        ResultadoAcceso condicion = ValidarCondiciones();
        if (!condicion.autorizado)
        {
            HUDManager.MostrarError(condicion.mensaje);
            onAccesoDenegado?.Invoke();
            return;
        }

        if (requiereCodigo)
            CodePanelUI.Abrir(this);
        else
            DesbloquearDesdePanel();
    }

    ResultadoAcceso ValidarCondiciones()
    {
        if (requiereEnergia && !PowerManager.EnergiaActiva)
            return new ResultadoAcceso(false, mensajeSinEnergia);

        if (requiereServidorActivo && !servidorActivo)
            return new ResultadoAcceso(false, mensajeServidor);

        if (requiereTarjetaAcceso && !tarjetaAccesoObtenida)
            return new ResultadoAcceso(false, mensajeTarjeta);

        if (requiereEventoCompleto && !eventoCompleto)
            return new ResultadoAcceso(false, mensajeEvento);

        return new ResultadoAcceso(true, mensajeAutorizado);
    }

    void MostrarPrompt()
    {
        ResultadoAcceso condicion = ValidarCondiciones();
        if (condicion.autorizado)
            HUDManager.MostrarMensaje(mensajePrompt);
        else
            HUDManager.MostrarError(condicion.mensaje);
    }
}
