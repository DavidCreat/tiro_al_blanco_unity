# Tiro al Blanco - Unity 3D

Juego de tiro al blanco en 3D desarrollado con Unity 6 y Universal Render Pipeline (URP).

El jugador mantiene presionado ESPACIO para cargar potencia, suelta para disparar una flecha con trayectoria en parabola, y solo el centro de la diana suma puntos.

## Mecanicas principales

- Diana que se mueve de lado a lado en tiempo real
- Solo el centro de la diana (collider separado) da puntos
- Carga de potencia con ESPACIO: mantener = carga, soltar = disparo
- Barra de potencia visual en el HUD
- Flecha fisica con gravedad y trayectoria en parabola
- Sistema de puntaje acumulativo de 10 en 10
- Combo multiplier: hits consecutivos aumentan los puntos

## Estructura del proyecto

### Assets/Scripts/Core

**GameManager.cs**
Singleton central que controla el estado del juego (Menu, Playing, Paused, GameOver). Maneja el temporizador de 60 segundos y expone eventos estaticos que el resto del sistema escucha: OnGameStart, OnGameOver, OnGamePause, OnGameResume, OnTimerUpdate.

**ScoreManager.cs**
Gestiona el puntaje actual, el record guardado en PlayerPrefs y el sistema de combo. El multiplicador sube un nivel por cada 3 hits consecutivos (maximo x8). Resetea el combo si pasa 2 segundos sin acertar. Cada hit al centro vale 10 puntos base multiplicado por el combo activo.

**AudioManager.cs**
Singleton con dos AudioSource: uno para efectos de sonido (SFX) y otro para musica de fondo en loop. Expone metodos publicos PlayShoot, PlayHit, PlayMiss, PlayReload, PlayCombo, PlayGameOver, etc. Se suscribe a los eventos de GameManager para controlar la musica automaticamente.

### Assets/Scripts/Gameplay

**ShootingController.cs**
Requiere Camera. Detecta cuando el jugador mantiene ESPACIO (carga de potencia 0 a 1 en 2 segundos) y al soltar instancia un prefab de Arrow con velocidad proporcional a la carga. Emite el evento OnPowerChanged para que el HUD actualice la barra de potencia. Minimo 5 unidades de fuerza, maximo 18.

**Arrow.cs**
Proyectil fisico. Requiere Rigidbody con gravedad activada. En Update rota el transform para alinearse con la velocidad (efecto de flecha en vuelo). En OnTriggerEnter detecta si colisiono con un CenterZone (bullseye, da puntos) o con el collider exterior de la diana (fallo). Al impactar se vuelve cinematico y se destruye despues de 4 segundos.

**Target.cs**
Representa una diana individual en la escena. Al activarse llama a TargetBuilder.Build() para construir los anillos visuales, inicia una corrutina de vida util y mueve el objeto de lado a lado usando una funcion seno si TargetData.moves es true. Emite OnTargetHit y OnTargetExpired para que el spawner lo devuelva al pool.

**TargetBuilder.cs**
Construye la apariencia visual de la diana en tiempo de ejecucion usando primitivos de Unity (cilindros). Crea N anillos concentricos con colores alternados segun TargetData, un SphereCollider trigger exterior para detectar flechas que no dan en el centro, y un GameObject hijo llamado CenterZone con un collider trigger de radio 25% para el bullseye.

**CenterZone.cs**
Componente marcador sin logica. Se coloca en el collider interior de la diana. Arrow.cs verifica su presencia con GetComponent para distinguir si el impacto fue en el bullseye o en la zona exterior.

**TargetSpawner.cs**
Administra un ObjectPool de dianas. Selecciona tipos de diana por peso aleatorio segun GameConfig.targetTypes, coloca cada diana en un punto de spawn libre y aumenta la dificultad reduciendo el intervalo de aparicion con el tiempo. Las dianas se devuelven al pool cuando expiran o son golpeadas, esperando a que termine la animacion de salida antes de desactivarlas.

**MouseLook.cs**
Controla la camara en primera persona usando el delta del raton via Input System. La camara rota en el eje X (vertical, limitada a 80 grados) y el padre Player rota en el eje Y (horizontal sin limite). Solo funciona mientras el juego esta en estado Playing. Incluye modo de suavizado opcional configurable desde el Inspector.

**PauseController.cs**
Detecta la tecla ESC en Update y alterna entre PauseGame y ResumeGame en GameManager. Solo actua si el estado actual es Playing o Paused.

### Assets/Scripts/Data

**TargetData.cs**
ScriptableObject que define los parametros de un tipo de diana: valor en puntos, tiempos de vida minimo y maximo, si se mueve, velocidad y distancia de movimiento, colores de anillos, escala y peso de aparicion para la seleccion aleatoria. Tipos disponibles: Normal, Moving, Fast, Bonus, Penalty.

**GameConfig.cs**
ScriptableObject con todos los parametros globales del juego: duracion de la partida, municion maxima, tiempo de recarga, cadencia de disparo, maximo de dianas simultaneas, intervalo inicial de aparicion, intervalo minimo y tasa de reduccion del intervalo.

### Assets/Scripts/Effects

**CameraShake.cs**
Corrutina que desplaza el transform local de la camara con un offset aleatorio que se atenua linealmente durante una duracion dada. Se llama desde ShootingController al disparar.

**HitEffect.cs**
Componente auxiliar para efectos visuales al impactar una diana.

### Assets/Scripts/UI

**UIManager.cs**
Centraliza toda la interfaz. Escucha eventos de GameManager, ScoreManager y ShootingController para actualizar el HUD en tiempo real: puntaje, temporizador con cambio de color en los ultimos 15 y 5 segundos, indicador de combo, barra de potencia (visible solo al cargar), y textos de game over con puntuacion final y record. Expone metodos publicos conectados a los botones: OnPlayButtonPressed, OnRestartButtonPressed, OnMainMenuButtonPressed, OnResumeButtonPressed, OnQuitButtonPressed.

**FloatingText.cs**
Texto que aparece en el mundo 3D cuando se consiguen puntos. Sube y se desvanece usando AnimationCurve durante 1 segundo y luego se destruye.

**CrosshairController.cs**
Controla la mira en pantalla. Se expande al disparar y vuelve al tamano de reposo con Lerp.

**ButtonDebugger.cs**
Herramienta de depuracion para verificar que los botones de la UI esten correctamente conectados al UIManager.

### Assets/Scripts/Editor

**SetupScene.cs**
Script de editor que automatiza la configuracion inicial de la escena desde el menu de Unity.

**SetupPlayButton.cs**
Script de editor auxiliar para configurar el boton de jugar en la escena.

## Controles

| Accion | Control |
|---|---|
| Apuntar | Mover el raton |
| Cargar potencia | Mantener ESPACIO |
| Disparar flecha | Soltar ESPACIO |
| Pausar | ESC |

## Requisitos

- Unity 6000.4.0f1 o superior
- Universal Render Pipeline (URP)
- Input System 1.x
- TextMeshPro
