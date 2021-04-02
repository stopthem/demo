using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class PlayerController : MonoBehaviour
{

    public static PlayerController Instance { get; private set; }
    private Camera mainCamera;

    [Header("Destroyable Cubes")]
    public LayerMask destroyableMask;

    public float stickRaycastRadius;

    public bool isGettingChased;

    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        Instance = this;
        navMeshAgent = GetComponent<NavMeshAgent>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            DestroySticksAndMove();
        }
    }

    private void DestroySticksAndMove()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            navMeshAgent.destination = hit.point;
            Collider[] colliders = Physics.OverlapSphere(hit.point, stickRaycastRadius, destroyableMask);
            for (int i = 0; i < colliders.Length; i++)
            {
                Destroy(colliders[i].gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("FinishLine"))
        {
            if (FinishLine.Instance.howManyPassed == 0)
            {
                FinishLine.Instance.howManyPassed++;
            }

            FinishLine.Instance.playerPassed = true;
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
