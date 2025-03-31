using UnityEngine;
using UnityEngine.AI;

public class Bomb : MonoBehaviour
{
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private LayerMask obstaclesMask; // Маска препятствий

    private Transform player;
    private bool isActivated = false;
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        meshRenderer.enabled = false;
        agent.enabled = false;
    }

    private void Start()
    {
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            agent.enabled = true;
        }
        else
        {
            Debug.LogError("Бомба не найдена на NavMesh! Переместите её вручную.");
            agent.enabled = false;
        }
    }

    private void Update()
    {
        if (!isActivated)
        {
            DetectPlayer();
        }
        else
        {
            if (player == null || !IsPlayerStillVisible())
            {
                DeactivateBomb();
            }
            else
            {
                ChasePlayer();
            }
        }
    }

    private void DetectPlayer()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player") && CanSeePlayer(hitCollider.transform))
            {
                player = hitCollider.transform;
                ActivateBomb();
                return;
            }
        }
    }

    private bool CanSeePlayer(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, target.position);

        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, distance, obstaclesMask))
        {
            return false;
        }

        return true; 
    }

    private bool IsPlayerStillVisible()
    {
        if (player == null) return false;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        return distanceToPlayer <= detectionRadius;
    }

    private void ActivateBomb()
    {
        isActivated = true;
        meshRenderer.enabled = true;
        agent.isStopped = false;
    }

    private void DeactivateBomb()
    {
        isActivated = false;
        player = null;
        agent.ResetPath();
    }

    private void ChasePlayer()
    {
        if (player != null && agent.enabled)
        {
            agent.SetDestination(player.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Bullet"))
        {
            if (other.CompareTag("Bullet"))
            {
                Destroy(other.gameObject);
            }
            Explode();
        }
    }

    private void Explode()
    {
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (player != null && isActivated)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}
