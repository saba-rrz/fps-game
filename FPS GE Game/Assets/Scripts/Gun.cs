using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 100f;

    public Camera playerCamera;
    public ParticleSystem muzzleFlash;
    private void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            Shoot();
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    void Shoot()
    {
        muzzleFlash.Play();
        
        RaycastHit raycastHit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out raycastHit, range))
        {
            Debug.Log(raycastHit.transform.name);

            Target target = raycastHit.transform.GetComponent<Target>();

            if (target != null)
            {
                target.TakeDamage(damage);
            }
        }
    }
}
