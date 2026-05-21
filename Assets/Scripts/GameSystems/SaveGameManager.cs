// SaveGameManager.cs — Assets/Scripts/GameSystems/
// ARIA: Último Protocolo — Perfiles de partida y guardado JSON

using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;

[Serializable]
public class DoorCodeEntry
{
    public string idPuerta;
    public string codigo;
}

[Serializable]
public class SaveGameData
{
    public string slotId;
    public string nombrePartida;
    public string escenaActual;
    public string fechaCreacion;
    public string fechaActualizacion;

    public Vector3 posicionJugador;
    public Vector3 rotacionJugador;

    public bool         energiaActiva;
    public List<string> notasRecolectadas    = new List<string>();
    public List<string> objetivosCompletados = new List<string>();
    public List<string> objetivosVisibles    = new List<string>();
    public List<DoorCodeEntry> codigosPuertas = new List<DoorCodeEntry>();
    public List<string> puertasDesbloqueadas = new List<string>();
}

public class SaveGameManager : MonoBehaviour
{
    public static SaveGameManager instancia;

    const string CurrentSlotKey = "ARIA_CurrentSaveSlot";
    const string SaveExtension  = ".json";

    static readonly string[] EscenasMenu = { "samplesscene", "mainmenu", "splashscreen" };

    [Header("Auto guardado")]
    public bool  autoGuardar           = true;
    public float intervaloAutoGuardado = 30f;

    public static SaveGameData PartidaActual { get; private set; }
    public static string       SlotActualId  => PlayerPrefs.GetString(CurrentSlotKey, "");

    // ── Flag: nueva partida vs partida cargada ─────────────
    static bool esNuevaPartida = false;
    public static bool EsNuevaPartida => esNuevaPartida;

    float proximoAutoGuardado = 0f;

