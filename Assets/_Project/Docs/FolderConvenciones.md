# ARIA: Último Protocolo — Convenciones de carpetas

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
