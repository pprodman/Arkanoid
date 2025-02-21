using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuController : MonoBehaviour
{
    [SerializeField] private Button playGame; // Referencia al botón "Jugar"
    [SerializeField] private Button exitGame; // Referencia al botón "Salir"
    [SerializeField] private AudioClip startGame; // Sonido al hacer clic en playGame
    private AudioSource audioSource;

    private void Awake()
    {
        // Asigna la acción al botón "Jugar"
        if (playGame != null)
        {
            playGame.onClick.AddListener(() =>
            {
                Debug.Log("Iniciando el juego...");
                StartCoroutine(LoadLevelWithDelay());
            });
        }
        else
        {
            Debug.LogError("El botón Jugar no está asignado en el Inspector.");
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
            Debug.LogError("AudioSource no encontrado en el MenuController.");
        }

        // Reproduce la música del Main Menu
        MusicManager.Instance.PlayMainMenuMusic();


    }

    // Corrutina para cargar el nivel con un retraso
    private IEnumerator LoadLevelWithDelay()
    {
        Debug.Log("Cargando nivel con retraso...");
        GameManager.Instance.ResetGame(); // Reinicia el juego antes de cargar el nivel
        MusicManager.Instance.StopMusic();
        PlaySound(startGame);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Level1");
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