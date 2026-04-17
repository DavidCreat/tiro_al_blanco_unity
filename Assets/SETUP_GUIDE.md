# Tiro al Blanco 3D — Guía de Configuración de Escena

## 1. ESTRUCTURA DE SCRIPTS CREADOS

```
Assets/Scripts/
├── Core/
│   ├── GameManager.cs       - Estado del juego, timer
│   ├── ScoreManager.cs      - Puntuación, combos, high score
│   └── AudioManager.cs      - Todos los sonidos
├── Gameplay/
│   ├── Target.cs            - Comportamiento del blanco
│   ├── TargetBuilder.cs     - Crea el visual del blanco con primitivos
│   ├── TargetSpawner.cs     - Spawneo con object pooling
│   ├── ShootingController.cs- Raycast shooting, munición, recarga
│   ├── MouseLook.cs         - Control de cámara FPS
│   └── PauseController.cs   - Manejo de pausa con ESC
├── Effects/
│   ├── CameraShake.cs       - Vibración de cámara al disparar
│   └── HitEffect.cs         - Partículas al impacto
├── UI/
│   ├── UIManager.cs         - Toda la interfaz
│   ├── FloatingText.cs      - Texto flotante de puntos
│   └── CrosshairController.cs - Mira dinámica
└── Data/
    ├── GameConfig.cs        - ScriptableObject config general
    └── TargetData.cs        - ScriptableObject por tipo de blanco
```

---

## 2. SETUP DE ESCENA PASO A PASO

### A. Managers (GameObject vacío "Managers")
Crea un GameObject llamado `Managers` y agrega:
- `GameManager`
- `ScoreManager`
- `AudioManager`
- `PauseController`

### B. Player (GameObject "Player")
```
Player (vacío - en posición 0,0,0)
└── Camera Main (Camera component)
    ├── ShootingController
    ├── MouseLook
    └── CameraShake
```
**Camera:** Tag = "MainCamera", Position = (0, 1.7, 0)

En ShootingController:
- `Shootable Layers` = "Target" + "Default"
- Arrastra `CameraShake` al campo correspondiente

### C. Escenario de Galería
Crea el siguiente escenario con primitivos:

```
Environment
├── Floor (Plane) - Scale (20,1,30), Position (0,0,15)
├── BackWall (Cube) - Scale (20,5,0.5), Position (0,2.5,30)
├── LeftWall (Cube) - Scale (0.5,5,30), Position (-10,2.5,15)
├── RightWall (Cube) - Scale (0.5,5,30), Position (10,2.5,15)
└── Ceiling (Plane) - Scale (20,1,30), Position (0,5,15), rotado 180° en X
```

Asigna materiales con texturas de madera/ladrillo para dar el look de galería de tiro.

### D. Spawn Points
Crea GameObjects vacíos como hijos de `SpawnPoints`:
```
SpawnPoints
├── SpawnPoint_01 - Position (-4, 1.2, 10)
├── SpawnPoint_02 - Position (-2, 1.2, 12)
├── SpawnPoint_03 - Position (0, 1.5, 15)
├── SpawnPoint_04 - Position (2, 1.2, 12)
├── SpawnPoint_05 - Position (4, 1.2, 10)
├── SpawnPoint_06 - Position (-3, 2.5, 14)
├── SpawnPoint_07 - Position (3, 2.5, 14)
└── SpawnPoint_08 - Position (0, 1.2, 8)
```

### E. TargetSpawner
Crea un GameObject `TargetSpawner` y agrega el componente `TargetSpawner`.
- Asigna el prefab `Target` (ver abajo)
- Arrastra todos los SpawnPoints al array

### F. Target Prefab
1. Crea un GameObject vacío llamado `Target`
2. Agrega componentes: `Target`, `TargetBuilder`
3. En TargetBuilder: asigna un Material URP (Lit)
4. Guárdalo como Prefab en `Assets/Prefabs/`

---

## 3. CREAR SCRIPTABLE OBJECTS

### GameConfig
1. Right-click en Assets → Create → TiroAlBlanco → Game Config
2. Configura: Duration=60, MaxAmmo=6, ReloadTime=1.5, MaxTargets=5
3. Asígnalo al GameManager y TargetSpawner

### TargetData (crear uno por tipo)
Right-click → Create → TiroAlBlanco → Target Data

