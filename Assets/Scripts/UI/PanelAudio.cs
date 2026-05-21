// ============================================================
//  PanelAudio.cs  →  Assets/Scripts/UI/
//  ARIA: Último Protocolo — Sonido al cambiar paneles
//
//  Adjuntar al mismo GameObject que UIManager (UI_Root)
//  Reemplaza el UIManager.Mostrar() con versión con audio.
//  Responsable: Cristian Ramírez
// ============================================================

using UnityEngine;

/// <summary>
/// Extiende UIManager para reproducir sonidos al cambiar paneles.
/// Adjuntar al UI_Root junto con UIManager.
/// </summary>
public class PanelAudio : MonoBehaviour
{
    [Header("Sonido al abrir/cerrar paneles")]
    [Tooltip("Si está vacío usa AudioManager.instancia automáticamente")]
    public AudioManager audioManager;

    void Awake()
    {
        if (audioManager == null)
            audioManager = FindFirstObjectByType<AudioManager>();
    }

    /// <summary>
    /// Llama a esto en vez de UIManager.instancia.Mostrar()
    /// para que suene el audio de transición.
    /// </summary>
    public void MostrarConAudio(GameObject panel)
    {
        if (UIManager.instancia == null) return;

        // ¿Estamos cerrando el menú principal?
        bool estaEnMenu = UIManager.instancia.EstaVisible(
            GameObject.Find("Panel_MainMenu"));

        if (estaEnMenu)
            audioManager?.PlayAbrirPanel();
        else
            audioManager?.PlayCerrarPanel();

        UIManager.instancia.Mostrar(panel);
    }
}
