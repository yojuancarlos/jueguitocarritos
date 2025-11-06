using UnityEngine;

public class AutoObstaculo : MonoBehaviour
{
    [Header("CONFIGURACIÓN DE MOVIMIENTO")]
    [Range(5f, 50f)]
    [Tooltip("Velocidad constante del auto obstáculo")]
    public float velocidad = 20f;
    
    [Tooltip("Dirección del movimiento (Forward = hacia adelante del auto)")]
    public DireccionMovimiento direccion = DireccionMovimiento.Forward;
    
    [Header("CONFIGURACIÓN DE FÍSICA")]
    [Tooltip("Usar Rigidbody para el movimiento (recomendado)")]
    public bool usarRigidbody = true;
    
    [Tooltip("Mantener velocidad constante sin aceleración")]
    public bool velocidadConstante = true;
    
    [Header("LÍMITES OPCIONALES")]
    [Tooltip("¿Destruir el auto después de cierta distancia?")]
    public bool destruirDespuesDeDistancia = false;
    
    [Range(10f, 500f)]
    [Tooltip("Distancia máxima antes de destruir el auto")]
    public float distanciaMaxima = 100f;
    
    [Tooltip("¿Reiniciar posición al llegar al límite?")]
    public bool reiniciarPosicion = false;
    
    [Tooltip("Posición inicial para reiniciar (déjalo en 0,0,0 para usar la posición actual)")]
    public Vector3 posicionInicial = Vector3.zero;
    
    [Header("CONFIGURACIÓN DE RUEDAS (Opcional)")]
    [Tooltip("¿Girar las ruedas visualmente?")]
    public bool animarRuedas = false;
    
    public Transform ruedaDelanteraIzquierda;
    public Transform ruedaDelanteraDerecha;
    public Transform ruedaTraseraIzquierda;
    public Transform ruedaTraseraDerecha;
    
    [Range(1f, 10f)]
    public float velocidadRotacionRuedas = 5f;
    
    // Enum para las direcciones
    public enum DireccionMovimiento
    {
        Forward,    // Hacia adelante
        Backward,   // Hacia atrás
        Right,      // Hacia la derecha
        Left,       // Hacia la izquierda
        Up,         // Hacia arriba (para autos voladores?)
        Down        // Hacia abajo
    }
    
    // Variables privadas
    private Rigidbody rb;
    private Vector3 direccionMovimiento;
    private Vector3 puntoInicio;
    private float distanciaRecorrida;
    
