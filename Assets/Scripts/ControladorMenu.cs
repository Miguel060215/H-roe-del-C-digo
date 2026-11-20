using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Administra la interfaz del grimorio o menu de cartas coleccionables, controlando su animacion, navegacion por pestanas, reproduccion de audios y estado de desbloqueo.
/// </summary>
public class ControladorMenu : MonoBehaviour
{
    [Header("Componentes Visuales")]
    public GameObject fondoOsculo;
    public RectTransform panelMenu;
    public ScrollRect scrollCartas;

    [Header("Configuracion de animacion")]
    public float tiempoAnimacion = 0.5f;
    public Vector2 posicionOculta = new Vector2(0, -2000f);
    public Vector2 posicionVisible = new Vector2(0, 0);


    [Header("Referencias UI")]
    public GameObject contenedorCartas;
    public Image imagenContenidoCentral;
    public Button[] botonesPestanas;
    public Button botonCerrar;


    [Header("Sprites de estados")]
    public Sprite[] spritesBloqueado;
    public Sprite[] SpritesDisponible;
    public Sprite[] spritesSeleccionado;

    [Header("Base de datos de las cartas")]
    public Sprite[] contenidoCartas;

    [Header("Audio Menu")]
    public AudioSource fuenteAudio;
    public AudioClip sonidoClick;
    [Range(0f, 3f)] public float volumenClick = 1f;

    [Header("Audio de las cartas")]
    public AudioSource fuenteAudioCartas;
    public AudioClip[] audiosCartas;
    public Image iconoBtnAudio;
    public Sprite iconoPlay;
    public Sprite iconoPause;
    [Range(0f, 3f)] public float volumenCartas = 1f;


    private int cartaActual = 0;

    private bool estaBloqueado = false; // Impide cerrar el menu o cambiar de pestana mientras dura la presentacion forzada de una carta nueva

    [Header("Modo Forzado")]
    public float tiempoBloqueoForzado = 3f;

    [Header("Progreso del jugador")]
    public static bool[] cartasDesbloqueadas = new bool[7]; // Arreglo global que persiste entre escenas indicando que cartas ha obtenido el jugador

    /// <summary>
    /// Restablece el arreglo estatico de cartas desbloqueadas antes de cargar la primera escena al iniciar el juego.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void ReiniciarDatosMenu()
    {

        cartasDesbloqueadas = new bool[7];
        //  cartasDesbloqueadas[0] = true; 
    }

    /// <summary>
    /// Posiciona el menu fuera de pantalla, oculta el fondo oscuro y vincula dinamicamente los eventos de clic de cada pestana.
    /// </summary>
    void Start()
    {
        panelMenu.anchoredPosition = posicionOculta;
        fondoOsculo.SetActive(false);

        for (int i = 0; i < botonesPestanas.Length; i++)
        {
            int indice = i; // Se guarda una copia local del indice para evitar problemas de captura en la expresion lambda
            botonesPestanas[i].onClick.AddListener(() => CambiarCarta(indice));
        }

        // Actualiza el estado visual inicial de los botones y la vista previa sin reproducir audio al cargar la escena
        ActualizarSpritesBotones();
        if (cartasDesbloqueadas[0] && contenidoCartas.Length > 0)
        {
            imagenContenidoCentral.sprite = contenidoCartas[0];
            AjustarImagenAlAncho();
        }
    }

    /// <summary>
    /// Alterna entre abrir o cerrar el menu dependiendo de su estado actual.
    /// </summary>
    public void AbrirCerrarMenu()
    {
        if (MA == false)
        {
            AbrirMenu();
        }
        else
        {
            CerrarMenu();
        }
    }

