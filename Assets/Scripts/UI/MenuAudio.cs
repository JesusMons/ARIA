// ============================================================
//  MenuAudio.cs  →  Assets/Scripts/UI/
//  ARIA: Último Protocolo — Audio del menú principal
//
//  Adjuntar al mismo GameObject que MenuManager (Panel_MainMenu)
//  Conecta automáticamente el audio a todos los botones.
//  Responsable: Cristian Ramírez
// ============================================================

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class MenuAudio : MonoBehaviour
{
    // ──────────────────────────────────────────────────────
    void Start()
    {
        ConectarBotonesEnEscena();
    }

    // ── Conecta audio a TODOS los botones de la escena ───
    void ConectarBotonesEnEscena()
    {
        Button[] botones = FindObjectsByType<Button>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (Button btn in botones)
        {
            // Evitar doble registro
            MenuButtonAudio existing = btn.GetComponent<MenuButtonAudio>();
            if (existing != null) continue;

            // Detectar si es el botón de salir (para sonido diferente)
            bool esSalir = btn.name.ToLower().Contains("salir") ||
                           btn.name.ToLower().Contains("confirmar");

            MenuButtonAudio audio = btn.gameObject.AddComponent<MenuButtonAudio>();
            audio.esBotonPeligroso = esSalir;
        }
    }
}

// ============================================================
//  MenuButtonAudio — Componente por botón
//  Se agrega automáticamente por MenuAudio
// ============================================================
public class MenuButtonAudio : MonoBehaviour,
    IPointerEnterHandler, IPointerClickHandler
{
    public bool esBotonPeligroso = false;

    private Button btn;

    void Awake() => btn = GetComponent<Button>();

    // Hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (AudioManager.instancia == null) return;
        if (btn != null && !btn.interactable)
        {
            AudioManager.instancia.PlayError();
            return;
        }
        AudioManager.instancia.PlayHover();
    }

    // Clic
    public void OnPointerClick(PointerEventData eventData)
    {
        if (AudioManager.instancia == null) return;
        if (btn != null && !btn.interactable) return;

        if (esBotonPeligroso)
            AudioManager.instancia.PlayConfirmar();
        else if (name.ToLower().Contains("volver") ||
                 name.ToLower().Contains("cancelar"))
            AudioManager.instancia.PlayVolver();
        else
            AudioManager.instancia.PlayClic();
    }
}
