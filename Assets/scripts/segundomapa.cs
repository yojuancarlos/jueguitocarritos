using UnityEngine;
using UnityEngine.SceneManagement;

public class segundomapa : MonoBehaviour
{
   

    private void OnTriggerEnter(Collider other)
    {
            Debug.Log("paso la escena");
            SceneManager.LoadScene("Level_1");
            
        
        
    }


}
