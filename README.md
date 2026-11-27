# Héroe del Código

Videojuego educativo en 2D desarrollado para **Mech Robotix, A.C.** con el propósito de enseñar fundamentos de programación en **Python** de forma interactiva. El jugador controla a *Otix* a través de distintos escenarios (Desierto, Sierra y Domo en Marte) recolectando cartas de conocimiento, superando obstáculos y resolviendo retos de lógica y sintaxis en terminales de código.

🌐 **Versión Web en producción:** [https://mechrobotix.com/heroedelcodigo/](https://mechrobotix.com/heroedelcodigo/)

---

## Stack Tecnológico y Herramientas

* **Motor de Videojuegos:** Unity `6000.5.10f1` (Unity 6.5 — 2D, TextMeshPro, UI/Canvas).
* **Lenguaje de Programación:** C# (.NET / Mono / IL2CPP para WebAssembly).
* **Arte y Recursos Visuales:** LibreSprite (diseño de personajes pixel-art como *Otix* y la *Heroína*, tilesets ambientales, banners de lecciones e interfaces).
* **Control de Versiones:** Git y GitHub (flujo colaborativo basado en ramas integradas hacia `main`).
* **Plataformas de Despliegue:**
  * **Web (WebGL):** Compilación en HTML5 / WebAssembly alojada en **Hostinger** ([mechrobotix.com/heroedelcodigo/](https://mechrobotix.com/heroedelcodigo/)).
  * **Escritorio:** Ejecutable independiente para Windows (`.exe`).

---

## Estructura del Proyecto

La organización dentro de la carpeta `Assets/` sigue una arquitectura modular que separa la lógica de programación, las escenas, los recursos audiovisuales y los materiales didácticos por nivel:

```text
Assets/
├── Animaciones/              # Controladores de animación y clips (HeroinaAnim, OtixAnim)
├── Audio/                    # Efectos de sonido y música por contexto (MenuAudio, Nivel1Audio, OTIXAudio)
├── Prefab/                   # Objetos reutilizables de escena (Letreros interactivos, ítems, plataformas)
├── Resources/                # Recursos de carga dinámica en tiempo de ejecución
├── Scenes/                   # Escenas del juego (Menu, Nivel1, Nivel2, Nivel3)
├── Scripts/                  # Código fuente en C# (21 scripts)
│   ├── Menu/                 # SistemaMenu.cs: acciones principales del menú inicial
│   ├── Retos/                # ControladorReto.cs y TerminalRetoFinal.cs (terminales de Python y validación)
│   └── (raíz)                # Jugador, cámara, pausa, HUD, cinemática, ControladorMenu y mecánicas de entorno
├── Settings/                 # Configuraciones del proyecto, Build Profiles (Web / Windows) y Scene List
├── Sprites/                  # Recursos gráficos en pixel-art organizados por función y nivel
│   ├── ElementosExtra/       # Decoración adicional de entorno
│   ├── ElementosRetos/       # Recursos visuales de los retos por nivel (OtixNivel1, OtixNivel2, OtixNivel3)
│   ├── Heroina/              # Sprites del personaje secundario
│   ├── LeccionesPython/      # Banners educativos y audios de las cartas de aprendizaje (ContNivel1 - 3)
│   ├── MenuCartas/           # Interfaz visual del compendio de cartas desbloqueadas
│   ├── Nivel3/               # Tilesets temáticos de Marte (TileDomo, TileMarte, TileObstaculos)
│   ├── OTIX/                 # Hoja de sprites del protagonista
│   ├── Otros elementos/      # Capas de paralaje (CapasDesierto), ítems de vida/cartas, letreros y teclas UI
│   ├── Paletas/              # Paletas de pintado de niveles (Tile Palette)
│   └── Tilesets/             # Conjuntos de bloques y colisiones para Nivel 1 y Nivel 2
├── TextMesh Pro/             # Fuentes, shaders y recursos de tipografía UI
└── UI/                       # Elementos gráficos de interfaz de usuario y HUD
```

---

## Requisitos e Instalación Local

### 1. Prerrequisitos
* **Unity Hub** con la versión exacta **Unity `6000.5.10f1`** instalada.
* Módulos necesarios en Unity Hub:
  * *Windows Build Support (IL2CPP / Mono)*
  * *Web Build Support* (requerido para exportar a navegador)
* **Git** instalado en el sistema.

### 2. Clonar y abrir el proyecto
1. Clona el repositorio en tu equipo local:
   ```bash
   git clone https://github.com/Miguel060215/HeroeDelCodigo.git
   ```
2. Abre **Unity Hub**, haz clic en **Add > Add project from disk** y selecciona la carpeta raíz `HeroeDelCodigo`.
3. Asegúrate de seleccionar la versión `6000.5.10f1` en la lista de instalaciones y abre el proyecto.

### 3. Flujo de trabajo con Git (Sincronización de ramas)
Antes de iniciar cambios o compilar una nueva versión, asegúrate de guardar tus avances locales y traer los últimos cambios integrados en `main`:

1. Guarda tus modificaciones locales (o usa `git stash` si no deseas hacer commit aún):
   ```bash
   git add .
   git commit -m "Guardo avances locales"
   ```
2. Descarga y fusiona los últimos cambios desde `main`:
   ```bash
   git fetch origin main
   git merge origin/main
   ```
3. Resuelve cualquier conflicto de escenas o scripts si se presenta y sube tu rama actualizada:
   ```bash
   git push
   ```

### 4. Recomendaciones de control de versiones para proyectos Unity
* Mantén un `.gitignore` para Unity que excluya `Library/`, `Temp/`, `Obj/`, `Logs/`, `UserSettings/` y las carpetas de compilación (`Builds/`).
* Conserva los archivos `.meta` bajo control de versiones y usa **Force Text** en la serialización de assets para facilitar las fusiones.
* Evita que dos personas editen la misma escena a la vez y considera **Git LFS** para audio y sprites pesados.

---

## Documentación Técnica Adicional

Para consultar los detalles operativos de compilación y el análisis interno del código, revisa los siguientes documentos del repositorio:

* **[Guía de Despliegue y Mantenimiento (`DEPLOY.md`)](./DEPLOY.md):** Procedimiento paso a paso para exportar el proyecto a WebGL (sin compresión Brotli/Gzip), configurar Hostinger (`.htaccess`) y actualizar compilaciones de escritorio.
* **[Arquitectura y Diccionario de Scripts (`ARCHITECTURE.md`)](./ARCHITECTURE.md):** Explicación del diseño desacoplado, reinicio de estado entre niveles y catálogo detallado de cada clase en C#.
