using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Gestiona el traslado del jugador hacia un punto de destino especifico mediante una transicion visual de oscurecimiento y aclarado de pantalla.
/// </summary>
public class Teletransportador : MonoBehaviour
{
    [Header("Configuración de Destino")]
    public Transform puntoDestino;

    [Header("Transición")]
    public Image imagenOscura;
    public float duracionFade = 10f;

    private bool enTransicion = false; // Bandera que evita reiniciar la corrutina si el jugador permanece dentro del area durante el teletransporte

    /// <summary>
    /// Detecta el ingreso del jugador al area de activacion e inicia la rutina de traslado si no hay una transicion en curso.
    /// </summary>
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

    /// <summary>
    /// Ejecuta la secuencia completa de teletransporte: inmoviliza al jugador, oscurece la pantalla, reubica al personaje junto con la camara y aclara la imagen.
    /// </summary>
    private IEnumerator RutinaTeletransporte(Transform jugadorTransform, Player jugadorScript)
    {
        enTransicion = true;

        // Calcula el tiempo total de bloqueo sumando las fases de oscurecimiento, aclarado y un margen de seguridad
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

        // Traslada al personaje y actualiza su punto seguro para evitar que una trampa posterior lo devuelva a la zona anterior
        jugadorTransform.position = puntoDestino.position;
        jugadorScript.ultimaPosicionSegura = puntoDestino.position;

        if (Camera.main != null)
        {
            // Reposiciona la camara instantaneamente conservando su coordenada Z original para evitar el barrido visual a traves del nivel
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