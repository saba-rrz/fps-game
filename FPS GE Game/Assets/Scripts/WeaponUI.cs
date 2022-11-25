using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.UI;


public class WeaponUI : MonoBehaviour
{

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject missingBulletPrefab;

    public void DrawBullets(int bullets, int maxBullets)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < maxBullets; i++)
        {
            if (i + 1 <= bullets)
            {
                GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                bullet.transform.parent = transform;
            }
            else
            {
                GameObject bullet = Instantiate(missingBulletPrefab, transform.position, Quaternion.identity);
                bullet.transform.parent = transform;
            }
        }
    }

}
