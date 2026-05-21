// ============================================================
//  OpcionesController.cs  →  Assets/Scripts/UI/
//  ARIA: Último Protocolo — Panel de Opciones funcional
//
//  Adjuntar al Panel_Opciones
// ============================================================

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OpcionesController : MonoBehaviour
{
    [Header("Sliders de Audio")]
    public Slider sliderVolumenGeneral;
    public Slider sliderSFX;
    public Slider sliderMusica;

    [Header("Toggles de Gráficos")]
    public Toggle toggleFullscreen;
    public Toggle toggleVSync;

    // ──────────────────────────────────────────────────────
    void Start()
    {
        BuscarComponentes();
        CargarValoresGuardados();
        SuscribirEventos();
    }

    void BuscarComponentes()
    {
        if (sliderVolumenGeneral == null)
            sliderVolumenGeneral = Buscar<Slider>("SliderVolumen");
        if (sliderSFX == null)
            sliderSFX = Buscar<Slider>("SliderSFX");
        if (sliderMusica == null)
            sliderMusica = Buscar<Slider>("SliderMusica");
        if (toggleFullscreen == null)
            toggleFullscreen = Buscar<Toggle>("ToggleFullscreen");
        if (toggleVSync == null)
            toggleVSync = Buscar<Toggle>("ToggleVSync");
    }

    T Buscar<T>(string nombre) where T : Component
    {
        // Busca en hijos directos y nietos
        T comp = GetComponentsInChildren<T>(true).Length > 0
            ? System.Array.Find(GetComponentsInChildren<T>(true),
                c => c.name == nombre)
            : null;
        return comp;
    }

    void CargarValoresGuardados()
    {
        float vG = PlayerPrefs.GetFloat("Vol_General", 0.8f);
        float vS = PlayerPrefs.GetFloat("Vol_SFX",     0.8f);
        float vM = PlayerPrefs.GetFloat("Vol_Musica",  0.4f);
        bool  fs = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        bool  vs = PlayerPrefs.GetInt("VSync", 1) == 1;

        if (sliderVolumenGeneral != null) sliderVolumenGeneral.value = vG;
        if (sliderSFX            != null) sliderSFX.value            = vS;
        if (sliderMusica         != null) sliderMusica.value         = vM;
        if (toggleFullscreen     != null) toggleFullscreen.isOn      = fs;
        if (toggleVSync          != null) toggleVSync.isOn           = vs;

        AplicarTodo(vG, vS, vM, fs, vs);
    }

    void SuscribirEventos()
    {
        sliderVolumenGeneral?.onValueChanged.AddListener(v => {
            AudioListener.volume = v;
            PlayerPrefs.SetFloat("Vol_General", v);
            PlayerPrefs.Save();
        });

        sliderSFX?.onValueChanged.AddListener(v => {
            AudioManager.instancia?.SetVolumenUI(v);
            PlayerPrefs.SetFloat("Vol_SFX", v);
            PlayerPrefs.Save();
        });

        sliderMusica?.onValueChanged.AddListener(v => {
            AudioManager.instancia?.SetVolumenMusica(v);
            PlayerPrefs.SetFloat("Vol_Musica", v);
            PlayerPrefs.Save();
        });

        toggleFullscreen?.onValueChanged.AddListener(v => {
            Screen.fullScreen = v;
            PlayerPrefs.SetInt("Fullscreen", v ? 1 : 0);
            PlayerPrefs.Save();
        });

        toggleVSync?.onValueChanged.AddListener(v => {
            QualitySettings.vSyncCount = v ? 1 : 0;
            PlayerPrefs.SetInt("VSync", v ? 1 : 0);
            PlayerPrefs.Save();
        });
    }

    void AplicarTodo(float vG, float vS, float vM, bool fs, bool vs)
    {
        AudioListener.volume = vG;
        AudioManager.instancia?.SetVolumenUI(vS);
        AudioManager.instancia?.SetVolumenMusica(vM);
        Screen.fullScreen = fs;
        QualitySettings.vSyncCount = vs ? 1 : 0;
    }

    public void ResetearDefecto()
    {
        if (sliderVolumenGeneral != null) sliderVolumenGeneral.value = 0.8f;
        if (sliderSFX            != null) sliderSFX.value            = 0.8f;
        if (sliderMusica         != null) sliderMusica.value         = 0.4f;
        if (toggleFullscreen     != null) toggleFullscreen.isOn      = true;
        if (toggleVSync          != null) toggleVSync.isOn           = true;
    }
}
