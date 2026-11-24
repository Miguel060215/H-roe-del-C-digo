# Arquitectura de Software y Diccionario de Scripts (`ARCHITECTURE.md`)

Este documento describe la arquitectura técnica de **Héroe del Código**, los patrones de diseño empleados para desacoplar el contenido educativo de la lógica de juego, la gestión de estados globales entre escenas y el catálogo completo de los scripts en C# dentro de `Assets/Scripts/`.

---

## 1. Principios Arquitectónicos y Patrones de Diseño

### Separación de Datos y Lógica (Desacoplamiento)
El núcleo educativo del videojuego (preguntas de Python, opciones múltiples, retroalimentación visual y pistas) no está codificado de forma rígida (*hardcoded*) dentro de condicionales por nivel. En su lugar:
* `ControladorReto.cs` define estructuras de datos serializables (`ProblemaPython`) configurables directamente desde el **Inspector de Unity**.
* Esto permite crear, editar o reordenar retos para `Nivel1`, `Nivel2` y `Nivel3` reutilizando el mismo prefab y controlador lógico sin alterar el código fuente.

### Gestión Centralizada de Estado Global y Limpieza de Escena
Para evitar fugas de estado al reiniciar niveles, regresar al menú principal o avanzar de escenario, los datos persistentes de sesión (como el inventario de cartas recolectadas o el estado de pausa) cuentan con mecanismos explícitos de limpieza:
* `MenuPausa.ResetearDatosNivel()` restablece las colecciones estáticas y flags globales antes de ejecutar `SceneManager.LoadScene(...)`, garantizando que el jugador inicie cada intento con el estado limpio y el tiempo reanudado (`Time.timeScale = 1f`).

### Transiciones Asíncronas Independientes de `Time.timeScale`
* Durante la resolución de retos en terminales de código, el juego congela la física y el movimiento de escena (`Time.timeScale = 0f`).
* Para ejecutar transiciones visuales (como el destello de confirmación `RutinaCambioDeReto` en `ControladorReto.cs`), se emplean corrutinas basadas en tiempo real (`WaitForSecondsRealtime`) combinadas con componentes `CanvasGroup`, evitando bloqueos permanentes cuando el juego está pausado.

---

## 2. Flujo de Ejecución del Juego

1. **Inicio y Menú Principal (`Menu`):** `CinematicaInicio.cs` gestiona la secuencia introductoria y da paso a `SistemaMenu.cs` / `ControladorMenu.cs` para la selección de nivel y consulta del compendio de cartas.
2. **Exploración y Plataformas (`Nivel1`, `Nivel2`, `Nivel3`):** `Player.cs` controla el movimiento 2D, interacciones y habilidades temporales (`ObjetoDobleSalto.cs`), acompañado por `SeguimientoCamara.cs` y `EfectoParallax.cs`.
3. **Recolección y Aprendizaje:** Al recoger ítems (`CartaColeccionable.cs`) se actualiza el registro de cartas reflejado en `HUDCartas.cs` y se desbloquea la lección correspondiente. Los letreros (`Letrero.cs`) solo muestran mensajes contextuales y no modifican dicho registro.
4. **Evaluación en Terminal (`TerminalRetoFinal.cs` + `ControladorReto.cs`):** Al activar la terminal al final de cada escenario, se despliega la interfaz de retos de Python. Superar los ejercicios desbloquea el avance hacia la siguiente escena.

---

## 3. Catálogo de Scripts (`Assets/Scripts/`)

Los 21 scripts del proyecto se dividen en módulos especializados por carpeta:

### A. Módulo de Menús (`Assets/Scripts/Menu/`)
| Script | Responsabilidad Principal |
| :--- | :--- |
| **`SistemaMenu.cs`** | Controla las acciones principales de la interfaz del menú inicial (iniciar partida, selección de niveles y salida de la aplicación); delega la presentación de subpaneles en `ControladorMenu.cs`. |

### B. Módulo de Retos Educativos (`Assets/Scripts/Retos/`)
| Script | Responsabilidad Principal |
| :--- | :--- |
| **`ControladorReto.cs`** | Motor de evaluación de retos de Python. Administra la lista de problemas (`ProblemaPython`), valida respuestas, actualiza imágenes de retroalimentación/errores, controla el `CanvasGroup` de opciones y ejecuta la corrutina de transición (`RutinaCambioDeReto`). |
| **`TerminalRetoFinal.cs`** | Objeto interactivo en escena que detecta la proximidad y acción del jugador (`E`) para activar el panel de `ControladorReto` al finalizar cada nivel. |

