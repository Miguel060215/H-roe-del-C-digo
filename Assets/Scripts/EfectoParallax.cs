using UnityEngine;

/// <summary>
/// Aplica un efecto de desplazamiento parallax horizontal continuo e infinito a una capa de fondo en relacion con el movimiento de la camara.
/// </summary>
public class EfectoParallax : MonoBehaviour
{
    [SerializeField] private float parallaxEffectMultiplier; // Factor que determina la velocidad relativa del fondo respecto a la camara (valores cercanos a 1 simulan mayor lejania)
    private Transform cameraTransform;
    private Vector3 previousCameraPosition;
    private float spriteWidth; // Ancho horizontal del sprite en unidades de mundo, utilizado como umbral para reposicionar el fondo
    private float startPositionX;

    /// <summary>
    /// Obtiene la referencia a la camara principal, calcula el ancho real del sprite y registra las posiciones iniciales.
    /// </summary>
    void Start()
    {
        cameraTransform = Camera.main.transform;
        previousCameraPosition = cameraTransform.position;
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
        startPositionX = transform.position.x;
    }

    /// <summary>
    /// Desplaza el fondo despues de que la camara actualiza su posicion y lo reposiciona ciclicamente para mantener un scroll infinito.
    /// </summary>
    void LateUpdate()
    {
        float deltaX = (cameraTransform.position.x - previousCameraPosition.x) * parallaxEffectMultiplier;
        // Calcula el desplazamiento relativo acumulado de la camara respecto al movimiento de la capa para detectar el limite del sprite
        float moveAmount = cameraTransform.position.x * (1 - parallaxEffectMultiplier);
        transform.Translate(new Vector3(deltaX, 0, 0));
        previousCameraPosition = cameraTransform.position;

        if (moveAmount > startPositionX + spriteWidth)
        {
            transform.Translate(new Vector3(spriteWidth, 0, 0));
            startPositionX += spriteWidth;
        }
        else if (moveAmount < startPositionX - spriteWidth)
        {
            transform.Translate(new Vector3(-spriteWidth, 0, 0));
            startPositionX -= spriteWidth;
        }
    }
}