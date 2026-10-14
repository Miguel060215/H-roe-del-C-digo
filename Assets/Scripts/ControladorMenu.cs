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

        if (Input.GetKeyDown(KeyCode.Q)) {

            if (MA == false) {
                AbrirMenu();
            }
            else {
                CerrarMenu();
            }

        }
    }

    public void CambiarCarta(int indice) {
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
            if (i == cartaActual)
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

}
