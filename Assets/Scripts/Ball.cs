using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour
{
    public float initialSpeed = 100.0f;
    public float speedIncreaseInterval = 5.0f;
    public float speedIncreasePercentage = 0.1f;
    private float speed; // Velocidad actual de la bola
    public float maxSpeed = 500.0f;
    private Rigidbody2D rb;
    private bool hasFallen = false; // Bandera para evitar múltiples llamadas
    public AudioClip racketHitSound; // Sonido cuando la pelota golpea la raqueta
    public AudioClip obstacleHitSound; // Sonido al golpear obstaculo
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D no encontrado en la bola.");
            return;
        }

        // Obtiene el componente AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource no encontrado en la bola.");
        }

        speed = initialSpeed; // Reinicia la velocidad al valor inicial al cargar la escena
        
        // Desactiva la gravedad y detiene cualquier movimiento inicial
        rb.gravityScale = 0; // Quita la gravedad
        rb.linearVelocity = Vector2.zero; // Detiene cualquier movimiento
        
       
        GameManager.Instance.RegisterBall(); // Registra la bola en el GameManager

        StartCoroutine(IncreaseSpeedOverTime()); // Inicia la corrutina para aumentar la velocidad con el tiempo
        StartCoroutine(DelayedLaunch()); // Pequeña pausa antes de lanzar la bola
    }

    IEnumerator IncreaseSpeedOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(speedIncreaseInterval); // Espera el tiempo configurado
            speed = Mathf.Min(speed * (1 + speedIncreasePercentage), maxSpeed);
            rb.linearVelocity = rb.linearVelocity.normalized * speed; // Ajusta la velocidad manteniendo la dirección
        }
    }

    IEnumerator DelayedLaunch()
    {
        yield return new WaitForSeconds(1f); // Pausa de 1 segundos
        rb.gravityScale = 1; // Restaura la gravedad
        rb.linearVelocity = Vector2.up * speed; // Reinicia la velocidad hacia arriba
    }

    private void Update()
    {
        // Verifica si la bola ha caído por debajo de un límite y no ha sido desactivada antes
        if (transform.position.y < -105f && !hasFallen) // Umbral de caída
        {
            hasFallen = true; // Marca la bola como caída
            Debug.Log("Ball fell below the racket!");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.UnregisterBall(); // Notifica al GameManager que esta bola ha caído
            }
            else
            {
                Debug.LogError("GameManager.Instance es nulo.");
            }
            gameObject.SetActive(false); // Desactiva la bola
        }
    }

    // Método público para reducir la velocidad
    public void SlowDown(float factor)
    {
        speed *= factor;
        GetComponent<Rigidbody2D>().linearVelocity = GetComponent<Rigidbody2D>().linearVelocity.normalized * speed;
    }

    // Método para restaurar la velocidad
    public void RestoreSpeed(float newSpeed)
    {
        speed = newSpeed; // Restaura la velocidad
        rb.linearVelocity = rb.linearVelocity.normalized * speed; // Ajusta la velocidad manteniendo la dirección
    }

    // Método para obtener la velocidad inicial
    public float GetInitialSpeed()
    {
        return initialSpeed;
    }


    // Método para calcular el factor de golpeo
    float hitFactor(Vector2 ballPos, Vector2 racketPos, float racketWidth)
    {
        // Normaliza la posición relativa de la bola respecto a la raqueta
        return (ballPos.x - racketPos.x) / racketWidth;
    }

    // Detecta colisiones
    private void OnCollisionEnter2D(Collision2D col)
    {
        // ¿Chocó con un obstáculo indestructible?
        if (col.gameObject.CompareTag("Obstacle"))
        {
            PlaySound(obstacleHitSound); // Reproduce el sonido del obstáculo
        }
        // ¿Chocó con la raqueta?
        if (col.gameObject.CompareTag("Racket"))
        {
            PlaySound(racketHitSound); // Reproduce el sonido de la raqueta
            // Calcula el factor de golpeo basado en la posición relativa entre la bola y la raqueta
            float x = hitFactor(transform.position, col.transform.position, col.collider.bounds.size.x);

            // Calcula la nueva dirección de la bola
            Vector2 dir = new Vector2(x, 1).normalized;

            // Aplica la nueva velocidad a la bola
            rb.linearVelocity = dir * speed;
        }
    }

    // Método para reiniciar la posición de la bola
    public void ResetPosition()
    {
        hasFallen = false; // Reinicia la bandera al reactivar la bola
        gameObject.SetActive(true); // Activa el objeto de la bola antes de reiniciar su posición
        transform.position = new Vector2(0, -35f);  // Restaura la posición inicial
        rb.linearVelocity = Vector2.zero; // Detiene cualquier movimiento

        StartCoroutine(BlinkAndLaunch()); // Inicia la corrutina de parpadeo y lanzamiento
    }

    IEnumerator BlinkAndLaunch()
    {
        float blinkDuration = 3f; // Duración total del parpadeo (3 segundos)
        float blinkInterval = 0.2f; // Intervalo de tiempo entre cada parpadeo (0.2 segundos)
        float elapsedTime = 0f; // Tiempo transcurrido

        Renderer ballRenderer = GetComponent<Renderer>(); // Obtén el componente Renderer de la bola
        rb.simulated = false; // Desactiva el Rigidbody2D para evitar movimientos no deseados

        // Parpadeo durante 3 segundos
        while (elapsedTime < blinkDuration)
        {
            ballRenderer.enabled = !ballRenderer.enabled; // Alterna la visibilidad de la bola
            yield return new WaitForSeconds(blinkInterval); // Espera el intervalo de parpadeo
            elapsedTime += blinkInterval; // Actualiza el tiempo transcurrido
        }

        // Asegúrate de que la bola esté visible al finalizar el parpadeo
        ballRenderer.enabled = true;
        rb.simulated = true; // Reactiva el Rigidbody2D

        // Lanza la bola después del parpadeo
        rb.linearVelocity = Vector2.up * speed; // Reinicia la velocidad hacia arriba
    }

    // Método para reproducir un sonido
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip); // Reproduce el sonido una vez
        }
    }
}