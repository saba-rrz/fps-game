using System.Security.Cryptography;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject healthPrefab;
    [SerializeField] GameObject missingHealthPrefab;

    public void DrawHealth(int currHealth, int maxHealth)
    {
        foreach (Transform child in transform) 
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < maxHealth; i++)
        {
            if (i + 1 <= currHealth)
            {
                GameObject health = Instantiate(healthPrefab, transform.position, Quaternion.identity);
                health.transform.parent = transform;
            }
            else
            {
                GameObject health = Instantiate(missingHealthPrefab, transform.position, Quaternion.identity);
                health.transform.parent = transform;
            }
        }
    }
}
