// ZonaTriggerAudio.cs — Assets/Scripts/GameSystems/
// ARIA: Último Protocolo — Cambia el ambiente al entrar a una zona
//
// SETUP:
// 1. Crear GameObject vacío en cada habitación
// 2. Add Component → ZonaTriggerAudio
// 3. Agregar BoxCollider → Is Trigger = true
// 4. Asignar el clip de ambiente de esa zona

using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ZonaTriggerAudio : MonoBehaviour
{
    [Header("Audio de esta zona")]
    public AudioClip ambienciaZona;

    [Header("Estado musical al entrar")]
    public GameAudioController.EstadoMusica estadoAlEntrar
        = GameAudioController.EstadoMusica.Exploracion;

    [Header("Opciones")]
    public bool cambiarEstadoMusical = false;

    void Start()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("GameController") &&
            !other.CompareTag("Player")) return;

        if (ambienciaZona != null)
            GameAudioController.SetAmbiente(ambienciaZona);

        if (cambiarEstadoMusical)
            GameAudioController.SetEstado(estadoAlEntrar);
    }
}
