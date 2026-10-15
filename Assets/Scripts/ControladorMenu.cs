using System.Collections;
using UnityEngine;
using UnityEngine.UI;
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

    [Header("Audio")]
    public AudioSource fuenteAudio;
    public AudioClip sonidoClick;
    [Range(0f, 3f)] public float volumenClick = 1f;


    private int cartaActual = 0;

    private bool estaBloqueado = false;

    [Header("Modo Forzado")]
    public float tiempoBloqueoForzado = 3f;

    [Header("Progreso del jugador")]
    public static bool[] cartasDesbloqueadas = new bool[7];

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void ReiniciarDatosMenu()
    {
        
        cartasDesbloqueadas = new bool[7];
      //  cartasDesbloqueadas[0] = true; 
    }

    void Start()
    {
        panelMenu.anchoredPosition = posicionOculta;
        fondoOsculo.SetActive(false);

        for (int i = 0; i < botonesPestanas.Length; i++) {
            int indice = i;
            botonesPestanas[i].onClick.AddListener(() => CambiarCarta(indice));
        }
        CambiarCarta(0);
    }
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
    public void AbrirMenu() {
        ReproducirSonidoClick();
        fondoOsculo.SetActive(true);
        StartCoroutine(AnimarMenu(posicionOculta, posicionVisible));
        MA = true;

        bool tieneCarta = false;
        for (int i = 0; i < cartasDesbloqueadas.Length; i++) {
            if (cartasDesbloqueadas[i] == true) { 
                tieneCarta = true;
                break;
            }
        }

        if (contenedorCartas != null) { 
            contenedorCartas.SetActive(tieneCarta);
        }

        if (tieneCarta && !cartasDesbloqueadas[cartaActual]) {
            for (int i = 0; i < cartasDesbloqueadas.Length; i++) {
                if (cartasDesbloqueadas[i] == true) { 
                    CambiarCarta(i);
                    break;
                }
            }
        }
    }

    public void CerrarMenu() {
        ReproducirSonidoClick();
        StartCoroutine(AnimarMenu(posicionVisible, posicionOculta, true));
        MA = false;
    }

    private IEnumerator AnimarMenu(Vector2 inicio, Vector2 fin, bool apagando = false) {
        float tiempo = 0;
        panelMenu.anchoredPosition = inicio;

        while (tiempo < tiempoAnimacion) {
            tiempo += Time.deltaTime;
            panelMenu.anchoredPosition = Vector2.Lerp(inicio, fin, tiempo / tiempoAnimacion);
            yield return null;
        }

        panelMenu.anchoredPosition = fin;

        if (apagando) { 
            fondoOsculo.SetActive(false);
        }
    }
    public bool MA = false;
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q) && !estaBloqueado) {

            if (MA == false) {
                AbrirMenu();
            }
            else {
                CerrarMenu();
            }

        }
    }

    public void CambiarCarta(int indice) {

        if (!cartasDesbloqueadas[indice]) {
            return;
        }

        ReproducirSonidoClick();
        cartaActual = indice;

        imagenContenidoCentral.sprite = contenidoCartas[indice];
       // imagenContenidoCentral.SetNativeSize();

        if (scrollCartas != null) { 
            scrollCartas.verticalNormalizedPosition = 1f;
        }
        ActualizarSpritesBotones();
    }

    private void ActualizarSpritesBotones() {
        for (int i = 0; i < botonesPestanas.Length; i++) {
            if (!cartasDesbloqueadas[i])
            {
                botonesPestanas[i].GetComponent<Image>().sprite = spritesBloqueado[i];

            }
            else if (i == cartaActual)
            {
                botonesPestanas[i].GetComponent<Image>().sprite = spritesSeleccionado[i];
            }
            else {
                botonesPestanas[i].GetComponent<Image>().sprite = SpritesDisponible[i];
            }
        }
    }

    public void ReproducirSonidoClick()
    {
        if (fuenteAudio != null && sonidoClick != null)
        {
            fuenteAudio.PlayOneShot(sonidoClick, volumenClick);
        }
    }

    public void AbrirMenuForzado(int indice) {
        if (estaBloqueado) { return; }

        cartasDesbloqueadas[indice] = true;

        CambiarCarta(indice);
        AbrirMenu();
        MA = true;

        StartCoroutine(RutinaDeBloqueo(tiempoBloqueoForzado));
    }

    private IEnumerator RutinaDeBloqueo(float tiempo) {
        estaBloqueado = true;

        botonCerrar.interactable = false;
        for (int i = 0; i < botonesPestanas.Length; i++) { 
            botonesPestanas[i].interactable = false;
        }
        yield return new WaitForSeconds(tiempo);

        botonCerrar.interactable = true;
        for (int i = 0; i < botonesPestanas.Length; i++) { 
            botonesPestanas[i].interactable = true;
        }
        estaBloqueado = false;
    }

    public void AbrirMenuConRetraso(int indice, float tiempoDeRetraso)
    {
        StartCoroutine(RutinaAbrirConRetraso(indice, tiempoDeRetraso));
    }

    private IEnumerator RutinaAbrirConRetraso(int indice, float retraso)
    {
        yield return new WaitForSeconds(retraso); 
        AbrirMenuForzado(indice);                 
    }
}
