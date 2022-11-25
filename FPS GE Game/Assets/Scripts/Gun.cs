using System;
using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float fireRate = 15f;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private ParticleSystem shootingSystem;
    [SerializeField] private TrailRenderer bulletTrail;
    [SerializeField] private ParticleSystem impactParticleSystem;
    [SerializeField] private LayerMask mask;

    private Animator _animator;
    

    public int maxBullets = 3;
    public int bullets;
    [SerializeField] private WeaponUI bulletSystem;
    [SerializeField] private float reloadTime = 1f;
    private bool _isReloading=false;


    //public Animator animator;

    private float _nextTimeToFire = 0f;
    
    public Camera playerCamera;
    public ParticleSystem muzzleFlash;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        bulletSystem.DrawBullets(bullets, maxBullets);
        bullets = maxBullets;
    }
    private void OnEnable()
    {
        _isReloading = false;
        //animator.SetBool("Reloading", false);
    }
    private void Update()
    {
        bulletSystem.DrawBullets(bullets, maxBullets);
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
        //animator.SetBool("Reloading",true);

        yield return new WaitForSeconds(reloadTime - .25f);
       // animator.SetBool("Realoding",false);
        yield return new WaitForSeconds(1f);


        bullets = maxBullets;
        _isReloading = false;

    }

    // ReSharper disable Unity.PerformanceAnalysis
    void Shoot()
    {
        
        muzzleFlash.Play();
        shootingSystem.Play();
        Vector3 direction = GetDirection();

        if (Physics.Raycast(bulletSpawnPoint.position, direction, out RaycastHit hit, float.MaxValue, mask))
        {
            TrailRenderer trail = Instantiate(bulletTrail, bulletSpawnPoint.position, Quaternion.identity);
            
            StartCoroutine(SpawnTrail(trail,hit));
        }
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
            //update damage
        }
        
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = transform.forward;

        return direction;
    }

    private IEnumerator SpawnTrail(TrailRenderer Trail, RaycastHit Hit)
    {
        float time = 0;
        Vector3 startPosition = Trail.transform.position;

        while (time < 1)
        {
            Trail.transform.position = Vector3.Lerp(startPosition, Hit.point, time);
            time += Time.deltaTime / Trail.time;

            yield return null;
        }

        Trail.transform.position = Hit.point;
        //Instantiate(impactParticleSystem, Hit.point, Quaternion.LookRotation(Hit.normal));
        
        Destroy(Trail.gameObject, Trail.time);
    }

}
