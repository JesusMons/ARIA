// ============================================================
//  GlitchEffect.cs
//  ARIA: Último Protocolo — Efecto Glitch en el Logo
//
//  CÓMO USARLO:
//  1. Adjuntar este script al GameObject del TMP "ARIA"
//  2. El texto debe ser TextMeshPro (no TextMeshProUGUI)
//     o TextMeshProUGUI si está en Canvas
//
//  Responsable: Kevin Estrada (VFX)
// ============================================================

using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(TextMeshProUGUI))]
public class GlitchEffect : MonoBehaviour
{
    [Header("Configuración del Glitch")]
    [Tooltip("Cuántos segundos espera entre glitches")]
    public float intervaloEntreGlitches = 4f;

    [Tooltip("Duración de cada glitch en segundos")]
    public float duracionGlitch = 0.15f;

    [Tooltip("Desplazamiento máximo del texto en X (píxeles)")]
    public float desplazamientoMaxX = 6f;

    [Tooltip("Color del glitch superior (azul protocolo)")]
    public Color colorGlitchA = new Color(0f, 0.78f, 1f, 0.85f);      // #00C6FF

    [Tooltip("Color del glitch inferior (rojo fractura)")]
    public Color colorGlitchB = new Color(1f, 0.28f, 0.34f, 0.85f);   // #FF4757

    // ── Internos ───────────────────────────────────────────
    private TextMeshProUGUI tmp;
    private RectTransform rect;
    private Vector2 posicionOriginal;
    private Color colorOriginal;

    // ──────────────────────────────────────────────────────
    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        rect = GetComponent<RectTransform>();
        posicionOriginal = rect.anchoredPosition;
        colorOriginal = tmp.color;

        StartCoroutine(BucleGlitch());
    }

    IEnumerator BucleGlitch()
    {
        while (true)
        {
            // Esperar intervalo aleatorio (±20% de variación)
            float espera = intervaloEntreGlitches + Random.Range(-intervaloEntreGlitches * 0.2f,
                                                                   intervaloEntreGlitches * 0.2f);
            yield return new WaitForSeconds(espera);

            // Ejecutar 2-3 glitches rápidos seguidos
            int repeticiones = Random.Range(2, 4);
            for (int i = 0; i < repeticiones; i++)
            {
                yield return StartCoroutine(EjecutarGlitch());
                yield return new WaitForSeconds(Random.Range(0.02f, 0.06f));
            }

            // Resetear al estado original
            Restaurar();
        }
    }

    IEnumerator EjecutarGlitch()
    {
        float duracion = duracionGlitch * Random.Range(0.5f, 1.5f);
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;

            // Desplazar posición
            float offsetX = Random.Range(-desplazamientoMaxX, desplazamientoMaxX);
            rect.anchoredPosition = posicionOriginal + new Vector2(offsetX, 0f);

            // Cambiar color aleatoriamente entre A y B
            tmp.color = Random.value > 0.5f ? colorGlitchA : colorGlitchB;

            yield return null;
        }
    }

    void Restaurar()
{
    if (rect != null) rect.anchoredPosition = posicionOriginal;
    if (tmp != null) tmp.color = colorOriginal;
}

    // Restaurar al desactivar el objeto
    void OnDisable()
    {
        Restaurar();
    }
}
