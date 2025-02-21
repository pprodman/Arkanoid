using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance; // Singleton
    public AudioClip mainMenuMusic; // Música del Main Menu
    private AudioSource audioSource;

    private void Awake()
    {
        // Singleton: Asegura que solo haya una instancia del MusicManager
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); // Destruye duplicados

        // Obtiene el componente AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource no encontrado en el MusicManager.");
        }
    }

    // Método para reproducir la música del Main Menu
    public void PlayMainMenuMusic()
    {
        if (mainMenuMusic != null && audioSource != null)
        {
            audioSource.clip = mainMenuMusic;
            audioSource.loop = true; // Repite la música
            audioSource.Play();
        }
    }

    // Método para detener la música
    public void StopMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
}