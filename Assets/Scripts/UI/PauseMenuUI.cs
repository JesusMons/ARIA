// PauseMenuUI.cs — Assets/Scripts/UI/
// ARIA: Último Protocolo — Menú de pausa en juego
//
// JERARQUÍA ESPERADA EN LA ESCENA DE JUEGO:
//
//  Canvas_HUD
//    ├── BTN_Pausa                 ← asignar en btnPausa  (visible siempre, esquina)
//    └── Panel_Pausa               ← asignar en panelPausa (oculto al inicio)
//          ├── TXT_Titulo          (opcional, texto "PAUSA")
//          ├── BTN_Reanudar        ← asignar en btnReanudar
//          ├── BTN_Guardar         ← asignar en btnGuardar
//          └── BTN_SalirMenu       ← asignar en btnSalirMenu
//
//  El Canvas_HUD debe existir solo en la escena de juego (no es DontDestroyOnLoad).

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    public static PauseMenuUI instancia;

    [Header("Botón HUD (siempre visible)")]
    public Button btnPausa;

    [Header("Panel de pausa")]
    public GameObject panelPausa;
    public Button btnReanudar;
    public Button btnGuardar;
    public Button btnSalirMenu;

    bool pausado = false;

    void Awake()
    {
        instancia = this;

        btnPausa?.onClick.AddListener(TogglePausa);
        btnReanudar?.onClick.AddListener(Reanudar);
        btnGuardar?.onClick.AddListener(OnGuardar);
        btnSalirMenu?.onClick.AddListener(OnSalirMenu);

        if (panelPausa != null) panelPausa.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current == null) return;
        if (!Keyboard.current.escapeKey.wasPressedThisFrame) return;

        if (pausado)
            Reanudar();
        else if (Time.timeScale == 1f)
            Pausar();
    }

    public void TogglePausa()
    {
        if (pausado) Reanudar();
        else Pausar();
    }

    public void Pausar()
    {
        pausado = true;
        if (panelPausa != null) panelPausa.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        AudioManager.instancia?.PlayAbrirPanel();
    }

    public void Reanudar()
    {
        pausado = false;
        if (panelPausa != null) panelPausa.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        AudioManager.instancia?.PlayCerrarPanel();
    }

    void OnGuardar()
    {
        SaveGameData data = SaveGameManager.PartidaActual;
        if (data != null)
        {
            SaveGameManager.CapturarEstadoEnPartida(data);
            SaveGameManager.Guardar(data);
        }
        else
        {
            SaveGameManager.GuardarPartidaActual();
        }
        HUDManager.MostrarMensajeTemporal("PARTIDA GUARDADA", 2f);
    }

    void OnSalirMenu()
    {
        OnGuardar();
        pausado = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneLoader.instancia.IrAlMenu();
    }
}
