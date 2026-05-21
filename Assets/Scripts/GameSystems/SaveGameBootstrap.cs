// SaveGameBootstrap.cs — Assets/Scripts/GameSystems/
// ARIA: Último Protocolo — Aplica estado guardado al entrar a escena

using UnityEngine;

public class SaveGameBootstrap : MonoBehaviour
{
    public float retrasoAplicacion = 0.3f;

    void Start()
    {
        Invoke(nameof(Aplicar), retrasoAplicacion);
    }

    void Aplicar()
    {
        // No aplicar estado en partidas nuevas — tienen estado limpio
        // y los códigos ya fueron generados en NoteManager.Awake()
        if (SaveGameManager.EsNuevaPartida)
        {
            Debug.Log("[SaveGameBootstrap] Partida nueva — omitiendo AplicarEstadoCargado.");
            return;
        }

        SaveGameManager.AplicarEstadoCargado();
    }
}