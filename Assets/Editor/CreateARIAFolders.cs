// ============================================================
//  CreateARIAFolders.cs
//  ARIA: Último Protocolo — Setup de estructura de carpetas
//
//  USO:
//  1. Colocar este archivo en cualquier carpeta "Editor" del proyecto
//     (créala si no existe: Assets/Editor/CreateARIAFolders.cs)
//  2. En Unity: menú superior → ARIA → Create Project Folders
//  3. Ejecutar UNA SOLA VEZ al inicio del proyecto
// ============================================================

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

public static class CreateARIAFolders
{
    [MenuItem("ARIA/Create Project Folders")]
    public static void CreateFolders()
    {
        string[] folders = new string[]
        {
            // ── CONFIG GLOBAL ──────────────────────────────────────
            "Assets/_Project",
            "Assets/_Project/Settings",
            "Assets/_Project/Docs",

            // ── SCENES ────────────────────────────────────────────
            "Assets/Scenes",

            // ── SCRIPTS ───────────────────────────────────────────
            "Assets/Scripts",
            "Assets/Scripts/Player",
            "Assets/Scripts/Enemies",
            "Assets/Scripts/Weapons",
            "Assets/Scripts/UI",
            "Assets/Scripts/GameSystems",
            "Assets/Scripts/Narrative",

            // ── PREFABS ───────────────────────────────────────────
            "Assets/Prefabs",
            "Assets/Prefabs/Player",
            "Assets/Prefabs/Enemies",
            "Assets/Prefabs/Weapons",
            "Assets/Prefabs/UI",
            "Assets/Prefabs/Environment",
            "Assets/Prefabs/VFX",

            // ── ART ───────────────────────────────────────────────
            "Assets/Art",
            "Assets/Art/Models",
            "Assets/Art/Models/Characters",
            "Assets/Art/Models/Enemies",
            "Assets/Art/Models/Environment",
            "Assets/Art/Models/Weapons",
            "Assets/Art/Models/Props",
            "Assets/Art/Textures",
            "Assets/Art/Textures/Characters",
            "Assets/Art/Textures/Environment",
            "Assets/Art/Textures/UI",
            "Assets/Art/Textures/VFX",
            "Assets/Art/Materials",
            "Assets/Art/Animations",
            "Assets/Art/Animations/Player",
            "Assets/Art/Animations/Enemies",
            "Assets/Art/Shaders",

            // ── AUDIO ─────────────────────────────────────────────
            "Assets/Audio",
            "Assets/Audio/Music",
            "Assets/Audio/SFX",
            "Assets/Audio/SFX/Player",
            "Assets/Audio/SFX/Enemies",
            "Assets/Audio/SFX/Weapons",
            "Assets/Audio/SFX/UI",
            "Assets/Audio/SFX/Ambient",

            // ── UI ────────────────────────────────────────────────
            "Assets/UI",
            "Assets/UI/Fonts",
            "Assets/UI/Sprites",
            "Assets/UI/Icons",

            // ── DISEÑO DE NIVELES ─────────────────────────────────
            "Assets/Levels",
            "Assets/Levels/Level01",
            "Assets/Levels/Level02",
            "Assets/Levels/Level03",
            "Assets/Levels/Level04",
            "Assets/Levels/Level05",

            // ── THIRD PARTY (NO MODIFICAR) ────────────────────────
            "Assets/ThirdParty",
            "Assets/ThirdParty/SciFiFacility",
            "Assets/ThirdParty/Cinemachine",
        };

        int created = 0;
        int skipped = 0;

        foreach (string folder in folders)
        {
            if (!AssetDatabase.IsValidFolder(folder))
            {
                // Crear carpeta padre si no existe
                string parent = Path.GetDirectoryName(folder).Replace('\\', '/');
                string child  = Path.GetFileName(folder);
                AssetDatabase.CreateFolder(parent, child);
                created++;
            }
            else
            {
                skipped++;
            }
        }

        // Crear archivo README en _Project/Docs
        CreateReadme();

        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(
            "ARIA — Carpetas creadas",
            $"Estructura lista.\n\n" +
            $"✅ Creadas: {created}\n" +
            $"⏭ Ya existían: {skipped}\n\n" +
            "Recuerda:\n" +
            "• ThirdParty → solo importar, no editar\n" +
            "• Cada quien trabaja en su carpeta asignada\n" +
            "• Hacer commit de .meta files siempre",
            "OK"
        );

        Debug.Log($"[ARIA Setup] {created} carpetas creadas, {skipped} omitidas.");
    }

    static void CreateReadme()
    {
        string path = "Assets/_Project/Docs/FolderConvenciones.md";

        if (File.Exists(path)) return;

        string content =
@"# ARIA: Último Protocolo — Convenciones de carpetas

## Reglas generales
- **No mover ni renombrar** carpetas sin avisarle al equipo
- Siempre hacer commit de los archivos `.meta` junto con su asset
- `ThirdParty/` es de solo lectura — no modificar assets importados
- Prefijos de nombre de archivo: `ARIA_NombreDelAsset`

## Responsables por carpeta

| Carpeta | Responsable |
|---|---|
| Scripts/Player, Scripts/Enemies, Scripts/Weapons | Sebastian |
| Scripts/UI, Scripts/GameSystems, Audio | Cristian |
| Art/, UI/Fonts, UI/Sprites | Kevin |
| Scenes/, Levels/, Scripts/Narrative, _Project/Docs | Jesús |

## Convención de nombres
- Scripts: `PascalCase.cs`
- Prefabs: `PFB_NombreDelPrefab`
- Materiales: `MAT_NombreDelMaterial`
- Texturas: `TEX_NombreDeTextura`
- Sonidos: `SFX_NombreDelSonido` / `MUS_NombreDeMusica`
- Escenas: `Level_01_NombreDelNivel`

## Git — qué NO commitear
Agregar al `.gitignore`:
```
/[Ll]ibrary/
/[Tt]emp/
/[Oo]bj/
/[Bb]uild/
/[Bb]uilds/
*.userprefs
*.suo
.DS_Store
Thumbs.db
```
";

        // Asegurarse de que la carpeta existe antes de escribir
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, content);
        AssetDatabase.ImportAsset(path);
    }
}
#endif