    void Start()
    {
        // Guardar posición inicial
        if (posicionInicial == Vector3.zero)
        {
            puntoInicio = transform.position;
        }
        else
        {
            puntoInicio = posicionInicial;
        }
        
        // Configurar Rigidbody si se va a usar
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
                // Configurar el Rigidbody
                rb.freezeRotation = true; // Evita que se voltee
                
                if (velocidadConstante)
                {
                    rb.linearDamping = 0; // Sin resistencia
                    rb.angularDamping = 0;
                }
            }
        }
        
        // Calcular la dirección de movimiento según la configuración
        CalcularDireccion();
        
        // Aplicar velocidad inicial si usa Rigidbody
        if (usarRigidbody && rb != null)
        {
            rb.linearVelocity = direccionMovimiento * velocidad;
        }
    }
    
    void Update()
    {
        // Mover el auto si no usa Rigidbody
        if (!usarRigidbody)
        {
            MoverSinRigidbody();
        }
        
        // Animar ruedas si está configurado
        if (animarRuedas)
        {
            AnimarRuedas();
        }
        
        // Verificar distancia recorrida
        if (destruirDespuesDeDistancia || reiniciarPosicion)
        {
            VerificarDistancia();
        }
    }
    
    void FixedUpdate()
    {
        // Mantener velocidad constante si usa Rigidbody
        if (usarRigidbody && rb != null && velocidadConstante)
        {
            rb.linearVelocity = direccionMovimiento * velocidad;
        }
    }
    
    void CalcularDireccion()
    {
        // Determinar la dirección basada en la configuración
        switch (direccion)
        {
            case DireccionMovimiento.Forward:
                direccionMovimiento = transform.forward;
                break;
            case DireccionMovimiento.Backward:
                direccionMovimiento = -transform.forward;
                break;
            case DireccionMovimiento.Right:
                direccionMovimiento = transform.right;
                break;
            case DireccionMovimiento.Left:
                direccionMovimiento = -transform.right;
                break;
            case DireccionMovimiento.Up:
                direccionMovimiento = transform.up;
                break;
            case DireccionMovimiento.Down:
                direccionMovimiento = -transform.up;
                break;
        }
    }
    
    void MoverSinRigidbody()
    {
        // Movimiento simple usando Transform
        transform.position += direccionMovimiento * velocidad * Time.deltaTime;
    }
    
    void AnimarRuedas()
    {
        // Calcular rotación basada en la velocidad
        float rotacion = velocidad * velocidadRotacionRuedas * Time.deltaTime;
        Vector3 rotacionRueda = new Vector3(rotacion, 0, 0);
        
        // Rotar cada rueda
        if (ruedaDelanteraIzquierda != null)
            ruedaDelanteraIzquierda.Rotate(rotacionRueda, Space.Self);
        
        if (ruedaDelanteraDerecha != null)
            ruedaDelanteraDerecha.Rotate(rotacionRueda, Space.Self);
        
        if (ruedaTraseraIzquierda != null)
            ruedaTraseraIzquierda.Rotate(rotacionRueda, Space.Self);
        
        if (ruedaTraseraDerecha != null)
            ruedaTraseraDerecha.Rotate(rotacionRueda, Space.Self);
    }
    
    void VerificarDistancia()
    {
        // Calcular distancia recorrida desde el punto inicial
        distanciaRecorrida = Vector3.Distance(puntoInicio, transform.position);
        
        // Si superó la distancia máxima
        if (distanciaRecorrida >= distanciaMaxima)
        {
            if (reiniciarPosicion)
            {
                ReiniciarPosicion();
            }
            else if (destruirDespuesDeDistancia)
            {
                Destroy(gameObject);
            }
        }
    }
    
    void ReiniciarPosicion()
    {
        // Volver a la posición inicial
        transform.position = puntoInicio;
        
        // Reiniciar velocidad si usa Rigidbody
        if (usarRigidbody && rb != null)
        {
            rb.linearVelocity = direccionMovimiento * velocidad;
        }
    }
    
    // Método público para cambiar la velocidad en tiempo de ejecución
    public void CambiarVelocidad(float nuevaVelocidad)
    {
        velocidad = nuevaVelocidad;
        
        if (usarRigidbody && rb != null)
        {
            rb.linearVelocity = direccionMovimiento * nuevaVelocidad;
        }
    }
    
    // Método público para cambiar la dirección en tiempo de ejecución
    public void CambiarDireccion(DireccionMovimiento nuevaDireccion)
    {
        direccion = nuevaDireccion;
        CalcularDireccion();
        
        if (usarRigidbody && rb != null)
        {
            rb.linearVelocity = direccionMovimiento * velocidad;
        }
    }
    
    // Método público para detener el auto
    public void Detener()
    {
        velocidad = 0;
        
        if (usarRigidbody && rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }
    }
    
    // Visualizar la dirección en el editor
    void OnDrawGizmos()
    {
        // Dibujar una flecha mostrando la dirección
        Gizmos.color = Color.cyan;
        Vector3 direccionTemp;
        
        switch (direccion)
        {
            case DireccionMovimiento.Forward:
                direccionTemp = transform.forward;
                break;
            case DireccionMovimiento.Backward:
                direccionTemp = -transform.forward;
                break;
            case DireccionMovimiento.Right:
                direccionTemp = transform.right;
                break;
            case DireccionMovimiento.Left:
                direccionTemp = -transform.right;
                break;
            case DireccionMovimiento.Up:
                direccionTemp = transform.up;
                break;
            case DireccionMovimiento.Down:
                direccionTemp = -transform.up;
                break;
            default:
                direccionTemp = transform.forward;
                break;
        }
        
        Gizmos.DrawRay(transform.position, direccionTemp * 5f);
        Gizmos.DrawSphere(transform.position + direccionTemp * 5f, 0.5f);
    }
    
    void OnDrawGizmosSelected()
    {
        // Dibujar el rango de destrucción si está activo
        if (destruirDespuesDeDistancia || reiniciarPosicion)
        {
            Gizmos.color = Color.red;
            Vector3 inicio = Application.isPlaying ? puntoInicio : transform.position;
            Gizmos.DrawWireSphere(inicio, distanciaMaxima);
        }
    }
}
