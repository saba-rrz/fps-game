using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Nest : MonoBehaviour
{
    public GameObject[] spawners= new GameObject[5];
    public GameObject enemy;
    // Start is called before the first frame update
    void Start()
    {
        for(int i=0;i<spawners.Length;i++)
        {
            spawners[i] = transform.GetChild(i).gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        int spawnerID= Random.Range(0, spawners.Length);
        Instantiate(enemy, spawners[spawnerID].transform.position, spawners[spawnerID].transform.rotation);
    }
}
