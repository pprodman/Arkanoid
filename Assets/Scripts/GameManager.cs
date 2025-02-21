using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Instancia única del GameManager (Singleton)
    public int score; // Puntuación actual del jugador
    private int highScore; // Variable para almacenar la puntuación máxima
    public int lives = 3; // Vidas restantes del jugador
    public int currentLevel = 1; // Nivel actual del juego
    private int maxLevels = 5; // Nivel maximo
    private int activeBalls = 0; // Contador de bolas activas en la escena
    public AudioClip lifeLostSound; // Sonido cuando el jugador pierde una vida
    public AudioClip winRound; // Sonido cuando completa ronda
    public AudioClip gameOverSound; // Sonido de Game Over
    private AudioSource audioSource;


    private void Awake()
{
    // Singleton: Asegura que solo haya una instancia del GameManager
    if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persiste entre escenas
    }
    else
    {
        Destroy(gameObject); // Destruye duplicados
    }
}

    private void Start()
    {
        UIManager.Instance.InitializeLives(lives); // Inicializa los iconos de las vidas
        highScore = PlayerPrefs.GetInt("HighScore", 0); // Carga el High Score
        UIManager.Instance.UpdateHighScore(highScore); // Actualiza la UI con el HighScore cargado
        UIManager.Instance.UpdateRound(currentLevel); // Muestra el nivel inicial
        
        // Obtiene el componente AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource no encontrado en el GameManager.");
        }
    }

    // Método para avanzar al siguiente nivel con un retraso
    public IEnumerator StartLevelWithDelay()
    {
        Debug.Log("Iniciando el nivel con un retraso...");
        yield return new WaitForSeconds(1f); // Espera 1 segundos antes de activar el juego
        Debug.Log("Nivel cargado. ¡A jugar!");

        // Activa la bola después del retraso
        ResetBall();
    }

    // Método para registrar una bola activa
    public void RegisterBall()
    {
        activeBalls++;
        Debug.Log("Bola registrada. Bolas activas: " + activeBalls);
    }

    // Método para desregistrar una bola cuando cae
    public void UnregisterBall()
    {
        activeBalls--;
        Debug.Log("Bola desregistrada. Bolas activas: " + activeBalls);

        // Si no quedan bolas activas, el jugador pierde una vida
        if (activeBalls <= 0)
        {
            Debug.Log("No quedan bolas activas. Perdiendo una vida...");
            LoseLife();
        }
    }

    // Método para reducir las vidas del jugador
    public void LoseLife()
    {
        lives--; // Reduce una vida
        UIManager.Instance.UpdateLives(lives); // Actualiza las vidas en la interfaz gráfica
        PlaySound(lifeLostSound); // Reproduce el sonido de perder una vida

        if (lives <= 0) // Si no quedan vidas, termina el juego
        {
            GameOver();
        }
        else
        {
            ResetBall(); // Reinicia la bola si aún quedan vidas
        }
    }

    // Método para manejar el fin del juego
    public void GameOver()
    {
        PlaySound(gameOverSound);
        ClearActiveBalls();
        UpdateHighScore(score); // Actualiza la puntuación máxima antes de cargar la escena de Game Over
        SceneManager.LoadScene("GameOver"); // Carga la escena de Game Over
    }

    // Método para avanzar al siguiente nivel
    public void NextLevel()
    {
        if (currentLevel > maxLevels)
        {
            Debug.Log("¡Has completado todos los niveles!");
            ClearActiveBalls();
            ResetGame(); // Reinicia el juego al completar todos los niveles
            SceneManager.LoadScene("GameOver"); // Finaliza juego
        }
        else
        {
            PlaySound(winRound);
            currentLevel++;
            Debug.Log("Avanzando al nivel " + currentLevel);
            UIManager.Instance.UpdateRound(currentLevel); // Actualiza el nivel en la UI
            ClearActiveBalls();
            StartCoroutine(LoadLevelWithDelay());

        }
    }

    // Corrutina para cargar el siguiente nivel con un retraso
    private IEnumerator LoadLevelWithDelay()
    {
        Debug.Log("Cargando el siguiente nivel...");
        yield return new WaitForSeconds(1f); // Espera 1 segundo antes de cargar la escena
        SceneManager.LoadScene("Level" + currentLevel);
    }

    // Método para reiniciar la posición de la bola
    private void ResetBall()
    {
        // Obtiene una bola del pool
        GameObject ballPrefab = ObjectPoolManager.Instance.GetPooledBall();
        if (ballPrefab != null)
        {
            Ball ball = ballPrefab.GetComponent<Ball>();
            if (ball != null)
            {
                ball.ResetPosition(); // Llama al método de la bola para reiniciar su posición
                RegisterBall(); // Registra la nueva bola
            }
        }
        else
        {
            Debug.LogError("No se pudo obtener una bola del pool.");
        }
    }

    public void AddScore(int points)
    {
        score += points; // Incrementa la puntuación
        Debug.Log("Score: " + score); // Muestra la puntuación en la consola
        UIManager.Instance.UpdateScore(score); // Actualiza la puntuación en la interfaz gráfica
    }

    // Método para obtener la puntuación máxima
    public int GetHighScore()
    {
        return highScore;
    }

    // Método para actualizar la puntuación máxima
    public void UpdateHighScore(int newScore)
    {
        if (newScore > highScore)
        {
            highScore = newScore; // Actualiza la puntuación máxima si es mayor
            UIManager.Instance.UpdateHighScore(highScore); // Actualiza la UI

            // Guardar el nuevo HighScore
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save(); // Guarda los cambios en PlayerPrefs
        }
    }

    public void ClearActiveBalls()
    {
        Ball[] activeBallsArray = Resources.FindObjectsOfTypeAll<Ball>();
        foreach (Ball ball in activeBallsArray)
        {
            if (ball != null && ball.gameObject.scene.isLoaded)
            {
                ObjectPoolManager.Instance.ReturnToPool(ball.gameObject);
            }
        }

        ObjectPoolManager.Instance.ResetPool(); // Reinicia el pool
        activeBalls = 0; // Restablece el contador de bolas activas
        Debug.Log("Pool de bolas reiniciado. Contador de bolas reiniciado a 0.");
    }

    public void ResetGame()
    {
        // Reinicia las estadísticas del juego
        score = 0; // Reinicia la puntuación
        lives = 3; // Reinicia las vidas
        currentLevel = 1; // Reinicia el nivel
        activeBalls = 0; // Reinicia el contador de bolas activas

        // Actualiza la interfaz gráfica
        UIManager.Instance.UpdateScore(score); // Actualiza la puntuación en la UI
        UIManager.Instance.UpdateLives(lives); // Actualiza las vidas en la UI
        UIManager.Instance.UpdateRound(currentLevel); // Actualiza el nivel en la UI

        Debug.Log("Juego reiniciado: Score=0, Lives=3, Level=1, ActiveBalls=0");
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