    // ──────────────────────────────────────────────────────
    void Awake()
    {
        if (instancia != null) { Destroy(gameObject); return; }
        instancia = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        if (instancia == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (PartidaActual == null) return;

        string nombreNorm = scene.name.ToLowerInvariant().Trim();
        foreach (string escMenu in EscenasMenu)
            if (nombreNorm == escMenu) return;

        // No aplicar estado en partidas nuevas
        if (esNuevaPartida)
        {
            esNuevaPartida = false;
            return;
        }

        if (SceneLoader.NormalizarNombreEscena(scene.name) !=
            SceneLoader.NormalizarNombreEscena(PartidaActual.escenaActual)) return;

        StartCoroutine(AplicarEstadoAlFinalDelFrame());
    }

    void Update()
    {
        if (!autoGuardar || PartidaActual == null) return;
        if (!EscenaActualCorrespondeAPartida()) return;
        if (Time.unscaledTime < proximoAutoGuardado) return;

        proximoAutoGuardado = Time.unscaledTime + intervaloAutoGuardado;
        GuardarPartidaActual();
    }

    IEnumerator AplicarEstadoAlFinalDelFrame()
    {
        yield return null;
        yield return new WaitForSecondsRealtime(0.3f);
        AplicarEstadoCargado();
    }

    // ── Consulta ───────────────────────────────────────────
    public static bool HayPartidas()
        => ObtenerPartidas().Count > 0;

    public static List<SaveGameData> ObtenerPartidas()
    {
        AsegurarCarpeta();
        List<SaveGameData> partidas = new List<SaveGameData>();
        string[] archivos = Directory.GetFiles(CarpetaSaves(), "*" + SaveExtension);

        foreach (string archivo in archivos)
        {
            try
            {
                SaveGameData data = JsonUtility.FromJson<SaveGameData>(
                    File.ReadAllText(archivo));
                if (data != null && !string.IsNullOrEmpty(data.slotId))
                {
                    data.escenaActual = SceneLoader.NormalizarNombreEscena(data.escenaActual);
                    partidas.Add(data);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("[SaveGameManager] Save invalido: " + archivo + " — " + e.Message);
            }
        }

        partidas.Sort((a, b) =>
            string.CompareOrdinal(b.fechaActualizacion, a.fechaActualizacion));
        return partidas;
    }

    // ── Crear / seleccionar ────────────────────────────────
    public static SaveGameData CrearNuevaPartida(string nombrePartida, string escenaInicial)
    {
        AsegurarInstancia();

        string nombreLimpio = string.IsNullOrWhiteSpace(nombrePartida)
            ? "Partida ARIA" : nombrePartida.Trim();

        string ahora = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        NoteManager.LimpiarEstadoSesion();

        SaveGameData data = new SaveGameData
        {
            slotId             = CrearSlotId(nombreLimpio),
            nombrePartida      = nombreLimpio,
            escenaActual       = SceneLoader.NormalizarNombreEscena(escenaInicial),
            fechaCreacion      = ahora,
            fechaActualizacion = ahora,
            energiaActiva      = false
        };
        data.objetivosVisibles.Add("despertar");

        PartidaActual  = data;
        esNuevaPartida = true;

        PlayerPrefs.SetString(CurrentSlotKey, data.slotId);
        PlayerPrefs.SetInt("PartidaGuardada", 1);
        PlayerPrefs.SetString("UltimaNivel", data.escenaActual);
        PlayerPrefs.Save();

        Guardar(data);
        return data;
    }

    public static bool SeleccionarPartida(string slotId)
    {
        AsegurarInstancia();
        SaveGameData data = Cargar(slotId);
        if (data == null) return false;

        PartidaActual     = data;
        esNuevaPartida    = false;
        data.escenaActual = SceneLoader.NormalizarNombreEscena(data.escenaActual);

        PlayerPrefs.SetString(CurrentSlotKey, data.slotId);
        PlayerPrefs.SetInt("PartidaGuardada", 1);
        PlayerPrefs.SetString("UltimaNivel", data.escenaActual);
        PlayerPrefs.Save();
        return true;
    }

    // ── Cargar / guardar ───────────────────────────────────
    public static SaveGameData Cargar(string slotId)
    {
        if (string.IsNullOrEmpty(slotId)) return null;
        string ruta = RutaSave(slotId);
        if (!File.Exists(ruta)) return null;

        try { return JsonUtility.FromJson<SaveGameData>(File.ReadAllText(ruta)); }
        catch (Exception e)
        {
            Debug.LogWarning("[SaveGameManager] No se pudo cargar: " + e.Message);
            return null;
        }
    }

    public static void GuardarPartidaActual()
    {
        if (PartidaActual == null)
        {
            string slot = SlotActualId;
            if (!string.IsNullOrEmpty(slot))
                PartidaActual = Cargar(slot);
        }
        if (PartidaActual == null) return;

        if (EscenaActualCorrespondeAPartida())
            CapturarEstadoEnPartida(PartidaActual);

        Guardar(PartidaActual);
    }

    public static void Guardar(SaveGameData data)
    {
        if (data == null) return;
        AsegurarCarpeta();
        data.escenaActual      = SceneLoader.NormalizarNombreEscena(data.escenaActual);
        data.fechaActualizacion= DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        File.WriteAllText(RutaSave(data.slotId), JsonUtility.ToJson(data, true));
    }

    public static void EliminarPartida(string slotId)
    {
        if (string.IsNullOrEmpty(slotId)) return;
        string ruta = RutaSave(slotId);
        if (File.Exists(ruta)) File.Delete(ruta);

        if (SlotActualId == slotId)
        {
            PartidaActual = null;
            PlayerPrefs.DeleteKey(CurrentSlotKey);
            PlayerPrefs.Save();
        }
    }

    public static void ActualizarEscenaPartidaActual(string escena)
    {
        if (PartidaActual == null) return;
        PartidaActual.escenaActual = SceneLoader.NormalizarNombreEscena(escena);
        Guardar(PartidaActual);
    }

    // ── Capturar / aplicar estado ──────────────────────────
    public static void CapturarEstadoEnPartida(SaveGameData data)
    {
        if (data == null) return;

        data.escenaActual  = SceneLoader.NormalizarNombreEscena(
            SceneManager.GetActiveScene().name);
        data.energiaActiva = PowerManager.EnergiaActiva;

        Transform jugador = EncontrarJugador();
        if (jugador != null)
        {
            data.posicionJugador = jugador.position;
            data.rotacionJugador = jugador.eulerAngles;
        }

        data.notasRecolectadas = NoteManager.ObtenerIdsNotasRecolectadas();
        data.codigosPuertas    = NoteManager.ExportarCodigosPuertas();
        ObjectiveManager.ExportarEstado(
            data.objetivosCompletados, data.objetivosVisibles);
    }

    public static void AplicarEstadoCargado()
    {
        if (PartidaActual == null)
        {
            string slot = SlotActualId;
            if (!string.IsNullOrEmpty(slot))
                PartidaActual = Cargar(slot);
        }
        if (PartidaActual == null) return;

        NoteManager.RestaurarNotasRecolectadas(PartidaActual.notasRecolectadas);
        NoteManager.ImportarCodigosPuertas(PartidaActual.codigosPuertas);
        ObjectiveManager.ImportarEstado(
            PartidaActual.objetivosCompletados, PartidaActual.objetivosVisibles);
        SincronizarNotasFisicas();

        Transform jugador = EncontrarJugador();
        if (jugador != null && PartidaActual.posicionJugador != Vector3.zero)
        {
            CharacterController cc = jugador.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            jugador.position    = PartidaActual.posicionJugador;
            jugador.eulerAngles = PartidaActual.rotacionJugador;
            if (cc != null) cc.enabled = true;
        }

        if (PartidaActual.energiaActiva)
        {
            PowerManager pm = FindFirstObjectByType<PowerManager>();
            if (pm != null) pm.ActivarEnergia();
        }
    }

    // ── Puertas ────────────────────────────────────────────
    public static bool PuertaEstaDesbloqueada(string idPuerta)
        => PartidaActual != null &&
           PartidaActual.puertasDesbloqueadas != null &&
           PartidaActual.puertasDesbloqueadas.Contains(idPuerta);

    public static void MarcarPuertaDesbloqueada(string idPuerta)
    {
        if (PartidaActual == null || string.IsNullOrEmpty(idPuerta)) return;
        if (PartidaActual.puertasDesbloqueadas == null)
            PartidaActual.puertasDesbloqueadas = new List<string>();
        if (!PartidaActual.puertasDesbloqueadas.Contains(idPuerta))
            PartidaActual.puertasDesbloqueadas.Add(idPuerta);
        GuardarPartidaActual();
    }

    // ── Helpers ────────────────────────────────────────────
    void OnApplicationQuit() => GuardarPartidaActual();

    static void SincronizarNotasFisicas()
    {
        PhysicalNote[] notas = FindObjectsByType<PhysicalNote>(FindObjectsSortMode.None);
        foreach (PhysicalNote nota in notas)
            if (nota != null) nota.SincronizarEstadoGuardado();
    }

    static bool EscenaActualCorrespondeAPartida()
    {
        if (PartidaActual == null) return false;
        string activa  = SceneLoader.NormalizarNombreEscena(SceneManager.GetActiveScene().name);
        string partida = SceneLoader.NormalizarNombreEscena(PartidaActual.escenaActual);
        return activa == partida;
    }

    static Transform EncontrarJugador()
    {
        GameObject go = GameObject.FindWithTag("Player")
                     ?? GameObject.FindWithTag("GameController");
        if (go != null) return go.transform;

        CharacterController cc = FindFirstObjectByType<CharacterController>();
        if (cc != null) return cc.transform;

        go = GameObject.Find("MovementController") ?? GameObject.Find("Player");
        return go != null ? go.transform : null;
    }

    static void AsegurarInstancia()
    {
        if (instancia != null) return;
        instancia = new GameObject("SaveGameManager").AddComponent<SaveGameManager>();
    }

    static void AsegurarCarpeta()
    {
        string carpeta = CarpetaSaves();
        if (!Directory.Exists(carpeta))
            Directory.CreateDirectory(carpeta);
    }

    static string CarpetaSaves()
        => Path.Combine(Application.persistentDataPath, "Saves");

    static string RutaSave(string slotId)
        => Path.Combine(CarpetaSaves(), slotId + SaveExtension);

    static string CrearSlotId(string nombre)
    {
        string s = nombre.ToLowerInvariant();
        foreach (char c in Path.GetInvalidFileNameChars())
            s = s.Replace(c.ToString(), "");
        s = s.Replace(" ", "_");
        if (s.Length > 24) s = s.Substring(0, 24);
        return s + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
    }
}