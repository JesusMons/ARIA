// ============================================================
//  SplashScreenController.cs  →  Assets/Scripts/UI/
//  ARIA: Último Protocolo — Pantalla de intro/splash
//
//  SETUP:
//  1. Crear escena nueva: "SplashScreen"
//  2. Agregar a Build Settings como escena 0
//  3. La escena MainMenu debe ser escena 1
//  4. Crear Canvas con este script en la escena SplashScreen
// ============================================================

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;


public class SplashScreenController : MonoBehaviour
{
    [Header("Referencias UI")]
    public TextMeshProUGUI txtLogo;          // Texto "ARIA"
    public TextMeshProUGUI txtSubtitulo;     // "ÚLTIMO PROTOCOLO"
    public TextMeshProUGUI txtFrase;         // La frase larga
    public TextMeshProUGUI txtContinuar;     // "Presiona cualquier tecla"
    public Image           imgFondo;         // Fondo negro
    public Image           fadeOverlay;      // Para fade in/out

    [Header("Configuración")]
    public string escenaSiguiente   = "MainMenu";
    public float  tiempoAutoAvanzar = 12f;   // segundos antes de avanzar solo
    public AudioClip sonidoIntro;            // opcional

    [Header("Frases de intro (se muestran una a la vez)")]
    [TextArea(2, 4)]
    public string[] frases = {
        "En el año 2157, alguien está reescribiendo\nlas leyes del universo.",
        "Tú eres la única ecuación\nque no pueden borrar.",
        "ARIA — ÚLTIMO PROTOCOLO"
    };

    // ── Internos ───────────────────────────────────────────
    bool   puedeAvanzar = false;
    bool   avanzando    = false;

    // ──────────────────────────────────────────────────────
   void Start()
{
    InicializarUI();
    StartCoroutine(SecuenciaIntro());

    if (sonidoIntro != null)
    {
        AudioSource src = gameObject.AddComponent<AudioSource>();
        src.clip        = sonidoIntro;
        src.loop        = true;
        src.volume      = 0.35f;
        src.playOnAwake = false;
        src.Play();
    }
}

   void Update()
{
    if (puedeAvanzar && !avanzando)
    {
        if ((Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) ||
            (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame))
        {
            StartCoroutine(IrAlMenu());
        }
    }
}

    // ── Inicializar todo invisible ─────────────────────────
    void InicializarUI()
    {
        SetAlpha(txtLogo,      0f);
        SetAlpha(txtSubtitulo, 0f);
        SetAlpha(txtFrase,     0f);
        SetAlpha(txtContinuar, 0f);

        if (fadeOverlay != null)
            SetAlpha(fadeOverlay, 1f); // Empieza negro

        if (imgFondo != null)
            imgFondo.color = new Color32(10, 14, 26, 255);
    }

    // ── Secuencia principal ───────────────────────────────
    IEnumerator SecuenciaIntro()
    {
        // Fade in desde negro
        yield return StartCoroutine(FadeImagen(fadeOverlay, 1f, 0f, 1.5f));

        // Mostrar logo ARIA con glitch
        yield return StartCoroutine(MostrarLogo());

        // Mostrar subtítulo
        yield return StartCoroutine(FadeTexto(txtSubtitulo, 0f, 1f, 1f));
        yield return new WaitForSeconds(0.8f);

        // Ocultar logo y subtítulo
        yield return StartCoroutine(FadeTexto(txtLogo,      1f, 0f, 0.8f));
        yield return StartCoroutine(FadeTexto(txtSubtitulo, 1f, 0f, 0.6f));
        yield return new WaitForSeconds(0.5f);

        // Mostrar frases una a una con efecto typewriter
        for (int i = 0; i < frases.Length - 1; i++)
        {
            yield return StartCoroutine(MostrarFrase(frases[i]));
            yield return new WaitForSeconds(1.8f);
            yield return StartCoroutine(FadeTexto(txtFrase, 1f, 0f, 0.5f));
            yield return new WaitForSeconds(0.4f);
        }

        // Última frase → vuelve a aparecer el logo
        yield return StartCoroutine(FadeTexto(txtLogo, 0f, 1f, 1.2f));
        yield return new WaitForSeconds(0.4f);
        yield return StartCoroutine(FadeTexto(txtSubtitulo, 0f, 1f, 0.8f));

        // Mostrar "Presiona cualquier tecla"
        yield return new WaitForSeconds(0.6f);
        StartCoroutine(ParpadearTexto(txtContinuar));

        puedeAvanzar = true;

        // Auto-avanzar después de X segundos
        yield return new WaitForSeconds(tiempoAutoAvanzar);
        if (!avanzando)
            StartCoroutine(IrAlMenu());
    }

