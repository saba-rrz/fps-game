using System;
using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float fireRate = 15f;


    public int maxBullets = 3;
    public int bullets;
    [SerializeField] private WeaponUI bulletSystem;
    [SerializeField] private float reloadTime = 1f;
    private bool _isReloading;
    public AudioSource _gunFire;



    private float _nextTimeToFire;
    
    public Camera playerCamera;
    public ParticleSystem muzzleFlash;
    
    private void Start()
    {
        bulletSystem.DrawBullets(bullets, maxBullets);
        bullets = maxBullets;
        Debug.Log(bullets);
    }
    private void OnEnable()
    {
        _isReloading = false;
    }

    private void FixedUpdate()
    {
        bulletSystem.DrawBullets(bullets, maxBullets);
    }

    private void Update()
    {
        
        if (_isReloading)
        {
            return;

        }
        if (bullets <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        
        if (Input.GetButtonDown("Fire1")&& Time.time>=_nextTimeToFire)
        {
            _nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        
            
        }
    }

    IEnumerator Reload()
    {
        _isReloading = true;
       

        yield return new WaitForSeconds(reloadTime - .25f);
      
        yield return new WaitForSeconds(1f);


        bullets = maxBullets;
        _isReloading = false;

    }

  
    void Shoot()
    {
        
        muzzleFlash.Play();
        _gunFire.Play();
        bullets--;
        
        bulletSystem.DrawBullets(bullets, maxBullets);
        
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
