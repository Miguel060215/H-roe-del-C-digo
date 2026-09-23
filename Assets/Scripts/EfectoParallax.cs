using UnityEngine;

public class EfectoParallax : MonoBehaviour
{
    [SerializeField] private float parallaxEffectMultiplier;
    private Transform cameraTransform;
    private Vector3 previousCameraPosition;
    private float spriteWidth;
    private float startPositionX;


    void Start()
    {
        cameraTransform = Camera.main.transform;
        previousCameraPosition = cameraTransform.position;
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;//guado el ancho del sprite para poder hacer scroll infinito ;)
        startPositionX = transform.position.x;
    }

  
    void LateUpdate()
    {
        float deltaX = (cameraTransform.position.x - previousCameraPosition.x)*parallaxEffectMultiplier;
        float moveAmount = cameraTransform.position.x * (1 - parallaxEffectMultiplier);
        transform.Translate(new Vector3(deltaX,0,0));
        previousCameraPosition = cameraTransform.position;

        if (moveAmount > startPositionX + spriteWidth)
        {
            transform.Translate(new Vector3(spriteWidth, 0, 0));
            startPositionX += spriteWidth;
        }
        else if (moveAmount < startPositionX - spriteWidth) { 
            transform.Translate(new Vector3(-spriteWidth, 0, 0));
            startPositionX -= spriteWidth;
        }
    }
}
