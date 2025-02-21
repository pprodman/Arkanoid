using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private Button restartGame; // Referencia al botón "Reiniciar"
    [SerializeField] private Button exitGame; // Referencia al botón "Salir"
    [SerializeField] private AudioClip startGame; // Sonido al hacer clic en reiniciar
    private AudioSource audioSource;

    private void Awake()
    {
        // Asigna la acción al botón "Reiniciar"
        if (restartGame != null)
        {
            restartGame.onClick.AddListener(() =>
            {
                Debug.Log("Reiniciando el juego...");
                GameManager.Instance.ResetGame(); // Reinicia las estadísticas del juego
                StartCoroutine(LoadLevelWithDelay());
            });
        }
        else
        {
            Debug.LogError("El botón Reiniciar no está asignado en el Inspector.");
        }

        // Asigna la acción al botón "Salir"
        if (exitGame != null)
        {
            exitGame.onClick.AddListener(() =>
            {
                Debug.Log("Saliendo del juego...");
                Application.Quit(); // Cierra la aplicación (funciona en builds, no en el editor)
            });
        }
        else
        {
            Debug.LogError("El botón Salir no está asignado en el Inspector.");
        }
    }

    private void Start()
    {
        // Obtiene el componente AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource no encontrado en el GameOverManager.");
        }

        // Inicia la música con un retraso
        StartCoroutine(PlayMusicWithDelay(3f)); // 3 segundos de retraso
    }

    // Corrutina para cargar el nivel con un retraso
    private IEnumerator LoadLevelWithDelay()
    {
        Debug.Log("Cargando nivel con retraso...");
        GameManager.Instance.ResetGame(); // Reinicia el juego antes de cargar el nivel
        MusicManager.Instance.StopMusic();
        PlaySound(startGame); // Reproduce el sonido del botón
        yield return new WaitForSeconds(1f); // Espera 1 segundo
        SceneManager.LoadScene("Level1"); // Carga Level1
    }

    // Corrutina para reproducir la música con un retraso
    private IEnumerator PlayMusicWithDelay(float delay)
    {
        Debug.Log("Esperando para reproducir la música...");
        yield return new WaitForSeconds(delay); // Espera el tiempo especificado
        MusicManager.Instance.PlayMainMenuMusic(); // Reproduce la música del menú principal
        Debug.Log("Música del menú principal iniciada.");
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