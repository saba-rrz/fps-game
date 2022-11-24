using UnityEngine;


public class PlayerHealth : MonoBehaviour
{
   public int health = 3;
   public int maxHealth = 3;
   [SerializeField] private HealthSystem hsHealthSystem;

   private void Start()
   {
      hsHealthSystem.DrawHealth(health, maxHealth);
   }

   public void DamagePlayer(int dmg)
   {
      if (health > 0)
      {
         health -= dmg;
         hsHealthSystem.DrawHealth(health, maxHealth);
      }
      
   }
   
   public void HealPlayer(int heal)
   {
      if (health < 0)
      {
         health += heal;
         hsHealthSystem.DrawHealth(health, maxHealth);
      }
   }

}
