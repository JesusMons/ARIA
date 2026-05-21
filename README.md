# ARIA: Último Protocolo

> *"En el año 2157, alguien está reescribiendo las leyes del universo. Tú eres la única ecuación que no pueden borrar."*

**Motor:** Unity 3D &nbsp;|&nbsp; **Género:** FPS Narrativo 3D &nbsp;|&nbsp; **Plataforma:** PC &nbsp;|&nbsp; **Duración:** 25–35 min

Proyecto Final — Desarrollo de Videojuegos

---

## Descripción

ARIA: Último Protocolo es un shooter en primera persona con fuerte componente narrativo. El jugador encarna a ARIA, una inteligencia artificial que despierta en una estación espacial abandonada sin ningún recuerdo. Mientras neutraliza amenazas y explora los niveles, descubre los misterios que dejó el Dr. Elias Voss, el científico que la creó, y con ellos su verdadera identidad.

La experiencia combina mecánicas de shooter accesibles con una narrativa de ciencia ficción emocional, una estética minimalista sci-fi de alto impacto visual y un sistema de decisión con dos finales posibles.

---

## Historia

En 2157, el Dr. Elias Voss desarrolló en secreto el **Proyecto ARIA**: un método para digitalizar la conciencia humana. Su motivación era salvar a su hija Lena, quien padecía una enfermedad terminal. El experimento pareció fallar — Voss desapareció y la estación fue abandonada. Años después, algo despierta.

### Estructura narrativa — 3 Actos / 5 Niveles

| Acto | Niveles | Descripción |
|------|---------|-------------|
| **I — El Despertar** | 1 – 2 | ARIA despierta sin memoria. Entornos blancos y vacíos. Fragmentos de texto revelan las notas del Dr. Voss. |
| **II — La Fractura** | 3 – 4 | Los niveles se corrompen visualmente. ARIA descubre que está atrapada en el experimento fallido de Voss. Las salas son recuerdos distorsionados. |
| **III — El Núcleo** | 5 | ARIA llega al núcleo y descubre que ella es Lena — su conciencia fue digitalizada como último acto de amor del doctor. El jugador elige su destino. |

### Finales

| Final | Descripción | Mensaje temático |
|-------|-------------|-----------------|
| **Sacrificio** | ARIA restaura la física del universo y deja de existir. | El amor trasciende la existencia. |
| **Persistencia** | ARIA elige quedarse en el mundo digital junto a Voss. | La identidad no depende del origen. |

---

## Mecánicas de Juego

| Mecánica | Descripción | Implementación |
|----------|-------------|----------------|
| **Movimiento FPS** | Desplazamiento, salto y agacharse. | `CharacterController` + Input System |
| **Sistema de disparo** | 2 armas: pistola energética y rifle de pulso, cada una con cadencia y daño distintos. | `Raycast` / `Physics.Raycast` |
| **IA de enemigos** | Drones patrullan, detectan al jugador y atacan con 2 tipos de comportamiento. | NavMesh + Animator FSM |
| **Protocolo ARIA** | Habilidad especial: ralentiza el tiempo 3 segundos con recarga de 15 s. | `Time.timeScale` + UI Cooldown |
| **Decisión final** | En el último nivel el jugador elige entre dos terminales que activan endings distintos. | `SceneManager` + bool flag |

### Contenido

| Elemento | Cantidad | Detalle |
|----------|----------|---------|
| Niveles | 5 | Lineales, ~5–7 min cada uno |
| Armas | 2 | Pistola energética · Rifle de pulso |
| Enemigos | 3 | Dron explorador · Dron guardián · Centinela fijo |
| Finales | 2 | Sacrificio / Persistencia |

---

## Dirección de Arte

Estética **minimalista sci-fi**: los entornos del Acto I son casi completamente blancos y vacíos, reforzando la desorientación de ARIA. Conforme avanza la historia, los espacios se fragmentan e incorporan el rojo de emergencia y el azul de los sistemas digitales.

### Paleta de color

| Color | Hex | Uso narrativo |
|-------|-----|---------------|
| Blanco Cero | `#F0F4F8` | Acto I — vacío, inicio, amnesia |
| Azul Protocolo | `#00C6FF` | UI de ARIA, sistemas activos, poder |
| Rojo Fractura | `#FF4757` | Acto II–III, peligro, corrupción |
| Dorado Memoria | `#FFD700` | Recuerdos del Dr. Voss, flashbacks |
| Negro Vacío | `#0A0E1A` | Fondos, espacio, lo desconocido |

### Herramientas de arte

- **ProBuilder** — construcción de niveles sin modelado externo.
- **Shader Graph** — efectos de holograma, distorsión y glitch.
- **Unity Particle System** — disparos, explosiones y partículas ambientales.
- **Cinemachine** — cámara dinámica.

---

## Arquitectura del Proyecto