| Nombre    | Type    | Points | Color   | Scale | Weight | Moves |
|-----------|---------|--------|---------|-------|--------|-------|
| Normal    | Normal  | 100    | Red     | 1.0   | 0.5    | No    |
| Moving    | Moving  | 150    | Orange  | 1.0   | 0.25   | Yes   |
| Fast      | Fast    | 200    | Purple  | 0.8   | 0.15   | No    |
| Bonus     | Bonus   | 300    | Gold    | 0.9   | 0.07   | No    |
| Penalty   | Penalty | -100   | DarkRed | 1.0   | 0.03   | No    |

Para Fast: minActiveTime=0.8, maxActiveTime=1.5
Para Moving: moves=true, moveSpeed=2, moveDistance=3

---

## 4. UI SETUP

### Canvas (Screen Space - Overlay)

```
Canvas
├── HUDPanel
│   ├── ScoreText (TMP) - arriba izquierda
│   ├── TimerText (TMP) - arriba centro (grande)
│   ├── ComboText (TMP) - centro
│   ├── AmmoPanel
│   │   ├── AmmoSlot_1 (Image)
│   │   ├── AmmoSlot_2 (Image)
│   │   ├── AmmoSlot_3 (Image)
│   │   ├── AmmoSlot_4 (Image)
│   │   ├── AmmoSlot_5 (Image)
│   │   └── AmmoSlot_6 (Image)
│   └── ReloadingText (TMP) "RECARGANDO..."
│
├── Crosshair (GameObject con CrosshairController)
│   ├── CrossTop (Image - línea vertical arriba)
│   ├── CrossBottom (Image - línea vertical abajo)
│   ├── CrossLeft (Image - línea horizontal izq)
│   ├── CrossRight (Image - línea horizontal der)
│   └── CenterDot (Image - punto central)
│
├── MainMenuPanel
│   ├── Title (TMP) "TIRO AL BLANCO"
│   └── PlayButton → llama UIManager.OnPlayButtonPressed()
│
├── PauseMenuPanel
│   ├── Title (TMP) "PAUSA"
│   ├── ResumeButton → UIManager.OnResumeButtonPressed()
│   ├── MainMenuButton → UIManager.OnMainMenuButtonPressed()
│   └── QuitButton → UIManager.OnQuitButtonPressed()
│
└── GameOverPanel
    ├── GameOverTitle (TMP)
    ├── FinalScoreText (TMP)
    ├── HighScoreText (TMP)
    ├── NewHighScoreLabel (TMP) "¡NUEVO RECORD!"
    ├── RestartButton → UIManager.OnRestartButtonPressed()
    └── MainMenuButton → UIManager.OnMainMenuButtonPressed()
```

Agrega `UIManager` al Canvas y conecta todas las referencias.
Agrega `FloatingScoreCanvas` (Canvas en World Space) aparte para los textos flotantes.

---

## 5. LAYER SETUP

En Edit → Project Settings → Tags and Layers:
- Crea layer: `Target`

Asigna layer `Target` al prefab Target.

---

## 6. ASSETS GRATUITOS RECOMENDADOS (Unity Asset Store)

### Partículas / Efectos
- **Free Quick Effects Vol. 1** - Efectos de explosión/impacto
- **3D Games Effects Pack Free** - Pack completo de efectos 3D

### Sonidos
- Busca en Asset Store: "Gun sound effects free"
- Busca: "Cartoon shooting gallery sounds"

### Materiales / Texturas  
- **Yughues Free Wood Materials**
- **Brick Textures Pack Free**

### Fuentes
- Usa **TextMeshPro** (ya incluido) con la fuente "Impact" o similar

---

## 7. POST-PROCESSING (URP)

En el Global Volume de la escena, agrega:
- **Bloom**: Intensity=0.5, Threshold=0.9
- **Vignette**: Intensity=0.3 (aumenta durante el juego para tensión)
- **Color Adjustments**: Saturation leve para look estilizado

---

## 8. CONTROLES

| Acción   | Teclado/Mouse      |
|----------|--------------------|
| Disparar | Clic Izquierdo     |
| Recargar | E                  |
| Pausar   | Escape             |
| Mirar    | Mover Mouse        |

---

## 9. PRÓXIMOS PASOS

1. [ ] Descargar assets de audio (sonidos de disparo, hit, reload)
2. [ ] Descargar particle effects del Asset Store
3. [ ] Crear materiales para el escenario (madera, metal)
4. [ ] Agregar animaciones de pop-up para los blancos
5. [ ] Agregar un Animator Controller al prefab Target
6. [ ] Probar y balancear la dificultad
