using UnityEngine;
using UnityEngine.SceneManagement; // IMPORTANTE: Agregar esta línea

public class pinchos : MonoBehaviour
{
    [Header("CONFIGURACIÓN")]
    [Tooltip("Tag del jugador (por defecto: Player)")]
    public string tagJugador = "Player";
    
    [Tooltip("Tiempo de espera antes de reiniciar (en segundos)")]
    [Range(0f, 3f)]
    public float tiempoEspera = 0.5f;
    
    [Header("EFECTOS OPCIONALES")]
    [Tooltip("¿Mostrar mensaje en consola al tocar el obstáculo?")]
    public bool mostrarMensaje = true;
    
    [Tooltip("¿Destruir al jugador al tocar el obstáculo?")]
    public bool destruirJugador = false;
    
    [Tooltip("Sonido al tocar el obstáculo (opcional)")]
    public AudioClip sonidoMuerte;
    
    private AudioSource audioSource;
    private bool juegoTerminado = false;

    void Start()
    {
        // Obtener o agregar AudioSource si hay un sonido asignado
        if (sonidoMuerte != null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Verificar si el objeto que colisionó es el jugador
        if (collision.gameObject.CompareTag(tagJugador) && !juegoTerminado)
        {
            juegoTerminado = true;
            
            if (mostrarMensaje)
            {
                Debug.Log("¡El jugador tocó los pinchos! Reiniciando juego...");
            }
            
            // Reproducir sonido si existe
            if (sonidoMuerte != null && audioSource != null)
            {
                audioSource.PlayOneShot(sonidoMuerte);
            }
            
            // Destruir al jugador si está configurado
            if (destruirJugador)
            {
                Destroy(collision.gameObject);
            }
            
            // Reiniciar el juego después del tiempo de espera
            Invoke("ReiniciarJuego", tiempoEspera);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // También detectar si el obstáculo es un Trigger
        if (other.CompareTag(tagJugador) && !juegoTerminado)
        {
            juegoTerminado = true;
            
            if (mostrarMensaje)
            {
                Debug.Log("¡El jugador tocó los pinchos! Reiniciando juego...");
            }
            
            // Reproducir sonido si existe
            if (sonidoMuerte != null && audioSource != null)
            {
                audioSource.PlayOneShot(sonidoMuerte);
            }
            
            // Destruir al jugador si está configurado
            if (destruirJugador)
            {
                Destroy(other.gameObject);
            }
            
            // Reiniciar el juego después del tiempo de espera
            Invoke("ReiniciarJuego", tiempoEspera);
        }
    }
    
    void ReiniciarJuego()
    {
        // Reinicia la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    // Visualización en el editor (opcional)
    void OnDrawGizmos()
    {
        // Dibuja el obstáculo en rojo para identificarlo fácilmente
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}