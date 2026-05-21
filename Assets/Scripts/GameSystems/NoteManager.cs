// NoteManager.cs — Assets/Scripts/GameSystems/
// ARIA: Último Protocolo — Gestor de notas y códigos
// Responsable: Cristian Ramírez

using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class NoteManager : MonoBehaviour
{
    public static NoteManager instancia;

    [Header("Todas las notas del nivel")]
    public List<VossNoteData> todasLasNotas;

    [Header("Eventos")]
    public UnityEvent<VossNoteData> onNotaEncontrada;

    // ── Estado de sesión ───────────────────────────────────
    static List<string>              notasRecolectadas = new List<string>();
    static Dictionary<string,string> codigosPuertas    = new Dictionary<string,string>();

    // Flag para saber si los códigos fueron importados desde un save
    static bool codigosImportados = false;

    // ──────────────────────────────────────────────────────
    void Awake()
    {
        if (instancia != null) { Destroy(gameObject); return; }
        instancia = this;

        // Siempre regenerar si NO vienen de un save importado
        // Si vienen de save, ImportarCodigosPuertas ya los tiene
        if (!codigosImportados)
            GenerarCodigosAleatorios();

        // Resetear flag para la próxima sesión
        codigosImportados = false;
    }

    // ── Generación de códigos ──────────────────────────────
    void GenerarCodigosAleatorios()
    {
        codigosPuertas.Clear();
        if (todasLasNotas == null) return;

        foreach (VossNoteData nota in todasLasNotas)
        {
            if (nota == null || string.IsNullOrEmpty(nota.idPuertaAsociada)) continue;
            if (codigosPuertas.ContainsKey(nota.idPuertaAsociada)) continue;

            string codigo;
            int intentos = 0;
            do { codigo = Random.Range(1000, 9999).ToString(); intentos++; }
            while (codigosPuertas.ContainsValue(codigo) && intentos < 100);

            nota.codigoAcceso = codigo;
            codigosPuertas[nota.idPuertaAsociada] = codigo;

            Debug.Log($"[NoteManager] Puerta '{nota.idPuertaAsociada}' → código {codigo}");
        }
    }

    // ── API pública ────────────────────────────────────────
    public static void RegistrarNota(VossNoteData nota)
    {
        if (nota == null || string.IsNullOrEmpty(nota.id)) return;
        if (notasRecolectadas.Contains(nota.id)) return;

        notasRecolectadas.Add(nota.id);
        instancia?.onNotaEncontrada?.Invoke(nota);

        if (!string.IsNullOrEmpty(nota.idObjetivoCompletar))
            ObjectiveManager.Completar(nota.idObjetivoCompletar);
        if (!string.IsNullOrEmpty(nota.idObjetivoActivar))
            ObjectiveManager.Activar(nota.idObjetivoActivar);
    }

    public static bool VerificarCodigo(string idPuerta, string codigoIngresado)
    {
        if (!codigosPuertas.ContainsKey(idPuerta)) return false;
        return codigosPuertas[idPuerta] == codigoIngresado.Trim();
    }

    public static string ObtenerCodigo(string idPuerta)
    {
        return codigosPuertas.ContainsKey(idPuerta) ? codigosPuertas[idPuerta] : "????";
    }

    public static bool FueRecolectada(string idNota)
        => !string.IsNullOrEmpty(idNota) && notasRecolectadas.Contains(idNota);

    public static bool CodigoFueDescubierto(string idPuerta)
    {
        if (instancia == null || string.IsNullOrEmpty(idPuerta)) return false;
        if (instancia.todasLasNotas == null) return false;

        VossNoteData nota = instancia.todasLasNotas.Find(
            n => n != null &&
                 n.idPuertaAsociada == idPuerta &&
                 FueRecolectada(n.id));

        return nota != null;
    }

    public static List<VossNoteData> ObtenerNotasRecolectadas()
    {
        if (instancia == null) return new List<VossNoteData>();
        return instancia.todasLasNotas?.FindAll(
            n => n != null && FueRecolectada(n.id))
               ?? new List<VossNoteData>();
    }

    // ── Métodos para SaveGameManager ───────────────────────

    /// <summary>Limpia el estado para una nueva partida</summary>
    public static void LimpiarEstadoSesion()
    {
        notasRecolectadas.Clear();
        codigosPuertas.Clear();
        codigosImportados = false;

        if (instancia?.todasLasNotas != null)
            foreach (VossNoteData nota in instancia.todasLasNotas)
                if (nota != null) nota.codigoAcceso = "";
    }

    public static List<string> ObtenerIdsNotasRecolectadas()
        => new List<string>(notasRecolectadas);

    public static List<DoorCodeEntry> ExportarCodigosPuertas()
    {
        List<DoorCodeEntry> lista = new List<DoorCodeEntry>();
        foreach (var par in codigosPuertas)
            lista.Add(new DoorCodeEntry { idPuerta = par.Key, codigo = par.Value });
        return lista;
    }

    public static void RestaurarNotasRecolectadas(List<string> ids)
    {
        notasRecolectadas.Clear();
        if (ids == null) return;
        foreach (string id in ids)
            if (!string.IsNullOrEmpty(id))
                notasRecolectadas.Add(id);
    }

    /// <summary>
    /// Importa códigos desde un save — marca el flag para que
    /// Awake() no sobrescriba los códigos con nuevos aleatorios
    /// </summary>
    public static void ImportarCodigosPuertas(List<DoorCodeEntry> entradas)
    {
        codigosPuertas.Clear();
        codigosImportados = true;  // ← Awake() no regenerará

        if (entradas == null) return;

        foreach (DoorCodeEntry entrada in entradas)
        {
            if (entrada == null || string.IsNullOrEmpty(entrada.idPuerta)) continue;
            codigosPuertas[entrada.idPuerta] = entrada.codigo;
        }

        // Sincronizar con ScriptableObjects
        if (instancia?.todasLasNotas == null) return;
        foreach (VossNoteData nota in instancia.todasLasNotas)
            if (nota != null && !string.IsNullOrEmpty(nota.idPuertaAsociada))
                if (codigosPuertas.ContainsKey(nota.idPuertaAsociada))
                    nota.codigoAcceso = codigosPuertas[nota.idPuertaAsociada];
    }
}