using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
   // Velocidad inicial de la bola
    public float initialSpeed = 100.0f;
    public float speedIncreaseInterval = 5.0f;
    public float speedIncreasePercentage = 0.1f;

    private float speed; // Velocidad actual de la bola

    void Start()
    {
        // Reinicia la velocidad al valor inicial al cargar la escena
        speed = initialSpeed;

        // Inicializa la velocidad de la pelota hacia arriba
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.up * speed;

        // Inicia la corrutina para aumentar la velocidad con el tiempo
        StartCoroutine(IncreaseSpeedOverTime());
    }

    IEnumerator IncreaseSpeedOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(speedIncreaseInterval); // Espera el tiempo configurado
            speed *= (1 + speedIncreasePercentage); // Aumenta la velocidad según el porcentaje
            GetComponent<Rigidbody2D>().linearVelocity = GetComponent<Rigidbody2D>().linearVelocity.normalized * speed;
        }
    }

    // Calcula el factor de golpeo en la raqueta
    float hitFactor(Vector2 ballPos, Vector2 racketPos, float racketWidth)
    {
        //
        // 1  -0.5  0  0.5   1  <- valor de x
        // ===================  <- raqueta
        //
        return (ballPos.x - racketPos.x) / racketWidth;
    }

    // Detecta colisiones
    void OnCollisionEnter2D(Collision2D col)
    {
        // ¿Chocó con la raqueta?
        if (col.gameObject.name == "racket")
        {
            // Calcula el factor de golpeo
            float x = hitFactor(transform.position, col.transform.position, col.collider.bounds.size.x);

            // Calcula la nueva dirección
            Vector2 dir = new Vector2(x, 1).normalized;

            // Ajusta la velocidad con la nueva dirección y la velocidad actual
            GetComponent<Rigidbody2D>().linearVelocity = dir * speed;
        }
    }
}