using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.UI;


public class WeaponUI : MonoBehaviour
{
   
    public int bullets = 3;
    public GameObject[] bulletArray;

    private void Update()
    {
        DisplayBullets(bullets);
    }
    public void DisplayBullets(int index)
    {
        for (int i = 0; i < bulletArray.Length; i++)
        {
            if (i < bullets)
            {
                bulletArray[i].SetActive(true);
            }
            else
            {
                bulletArray[i].SetActive(false);
            }
        }
    }

}
