// CodePanelUI.cs - Assets/Scripts/UI/
// ARIA: Ultimo Protocolo - Panel futurista de ingreso de codigo

using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class CodePanelUI : MonoBehaviour
{
    public static CodePanelUI instancia;

    [Header("Panel")]
    public GameObject panelCodigo;
    public CanvasGroup canvasGroup;

    [Header("Texto")]
    public TextMeshProUGUI txtTitulo;
    public TextMeshProUGUI txtDisplay;
    public TextMeshProUGUI txtMensaje;
    public TextMeshProUGUI txtEstado;

    [Header("Luces de estado")]
    public UnityEngine.UI.Image luzVerde;
    public UnityEngine.UI.Image luzRoja;

    [Header("Audio")]
    public AudioClip sonidoTecla;
    public AudioClip sonidoAutorizado;
    public AudioClip sonidoDenegado;

    [Header("Config")]
    public int longitudCodigo = 4;
    public float duracionFade = 0.12f;

    string codigoIngresado = "";
    bool abierto = false;
    bool procesando = false;
    float tiempoAbierto = 0f;
    CodeDoor puertaActual;
    AudioSource audioSrc;
    const float DELAY_CERRAR = 0.2f;

    void Awake()
    {
        instancia = this;
        audioSrc = GetComponent<AudioSource>();
        if (audioSrc == null) audioSrc = gameObject.AddComponent<AudioSource>();

        if (canvasGroup == null && panelCodigo != null)
            canvasGroup = panelCodigo.GetComponent<CanvasGroup>();

        if (panelCodigo != null) panelCodigo.SetActive(false);
        ActualizarDisplay();
        SetLuces(false, false);
    }

    void Update()
    {
        if (!abierto || procesando || Keyboard.current == null) return;

        tiempoAbierto += Time.unscaledDeltaTime;

        LeerNumero(Key.Digit0, "0");
        LeerNumero(Key.Digit1, "1");
        LeerNumero(Key.Digit2, "2");
        LeerNumero(Key.Digit3, "3");
        LeerNumero(Key.Digit4, "4");
        LeerNumero(Key.Digit5, "5");
        LeerNumero(Key.Digit6, "6");
        LeerNumero(Key.Digit7, "7");
        LeerNumero(Key.Digit8, "8");
        LeerNumero(Key.Digit9, "9");
        LeerNumero(Key.Numpad0, "0");
        LeerNumero(Key.Numpad1, "1");
        LeerNumero(Key.Numpad2, "2");
        LeerNumero(Key.Numpad3, "3");
        LeerNumero(Key.Numpad4, "4");
        LeerNumero(Key.Numpad5, "5");
        LeerNumero(Key.Numpad6, "6");
        LeerNumero(Key.Numpad7, "7");
        LeerNumero(Key.Numpad8, "8");
        LeerNumero(Key.Numpad9, "9");

        if (Keyboard.current.backspaceKey.wasPressedThisFrame) BorrarUltimo();
        if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame) Confirmar();
        if (tiempoAbierto >= DELAY_CERRAR && Keyboard.current.escapeKey.wasPressedThisFrame) Cerrar();
    }

    public static void Abrir(CodeDoor puerta)
    {
        if (instancia == null)
        {
            Debug.LogWarning("[CodePanelUI] No hay instancia en la escena.");
            return;
        }
        instancia.AbrirInterno(puerta);
    }

    public void PulsarNumero(string numero)
    {
        if (!abierto || procesando || string.IsNullOrEmpty(numero)) return;
        AgregarDigito(numero[0].ToString());
    }

    public void BorrarUltimo()
    {
        if (!abierto || procesando || codigoIngresado.Length == 0) return;
        codigoIngresado = codigoIngresado.Substring(0, codigoIngresado.Length - 1);
        ActualizarDisplay();
        SetMensaje("INGRESO MANUAL", false);
    }

    public void Limpiar()
    {
        codigoIngresado = "";
        ActualizarDisplay();
        SetMensaje("ESPERANDO CODIGO", false);
        SetLuces(false, false);
    }

    public void Confirmar()
    {
        if (!abierto || procesando || puertaActual == null) return;
        StartCoroutine(ProcesarCodigo());
    }

    public void Cerrar()
    {
        if (!abierto) return;
        abierto = false;
        puertaActual = null;
        AudioManager.instancia?.PlayCerrarPanel();
        StartCoroutine(FadeCerrar());
    }

    void AbrirInterno(CodeDoor puerta)
    {
        puertaActual = puerta;
        abierto = true;
        procesando = false;
        tiempoAbierto = 0f;
        codigoIngresado = "";

        if (panelCodigo != null) panelCodigo.SetActive(true);
        if (txtTitulo != null)
            txtTitulo.text = puerta != null ? puerta.NombrePanel.ToUpper() : "ACCESO RESTRINGIDO";

        SetMensaje("ESPERANDO CODIGO", false);
        SetLuces(false, false);
        ActualizarDisplay();
        AudioManager.instancia?.PlayAbrirPanel();

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(FadeAbrir());
    }

    void LeerNumero(Key tecla, string valor)
    {
        if (Keyboard.current[tecla].wasPressedThisFrame)
            AgregarDigito(valor);
    }

    void AgregarDigito(string digito)
    {
        if (codigoIngresado.Length >= longitudCodigo) return;
        codigoIngresado += digito;
        if (sonidoTecla != null) audioSrc.PlayOneShot(sonidoTecla);
        ActualizarDisplay();
        SetMensaje("VALIDACION PENDIENTE", false);
    }

    IEnumerator ProcesarCodigo()
    {
        procesando = true;
        SetMensaje("AUTENTICANDO...", false);
        yield return new WaitForSecondsRealtime(0.35f);

        CodeDoor.ResultadoAcceso resultado = puertaActual.ValidarAcceso(codigoIngresado);
        if (resultado.autorizado)
        {
            SetLuces(true, false);
            SetMensaje(resultado.mensaje, true);
            if (sonidoAutorizado != null) audioSrc.PlayOneShot(sonidoAutorizado);
            AudioManager.instancia?.PlayConfirmar();
            puertaActual.DesbloquearDesdePanel();
            yield return new WaitForSecondsRealtime(0.55f);
            Cerrar();
        }
        else
        {
            SetLuces(false, true);
            SetMensaje(resultado.mensaje, true);
            if (sonidoDenegado != null) audioSrc.PlayOneShot(sonidoDenegado);
            AudioManager.instancia?.PlayError();
            codigoIngresado = "";
            ActualizarDisplay();
            yield return new WaitForSecondsRealtime(0.35f);
            procesando = false;
        }
    }

    IEnumerator FadeAbrir()
    {
        if (canvasGroup == null) yield break;
        canvasGroup.alpha = 0f;
        float t = 0f;
        while (t < duracionFade)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / duracionFade);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    IEnumerator FadeCerrar()
    {
        if (canvasGroup != null)
        {
            float inicio = canvasGroup.alpha;
            float t = 0f;
            while (t < duracionFade)
            {
                t += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(inicio, 0f, t / duracionFade);
                yield return null;
            }
        }

        if (panelCodigo != null) panelCodigo.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void ActualizarDisplay()
    {
        if (txtDisplay == null) return;

        string salida = "";
        for (int i = 0; i < longitudCodigo; i++)
            salida += i < codigoIngresado.Length ? codigoIngresado[i].ToString() : "_";
        txtDisplay.text = salida;
    }

    void SetMensaje(string mensaje, bool estadoPrincipal)
    {
        if (txtMensaje != null) txtMensaje.text = mensaje;
        if (txtEstado != null && estadoPrincipal) txtEstado.text = mensaje;
        else if (txtEstado != null) txtEstado.text = "PANEL INDUSTRIAL // ARIA";
    }

    void SetLuces(bool verde, bool rojo)
    {
        if (luzVerde != null) luzVerde.color = verde ? new Color32(0, 255, 160, 255) : new Color32(0, 80, 60, 160);
        if (luzRoja != null) luzRoja.color = rojo ? new Color32(255, 60, 70, 255) : new Color32(80, 20, 25, 160);
    }
}
