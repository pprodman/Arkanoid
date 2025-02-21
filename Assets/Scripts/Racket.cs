using System.Collections;
using UnityEngine;

public class Racket : MonoBehaviour
{
    public float speed = 150; // Velocidad de movimiento de la raqueta
    private float originalSpeed; // Velocidad original de la raqueta
    private Vector3 originalScale; // Escala original de la raqueta
    private float minX = -83f; // Límite izquierdo de la pantalla
    private float maxX = 84f; // Límite derecho de la pantalla
    public AudioClip bigRacketSound; // Sonido cuando se agranda la raqueta
    public AudioClip lifeUpSound; // Sonido al recoger una vida extra
    public AudioClip bonusPick; // Sonido al recoger un bonus
    private AudioSource audioSource;

    private void Start()
    {
        // Guarda la escala y velocidad originales de la raqueta al iniciar el juego
        originalScale = transform.localScale;
        originalSpeed = speed;

        // Obtiene el componente AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource no encontrado en la raqueta.");
        }
    }

    private void Update()
    {
        // Mueve la raqueta horizontalmente
        float h = Input.GetAxis("Horizontal");

        // Calcula la nueva posición de la raqueta
        Vector3 newPosition = transform.position + Vector3.right * h * speed * Time.deltaTime;

        // Restringe la posición dentro de los límites
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);

        // Aplica la nueva posición
        transform.position = newPosition;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si la raqueta colisiona con un bonus, aplica su efecto
        if (other.CompareTag("Bonus"))
        {
            Bonus bonus = other.GetComponent<Bonus>();
            if (bonus != null)
            {
                ApplyPowerUp(bonus.type); // Pasa el tipo de bonus
                ObjectPoolManager.Instance.ReturnToPool(other.gameObject); // Devuelve el bonus al pool
            }
        }
    }

    // Método para aplicar el efecto de un power-up
    private void ApplyPowerUp(BonusType type)
    {
        switch (type)
        {
            case BonusType.BigRacket:
                PlaySound(bigRacketSound);
                StartCoroutine(ApplyBigRacketEffect());
                break;

            case BonusType.MultiBall:
                PlaySound(bonusPick);
                SpawnExtraBalls();
                Debug.Log("Multi Ball activated!");
                break;

            case BonusType.SlowBall:
                PlaySound(bonusPick);
                SlowDownBalls();
                Debug.Log("Slow Ball activated!");
                break;
            case BonusType.SlowRacket:
                PlaySound(bonusPick);
                StartCoroutine(ApplySlowRacketEffect());
                Debug.Log("Slow Racket activated!");
                break;
            case BonusType.FastRacket:
                PlaySound(bonusPick);
                StartCoroutine(ApplyFastRacketEffect());
                Debug.Log("Fast Racket activated!");
                break;
            case BonusType.ExtraLife:
                PlaySound(lifeUpSound);
                GameManager.Instance.lives++;
                UIManager.Instance.UpdateLives(GameManager.Instance.lives);
                Debug.Log("Extra Life activated!");
                break;

            default:
                Debug.LogWarning("Unknown bonus type!");
                break;
        }
    }

    // Método para generar bolas adicionales
    private void SpawnExtraBalls()
    {
        for (int i = 0; i < 2; i++) // Genera 2 bolas adicionales
        {
            GameObject ballPrefab = ObjectPoolManager.Instance.GetPooledBall();
            if (ballPrefab != null)
            {
                Ball ball = ballPrefab.GetComponent<Ball>();
                if (ball != null)
                {
                    ballPrefab.transform.position = transform.position + new Vector3(Random.Range(-1f, 1f), 1, 0);
                    ballPrefab.SetActive(true); // Activa la bola

                    // Registra la nueva bola en el GameManager
                    //GameManager.Instance.RegisterBall(); // Registra la nueva bola
                }
            }
        }
    }

    // Corrutina para aplicar el efecto de BigRacket durante 10 segundos
    private IEnumerator ApplyBigRacketEffect()
    {
        // Aumenta la escala horizontal de la raqueta
        transform.localScale = new Vector3(originalScale.x * 1.5f, originalScale.y, originalScale.z);

        Debug.Log("Big Racket activated!");

        yield return new WaitForSeconds(10f); // Espera 10 segundos

        // Restaura la escala original de la raqueta
        transform.localScale = originalScale;

        Debug.Log("Big Racket effect ended.");
    }

    // Método para reducir la velocidad de las bolas y restaurarla después de 10 segundos
    public void SlowDownBalls()
    {
        StartCoroutine(SlowDownAndRestoreBalls());
    }

    // Corrutina para reducir la velocidad de las bolas y restaurarla después de 10 segundos
    private IEnumerator SlowDownAndRestoreBalls()
    {
        Ball[] balls = FindObjectsByType<Ball>(FindObjectsSortMode.None);
        float originalSpeed = balls[0].GetInitialSpeed(); // Guarda la velocidad original de las bolas

        // Reduce la velocidad de todas las bolas
        foreach (Ball ball in balls)
        {
            ball.SlowDown(0.5f); // Reduce la velocidad a la mitad
        }

        Debug.Log("Velocidad de las bolas reducida.");

        // Espera 10 segundos
        yield return new WaitForSeconds(10f);

        // Restaura la velocidad original de todas las bolas
        foreach (Ball ball in balls)
        {
            ball.RestoreSpeed(originalSpeed); // Restaura la velocidad original
        }

        Debug.Log("Velocidad de las bolas restaurada.");
    }

    // Corrutina para aplicar el efecto de reducción de velocidad de la raqueta durante 10 segundos
    private IEnumerator ApplySlowRacketEffect()
    {
        // Reduce la velocidad de la raqueta a la mitad
        speed = originalSpeed * 0.5f;
        Debug.Log("Velocidad de la raqueta reducida.");

        yield return new WaitForSeconds(10f); // Espera 10 segundos

        // Restaura la velocidad original de la raqueta
        speed = originalSpeed;
        Debug.Log("Velocidad de la raqueta restaurada.");
    }

    // Corrutina para aplicar el efecto de reducción de velocidad de la raqueta durante 10 segundos
    private IEnumerator ApplyFastRacketEffect()
    {
        // Aumenta la velocidad de la raqueta a la mitad
        speed = originalSpeed * 2f;
        Debug.Log("Velocidad de la raqueta aumentada.");

        yield return new WaitForSeconds(10f); // Espera 10 segundos

        // Restaura la velocidad original de la raqueta
        speed = originalSpeed;
        Debug.Log("Velocidad de la raqueta restaurada.");
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