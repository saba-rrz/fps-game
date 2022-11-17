using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{

    private NavMeshAgent agent = null;
    [SerializeField] private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        GetReferences();
    }

    // Update is called once per frame
    void Update()
    {
        MoveToTarget();
        CheckDistance();
    }
    private void MoveToTarget()
    {
        agent.SetDestination(target.position);
        RotateToTarget();
    }

    private void RotateToTarget()
    {
        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
    }
    private void GetReferences()
    {
        agent=GetComponent<NavMeshAgent>();
    }

    private bool CheckDistance()
    {
        if (agent.stoppingDistance <= 3f)
        {
            return true;
        }
        return false;
    }
}
