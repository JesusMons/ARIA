// MenuManager.cs — Assets/Scripts/UI/
// ARIA: Último Protocolo — Controlador del Menú Principal
// Responsable: Cristian Ramírez

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject menuPanel;
    public GameObject opcionesPanel;
    public GameObject creditosPanel;
    public GameObject salirPanel;
    public MenuSaveUI saveUI;

    [Header("Botones Menú Principal")]
    public Button btnNuevaPartida;
    public Button btnContinuar;
    public Button btnOpciones;
    public Button btnCreditos;
    public Button btnSalir;

    [Header("Botones Panel Salir")]
    public Button btnConfirmarSalir;
    public Button btnCancelarSalir;

    [Header("Botones Volver")]
    public Button btnVolverDeOpciones;
    public Button btnVolverDeCreditos;

    [Header("Configuración")]
    public string nombreEscenaJuego = "level_1";

    private bool hayPartidaGuardada = false;

    // ──────────────────────────────────────────────────────
    void Start()
    {
        if (saveUI == null)
            saveUI = GetComponent<MenuSaveUI>();

        nombreEscenaJuego = SceneLoader.NormalizarNombreEscena(nombreEscenaJuego);
        SuscribirBotones();
        MostrarMenuPrincipal();
    }

    // ── Verificar save ─────────────────────────────────────
    public void VerificarPartidaGuardada()
    {
        hayPartidaGuardada = SaveGameManager.HayPartidas();
        if (btnContinuar != null)
            btnContinuar.interactable = hayPartidaGuardada;
    }

    // ── Suscribir botones ──────────────────────────────────
    void SuscribirBotones()
    {
        btnNuevaPartida?.onClick.AddListener(OnNuevaPartida);
        btnContinuar?.onClick.AddListener(OnContinuar);
        btnOpciones?.onClick.AddListener(OnOpciones);
        btnCreditos?.onClick.AddListener(OnCreditos);
        btnSalir?.onClick.AddListener(OnSalir);

        btnConfirmarSalir?.onClick.AddListener(OnConfirmarSalir);
        btnCancelarSalir?.onClick.AddListener(OnCancelarSalir);

        btnVolverDeOpciones?.onClick.AddListener(MostrarMenuPrincipal);
        btnVolverDeCreditos?.onClick.AddListener(MostrarMenuPrincipal);
    }

    // ── Acciones ───────────────────────────────────────────
    public void OnNuevaPartida()
    {
        if (saveUI != null)
            saveUI.MostrarNuevaPartida();
        else
        {
            SaveGameManager.CrearNuevaPartida("Partida ARIA", nombreEscenaJuego);
            SceneLoader.instancia.CargarEscena(nombreEscenaJuego);
        }
    }

    public void OnContinuar()
    {
        if (!hayPartidaGuardada) return;

        if (saveUI != null)
            saveUI.MostrarContinuar();
        else
        {
            string ultimaNivel = PlayerPrefs.GetString("UltimaNivel", nombreEscenaJuego);
            ultimaNivel = SceneLoader.NormalizarNombreEscena(ultimaNivel);
            SceneLoader.instancia.CargarEscena(ultimaNivel);
        }
    }

    public void OnOpciones()  => UIManager.instancia.Mostrar(opcionesPanel);
    public void OnCreditos()  => UIManager.instancia.Mostrar(creditosPanel);
    public void OnSalir()     => UIManager.instancia.Mostrar(salirPanel);

    public void OnConfirmarSalir()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OnCancelarSalir() => MostrarMenuPrincipal();

    // ── Mostrar menú principal ─────────────────────────────
    public void MostrarMenuPrincipal()
    {
        // Verificar partidas CADA VEZ que se muestra el menú
        // así el botón Continuar se actualiza al regresar del juego
        VerificarPartidaGuardada();
        UIManager.instancia.Mostrar(menuPanel);
    }
}