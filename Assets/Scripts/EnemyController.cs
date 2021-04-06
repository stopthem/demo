using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;

    [SerializeField] private float rangeToChase;

    [HideInInspector] public bool friendChasingMe;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (PlayerController.Instance != null)
        {
            HandleMovement();
        }
    }

    private void HandleMovement()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, PlayerController.Instance.transform.position);
        if (distanceToPlayer < rangeToChase)
        {
            navMeshAgent.isStopped = false;
            PlayerController.Instance.isGettingChased = true;
            navMeshAgent.destination = PlayerController.Instance.transform.position;
        }
        else
        {
            PlayerController.Instance.isGettingChased = false;
            navMeshAgent.isStopped = true;
            navMeshAgent.velocity = Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Friend"))
        {
            PlayerController.Instance.isGettingChased = false;
            Destroy(gameObject);
        }
    }
}
