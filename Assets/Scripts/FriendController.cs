using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FriendController : MonoBehaviour
{
    private NavMeshAgent m_navMeshAgent;

    [SerializeField] private float radiusToRescued;
    [SerializeField] private float radiusToAttack;
    [SerializeField] private float stoppingRange;
    private float rescuedIteration;

    private Vector3 m_target;

    private bool m_targetIsPlayer;
    private bool m_targetIsEnemy;
    private bool m_IsRescued;

    private Renderer m_friendRenderer;
    private Color m_startColor;

    [SerializeField] private LayerMask enemyLayer;

    private void Awake()
    {
        m_friendRenderer = GetComponent<Renderer>();
        m_navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        m_startColor = m_friendRenderer.material.color;
    }

    private void Update()
    {
        if (PlayerController.Instance != null)
        {
            if (!PlayerController.Instance.isGettingChased)
            {
                HandleMovement();
            }
            else
            {
                HandleAttack();
            }
        }
        else
        {
            m_navMeshAgent.isStopped = false;
            m_target = FinishLine.Instance.transform.position;
        }

        if (m_navMeshAgent.isStopped == false && m_IsRescued)
        {
            m_navMeshAgent.destination = m_target;
        }
    }

    private void HandleMovement()
    {
        float friendToPlayerDistance = Vector3.Distance(transform.position, PlayerController.Instance.transform.position);

        if (friendToPlayerDistance < radiusToRescued && !m_targetIsEnemy)
        {
            if (rescuedIteration == 0)
            {
                GameManager.Instance.rescuedFriends++;
                rescuedIteration++;
            }

            m_friendRenderer.material.color = Color.green;

            m_navMeshAgent.isStopped = false;

            m_IsRescued = true;

            m_targetIsPlayer = true;

            m_target = PlayerController.Instance.transform.position;
        }
        else if (m_IsRescued)
        {
            m_target = PlayerController.Instance.transform.position;
        }
        else if (!m_targetIsEnemy)
        {
            m_friendRenderer.material.color = m_startColor;

            m_navMeshAgent.isStopped = true;
        }

        if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) < stoppingRange && m_targetIsPlayer)
        {
            m_navMeshAgent.isStopped = true;
        }
        else if (m_IsRescued)
        {
            m_navMeshAgent.isStopped = false;
        }
    }

    private void HandleAttack()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radiusToAttack, enemyLayer);
        if (colliders.Length >= 1)
        {
            m_targetIsEnemy = true;
            m_target = colliders[0].gameObject.transform.position;
        }
        else
        {
            m_navMeshAgent.isStopped = false;
            m_target = PlayerController.Instance.transform.position;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            GameManager.Instance.rescuedFriends--;
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("FinishLine"))
        {
            FinishLine.Instance.howManyPassed++;
            Destroy(gameObject);
        }
    }
}