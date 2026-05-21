// MenuSaveUI.cs — Assets/Scripts/UI/
// ARIA: Último Protocolo — Paneles de Nueva Partida y Continuar

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MenuSaveUI : MonoBehaviour
{
    [Header("Panel Nueva Partida")]
    public GameObject      panelNuevaPartida;
    public TMP_InputField  inputNombrePartida;
    public TextMeshProUGUI txtErrorNuevaPartida;
    public Button          btnCrearPartida;
    public Button          btnCancelarNueva;

    [Header("Panel Continuar")]
    public GameObject      panelContinuar;
    public Transform       contenedorPartidas;
    public GameObject      prefabPartida;
    public TextMeshProUGUI txtSinPartidas;
    public Button          btnVolverContinuar;

    [Header("Estilo lista")]
    public float fontSizePartida = 15f;
    public float altoItemPartida = 64f;

    [Header("Referencias")]
    public MenuManager menuManager;

    readonly List<GameObject> items = new List<GameObject>();

    // ──────────────────────────────────────────────────────
    void Awake()
    {
        if (menuManager == null)
            menuManager = GetComponent<MenuManager>();

        btnCrearPartida?.onClick.AddListener(CrearPartida);
        btnCancelarNueva?.onClick.AddListener(VolverMenu);
        btnVolverContinuar?.onClick.AddListener(VolverMenu);

        if (panelNuevaPartida != null) panelNuevaPartida.SetActive(false);
        if (panelContinuar    != null) panelContinuar.SetActive(false);
    }

    // ── Mostrar paneles ────────────────────────────────────
    public void MostrarNuevaPartida()
    {
        if (panelNuevaPartida == null)
        {
            CrearPartidaDirecta("Partida ARIA");
            return;
        }

        OcultarPanelesBase();
        panelNuevaPartida.SetActive(true);

        if (txtErrorNuevaPartida != null) txtErrorNuevaPartida.text = "";
        if (inputNombrePartida   != null)
        {
            inputNombrePartida.text = "";
            inputNombrePartida.ActivateInputField();
        }
    }

    public void MostrarContinuar()
    {
        if (panelContinuar == null)
        {
            List<SaveGameData> partidas = SaveGameManager.ObtenerPartidas();
            if (partidas.Count > 0) CargarPartida(partidas[0]);
            return;
        }

        OcultarPanelesBase();
        panelContinuar.SetActive(true);
        RefrescarListaPartidas();
    }

    // ── Crear partida ──────────────────────────────────────
    public void CrearPartida()
    {
        string nombre = inputNombrePartida != null ? inputNombrePartida.text : "";
        if (string.IsNullOrWhiteSpace(nombre))
        {
            if (txtErrorNuevaPartida != null)
                txtErrorNuevaPartida.text = "INGRESA UN NOMBRE PARA LA PARTIDA";
            return;
        }
        CrearPartidaDirecta(nombre);
    }

    void CrearPartidaDirecta(string nombre)
    {
        string escena = menuManager != null ? menuManager.nombreEscenaJuego : "level_1";
        escena = SceneLoader.NormalizarNombreEscena(escena);
        SaveGameManager.CrearNuevaPartida(nombre, escena);
        PrepararCargaJuego();
        SceneLoader.instancia.CargarEscena(escena);
    }

    // ── Lista de partidas ──────────────────────────────────
    void RefrescarListaPartidas()
    {
        foreach (GameObject item in items)
            if (item != null) Destroy(item);
        items.Clear();

        List<SaveGameData> partidas = SaveGameManager.ObtenerPartidas();
        if (txtSinPartidas != null)
            txtSinPartidas.gameObject.SetActive(partidas.Count == 0);

        if (contenedorPartidas == null || prefabPartida == null) return;

        foreach (SaveGameData partida in partidas)
        {
            GameObject go = Instantiate(prefabPartida, contenedorPartidas);
            go.SetActive(true);
            items.Add(go);

            TextMeshProUGUI txt = go.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null)
            {
                txt.fontSize          = fontSizePartida;
                txt.enableWordWrapping = false;
                txt.overflowMode      = TextOverflowModes.Ellipsis;
                txt.text = $"{partida.nombrePartida}\n<size=75%>{partida.escenaActual}  |  {partida.fechaActualizacion}</size>";

                RectTransform rtTxt = txt.rectTransform;
                rtTxt.anchorMin = Vector2.zero;
                rtTxt.anchorMax = Vector2.one;
                rtTxt.offsetMin = new Vector2(8f, 0f);
                rtTxt.offsetMax = new Vector2(-(altoItemPartida + 4f), 0f);
            }

            LayoutElement layout = go.GetComponent<LayoutElement>();
            if (layout == null) layout = go.AddComponent<LayoutElement>();
            layout.minHeight       = altoItemPartida;
            layout.preferredHeight = altoItemPartida;
            layout.flexibleHeight  = 0f;

            Button boton = go.GetComponent<Button>();
            if (boton == null) boton = go.AddComponent<Button>();
            SaveGameData local = partida;
            boton.onClick.AddListener(() => CargarPartida(local));

            AgregarBotonEliminar(go, partida);
        }
    }

    void AgregarBotonEliminar(GameObject item, SaveGameData partida)
    {
        GameObject btnDelObj = new GameObject("BtnEliminar", typeof(RectTransform));
        btnDelObj.transform.SetParent(item.transform, false);
        btnDelObj.AddComponent<Image>().color = new Color(0.65f, 0.1f, 0.1f, 0.9f);

        RectTransform rtDel = btnDelObj.GetComponent<RectTransform>();
        rtDel.anchorMin = new Vector2(1f, 0f);
        rtDel.anchorMax = new Vector2(1f, 1f);
        rtDel.pivot     = new Vector2(1f, 0.5f);
        rtDel.offsetMin = new Vector2(-altoItemPartida, 0f);
        rtDel.offsetMax = Vector2.zero;

        GameObject txtDelObj = new GameObject("TxtEliminar", typeof(RectTransform));
        txtDelObj.transform.SetParent(btnDelObj.transform, false);
        TextMeshProUGUI txtDel = txtDelObj.AddComponent<TextMeshProUGUI>();
        txtDel.text      = "X";
        txtDel.fontSize  = 22f;
        txtDel.alignment = TextAlignmentOptions.Center;
        txtDel.color     = Color.white;
        RectTransform rtTxtDel = txtDelObj.GetComponent<RectTransform>();
        rtTxtDel.anchorMin = Vector2.zero;
        rtTxtDel.anchorMax = Vector2.one;
        rtTxtDel.offsetMin = Vector2.zero;
        rtTxtDel.offsetMax = Vector2.zero;

        Button botonDel = btnDelObj.AddComponent<Button>();
        ColorBlock cb   = botonDel.colors;
        cb.highlightedColor = new Color(0.85f, 0.2f, 0.2f, 1f);
        cb.pressedColor     = new Color(0.45f, 0.05f, 0.05f, 1f);
        botonDel.colors = cb;

        SaveGameData localPartida = partida;
        botonDel.onClick.AddListener(() => EliminarPartida(localPartida));
    }

    void EliminarPartida(SaveGameData partida)
    {
        if (partida == null) return;
        SaveGameManager.EliminarPartida(partida.slotId);
        RefrescarListaPartidas();
        menuManager?.VerificarPartidaGuardada();
    }

    void CargarPartida(SaveGameData partida)
    {
        if (partida == null) return;
        partida.escenaActual = SceneLoader.NormalizarNombreEscena(partida.escenaActual);
        SaveGameManager.Guardar(partida);
        if (!SaveGameManager.SeleccionarPartida(partida.slotId)) return;
        PrepararCargaJuego();
        SceneLoader.instancia.CargarEscena(partida.escenaActual);
    }

    // ── Volver al menú ─────────────────────────────────────
    public void OcultarPanelesGuardado()
    {
        if (panelNuevaPartida != null) panelNuevaPartida.SetActive(false);
        if (panelContinuar    != null) panelContinuar.SetActive(false);
    }

    void PrepararCargaJuego()
    {
        OcultarPanelesGuardado();
        OcultarPanelesBase();
    }

    void VolverMenu()
    {
        OcultarPanelesGuardado();

        // Resetear estado del UIManager para que Mostrar() funcione
        // aunque menuPanel ya fuera el "panelActual" registrado
        UIManager.instancia?.ResetearPanelActual();
        UIManager.instancia?.MostrarCanvasMenu(true);

        menuManager?.MostrarMenuPrincipal();
    }

    void OcultarPanelesBase()
    {
        if (menuManager == null) return;
        if (menuManager.menuPanel    != null) menuManager.menuPanel.SetActive(false);
        if (menuManager.opcionesPanel!= null) menuManager.opcionesPanel.SetActive(false);
        if (menuManager.creditosPanel!= null) menuManager.creditosPanel.SetActive(false);
        if (menuManager.salirPanel   != null) menuManager.salirPanel.SetActive(false);
    }
}