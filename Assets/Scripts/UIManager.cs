using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // Singleton

    // Referencias a los textos de la UI
    [Header("Main Menu")]
    public TextMeshProUGUI highScoreText; // Puntuación máxima (visible en el menú)

    [Header("Gameplay")]
    public TextMeshProUGUI scoreText; // Puntuación actual (visible durante el juego)
    public GameObject livesIconPrefab; // Prefab del icono de las vidas
    public Transform livesIconsPanel; // Panel donde se colocan los iconos de las vidas
    private List<GameObject> livesIcons = new List<GameObject>(); // Lista de iconos activos
    public TextMeshProUGUI roundText; // Referencia al texto del nivel actual

    private void Awake()
    {
        // Singleton: Asegura que solo haya una instancia del UIManager
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); // Destruye duplicados

        // Actualizar la puntuación máxima al iniciar el juego
        UpdateHighScore(GameManager.Instance.GetHighScore());
    }

    private void Start()
    {
        // Actualiza la puntuación y la ronda al iniciar la escena
        UpdateScore(GameManager.Instance.score);
        UpdateRound(GameManager.Instance.currentLevel);

        // Inicializa los iconos de las vidas
        InitializeLives(GameManager.Instance.lives);
    }

    // Método para actualizar la puntuación máxima en la interfaz gráfica
    public void UpdateHighScore(int highScore)
    {
        if (highScoreText != null) // Verifica que el objeto de texto exista
        {
            highScoreText.text = "" + highScore; // Actualiza el texto con la puntuación máxima
        }
    }

    // Método para actualizar la puntuación en la interfaz gráfica
    public void UpdateScore(int score)
    {
        if (scoreText != null) // Verifica que el objeto de texto exista
        {
            scoreText.text = "" + score; // Actualiza el texto con la nueva puntuación
        }
    
    }

    public void UpdateRound(int round)
    {
        if (roundText != null) // Verifica que el objeto de texto exista
        {
            roundText.text = "" + round; // Actualiza el texto con la nueva puntuación
        }
    }

    // Método para inicializar los iconos de las vidas
    public void InitializeLives(int lives)
    {
        Debug.Log("Inicializando vidas: " + lives);
        ClearLivesIcons(); // Limpia los iconos existentes

        for (int i = 0; i < lives; i++)
        {
            GameObject icon = Instantiate(livesIconPrefab, livesIconsPanel);
            livesIcons.Add(icon); // Guarda el icono en la lista
        }
    }

    // Método para actualizar los iconos cuando el jugador pierde o gana una vida
    public void UpdateLives(int lives)
    {
        // Si hay más iconos que vidas, elimina el exceso
        while (livesIcons.Count > lives)
        {
            GameObject iconToRemove = livesIcons[livesIcons.Count - 1];
            livesIcons.Remove(iconToRemove);
            Destroy(iconToRemove);
        }

        // Si hay menos iconos que vidas, agrega nuevos
        while (livesIcons.Count < lives)
        {
            GameObject icon = Instantiate(livesIconPrefab, livesIconsPanel);
            livesIcons.Add(icon);
        }
    }

    // Método para limpiar todos los iconos de las vidas
    private void ClearLivesIcons()
    {
        foreach (GameObject icon in livesIcons)
        {
            Destroy(icon); // Elimina cada icono
        }
        livesIcons.Clear(); // Limpia la lista
    }
}