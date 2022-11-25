using JetBrains.Annotations;
using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float fireRate = 15f;
    

    [SerializeField] private int maxBulets = 3;
    [SerializeField] private int bullets;
    [SerializeField] private float reloadTime = 1f;
    private bool IsReloading=false;


    //public Animator animator;

    private float nextTimeToFire = 0f;
   

    [SerializeField] private Sprite icon;
    [SerializeField] private WeaponUI weaponUI;
    public Camera playerCamera;
    public ParticleSystem muzzleFlash;

    private void Start()
    {
        bullets = maxBulets;
    }
    private void OnEnable()
    {
        IsReloading = false;
        //animator.SetBool("Reloading", false);
    }
    private void Update()
    {
        if (IsReloading)
        {
            return;

        }
        if (bullets <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        
        if (Input.GetButtonDown("Fire1")&& Time.time>=nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
           
            
        }
    }

    IEnumerator Reload()
    {
        IsReloading = true;
        //animator.SetBool("Reloading",true);

        yield return new WaitForSeconds(reloadTime - .25f);
       // animator.SetBool("Realoding",false);
        yield return new WaitForSeconds(1f);


        bullets = maxBulets;
        IsReloading = false;

    }

    // ReSharper disable Unity.PerformanceAnalysis
    void Shoot()
    {
       


        muzzleFlash.Play();
        bullets--;
        
        RaycastHit raycastHit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out raycastHit, range))
        {
            Debug.Log(raycastHit.transform.name);

            Target target = raycastHit.transform.GetComponent<Target>();

            if (target != null)
            {
                target.TakeDamage(damage);
            }
            //update damage
        }
        
    }
   
}
