using System;
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

   private void OnTriggerEnter(Collider other)
   {
      if (other.name == "TryAgainCube")
      {
         Debug.Log("Try again restart!");
         ResetGame();
      }

      if (other.name == "MainMenuCube")
      {
         Debug.Log("To main menu");
         SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
      }
   }

   public void ResetGame()
   {
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
   }
}
