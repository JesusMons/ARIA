// LightingController.cs — Assets/Scripts/GameSystems/
// ARIA: Último Protocolo — Controlador de luces individuales
//
// SETUP:
// Adjuntar a cualquier luz que deba encenderse con la energía
// Se suscribe automáticamente al PowerManager

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class LightingController : MonoBehaviour
{
    [Header("Configuración")]
    public bool enciendeConEnergia = true;
    public float intensidadObjetivo = 1f;
    public float multiplicadorEnergiaRestaurada = 2.25f;
    public float duracionFade       = 2f;

    Light luz;
    float intensidadOriginal;
    bool encendiendo = false;

    void Start()
    {
        luz = GetComponent<Light>();
        intensidadOriginal = luz.intensity;

        // Empezar apagada si requiere energía
        if (enciendeConEnergia && !PowerManager.EnergiaActiva)
            luz.intensity = 0f;
    }

    void Update()
    {
        // Detectar cuando se activa la energía
        if (enciendeConEnergia && PowerManager.EnergiaActiva && !encendiendo && luz.intensity < ObtenerIntensidadObjetivo())
            StartCoroutine(EncenderLuz());
    }

    IEnumerator EncenderLuz()
    {
        encendiendo = true;
        float t = 0f;
        float inicio = luz.intensity;
        float objetivo = ObtenerIntensidadObjetivo();

        while (t < duracionFade)
        {
            t += Time.deltaTime;
            luz.intensity = Mathf.Lerp(inicio, objetivo, t / duracionFade);
            yield return null;
        }

        luz.intensity = objetivo;
        encendiendo = false;
    }

    float ObtenerIntensidadObjetivo()
    {
        return Mathf.Max(intensidadObjetivo, intensidadOriginal * multiplicadorEnergiaRestaurada);
    }

    public void ApagarInstante()
    {
        luz.intensity = 0f;
    }
}
