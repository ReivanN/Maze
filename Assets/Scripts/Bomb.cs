using UnityEngine;
using UnityEngine.AI;

public class Bomb : MonoBehaviour
{
    [SerializeField] private GameObject particles;
    [SerializeField] private MeshRenderer renderers;
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float moveSpeed = 2f;

    private Transform player;
    private bool isActivated = false;
    private NavMeshAgent agent;
    NavMeshHit hit;

    private void Start()
    {
        renderers.enabled = false;
        
    }



    private void Update()
    {
        if (!isActivated)
        {
            DetectPlayer();
        }
        else
        {
            ChasePlayer();
        }
    }

    private void DetectPlayer()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player") && CanSeePlayer(hitCollider.transform))
            {
                player = hitCollider.transform;
                ActivateBomb();
                break;
            }
        }
    }

    private bool CanSeePlayer(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, direction, out hit, detectionRadius))
        {
            return hit.collider.CompareTag("Player");
        }
        return false;
    }

    private void ActivateBomb()
    {
        isActivated = true;
        renderers.enabled = true;
        agent.isStopped = false;
    }

    private void ChasePlayer()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Explode();
        }
        else if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            Explode();
        }
    }

    private void Explode()
    {
        Instantiate(particles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (player != null && isActivated)
        {
            // Направление и длина Raycast
            Vector3 direction = (player.position - transform.position).normalized;
            float rayLength = detectionRadius; // длина луча равна радиусу детекции

            Gizmos.color = Color.green; // Цвет для Raycast
            Gizmos.DrawLine(transform.position, transform.position + direction * rayLength); // Линия от бомбы к игроку
        }
    }
}
