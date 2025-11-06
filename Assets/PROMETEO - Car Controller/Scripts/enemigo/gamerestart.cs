using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRestarter : MonoBehaviour
{
    public GameObject playerCar; // Arrastra tu auto principal aquí en el Inspector
    public float detectionDistance = 3f; // Distancia a la que se activa el reinicio
    
    private void Update()
    {
        // Verificar si el playerCar está asignado
        if (playerCar == null)
            return;
        
        // Calcular distancia entre el enemigo y el auto
        float distance = Vector3.Distance(transform.position, playerCar.transform.position);
        
        // Si la distancia es menor que la distancia de detección, reiniciar
        if (distance <= detectionDistance)
        {
            RestartGame();
        }
    }
    
    private void RestartGame()
    {
        // Reiniciar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    // Opcional: Visualizar la distancia de detección en el Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionDistance);
    }
}