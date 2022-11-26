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
    public Vector3 walkPoint;
    private bool _walkPointSet;
    public float walkPointRange;
    
    //Attacking
    public float timeBetweenShots, spread, projectileForce;
    private bool _attacked;
    public GameObject projectile;

    public float sightRange, attackRange;
    public bool playerSighted, playerInRange;
    public bool isPatroling, isChasing, isAttacking;
    public Animator chickenAnimator;

    private void Awake()
    {
        player = GameObject.Find("FPS Player").transform;
        agent = GetComponent<NavMeshAgent>();
        chickenAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        //Check for player in attack
        var position = transform.position;
        
        playerSighted = Physics.CheckSphere(position, sightRange, playerMask);
        playerInRange = Physics.CheckSphere(position, attackRange, playerMask);
        
        if (!playerSighted && !playerInRange) Patrol();
        if (playerSighted && !playerInRange) ChasePlayer();
        if (playerSighted && playerInRange) AttackPlayer();
    }

    private void Patrol()
    {
        if (!_walkPointSet) SearchWalkPoint();

        if (_walkPointSet)
        {
            chickenAnimator.SetBool("isAttacking", false);
            agent.SetDestination(walkPoint);
            
        }

        Vector3 distanceToPoint = transform.position - walkPoint;
        
        //WalkPoint Reached
        if (distanceToPoint.magnitude < 1f)
        {
            _walkPointSet = false;
        }
        
    }

    private void SearchWalkPoint()
    {
        float randomZRange = Random.Range(-walkPointRange, walkPointRange);
        float randomXRange = Random.Range(-walkPointRange, walkPointRange);

        var position = transform.position;
        walkPoint = new Vector3(position.x + randomXRange, position.y, position.z + randomZRange);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundMask))
        {
            _walkPointSet = true;
        }

    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);

    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void AttackPlayer()
    {
        
        chickenAnimator.SetBool("isAttacking", true);
        agent.SetDestination(transform.position);
        
        transform.LookAt(player.position + transform.forward);

        if (!_attacked)
        {
            isAttacking = true;
            Rigidbody rb = Instantiate(projectile, transform.position + Vector3.up, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * projectileForce, ForceMode.Impulse);
           
           
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