    /// <summary>
    /// Despliega el menu en pantalla, verifica si hay cartas obtenidas para mostrar el contenedor y selecciona la primera disponible si la actual esta bloqueada.
    /// </summary>
    public void AbrirMenu()
    {
        ReproducirSonidoClick();
        fondoOsculo.SetActive(true);
        StartCoroutine(AnimarMenu(posicionOculta, posicionVisible));
        MA = true;

        bool tieneCarta = false;
        for (int i = 0; i < cartasDesbloqueadas.Length; i++)
        {
            if (cartasDesbloqueadas[i] == true)
            {
                tieneCarta = true;
                break;
            }
        }

        if (contenedorCartas != null)
        {
            contenedorCartas.SetActive(tieneCarta);
        }

        if (tieneCarta && !cartasDesbloqueadas[cartaActual])
        {
            for (int i = 0; i < cartasDesbloqueadas.Length; i++)
            {
                if (cartasDesbloqueadas[i] == true)
                {
                    CambiarCarta(i);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Oculta el menu deslizando el panel hacia abajo y detiene cualquier narracion de audio en reproduccion.
    /// </summary>
    public void CerrarMenu()
    {
        ReproducirSonidoClick();
        StartCoroutine(AnimarMenu(posicionVisible, posicionOculta, true));
        if (fuenteAudioCartas != null) fuenteAudioCartas.Stop();
        MA = false;
    }

    /// <summary>
    /// Interpola la posicion del panel entre dos coordenadas durante el tiempo configurado.
    /// </summary>
    private IEnumerator AnimarMenu(Vector2 inicio, Vector2 fin, bool apagando = false)
    {
        float tiempo = 0;
        panelMenu.anchoredPosition = inicio;

        while (tiempo < tiempoAnimacion)
        {
            tiempo += Time.deltaTime;
            panelMenu.anchoredPosition = Vector2.Lerp(inicio, fin, tiempo / tiempoAnimacion);
            yield return null;
        }

        panelMenu.anchoredPosition = fin;

        if (apagando)
        {
            fondoOsculo.SetActive(false);
        }
    }
    public bool MA = false; // Bandera que indica si el menu se encuentra abierto (true) o cerrado (false)

    /// <summary>
    /// Escucha la pulsacion de la tecla Q para abrir o cerrar el menu cuando no existe un bloqueo activo.
    /// </summary>
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q) && !estaBloqueado)
        {

            if (MA == false)
            {
                AbrirMenu();
            }
            else
            {
                CerrarMenu();
            }

        }
    }

    /// <summary>
    /// Actualiza el contenido visual, reinicia el desplazamiento del scroll y reproduce el audio asociado a la carta seleccionada.
    /// </summary>
    public void CambiarCarta(int indice)
    {

        if (!cartasDesbloqueadas[indice])
        {
            return;
        }

        ReproducirSonidoClick();
        cartaActual = indice;

        imagenContenidoCentral.sprite = contenidoCartas[indice];
        AjustarImagenAlAncho();
        //imagenContenidoCentral.SetNativeSize();

        if (scrollCartas != null)
        {
            // Posiciona la barra de desplazamiento vertical en el extremo superior (1f)
            scrollCartas.verticalNormalizedPosition = 1f;
        }
        ActualizarSpritesBotones();

        if (fuenteAudioCartas != null && audiosCartas.Length > indice && audiosCartas[indice] != null)
        {
            fuenteAudioCartas.clip = audiosCartas[indice];
            fuenteAudioCartas.volume = volumenCartas;
            fuenteAudioCartas.Play();
            if (iconoBtnAudio != null)
            {
                iconoBtnAudio.sprite = iconoPause;
            }
        }
        else
        {
            if (fuenteAudioCartas != null)
            {
                fuenteAudioCartas.Stop();
                fuenteAudioCartas.clip = null;
            }
            if (iconoBtnAudio != null) iconoBtnAudio.sprite = iconoPlay;
        }
    }

    /// <summary>
    /// Redimensiona la altura del RectTransform de la imagen central para mantener su relacion de aspecto respecto al ancho del contenedor.
    /// </summary>
    private void AjustarImagenAlAncho()
    {
        Sprite sprite = imagenContenidoCentral.sprite;
        if (sprite == null) return;

        RectTransform rt = imagenContenidoCentral.rectTransform;

        RectTransform padre = rt.parent as RectTransform;
        float anchoDisponible = padre.rect.width;

        // Calcula el alto proporcional respetando la relacion de aspecto original del sprite
        float proporcion = sprite.rect.height / sprite.rect.width;

        rt.sizeDelta = new Vector2(0f, anchoDisponible * proporcion);
    }

    /// <summary>
    /// Actualiza el sprite de cada boton de pestana segun si la carta esta bloqueada, disponible o actualmente seleccionada.
    /// </summary>
    private void ActualizarSpritesBotones()
    {
        for (int i = 0; i < botonesPestanas.Length; i++)
        {
            if (!cartasDesbloqueadas[i])
            {
                botonesPestanas[i].GetComponent<Image>().sprite = spritesBloqueado[i];

            }
            else if (i == cartaActual)
            {
                botonesPestanas[i].GetComponent<Image>().sprite = spritesSeleccionado[i];
            }
            else
            {
                botonesPestanas[i].GetComponent<Image>().sprite = SpritesDisponible[i];
            }
        }
    }

    /// <summary>
    /// Reproduce el efecto de sonido de interaccion de botones si los recursos de audio estan asignados.
    /// </summary>
    public void ReproducirSonidoClick()
    {
        if (fuenteAudio != null && sonidoClick != null)
        {
            fuenteAudio.PlayOneShot(sonidoClick, volumenClick);
        }
    }

    /// <summary>
    /// Desbloquea una carta especifica, abre el menu mostrando su contenido y bloquea los controles temporalmente.
    /// </summary>
    public void AbrirMenuForzado(int indice)
    {
        if (estaBloqueado) { return; }

        cartasDesbloqueadas[indice] = true;

        CambiarCarta(indice);
        AbrirMenu();
        MA = true;

        StartCoroutine(RutinaDeBloqueo(tiempoBloqueoForzado));
    }

    /// <summary>
    /// Deshabilita la interaccion con los botones del menu durante los segundos especificados para evitar cierres accidentales.
    /// </summary>
    private IEnumerator RutinaDeBloqueo(float tiempo)
    {
        estaBloqueado = true;

        botonCerrar.interactable = false;
        for (int i = 0; i < botonesPestanas.Length; i++)
        {
            botonesPestanas[i].interactable = false;
        }
        yield return new WaitForSeconds(tiempo);

        botonCerrar.interactable = true;
        for (int i = 0; i < botonesPestanas.Length; i++)
        {
            botonesPestanas[i].interactable = true;
        }
        estaBloqueado = false;
    }

    /// <summary>
    /// Pausa o reanuda la narracion de audio de la carta actual y actualiza el icono del boton correspondiente.
    /// </summary>
    public void AlternarAudioCarta()
    {
        if (fuenteAudioCartas == null || fuenteAudioCartas.clip == null) return;

        if (fuenteAudioCartas.isPlaying)
        {
            fuenteAudioCartas.Pause();
            if (iconoBtnAudio != null)
            {
                iconoBtnAudio.sprite = iconoPlay;
            }
        }
        else
        {
            fuenteAudioCartas.Play();
            if (iconoBtnAudio != null)
            {
                iconoBtnAudio.sprite = iconoPause;
            }
        }
    }

    /// <summary>
    /// Inicia una corrutina para desbloquear y mostrar una carta tras un tiempo de espera determinado.
    /// </summary>
    public void AbrirMenuConRetraso(int indice, float tiempoDeRetraso)
    {
        StartCoroutine(RutinaAbrirConRetraso(indice, tiempoDeRetraso));
    }

    /// <summary>
    /// Espera los segundos indicados antes de ejecutar la apertura forzada del menu en la carta correspondiente.
    /// </summary>
    private IEnumerator RutinaAbrirConRetraso(int indice, float retraso)
    {
        yield return new WaitForSeconds(retraso);
        AbrirMenuForzado(indice);
    }
}