    // ── Mostrar logo ARIA con efecto glitch ───────────────
    IEnumerator MostrarLogo()
    {
        if (txtLogo == null) yield break;

        // Fade in
        yield return StartCoroutine(FadeTexto(txtLogo, 0f, 1f, 0.5f));

        // Glitch rápido 3 veces
        for (int i = 0; i < 3; i++)
        {
            yield return StartCoroutine(GlitchRapido(txtLogo));
            yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
        }

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator GlitchRapido(TextMeshProUGUI tmp)
    {
        Vector2 posOriginal = tmp.rectTransform.anchoredPosition;
        Color   colorOriginal = tmp.color;

        Color[] glitchColors = {
            new Color32(0,   198, 255, 220),  // azul protocolo
            new Color32(255, 71,  87,  200),  // rojo fractura
            colorOriginal
        };

        float duracion = 0.12f;
        float t = 0f;

        while (t < duracion)
        {
            t += Time.deltaTime;
            float offsetX = Random.Range(-6f, 6f);
            tmp.rectTransform.anchoredPosition = posOriginal + new Vector2(offsetX, 0);
            tmp.color = glitchColors[Random.Range(0, glitchColors.Length)];
            yield return null;
        }

        // Restaurar
        tmp.rectTransform.anchoredPosition = posOriginal;
        tmp.color = colorOriginal;
    }

    // ── Efecto typewriter ─────────────────────────────────
    IEnumerator MostrarFrase(string frase)
    {
        if (txtFrase == null) yield break;

        txtFrase.text = "";
        yield return StartCoroutine(FadeTexto(txtFrase, 0f, 1f, 0.3f));

        float velocidad = 0.04f; // segundos por carácter

        for (int i = 0; i <= frase.Length; i++)
        {
            txtFrase.text = frase.Substring(0, i);

            // Pequeña variación en la velocidad para efecto más natural
            float espera = velocidad + Random.Range(-0.01f, 0.02f);
            if (frase[Mathf.Max(0, i - 1)] == '\n') espera = 0.15f;

            yield return new WaitForSeconds(espera);
        }
    }

    // ── Parpadeo del texto "continuar" ─────────────────────
    IEnumerator ParpadearTexto(TextMeshProUGUI tmp)
    {
        if (tmp == null) yield break;

        while (puedeAvanzar && !avanzando)
        {
            yield return StartCoroutine(FadeTexto(tmp, 0f, 0.7f, 0.6f));
            yield return new WaitForSeconds(0.3f);
            yield return StartCoroutine(FadeTexto(tmp, 0.7f, 0f, 0.6f));
            yield return new WaitForSeconds(0.2f);
        }
    }

    // ── Ir al menú principal ──────────────────────────────
    IEnumerator IrAlMenu()
    {
        avanzando = true;
        puedeAvanzar = false;

        // Fade a negro
        yield return StartCoroutine(FadeImagen(fadeOverlay, 0f, 1f, 1f));

        SceneManager.LoadScene(escenaSiguiente);
    }

    // ── Utilidades de fade ─────────────────────────────────
    IEnumerator FadeTexto(TextMeshProUGUI tmp, float desde, float hasta, float duracion)
    {
        if (tmp == null) yield break;
        float t = 0f;
        Color c = tmp.color;
        while (t < duracion)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(desde, hasta, t / duracion);
            tmp.color = c;
            yield return null;
        }
        c.a = hasta;
        tmp.color = c;
    }

    IEnumerator FadeImagen(Image img, float desde, float hasta, float duracion)
    {
        if (img == null) yield break;
        float t = 0f;
        Color c = img.color;
        while (t < duracion)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(desde, hasta, t / duracion);
            img.color = c;
            yield return null;
        }
        c.a = hasta;
        img.color = c;
    }

    void SetAlpha(Graphic graphic, float alpha)
    {
        if (graphic == null) return;
        Color c = graphic.color;
        c.a = alpha;
        graphic.color = c;
    }

    
}
