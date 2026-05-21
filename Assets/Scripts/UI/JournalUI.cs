// JournalUI.cs - Assets/Scripts/UI/
// ARIA: Ultimo Protocolo - Registro de notas del Dr. Voss

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class JournalUI : MonoBehaviour
{
    public static JournalUI instancia;

    [Header("Panel")]
    public GameObject panelDiario;
    public CanvasGroup canvasGroup;

    [Header("Lista")]
    public Transform contenedorNotas;
    public GameObject prefabItemNota;
    public TextMeshProUGUI txtContador;

    [Header("Detalle")]
    public TextMeshProUGUI txtTitulo;
    public TextMeshProUGUI txtFecha;
    public TextMeshProUGUI txtUbicacion;
    public TextMeshProUGUI txtTipo;
    public TextMeshProUGUI txtContenido;
    public TextMeshProUGUI txtCodigo;
    public TextMeshProUGUI txtEstado;

    [Header("Config")]
    public Key teclaDiario = Key.J;
    public float altoItemNota = 44f;
    public float separacionItems = 6f;
    public float fontSizeItem = 11f;

    bool abierto = false;
    float tiempoAbierto = 0f;
    const float DELAY_CERRAR = 0.2f;
    readonly List<GameObject> items = new List<GameObject>();

    void Awake()
    {
        instancia = this;
        if (panelDiario != null) panelDiario.SetActive(false);
        if (canvasGroup == null && panelDiario != null)
            canvasGroup = panelDiario.GetComponent<CanvasGroup>();
    }

    void Start()
    {
        if (NoteManager.instancia != null)
            NoteManager.instancia.onNotaEncontrada.AddListener(OnNotaEncontrada);

        ConfigurarLayoutLista();
        ActualizarLista();
        MostrarEstadoVacio();
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        if (!abierto && Keyboard.current[teclaDiario].wasPressedThisFrame)
        {
            Abrir();
            return;
        }

        if (!abierto) return;

        tiempoAbierto += Time.unscaledDeltaTime;
        if (tiempoAbierto < DELAY_CERRAR) return;

        if (Keyboard.current[teclaDiario].wasPressedThisFrame ||
            Keyboard.current.escapeKey.wasPressedThisFrame)
            Cerrar();
    }

    public static void Refrescar()
    {
        instancia?.ActualizarLista();
    }

    void OnNotaEncontrada(VossNoteData nota)
    {
        ActualizarLista();
        if (abierto) SeleccionarNota(nota);
    }

    public void Abrir()
    {
        abierto = true;
        tiempoAbierto = 0f;

        ActualizarLista();
        if (panelDiario != null) panelDiario.SetActive(true);
        if (canvasGroup != null) canvasGroup.alpha = 1f;

        AudioManager.instancia?.PlayAbrirPanel();
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Cerrar()
    {
        abierto = false;
        tiempoAbierto = 0f;

        if (panelDiario != null) panelDiario.SetActive(false);
        AudioManager.instancia?.PlayCerrarPanel();

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void ActualizarLista()
    {
        ConfigurarLayoutLista();

        foreach (GameObject item in items)
            if (item != null) Destroy(item);
        items.Clear();

        List<VossNoteData> notas = NoteManager.ObtenerNotasRecolectadas();

        if (txtContador != null)
            txtContador.text = notas.Count.ToString("00") + " ARCHIVOS";

        if (contenedorNotas == null || prefabItemNota == null)
        {
            if (notas.Count > 0) SeleccionarNota(notas[0]);
            return;
        }

        foreach (VossNoteData nota in notas)
        {
            if (nota == null) continue;

            GameObject go = Instantiate(prefabItemNota, contenedorNotas);
            go.SetActive(true);
            items.Add(go);
            ConfigurarItemLista(go);

            TextMeshProUGUI txt = go.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null)
            {
                string tipo = nota.tipo == TipoNota.Digital ? "DIG" : "FIS";
                txt.fontSize = fontSizeItem;
                txt.enableWordWrapping = false;
                txt.overflowMode = TextOverflowModes.Ellipsis;
                txt.alignment = TextAlignmentOptions.MidlineLeft;
                txt.text = $"[{tipo}] {nota.titulo}\n<size=80%>{nota.ubicacion}</size>";
            }

            Button boton = go.GetComponent<Button>();
            if (boton == null) boton = go.AddComponent<Button>();
            VossNoteData notaLocal = nota;
            boton.onClick.AddListener(() => SeleccionarNota(notaLocal));
        }

        if (notas.Count > 0) SeleccionarNota(notas[0]);
        else MostrarEstadoVacio();
    }

    void ConfigurarLayoutLista()
    {
        if (contenedorNotas == null) return;

        VerticalLayoutGroup layout = contenedorNotas.GetComponent<VerticalLayoutGroup>();
        if (layout == null) return;

        layout.spacing = separacionItems;
        layout.childControlHeight = true;
        layout.childForceExpandHeight = false;
        layout.childControlWidth = true;
        layout.childForceExpandWidth = true;
    }

    void ConfigurarItemLista(GameObject item)
    {
        if (item == null) return;

        RectTransform rt = item.GetComponent<RectTransform>();
        if (rt != null)
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, altoItemNota);

        LayoutElement layoutElement = item.GetComponent<LayoutElement>();
        if (layoutElement == null) layoutElement = item.AddComponent<LayoutElement>();

        layoutElement.minHeight = altoItemNota;
        layoutElement.preferredHeight = altoItemNota;
        layoutElement.flexibleHeight = 0f;
    }

    void SeleccionarNota(VossNoteData nota)
    {
        if (nota == null)
        {
            MostrarEstadoVacio();
            return;
        }

        if (txtEstado != null) txtEstado.text = "// ARCHIVO RECUPERADO";
        if (txtTitulo != null) txtTitulo.text = nota.titulo?.ToUpper() ?? "";
        string tipo = nota.tipo == TipoNota.Digital ? "NOTA DIGITAL" : "NOTA FISICA";
        if (txtFecha != null && txtFecha == txtUbicacion && txtFecha == txtTipo)
        {
            txtFecha.text = $"{nota.fecha}  |  {nota.ubicacion}  |  {tipo}";
        }
        else
        {
            if (txtFecha != null) txtFecha.text = nota.fecha;
            if (txtUbicacion != null) txtUbicacion.text = nota.ubicacion;
            if (txtTipo != null) txtTipo.text = tipo;
        }

        string contenido = nota.contenido ?? "";
        if (!string.IsNullOrEmpty(nota.idPuertaAsociada))
        {
            string codigo = NoteManager.ObtenerCodigo(nota.idPuertaAsociada);
            contenido = contenido
                .Replace("[CODIGO_SERVER]", codigo)
                .Replace("[CODIGO]", codigo)
                .Replace("[CODE]", codigo);
        }

        if (txtContenido != null) txtContenido.text = contenido;

        if (txtCodigo != null)
        {
            bool tieneCodigo = !string.IsNullOrEmpty(nota.idPuertaAsociada);
            txtCodigo.gameObject.SetActive(tieneCodigo);
            if (tieneCodigo)
                txtCodigo.text = "// CODIGO ASOCIADO: " + NoteManager.ObtenerCodigo(nota.idPuertaAsociada);
        }
    }

    void MostrarEstadoVacio()
    {
        if (txtEstado != null) txtEstado.text = "// SIN ARCHIVOS RECUPERADOS";
        if (txtTitulo != null) txtTitulo.text = "REGISTRO VACIO";
        if (txtFecha != null && txtFecha == txtUbicacion && txtFecha == txtTipo)
        {
            txtFecha.text = "--  |  SIN UBICACION  |  DR. VOSS";
        }
        else
        {
            if (txtFecha != null) txtFecha.text = "--";
            if (txtUbicacion != null) txtUbicacion.text = "SIN UBICACION";
            if (txtTipo != null) txtTipo.text = "DR. VOSS";
        }
        if (txtContenido != null) txtContenido.text = "Las notas encontradas quedaran almacenadas aqui.";
        if (txtCodigo != null) txtCodigo.gameObject.SetActive(false);
    }
}
