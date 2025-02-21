using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private bool isHidden = false;
    public ParticleSystem explosion;
    public BlockColor blockColor; // Color del bloque
    private int points; // Puntos que otorga el bloque
    private bool isSpecial; // Indica si el bloque es especial (libera bonus)
    public AudioClip blockHitSound; // Sonido al golpear bloque
    public AudioClip explosionSound; // Sonido explosion
    private AudioSource audioSource;

    private void Start()
    {
        // Obtiene el componente AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource no encontrado en el block.");
        }

        // Asigna puntos y propiedades especiales según el color del bloque
        switch (blockColor)
        {
            case BlockColor.Pink:
                points = 5;
                isSpecial = true; // Los bloques rosas liberan bonus
                break;
            case BlockColor.Green:
                points = 4;
                isSpecial = false;
                break;
            case BlockColor.Blue:
                points = 3;
                isSpecial = false;
                break;
            case BlockColor.Red:
                points = -1;
                isSpecial = false;
                break;
            default:
                Debug.LogError("Color de bloque no definido.");
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si la bola colisiona con este bloque...
        if (collision.gameObject.CompareTag("Ball") && !isHidden)
        {
            // Reproduce el sonido de contacto solo para bloques no explosivos
            if (blockColor != BlockColor.Explosive)
            {
                PlaySound(blockHitSound);
            }

            GameManager.Instance.AddScore(points); // Añade puntos al jugador

            if (blockColor == BlockColor.Explosive)
            {
                Explode(); // Llama al método de explosión
            }
            else if (isSpecial) // Si el bloque es especial, genera un bonus
            {
                SpawnBonus();
            }

            HideBlock(); // Oculta el bloque en lugar de destruirlo
            Instantiate(explosion, transform.position, explosion.transform.rotation);

            // Verifica si todos los bloques azules han sido desactivados
            if (blockColor == BlockColor.Blue)
            {
                CheckBlueBlocksCleared();
            }
        }
    }

    // Método para generar un bonus cuando se destruye un bloque especial
    private void SpawnBonus()
    {
        // Genera un tipo de bonus aleatorio
        BonusType randomType = (BonusType)Random.Range(0, System.Enum.GetValues(typeof(BonusType)).Length);

        // Obtiene un bonus del pool según el tipo
        GameObject bonusPrefab = ObjectPoolManager.Instance.GetPooledBonus(randomType);
        if (bonusPrefab != null)
        {
            Bonus bonus = bonusPrefab.GetComponent<Bonus>();
            if (bonus != null)
            {
                bonus.type = randomType; // Asigna el tipo de bonus
            }

            bonusPrefab.transform.position = transform.position; // Coloca el bonus en la posición del bloque
            bonusPrefab.SetActive(true); // Activa el bonus
        }
    }

    // Método para ocultar el bloque
    private IEnumerator HideBlockWithDelay()
    {
        isHidden = true;
        GetComponent<Collider2D>().enabled = false; // Desactiva el collider para evitar más colisiones
        GetComponent<Renderer>().enabled = false; // Oculta visualmente el bloque

        yield return new WaitForSeconds(0.25f); // Pequeña pausa para permitir que el sonido se reproduzca

        gameObject.SetActive(false); // Desactiva el objeto (lo oculta)
        Debug.Log($"Bloque desactivado: {gameObject.name} de color {blockColor}");
    
        if (blockColor == BlockColor.Blue)
        {
            CheckBlueBlocksCleared();
        }
    }

    private void HideBlock()
    {
        StartCoroutine(HideBlockWithDelay());
    }

    // Método para verificar si todos los bloques azules han sido desactivados
    private void CheckBlueBlocksCleared()
    {
        // Busca todos los bloques azules activos usando la etiqueta
        GameObject[] blueBlocks = GameObject.FindGameObjectsWithTag("BlueBlock");

        foreach (GameObject blockObj in blueBlocks)
        {
            if (blockObj.activeSelf)
            {
                Debug.Log("Bloque azul activo encontrado: " + blockObj.name);
                return; // Si hay algún bloque azul activo, no cambia de nivel
            }
        }

        // Si no quedan bloques azules activos, avanza al siguiente nivel
        Debug.Log("Todos los bloques azules han sido desactivados. Cambiando de nivel...");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.NextLevel();
        }
        else
        {
            Debug.LogError("GameManager.Instance is null!");
        }
    }

    // Método para manejar la explosión
    private void Explode()
    {
        Debug.Log("¡Bloque bomba destruido!");

        // Reproduce el sonido de la explosión
        PlaySound(explosionSound);

        // Radio de explosión (ajusta según tus necesidades)
        float explosionRadius = 20f;

        // Encuentra todos los colliders dentro del radio de explosión
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D collider in hitColliders)
        {
            // Verifica si el objeto tiene una etiqueta válida
            if (collider.CompareTag("BlueBlock") || collider.CompareTag("GreenBlock") || collider.CompareTag("PinkBlock") || collider.CompareTag("RedBlock"))
            {
                Block block = collider.GetComponent<Block>();
                if (block != null && block.gameObject.activeSelf)
                {
                    Debug.Log("Bloque afectado por la explosión: " + block.gameObject.name);

                    // Aplica la explosión al bloque
                    GameManager.Instance.AddScore(block.points); // Añade puntos por el bloque destruido
                    if (block.isSpecial) // Si el bloque es especial, genera un bonus
                        block.SpawnBonus();

                    block.HideBlock(); // Oculta el bloque afectado
                    Instantiate(explosion, block.transform.position, explosion.transform.rotation);
                }
            }
            // Inicia una corrutina para esperar a que termine el sonido antes de desactivar el bloque
            StartCoroutine(WaitForExplosionSound());
        }

        // Desactiva el bloque "bomba" al finalizar la explosión
        //HideBlock();
    }

    private IEnumerator WaitForExplosionSound()
    {
        // Espera hasta que el sonido termine
        if (audioSource != null && explosionSound != null)
        {
            yield return new WaitForSeconds(explosionSound.length); // Espera la duración del clip de audio
        }

        // Desactiva el bloque "bomba" al finalizar la explosión
        HideBlock();
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

public enum BlockColor
{
    Pink,   // Rosa
    Green,  // Verde
    Blue,   // Azul
    Red,    // Rojo
    Explosive // Explosivo
}