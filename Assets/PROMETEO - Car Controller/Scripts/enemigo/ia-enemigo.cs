using UnityEngine;

public class IAEnemigo : MonoBehaviour
{
    [Header("OBJETIVO")]
    [Tooltip("Arrastra aquí el auto del jugador desde la jerarquía")]
    public Transform objetivo; // El auto del jugador
    
    [Header("CONFIGURACIÓN DE MOVIMIENTO")]
    [Range(5f, 50f)]
    [Tooltip("Velocidad de movimiento del enemigo")]
    public float velocidadMovimiento = 15f;
    
    [Range(1f, 20f)]
    [Tooltip("Velocidad de rotación hacia el objetivo")]
    public float velocidadRotacion = 5f;
    
    [Range(0.5f, 10f)]
    [Tooltip("Distancia mínima al objetivo (se detiene al llegar)")]
    public float distanciaMinima = 3f;
    
    [Header("DETECCIÓN")]
    [Range(5f, 100f)]
    [Tooltip("Distancia máxima para detectar al jugador")]
    public float rangoDeteccion = 50f;
    
    [Tooltip("¿El enemigo siempre persigue o solo cuando está cerca?")]
    public bool perseguirSiempre = true;
    
    [Header("CONFIGURACIÓN OPCIONAL")]
    [Tooltip("Usa Rigidbody para el movimiento (recomendado para física)")]
    public bool usarRigidbody = true;
    
    [Tooltip("¿Mantener al enemigo siempre en el suelo (eje Y fijo)?")]
    public bool mantenerEnSuelo = true;
    
    [Range(0f, 5f)]
    [Tooltip("Altura fija del enemigo si mantenerEnSuelo está activo")]
    public float alturaFija = 0.5f;
    
    // Variables privadas
    private Rigidbody rb;
    private float distanciaAlObjetivo;
    private Vector3 direccion;
    
    void Start()
    {
        // Intentar obtener el Rigidbody si se va a usar
        if (usarRigidbody)
        {
            rb = GetComponent<Rigidbody>();
            
            if (rb == null)
            {
                Debug.LogWarning("No se encontró Rigidbody en " + gameObject.name + 
                                ". Cambiando a movimiento sin física.");
                usarRigidbody = false;
            }
            else
            {
                // Configurar el Rigidbody para mejor control
                rb.freezeRotation = true; // Evita que se voltee
                rb.useGravity = !mantenerEnSuelo; // Desactiva gravedad si mantiene altura fija
            }
        }
        
        // Si no se asignó el objetivo, intentar encontrar al jugador automáticamente
        if (objetivo == null)
        {
            BuscarObjetivoAutomaticamente();
        }
    }
    
    void Update()
    {
        // Verificar que tengamos un objetivo
        if (objetivo == null)
        {
            Debug.LogWarning("No hay objetivo asignado para " + gameObject.name);
            return;
        }
        
        // Calcular distancia al objetivo
        distanciaAlObjetivo = Vector3.Distance(transform.position, objetivo.position);
        
        // Decidir si debe perseguir
        bool debePerseguir = perseguirSiempre || (distanciaAlObjetivo <= rangoDeteccion);
        
        if (debePerseguir && distanciaAlObjetivo > distanciaMinima)
        {
            PerseguirObjetivo();
        }
    }
    
    void PerseguirObjetivo()
    {
        // Calcular dirección hacia el objetivo
        direccion = (objetivo.position - transform.position).normalized;
        
        // Mantener la altura si está configurado
        if (mantenerEnSuelo)
        {
            direccion.y = 0;
            direccion.Normalize();
        }
        
        // Rotar hacia el objetivo
        if (direccion != Vector3.zero)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                rotacionObjetivo, 
                velocidadRotacion * Time.deltaTime
            );
        }
        
        // Mover al enemigo
        if (usarRigidbody && rb != null)
        {
            MoverConRigidbody();
        }
        else
        {
            MoverSinRigidbody();
        }
    }
    
    void MoverConRigidbody()
    {
        // Movimiento usando física
        Vector3 velocidad = direccion * velocidadMovimiento;
        
        if (mantenerEnSuelo)
        {
            velocidad.y = 0;
            rb.linearVelocity = new Vector3(velocidad.x, rb.linearVelocity.y, velocidad.z);
            
            // Mantener altura fija
            Vector3 pos = transform.position;
            pos.y = alturaFija;
            transform.position = pos;
        }
        else
        {
            rb.linearVelocity = velocidad;
        }
    }
    
    void MoverSinRigidbody()
    {
        // Movimiento usando transform
        Vector3 nuevaPosicion = transform.position + direccion * velocidadMovimiento * Time.deltaTime;
        
        if (mantenerEnSuelo)
        {
            nuevaPosicion.y = alturaFija;
        }
        
        transform.position = nuevaPosicion;
    }
    
    // Método para buscar automáticamente al jugador
    void BuscarObjetivoAutomaticamente()
    {
        // Buscar por tag
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        
        if (jugador != null)
        {
            objetivo = jugador.transform;
            Debug.Log("Objetivo encontrado automáticamente: " + jugador.name);
        }
        else
        {
            // Buscar por nombre del script
            PrometeoCarController autoJugador = FindFirstObjectByType<PrometeoCarController>();
            
            if (autoJugador != null)
            {
                objetivo = autoJugador.transform;
                Debug.Log("Objetivo encontrado por script: " + autoJugador.gameObject.name);
            }
            else
            {
                Debug.LogError("No se pudo encontrar al jugador. Asigna manualmente el objetivo.");
            }
        }
    }
    
    // Método público para cambiar el objetivo en tiempo de ejecución
    public void CambiarObjetivo(Transform nuevoObjetivo)
    {
        objetivo = nuevoObjetivo;
    }
    
    // Visualizar el rango de detección en el editor
    void OnDrawGizmosSelected()
    {
        // Dibujar rango de detección
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);
        
        // Dibujar distancia mínima
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaMinima);
        
        // Dibujar línea hacia el objetivo si existe
        if (objetivo != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, objetivo.position);
        }
    }
}

