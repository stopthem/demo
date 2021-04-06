using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FriendController : MonoBehaviour
{
    private NavMeshAgent m_navMeshAgent;

    [SerializeField] private float radiusToRescued;
    [SerializeField] private float radiusToAttack;
    [SerializeField] private float rangeToDeAggro;
    private float friendToPlayerDistance;
    private float defaultStoppingDistance;
    private float finishLineIteration;

    private Vector3 m_target;

    private List<GameObject> chaseTargets = new List<GameObject>();

    private bool m_IsRescued;
    private bool m_imChasingEnemy;

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
        defaultStoppingDistance = m_navMeshAgent.stoppingDistance;
        m_startColor = m_friendRenderer.material.color;
    }

    private void Update()
    {
        if (PlayerController.Instance != null)
        {
            friendToPlayerDistance = Vector3.Distance(transform.position, PlayerController.Instance.transform.position);

            if (!PlayerController.Instance.isGettingChased && !m_imChasingEnemy)
            {
                HandlePlayerFollow();
            }
            else if (friendToPlayerDistance > rangeToDeAggro && m_IsRescued)
            {
                HandleDeAggro();
            }
            else if (m_IsRescued)
            {
                HandleAttack();
            }
        }
        else if (m_IsRescued)
        {
            m_navMeshAgent.stoppingDistance = 0;
            m_navMeshAgent.isStopped = false;
            m_target = FinishLine.Instance.transform.position;
        }

        if (m_navMeshAgent.isStopped == false && m_IsRescued)
        {
            m_navMeshAgent.destination = m_target;
        }

    }

    private void HandlePlayerFollow()
    {
        chaseTargets.Clear();
        if (friendToPlayerDistance < radiusToRescued && !m_IsRescued)
        {
            GameManager.Instance.rescuedFriends++;

            m_IsRescued = true;

            m_friendRenderer.material.color = Color.green;
        }

        if (m_IsRescued)
        {
            m_navMeshAgent.stoppingDistance = defaultStoppingDistance;

            m_navMeshAgent.isStopped = false;

            m_target = PlayerController.Instance.transform.position;
        }
    }

    private void HandleDeAggro()
    {
        for (int i = 0; i < chaseTargets.Count; i++)
        {
            chaseTargets[i].GetComponent<EnemyController>().friendChasingMe = false;
        }
        chaseTargets.Clear();

        m_imChasingEnemy = false;

        HandlePlayerFollow();
    }

    private void HandleAttack()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radiusToAttack, enemyLayer);

        if (colliders.Length >= 1)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                EnemyController enemyController = colliders[i].GetComponent<EnemyController>();

                if (!enemyController.friendChasingMe && !m_imChasingEnemy)
                {
                    chaseTargets.Add(enemyController.gameObject);
                    if (chaseTargets.Count > 1)
                    {
                        break;
                    }

                    enemyController.friendChasingMe = true;

                    m_navMeshAgent.isStopped = false;

                    m_imChasingEnemy = true;

                }
            }

            if (m_imChasingEnemy)
            {
                m_navMeshAgent.stoppingDistance = 0;

                if (chaseTargets.Count == 1)
                {
                    m_target = chaseTargets[0].transform.position;
                }

            }

        }
        else
        {
            HandlePlayerFollow();
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
            if (finishLineIteration == 0)
            {
                FinishLine.Instance.howManyFriendsPassed++;
                finishLineIteration++;
                Destroy(gameObject);
            }

        }
    }
}
