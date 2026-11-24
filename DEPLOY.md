# Guía de Despliegue y Mantenimiento (`DEPLOY.md`)

Este documento detalla los procedimientos operativos para compilar, actualizar y desplegar **Héroe del Código** tanto en su versión **Web (WebGL en Hostinger)** como en su versión **Escritorio para PC (Windows `.exe`)** desde **Unity `6000.5.10f1`**.

---

## 1. Preparación previa a cualquier compilación

Antes de exportar una nueva versión (ya sea tras integrar nuevas escenas o modificar scripts en C#), verifica lo siguiente dentro del editor de Unity:

### Registro y orden en `Scene List`
1. Ve a **File > Build Profiles** (o abre `Scene List`).
2. Verifica que todas las escenas activas estén marcadas y respeten exactamente este orden de índices:
   * `0`: `Assets/Scenes/Menu.unity`
   * `1`: `Assets/Scenes/Nivel1.unity`
   * `2`: `Assets/Scenes/Nivel2.unity`
   * `3`: `Assets/Scenes/Nivel3.unity`
3. Si integraste una escena nueva desde otra rama de Git, haz clic en **Add Open Scenes** teniendo la escena abierta y arrástrala a su posición correspondiente.

### Verificación de referencias críticas
* Asegúrate de no tener errores rojos en la ventana **Console** antes de compilar.
* Comprueba que en los prefabs o canvas de retos (`ControladorReto`) estén asignadas las referencias visuales de transición (`panelBlanco`, `grupoInteractivo` y `ContenedorOpciones`).

---

## 2. Despliegue en Web (WebGL en Hostinger)

URL de producción actual: **[https://mechrobotix.com/heroedelcodigo/](https://mechrobotix.com/heroedelcodigo/)**

### Paso 1: Configurar el perfil Web en Unity 6.5
1. Abre **File > Build Profiles**.
2. Selecciona la plataforma **Web** y haz clic en **Switch Platform** (si aún no está activa).
3. Para evitar problemas de compatibilidad o errores de caché/configuración heredada, utiliza un perfil **Web** limpio y abre **Player Settings > Publishing Settings**:
   * **Compression Format:** `Disabled` *(evita errores de descompresión Brotli/Gzip en servidores compartidos sin cabeceras preconfiguradas)*.
   * **Decompression Fallback:** `Desmarcado` *(no es necesario cuando la compresión está desactivada)*.
   * **Name Files As Hashes:** `Desmarcado` *(facilita la identificación y reemplazo limpio de archivos en el servidor)*.

### Paso 2: Exportar la compilación WebGL
1. En **Build Profiles**, haz clic en **Build** (no *Build And Run* si solo deseas generar los archivos para subir).
2. Selecciona o crea una carpeta de salida limpia fuera de `Assets/` (por ejemplo: `C:\Builds\HeroeDelCodigo_Web\`).
3. Al finalizar, Unity generará tres elementos principales:
   * `index.html`
   * Carpeta `Build/` (contiene `.data`, `.framework.js`, `.loader.js` y `.wasm`)
   * Carpeta `TemplateData/` (estilos, iconos y barra de carga)

### Paso 3: Subir los archivos a Hostinger (hPanel)
1. Ingresa al **hPanel de Hostinger** y abre el **Administrador de Archivos** del dominio `mechrobotix.com`.
2. Navega hasta la carpeta destino: `/public_html/heroedelcodigo/`.
3. **Reemplazo seguro:** Para no dejar el sitio roto durante la subida, sube la nueva compilación a una carpeta temporal (por ejemplo `/public_html/heroedelcodigo_nuevo/`), verifica que cargue e intercambia los nombres de carpeta. Si subes directamente a `/public_html/heroedelcodigo/`, elimina antes `index.html`, `Build/` y `TemplateData/` para no mezclar versiones. *(Nota: **no borres** el archivo `.htaccess`; si usas carpeta temporal, cópialo también).*
4. Sube el nuevo `index.html`, la carpeta `Build/` y la carpeta `TemplateData/` generados por Unity.

### Paso 4: Configuración del servidor (`.htaccess`)
Para que el servidor de Hostinger entregue correctamente el binario de WebAssembly (`.wasm`) sin bloquear la ejecución en el navegador, verifica que dentro de `/public_html/heroedelcodigo/` exista un archivo llamado `.htaccess` con la siguiente directiva:

```apache
<IfModule mod_mime.c>
    AddType application/wasm .wasm
</IfModule>
```

*Si el archivo `.htaccess` no existe, créalo directamente desde el Administrador de Archivos de Hostinger y pega esa regla.*

**Control de caché (recomendado):** como *Name Files As Hashes* está desmarcado, los archivos de `Build/` conservan el mismo nombre entre versiones y el navegador podría servir un `.wasm` o `.data` antiguo. Si `mod_headers` está disponible en el hosting, agrega:

```apache
<IfModule mod_headers.c>
    <FilesMatch "\.(data|wasm|js)$">
        Header set Cache-Control "no-cache, must-revalidate"
    </FilesMatch>
</IfModule>
```

### Paso 5: Verificación y evasión de caché
1. Abre una ventana de incógnito o realiza una recarga forzada (`Ctrl + Shift + R` en Windows) en `https://mechrobotix.com/heroedelcodigo/`.
2. Abre las herramientas de desarrollador del navegador (`F12` > pestaña **Console** y **Network**) y confirma que el archivo `.wasm` responda con estado `200 OK` y tipo MIME `application/wasm`.

---

## 3. Despliegue en PC (Ejecutable para Windows)

### Paso 1: Configurar el perfil Windows en Unity 6.5
1. Abre **File > Build Profiles**.
2. Selecciona la plataforma **Windows** y haz clic en **Switch Platform**.
3. En **Player Settings > Resolution and Presentation**:
   * **Fullscreen Mode:** `Fullscreen Window` (recomendado para adaptarse automáticamente a distintas resoluciones de monitor) o `Windowed`.
   * **Resizable Window:** `Marcado` (si se usa modo ventana).
4. En **Player Settings > Other Settings**:
   * **Scripting Backend:** `Mono` (compilación rápida para pruebas y distribución estándar) o `IL2CPP` (mayor optimización y protección de código fuente).

### Paso 2: Generar el ejecutable (`.exe`)
1. Haz clic en **Build** y selecciona una carpeta vacía (por ejemplo: `C:\Builds\HeroeDelCodigo_Windows\`).
2. Unity generará la estructura distribuible completa:
   * `HeroeDelCodigo.exe`
   * `HeroeDelCodigo_Data/`
   * `UnityPlayer.dll`
   * `UnityCrashHandler64.exe`
   * Carpeta `MonoBleedingEdge/` (o `GameAssembly.dll` si se compiló con IL2CPP)

### Paso 3: Empaquetado y distribución
* **Regla crítica:** Nunca distribuyas el archivo `.exe` por separado; requiere exactamente las carpetas y librerías `.dll` generadas junto a él en el mismo directorio raíz.
* Para compartir o publicar una nueva versión de escritorio, comprime **todo el contenido de la carpeta de compilación** en un único archivo `.zip` (por ejemplo: `HeroeDelCodigo_Win64_v1.0.zip`).

---

## 4. Integración opcional en páginas web externas (`<iframe>`)

Si en el futuro **Mech Robotix, A.C.** desea incrustar el videojuego dentro de otra sección o página de su portal web principal sin redirigir al usuario fuera del sitio, utiliza el siguiente bloque HTML manteniendo la compilación alojada en `/public_html/heroedelcodigo/`:

```html
<div style="position: relative; width: 100%; max-width: 1280px; margin: 0 auto; aspect-ratio: 16 / 9;">
    <iframe 
        src="https://mechrobotix.com/heroedelcodigo/" 
        title="Héroe del Código - Mech Robotix, A.C."
        style="width: 100%; height: 100%; border: none; display: block;"
        allow="autoplay; fullscreen; gamepad"
        allowfullscreen>
    </iframe>
</div>
```

* **Nota de usabilidad:** Si se incrusta mediante `<iframe>`, el usuario debe hacer clic una vez dentro del área del juego para que el navegador enfoque el canvas y capture las teclas de movimiento (`A/D`, flechas, `Espacio` y `E`) junto con el audio.
