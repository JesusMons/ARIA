// ObjectiveManager.cs — Assets/Scripts/GameSystems/
// ARIA: Último Protocolo — Sistema de objetivos dinámicos
// Responsable: Cristian Ramírez

using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[System.Serializable]
public class Objetivo
{
    public string id;
    public string titulo;
    public string descripcion;
    public bool   completado;
    public bool   oculto;
}

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager instancia;

    [Header("Objetivos iniciales del Nivel 1")]
    public List<Objetivo> objetivos = new List<Objetivo>()
    {
        new Objetivo { id = "despertar",  titulo = "Evaluar el entorno",
                       descripcion = "Algo no va bien. Explora el laboratorio." },
        new Objetivo { id = "energia",    titulo = "Restaurar energía de la estación",
                       descripcion = "Sin energía no hay acceso. Busca el servidor.",
                       oculto = true },
        new Objetivo { id = "nota_voss",  titulo = "Revisar los archivos del Dr. Voss",
                       descripcion = "Hay notas dispersas. Encuentra la primera.",
                       oculto = true },
        new Objetivo { id = "codigo",     titulo = "Encontrar el código de acceso",
                       descripcion = "Una puerta bloqueada requiere un código.",
                       oculto = true },
        new Objetivo { id = "salida",     titulo = "Acceder a la zona de mantenimiento",
                       descripcion = "Hay algo más allá de las puertas selladas.",
                       oculto = true },
    };

    [Header("Eventos")]
    public UnityEvent<Objetivo> onObjetivoActivado;
    public UnityEvent<Objetivo> onObjetivoCompletado;
    public UnityEvent<Objetivo> onObjetivoActualizado;

    // ──────────────────────────────────────────────────────
    void Awake()
    {
        if (instancia != null) { Destroy(gameObject); return; }
        instancia = this;
    }

    void Start()
    {
        PowerManager pm = FindFirstObjectByType<PowerManager>();
        if (pm != null) pm.onEnergiaActivada.AddListener(OnEnergiaActivada);

        // Solo activar objetivo inicial si no hay estado importado
        bool hayVisibles = objetivos.Exists(o => !o.oculto);
        if (!hayVisibles) ActivarObjetivo("despertar");
    }

    // ── API pública ────────────────────────────────────────
    public static void Activar(string id)   => instancia?.ActivarObjetivo(id);
    public static void Completar(string id) => instancia?.CompletarObjetivo(id);

    public static Objetivo ObtenerActual()
    {
        if (instancia == null) return null;
        return instancia.objetivos.Find(o => !o.completado && !o.oculto);
    }

    public static List<Objetivo> ObtenerVisibles()
    {
        if (instancia == null) return new List<Objetivo>();
        return instancia.objetivos.FindAll(o => !o.oculto);
    }

    // ── Métodos para SaveGameManager ───────────────────────

    /// <summary>Exporta el estado de objetivos para guardar</summary>
    public static void ExportarEstado(List<string> completados, List<string> visibles)
    {
        if (instancia == null) return;

        completados.Clear();
        visibles.Clear();

        foreach (Objetivo obj in instancia.objetivos)
        {
            if (obj.completado) completados.Add(obj.id);
            if (!obj.oculto)    visibles.Add(obj.id);
        }
    }

    /// <summary>Importa el estado de objetivos desde un guardado</summary>
    public static void ImportarEstado(List<string> completados, List<string> visibles)
    {
        if (instancia == null) return;
        if (completados == null || visibles == null) return;

        foreach (Objetivo obj in instancia.objetivos)
        {
            obj.completado = completados.Contains(obj.id);
            obj.oculto     = !visibles.Contains(obj.id);
        }

        // Notificar UI
        instancia.onObjetivoActualizado?.Invoke(ObtenerActual());
    }

    // ── Internos ───────────────────────────────────────────
    void ActivarObjetivo(string id)
    {
        Objetivo obj = objetivos.Find(o => o.id == id);
        if (obj == null || !obj.oculto) return;

        obj.oculto = false;
        onObjetivoActivado?.Invoke(obj);
        onObjetivoActualizado?.Invoke(obj);
    }

    void CompletarObjetivo(string id)
    {
        Objetivo obj = objetivos.Find(o => o.id == id);
        if (obj == null || obj.completado) return;

        obj.completado = true;
        onObjetivoCompletado?.Invoke(obj);
        onObjetivoActualizado?.Invoke(obj);
        ActivarSiguiente(id);
    }

    void ActivarSiguiente(string idCompletado)
    {
        switch (idCompletado)
        {
            case "despertar":  ActivarObjetivo("energia");   break;
            case "energia":    ActivarObjetivo("nota_voss"); break;
            case "nota_voss":  ActivarObjetivo("codigo");    break;
            case "codigo":     ActivarObjetivo("salida");    break;
        }
    }

    void OnEnergiaActivada() => CompletarObjetivo("energia");
}