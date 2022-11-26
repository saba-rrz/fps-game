using UnityEngine;


public class PlayerHealth : MonoBehaviour
{
   public int health ;
   public int maxHealth;
   [SerializeField] private HealthSystem hsHealthSystem;
   public AudioSource healthCollect;
   public AudioSource damageTaken;
   public DeathZone gameManager;

   private void Start()
   {
      hsHealthSystem.DrawHealth(health, maxHealth);
   }

   private void Update()
   {
      if (health == 0)
      {
         gameManager.ResetGame();
      }
   }
   

   private void OnTriggerEnter(Collider other)
   {
      if (other.gameObject.CompareTag($"Bullets"))
      {
         DamagePlayer(1);
      }

      if (other.gameObject.CompareTag($"Heals"))
      {
         HealPlayer(1);
      }
   }

   void DamagePlayer(int dmg)
   {
      if (health <= 0) return;
      damageTaken.Play();
      health -= dmg;
      hsHealthSystem.DrawHealth(health, maxHealth);

   }
   
   void HealPlayer(int heal)
   {
      healthCollect.Play();
      if (health < 1) return;
      health = maxHealth;
      hsHealthSystem.DrawHealth(health, maxHealth);
   }

}
