// ============================================================
//  ClockDisplay.cs
//  ARIA: Último Protocolo — Reloj en tiempo real
//
//  Adjuntar al TMP del reloj en Panel_Stats
//  O dejar que FixARIAMenuSizes lo agregue automáticamente
// ============================================================

using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ClockDisplay : MonoBehaviour
{
    TextMeshProUGUI tmp;
    float sessionStart;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        sessionStart = Time.realtimeSinceStartup;
    }

    void Update()
    {
        float elapsed = Time.realtimeSinceStartup - sessionStart;
        int h = (int)(elapsed / 3600);
        int m = (int)((elapsed % 3600) / 60);
        int s = (int)(elapsed % 60);
        tmp.text = $"{h:00}:{m:00}:{s:00}";
    }
}
