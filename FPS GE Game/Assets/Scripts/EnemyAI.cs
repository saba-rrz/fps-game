using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask groundMask, playerMask;

    //Patroling
    public Vector3 WalkPoint;
    private bool walkPointSet;
    public float walkPointRange;
    
    //Attacking
    public float timeBetweenShots, spread, projectileForce;
    private bool _attacked;
    public GameObject projectile;
    public Transform attackPoint;
    public Camera aiCamera;

    public float sightRange, attackRange;
    public bool _playerSighted, _playerInRange;

    private void Awake()
    {
        player = GameObject.Find("FPS Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //Check for player in attack
        var position = transform.position;
        
        _playerSighted = Physics.CheckSphere(position, sightRange, playerMask);
        _playerInRange = Physics.CheckSphere(position, attackRange, playerMask);
        
        if (!_playerSighted && !_playerInRange) Patrol();
        if (_playerSighted && !_playerInRange) ChasePlayer();
        if (_playerSighted && _playerInRange) AttackPlayer();
    }

    private void Patrol()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            agent.SetDestination(WalkPoint);
        }

        Vector3 distanceToPoint = transform.position - WalkPoint;
        
        //WalkPoint Reached
        if (distanceToPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
        
    }

    private void SearchWalkPoint()
    {
        float randomZRange = Random.Range(-walkPointRange, walkPointRange);
        float randomXRange = Random.Range(-walkPointRange, walkPointRange);

        var position = transform.position;
        WalkPoint = new Vector3(position.x + randomXRange, position.y, position.z + randomZRange);

        if (Physics.Raycast(WalkPoint, -transform.up, 2f, groundMask))
        {
            walkPointSet = true;
        }

    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);

    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        
        transform.LookAt(player);

        if (!_attacked)
        {
           
            Ray ray = aiCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            Vector3 targetPoint;
            if (Physics.Raycast(ray, out hit))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.GetPoint(125);
            }

            var position = attackPoint.position;
            Vector3 directionWithoutSpred = targetPoint - position;

            float x = Random.Range(-spread, spread);
            float y = Random.Range(-spread, spread);

            Vector3 directionWithSpread = directionWithoutSpred + new Vector3(x, y, 0);
            
            GameObject currentBullet = Instantiate(projectile, position, Quaternion.identity);
            currentBullet.transform.forward = directionWithSpread.normalized;
            
            currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * projectileForce, ForceMode.Impulse);
           
            _attacked = true;
            Invoke(nameof(ResetAttack), timeBetweenShots);
            
        }
    }

    private void ResetAttack()
    {
        _attacked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.15f);
        Gizmos.DrawSphere(transform.position, attackRange);
        Gizmos.color = new Color(1f, 0.92f, 0.02f, 0.15f);
        Gizmos.DrawSphere(transform.position, sightRange);
    }
}