### C. Jugador, Cámara y Habilidades (`Assets/Scripts/`)
| Script | Responsabilidad Principal |
| :--- | :--- |
| **`Player.cs`** | Controlador principal del protagonista (*Otix*). Gestiona desplazamiento horizontal, salto, detección de suelo, animaciones (`Animator`), colisiones e interacción con objetos del entorno. |
| **`SaludJugador.cs`** | Administra los puntos de vida del jugador, recepción de daño, invulnerabilidad temporal, actualización de corazones en HUD y reinicio ante derrota. |
| **`ObjetoDobleSalto.cs`** | Potenciador (*power-up*) recolectable que habilita temporalmente la mecánica de doble salto en `Player.cs`, gestionando su reactivación tras un tiempo de espera o reinicio. |
| **`SeguimientoCamara.cs`** | Controla el desplazamiento suavizado de la cámara principal siguiendo la posición de *Otix* dentro de los límites establecidos en cada nivel. |
| **`EfectoParallax.cs`** | Genera profundidad visual desplazando las capas de fondo (como `CapasDesierto`) a distintas velocidades relativas al movimiento de la cámara. |

### D. Interfaz de Usuario, Narrativa y Flujo de Escena (`Assets/Scripts/`)
| Script | Responsabilidad Principal |
| :--- | :--- |
| **`CinematicaInicio.cs`** | Controla la reproducción y omisión de la cinemática introductoria al arrancar el juego antes de habilitar el menú principal. |
| **`ControladorMenu.cs`** | Coordina la navegación entre subpaneles de interfaz (compendio de cartas desbloqueadas, controles, créditos y ventanas emergentes). |
| **`MenuPausa.cs`** | Gestiona la pausa en partida (`Time.timeScale = 0f`), reinicio del nivel actual, retorno al menú principal y la limpieza global de datos mediante `ResetearDatosNivel()`. |
| **`HUDCartas.cs`** | Actualiza en tiempo real el indicador visual de cartas recolectadas durante el nivel y sincroniza su estado con el visor de lecciones de Python. |
| **`CartaColeccionable.cs`** | Define el comportamiento de los ítems de conocimiento en escena; al ser recolectados, desbloquean la lección correspondiente (banner y audio) y notifican al HUD. |
| **`Letrero.cs`** | Muestra mensajes contextuales, pistas de controles o instrucciones en pantalla cuando el jugador entra en el área de activación del letrero. |

### E. Entorno, Obstáculos y Mecánicas de Nivel (`Assets/Scripts/`)
| Script | Responsabilidad Principal |
| :--- | :--- |
| **`PlataformaMovil.cs`** | Desplaza plataformas entre puntos definidos y emparenta temporalmente al jugador para mantener su estabilidad durante el trayecto. |
| **`ObstaculoMovil.cs`** | Controla el patrón de movimiento (patrullaje o oscilación) de peligros dinámicos dentro de los escenarios. |
| **`DannoTrampa.cs`** | Aplica daño al jugador al entrar en contacto con trampas fijas del escenario (como pinchos o zonas peligrosas). |
| **`DañoObstaculos.cs`** | Gestiona la detección de colisión y reducción de salud provocada por obstáculos específicos del entorno. |
| **`PuntoControl.cs`** | Activa puntos de guardado (*checkpoints*) intermedios dentro del nivel para reaparecer al jugador sin perder todo el progreso del recorrido. |
| **`EstacionCarga.cs`** | Punto interactivo o de restauración energética dentro del escenario que recarga el estado del jugador. |
| **`Teletransportador.cs`** | Traslada al jugador de forma instantánea entre dos coordenadas de la escena o hacia una nueva zona del nivel al interactuar o cruzar el portal. |

---

## 4. Notas de Diseño y Limitaciones Conocidas

* **Ubicación de `ResetearDatosNivel()`:** actualmente reside en `MenuPausa.cs` (clase de interfaz). Como mejora, puede migrarse a una clase de estado de sesión dedicada. Debe verificarse que la ruta de reinicio por derrota (`SaludJugador.cs`) y el fin de nivel invoquen la misma limpieza.
* **Uso de `Time.timeScale = 0f`:** toda ruta de salida de un reto (cambio de escena, derrota, cierre de terminal) debe restablecer `Time.timeScale = 1f`; centralizarlo en un único gestor reduce el riesgo de congelar el juego.
* **Convención de nombres:** existe mezcla de inglés y español (`Player.cs`) y de caracteres (`DannoTrampa.cs` frente a `DañoObstaculos.cs`). Se recomienda evitar la `ñ` en nombres de archivo y clase C# y unificar el idioma y el estilo (`PascalCase` para tipos y métodos públicos, `camelCase` para campos).
* **Separación de menús:** `SistemaMenu.cs` agrupa las acciones globales de la aplicación y `ControladorMenu.cs` la navegación entre subpaneles; sus responsabilidades deben mantenerse delimitadas para evitar solapamientos.
