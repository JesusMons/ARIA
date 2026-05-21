// MenuDynamicEffects.cs — Assets/Scripts/UI/
// ARIA: Último Protocolo — Efectos dinámicos del menú
// Adjuntar al Canvas

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class MenuDynamicEffects : MonoBehaviour
{
    [Header("Configuración")]
    public int   particleCount   = 22;
    public float scanBarDuration = 2.5f;

    float          sessionStart;
    RectTransform  canvasRect;
    float          cW, cH;

    List<RectTransform>    pRects  = new List<RectTransform>();
    List<TextMeshProUGUI>  pTexts  = new List<TextMeshProUGUI>();
    List<float>            pSpeeds = new List<float>();

    Image         scanFill;
    RectTransform scanTrack;
    TextMeshProUGUI clockTMP;

    void Start()
    {
        sessionStart = Time.realtimeSinceStartup;
        canvasRect   = GetComponent<RectTransform>();
        cW = canvasRect.rect.width;
        cH = canvasRect.rect.height;

        BuscarReloj();
        CrearParticulas();
        CrearScanBar();
        StartCoroutine(AnimarScanBar());
    }

    void Update()
    {
        ActualizarReloj();
        MoverParticulas();
    }

    // ── Reloj ─────────────────────────────────────────────
    void BuscarReloj()
    {
        foreach (var t in FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None))
        {
            if (t.text.Contains("00:00:00") || t.name.Contains("clock") || t.name.Contains("Clock"))
            { clockTMP = t; break; }
        }
    }

    void ActualizarReloj()
    {
        if (clockTMP == null) return;
        float e = Time.realtimeSinceStartup - sessionStart;
        clockTMP.text = $"{(int)(e/3600):00}:{(int)(e%3600/60):00}:{(int)(e%60):00}";
    }

    // ── Partículas ────────────────────────────────────────
    static readonly string[] WORDS = {
        "ARIA","VOSS","2157","01001","11010","PROTOCOL",
        "ACCESS","NODE","DATA","0xFF","SYS","CORE",
        "LENA","VOSS-7","ENCRYPT","DELTA","ARIA PSRS",
        "B10110","ARCH","MEM","01101","ERROR","NULL","010011"
    };

    void CrearParticulas()
    {
        Transform ct = transform.Find("ParticleContainer");
        if (ct != null) Destroy(ct.gameObject);

        GameObject container = new GameObject("ParticleContainer");
        container.transform.SetParent(transform, false);
        container.transform.SetSiblingIndex(2);

        RectTransform rtC = container.AddComponent<RectTransform>();
        rtC.anchorMin = Vector2.zero;
        rtC.anchorMax = Vector2.one;
        rtC.offsetMin = Vector2.zero;
        rtC.offsetMax = Vector2.zero;

        for (int i = 0; i < particleCount; i++)
        {
            GameObject p = new GameObject($"P_{i}");
            p.transform.SetParent(container.transform, false);

            TextMeshProUGUI tmp = p.AddComponent<TextMeshProUGUI>();
            tmp.text     = WORDS[Random.Range(0, WORDS.Length)];
            tmp.fontSize = Random.Range(10f, 14f);
            tmp.color    = new Color32(0, 198, 255, (byte)Random.Range(25, 65));

            RectTransform rt = p.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot     = new Vector2(0, 1);
            rt.sizeDelta = new Vector2(120f, 20f);
            rt.anchoredPosition = new Vector2(
                Random.Range(0f, cW),
                -Random.Range(0f, cH)
            );

            pRects.Add(rt);
            pTexts.Add(tmp);
            pSpeeds.Add(Random.Range(15f, 45f));
        }
    }

    void MoverParticulas()
    {
        for (int i = 0; i < pRects.Count; i++)
        {
            if (pRects[i] == null) continue;
            Vector2 pos = pRects[i].anchoredPosition;
            pos.y -= pSpeeds[i] * Time.deltaTime;

            if (pos.y < -cH - 30f)
            {
                pos.y = Random.Range(-5f, -40f);
                pos.x = Random.Range(0f, cW);
                if (pTexts[i] != null)
                {
                    pTexts[i].text = WORDS[Random.Range(0, WORDS.Length)];
                    Color c = pTexts[i].color;
                    c.a = Random.Range(0.1f, 0.27f);
                    pTexts[i].color = c;
                }
            }
            pRects[i].anchoredPosition = pos;
        }
    }

    // ── Scan bar ──────────────────────────────────────────
    void CrearScanBar()
    {
        GameObject stats = GameObject.Find("Panel_Stats");
        if (stats == null) return;

        Transform old = stats.transform.Find("ScanTrack");
        if (old != null) Destroy(old.gameObject);

        GameObject trackGO = new GameObject("ScanTrack");
        trackGO.transform.SetParent(stats.transform, false);
        trackGO.AddComponent<Image>().color = new Color32(0, 198, 255, 20);

        scanTrack = trackGO.GetComponent<RectTransform>();
        scanTrack.anchorMin        = new Vector2(0, 1f);
        scanTrack.anchorMax        = new Vector2(1, 1f);
        scanTrack.pivot            = new Vector2(0, 1f);
        scanTrack.anchoredPosition = new Vector2(0f, -320f);
        scanTrack.sizeDelta        = new Vector2(0f, 2f);

        GameObject fillGO = new GameObject("ScanFill");
        fillGO.transform.SetParent(trackGO.transform, false);
        scanFill = fillGO.AddComponent<Image>();
        scanFill.color = new Color32(0, 198, 255, 230);

        RectTransform rtF = fillGO.GetComponent<RectTransform>();
        rtF.anchorMin        = new Vector2(0, 0);
        rtF.anchorMax        = new Vector2(0, 1);
        rtF.pivot            = new Vector2(0, 0.5f);
        rtF.anchoredPosition = Vector2.zero;
        rtF.sizeDelta        = new Vector2(0f, 0f);
    }

    IEnumerator AnimarScanBar()
    {
        yield return new WaitForSeconds(0.3f);
        while (true)
        {
            if (scanFill == null || scanTrack == null) { yield return null; continue; }
            float trackW = scanTrack.rect.width;
            if (trackW <= 0) { yield return new WaitForSeconds(0.1f); continue; }

            // Crecer
            float t = 0f;
            while (t < scanBarDuration)
            {
                t += Time.deltaTime;
                float w = Mathf.Lerp(0f, trackW, Mathf.SmoothStep(0, 1, t / scanBarDuration));
                scanFill.rectTransform.sizeDelta        = new Vector2(w, 0f);
                scanFill.rectTransform.anchoredPosition = Vector2.zero;
                yield return null;
            }

            // Contraer
            t = 0f;
            float shrink = scanBarDuration * 0.5f;
            while (t < shrink)
            {
                t += Time.deltaTime;
                float prog   = Mathf.SmoothStep(0, 1, t / shrink);
                float offset = Mathf.Lerp(0f, trackW, prog);
                scanFill.rectTransform.sizeDelta        = new Vector2(Mathf.Max(0, trackW - offset), 0f);
                scanFill.rectTransform.anchoredPosition = new Vector2(offset, 0f);
                yield return null;
            }

            yield return new WaitForSeconds(0.4f);
        }
    }
}