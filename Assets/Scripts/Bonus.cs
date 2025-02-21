using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonus : MonoBehaviour
{
    public BonusType type; // Tipo de power-up (ejemplo: "BigRacket", "MultiBall")
    public float fallSpeed = 40f; // Velocidad de caída (configurable desde el Inspector)


    private void Update()
    {
        // Hace que el bonus caiga hacia abajo
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si el bonus colisiona con la raqueta, desaparece
        if (other.CompareTag("Racket"))
        {
            CollectBonus(); // Recoge el bonus
            ObjectPoolManager.Instance.ReturnToPool(gameObject); // Devuelve el bonus al pool
        }
    }

    private void CollectBonus()
    {
        // Incrementa la puntuación del jugador
        GameManager.Instance.AddScore(30); // Por ejemplo, +30 puntos por recoger el bonus

        Debug.Log("Bonus collected! +30 points");
    }
}
public enum BonusType
{
    BigRacket,  // Aumenta el tamaño de la raqueta
    MultiBall,  // Genera bolas adicionales
    SlowBall,   // Reduce la velocidad de la bola
    ExtraLife,   // Otorga una vida extra
    SlowRacket, // Raqueta lenta
    FastRacket  // Raqueta rapida
}