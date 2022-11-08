
using UnityEngine;

public class Gun : MonoBehaviour
{



    public float damage;
    public float range;
    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    
    private void Start()
    {
        damage = 10f;
        range = 100f;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }


        void Shoot()
        {

            muzzleFlash.Play();
            RaycastHit hit;
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
            {
                Debug.Log(hit.transform.name);

                Target target = hit.transform.GetComponent<Target>();

                if (target != null)
                {
                    target.TakeDamage(damage);
                }
            }

        }
    }
}
