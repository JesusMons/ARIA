// HUDManager.cs — Assets/Scripts/UI/
// ARIA: Último Protocolo — HUD de alertas estilizado

using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager instancia;

    [Header("UI")]
    public TextMeshProUGUI txtMensaje;
    public TextMeshProUGUI txtIcono;

    // Iconos ASCII (compatibles con cualquier fuente)
    const string ICONO_INFO    = ">";
    const string ICONO_ERROR   = "X";
    const string ICONO_WARNING = "!";
    const string ICONO_OK      = "+";

    static readonly Color COL_BLUE  = new Color32(0,   198, 255, 230);
    static readonly Color COL_RED   = new Color32(255,  71,  87, 230);
    static readonly Color COL_GOLD  = new Color32(255, 215,   0, 230);
    static readonly Color COL_WHITE = new Color32(240, 244, 248, 230);

    void Awake()
    {
        instancia = this;
        Limpiar();
    }

    public static void MostrarMensaje(string texto)
    {
        if (instancia == null) return;
        instancia.Mostrar(texto, ICONO_INFO, COL_BLUE, COL_WHITE);
    }

    public static void MostrarError(string texto)
    {
        if (instancia == null) return;
        instancia.Mostrar(texto, ICONO_ERROR, COL_RED, COL_RED);
    }

    public static void MostrarWarning(string texto)
    {
        if (instancia == null) return;
        instancia.Mostrar(texto, ICONO_WARNING, COL_GOLD, COL_GOLD);
    }

    public static void MostrarOK(string texto)
    {
        if (instancia == null) return;
        instancia.Mostrar(texto, ICONO_OK, COL_BLUE, COL_BLUE);
    }

    public static void OcultarMensaje()
    {
        if (instancia == null) return;
        instancia.Limpiar();
    }

    public static void MostrarMensajeTemporal(string texto, float segundos = 2f)
    {
        if (instancia == null) return;
        instancia.Mostrar(texto, ICONO_OK, COL_BLUE, COL_BLUE);
        instancia.CancelInvoke(nameof(instancia.Limpiar));
        instancia.Invoke(nameof(instancia.Limpiar), segundos);
    }

    void Mostrar(string texto, string icono, Color colorIcono, Color colorTexto)
    {
        if (txtMensaje != null)
        {
            txtMensaje.text  = texto;
            txtMensaje.color = colorTexto;
        }
        if (txtIcono != null)
        {
            txtIcono.text  = icono;
            txtIcono.color = colorIcono;
        }
    }

    void Limpiar()
    {
        if (txtMensaje != null) txtMensaje.text = "";
        if (txtIcono   != null) txtIcono.text   = "";
    }
}