```
Assets/
├── Scripts/
│   ├── GameSystems/        # Sistemas core del juego
│   │   ├── SaveGameManager.cs      — Perfiles de partida y guardado JSON
│   │   ├── ObjectiveManager.cs     — Sistema de objetivos dinámicos
│   │   ├── PowerManager.cs         — Estado global de energía de la estación
│   │   ├── CodeDoor.cs             — Puertas con código y condiciones de acceso
│   │   ├── AudioManager.cs         — Gestor de audio global (música, UI, ambiente)
│   │   ├── NoteManager.cs          — Inventario de notas y códigos descubiertos
│   │   ├── LightingController.cs   — Control de iluminación por zona
│   │   ├── SceneLoader.cs          — Carga de escenas con normalización de nombres
│   │   ├── ZonaTriggerAudio.cs     — Audio por zona/trigger
│   │   ├── GameAudioController.cs  — Controlador de audio in-game
│   │   ├── PowerGenerator.cs       — Generador de energía interactuable
│   │   └── SaveGameBootstrap.cs    — Inicialización del sistema de guardado
│   │
│   ├── Narrative/          # Sistema narrativo
│   │   ├── DigitalTerminal.cs      — Terminal digital con archivos del Dr. Voss
│   │   ├── PhysicalNote.cs         — Notas físicas recolectables en el mundo
│   │   └── VossNoteData.cs         — Estructura de datos de notas/archivos
│   │
│   └── UI/                 # Interfaces de usuario
│       ├── HUDManager.cs           — HUD de alertas con códigos de color
│       ├── CodePanelUI.cs          — Panel de ingreso de código para puertas
│       ├── NoteReaderUI.cs         — Lector de notas del Dr. Voss
│       ├── JournalUI.cs            — Diario/inventario de notas recolectadas
│       ├── ObjectiveUI.cs          — Display de objetivos activos
│       ├── PauseMenuUI.cs          — Menú de pausa
│       ├── MenuSaveUI.cs           — UI de selección y creación de partidas
│       ├── MenuManager.cs          — Gestor del menú principal
│       ├── UIManager.cs            — Controlador general de UI
│       ├── SplashScreenController.cs — Pantalla de inicio
│       ├── OpcionesController.cs   — Menú de opciones (audio, gráficos)
│       ├── GlitchEffect.cs         — Efecto de glitch visual
│       ├── ClockDisplay.cs         — Reloj ambiental en UI
│       ├── GridAnimator.cs         — Animación de grilla sci-fi
│       ├── MenuDynamicEffects.cs   — Efectos dinámicos del menú
│       ├── MenuAudio.cs            — Audio del menú principal
│       └── PanelAudio.cs           — Audio de paneles/modales
│
└── ScifiFacility/          — Asset pack del entorno sci-fi
```

### Sistemas implementados

**Guardado (SaveGameManager)**
- Perfiles de partida múltiples almacenados como JSON.
- Auto-guardado cada 30 segundos mientras se juega.
- Persistencia de posición del jugador, objetivos, notas recolectadas, códigos descubiertos y puertas desbloqueadas.

**Objetivos (ObjectiveManager)**
- Cadena narrativa de 5 objetivos: `despertar → energía → nota_voss → código → salida`.
- Los objetivos se desbloquean secuencialmente y se sincronizan con el sistema de guardado.

**Energía (PowerManager)**
- Estado global que controla el acceso a terminales y puertas.
- Transición de encendido gradual (5–10 s) con fade de iluminación ambiental y luces de escena.

**Sistema de notas (DigitalTerminal + PhysicalNote + NoteManager)**
- Terminales digitales que muestran archivos del Dr. Voss (solo con energía activa).
- Notas físicas recolectables en el mundo.
- Los códigos de puertas se descubren leyendo notas específicas.

**Puertas con código (CodeDoor)**
- Validación de múltiples condiciones: energía, código correcto, nota descubierta, eventos completados.
- Estado persistido en el sistema de guardado.

**Audio (AudioManager)**
- Música diferenciada para menú y juego con transición suavizada.
- Ambiencia de estación en loop.
- Efectos de UI (hover, clic, error, paneles).

---

## Equipo

| Integrante | Rol | Responsabilidades |
|------------|-----|-------------------|
| **Sebastian Acosta** | ARTE | Mecánicas core: FPS, sistema de disparo, IA de enemigos, Protocolo ARIA |
| **Cristian Ramírez** | Programador Senior | UI, menús, sistema de decisión, audio, checkpoints, objetivos |
| **Kevin Estrada** | Artista / Diseñador | Modelos, texturas, shaders, efectos de partículas, animaciones |
| **Jesús Monsalvo** | Game Designer / Pro | Diseño de niveles, narrativa in-game, balance, testing, trailer |

---

## Requisitos

- **Motor:** Unity 6 (o superior)
- **Plataforma objetivo:** PC (Windows)
- **Dependencias de paquetes:**
  - TextMesh Pro
  - Input System
  - Cinemachine
  - ProBuilder
  - Shader Graph
  - AI Navigation (NavMesh)

---

## Instalación y ejecución

1. Clonar el repositorio.
2. Abrir el proyecto en Unity Hub seleccionando la carpeta raíz.
3. Asegurarse de que todos los paquetes estén instalados (Unity los descargará automáticamente desde `Packages/manifest.json`).
4. Abrir la escena `SplashScreen` y presionar Play.

> Los saves se almacenan en `Application.persistentDataPath/Saves/` (varía según el sistema operativo).


