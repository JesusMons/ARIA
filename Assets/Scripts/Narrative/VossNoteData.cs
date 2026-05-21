// VossNoteData.cs — Assets/Scripts/Narrative/
// ARIA: Último Protocolo — Datos de una nota del Dr. Voss
// ScriptableObject: clic derecho en Project → Create → ARIA → Nota Voss

using UnityEngine;

public enum TipoNota { Fisica, Digital }

[CreateAssetMenu(fileName = "Nota_Voss_", menuName = "ARIA/Nota del Dr. Voss")]
public class VossNoteData : ScriptableObject
{
    [Header("Identificación")]
    public string id;
    public string titulo;
    public string fecha       = "2157.03.14";
    public string ubicacion;  // ej: "Laboratorio Principal - Sector A"

    [Header("Contenido")]
    [TextArea(4, 12)]
    public string contenido;

    [Tooltip("Partes del texto que aparecen ilegibles (usar [ILEGIBLE] en el contenido)")]
    public bool   deteriorada   = false;

    [Header("Tipo")]
    public TipoNota tipo = TipoNota.Fisica;

    [Header("Código de acceso")]
    [Tooltip("ID de la puerta que desbloquea esta nota")]
    public string idPuertaAsociada = "";

    [Tooltip("El código se genera aleatoriamente al iniciar la partida")]
    [HideInInspector]
    public string codigoAcceso = "";

    [Header("Progresión")]
    [Tooltip("ID del objetivo que se completa al leer esta nota")]
    public string idObjetivoCompletar = "";
    [Tooltip("ID del objetivo que se activa al leer esta nota")]
    public string idObjetivoActivar   = "";
}
