// NoteReaderUI.cs — Assets/Scripts/UI/
// ARIA: Último Protocolo — UI para leer notas del Dr. Voss

using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class NoteReaderUI : MonoBehaviour
{
    public static NoteReaderUI instancia;

    [Header("Panel de lectura")]
    public GameObject      panelNota;
    public TextMeshProUGUI txtTitulo;
    public TextMeshProUGUI txtFecha;
    public TextMeshProUGUI txtUbicacion;
    public TextMeshProUGUI txtContenido;
    public TextMeshProUGUI txtCodigo;

    bool  panelAbierto  = false;
    float tiempoAbierto = 0f;          // evita cerrar en el mismo frame
    const float DELAY_CERRAR = 0.2f;   // segundos antes de permitir cerrar

    void Awake()
    {
        instancia = this;
        if (panelNota != null) panelNota.SetActive(false);
    }

    void Update()
    {
        if (!panelAbierto) return;

        // Esperar un pequeño delay antes de escuchar la tecla de cierre
        tiempoAbierto += Time.unscaledDeltaTime;
        if (tiempoAbierto < DELAY_CERRAR) return;

        if (Keyboard.current.eKey.wasPressedThisFrame      ||
            Keyboard.current.escapeKey.wasPressedThisFrame  ||
            Keyboard.current.tabKey.wasPressedThisFrame)
            Cerrar();
    }

    // ── API estática ───────────────────────────────────────
    public static void MostrarNota(VossNoteData nota)
    {
        if (instancia == null)
        {
            Debug.LogWarning("[NoteReaderUI] No hay instancia en la escena.");
            return;
        }
        if (nota != null && !NoteManager.FueRecolectada(nota.id))
            NoteManager.RegistrarNota(nota);
        instancia.Mostrar(nota);
    }

    // ── Internos ───────────────────────────────────────────
    void Mostrar(VossNoteData nota)
    {
        if (nota == null) return;

        panelAbierto  = true;
        tiempoAbierto = 0f;   // resetear timer anti-cierre

        if (panelNota != null) panelNota.SetActive(true);

        // Título
        if (txtTitulo != null)
            txtTitulo.text = nota.titulo?.ToUpper() ?? "";

        // Fecha y ubicación (mismo campo o separados)
        string meta = $"{nota.fecha}  |  {nota.ubicacion}";
        if (txtFecha     != null) txtFecha.text     = meta;
        if (txtUbicacion != null) txtUbicacion.text = meta;

        // Contenido — reemplazar placeholder del código
        string contenido = nota.contenido ?? "";
        if (!string.IsNullOrEmpty(nota.idPuertaAsociada))
        {
            string codigo = NoteManager.ObtenerCodigo(nota.idPuertaAsociada);
            contenido = contenido
                .Replace("[CODIGO_SERVER]", codigo)
                .Replace("[CODIGO]",        codigo)
                .Replace("[CODE]",          codigo);
        }
        if (txtContenido != null)
            txtContenido.text = contenido;

        // Código de acceso
        if (txtCodigo != null)
        {
            if (!string.IsNullOrEmpty(nota.idPuertaAsociada))
            {
                string codigo = NoteManager.ObtenerCodigo(nota.idPuertaAsociada);
                txtCodigo.text = $"// CODIGO DE ACCESO: {codigo}";
                txtCodigo.gameObject.SetActive(true);
            }
            else
            {
                txtCodigo.gameObject.SetActive(false);
            }
        }

        // Pausar juego
        Time.timeScale       = 0f;
        Cursor.lockState     = CursorLockMode.None;
        Cursor.visible       = true;
    }

    void Cerrar()
    {
        panelAbierto  = false;
        tiempoAbierto = 0f;

        if (panelNota != null) panelNota.SetActive(false);

        Time.timeScale       = 1f;
        Cursor.lockState     = CursorLockMode.Locked;
        Cursor.visible       = false;
    }
}
