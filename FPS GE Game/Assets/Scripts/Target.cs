using System.Collections;
using UnityEngine;

public class Target : MonoBehaviour
{
   public float health = 50f;
   public AudioSource deathNoise;
   public PlayerHealth playerHealth;

    public void TakeDamage (float amount)
    {
        health -= amount; 
        
        if (health <= 0f)
        {
            playerHealth.health++;
            StartCoroutine(Die());
        }
        
    }

    IEnumerator Die()
    {
        deathNoise.Play();
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
