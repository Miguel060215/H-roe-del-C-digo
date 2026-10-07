using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Teletransportador : MonoBehaviour
{
    [Header("Configuración de Destino")]
    public Transform puntoDestino;

    [Header("Transición")]
    public Image imagenOscura;
    public float duracionFade = 10f;

    private bool enTransicion = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !enTransicion)
        {
            Player jugadorScript = collision.GetComponent<Player>();
            if (jugadorScript != null)
            {
                StartCoroutine(RutinaTeletransporte(collision.transform, jugadorScript));
            }
        }
    }

    private IEnumerator RutinaTeletransporte(Transform jugadorTransform, Player jugadorScript)
    {
        enTransicion = true;

        jugadorScript.BloquarControles((duracionFade * 2f) + 0.2f);

        Rigidbody2D rb = jugadorTransform.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (imagenOscura != null)
        {
            float tiempo = 0;
            while (tiempo < duracionFade)
            {
                tiempo += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, 1f, tiempo / duracionFade);
                imagenOscura.color = new Color(0f, 0f, 0f, alpha);
                yield return null;
            }
            imagenOscura.color = new Color(0f, 0f, 0f, 1f); 
        }

        yield return new WaitForSeconds(0.7f); 

        jugadorTransform.position = puntoDestino.position;
        jugadorScript.ultimaPosicionSegura = puntoDestino.position;

        if (Camera.main != null)
        {
            Camera.main.transform.position = new Vector3(puntoDestino.position.x, puntoDestino.position.y, Camera.main.transform.position.z);
        }

        yield return new WaitForSeconds(0.3f);

        if (imagenOscura != null)
        {
            float tiempo = 0;
            while (tiempo < duracionFade)
            {
                tiempo += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, tiempo / duracionFade);
                imagenOscura.color = new Color(0f, 0f, 0f, alpha);
                yield return null;
            }
            imagenOscura.color = new Color(0f, 0f, 0f, 0f);
        }

        enTransicion = false;
    }
}