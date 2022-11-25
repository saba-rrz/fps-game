using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathZone : MonoBehaviour
{
   private void OnTriggerExit(Collider other)
   {
      if (other.name == "MapLimit")
      {
         Debug.Log("Respawn");
         ResetGame();
      }
        
   }

   private void ResetGame()
   {
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
   }
}
