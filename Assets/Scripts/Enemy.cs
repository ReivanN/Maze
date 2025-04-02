using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Enemy : MonoBehaviour, IDamageable
{
    private Rigidbody[] ragdollBodies;
    private Animator animator;
    private NavMeshAgent agent;
    private Transform player;

    [SerializeField] private float health = 100f;
    [SerializeField] private float currenthealth;
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float stoppingDistance = 2f;
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private LayerMask obstaclesMask;

    [HideInInspector] public UnityAction EnemyDeath;
    [HideInInspector] public bool isAlive = true;
    private bool isActivated = false;

    private IHealthBar healthBar;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(10, TrapType.NewMaze);
        }
    }

    void Start()
    {
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        healthBar = GetComponentInChildren<IHealthBar>();
        DisableRagdoll();
        agent.speed = speed;
        agent.stoppingDistance = stoppingDistance;
        currenthealth = health;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (!isActivated)
        {
            DetectPlayer();
        }
        else
        {
            if (player == null || !IsPlayerStillVisible())
            {
                DeactivateEnemy();
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
            if (hitCollider.CompareTag("Player"))
            {
                player = hitCollider.transform;
                ActivateEnemy();
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
        return distanceToPlayer <= detectionRadius && CanSeePlayer(player);
    }

    private void ActivateEnemy()
    {
        isActivated = true;
        agent.isStopped = false;
    }

    private void DeactivateEnemy()
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

    public void TakeDamage(int damage, TrapType trapType)
    {
        //if (!isAlive) return;

        currenthealth -= damage;
        healthBar?.UpdateHealthBar(currenthealth, health);
        if (currenthealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        healthBar.HideHealthBar();
        isAlive = false;
        GetComponent<Collider>().enabled = false;
        EnableRagdoll();
        agent.enabled = false;
        EnemyDeath?.Invoke();
    }

    public void EnableRagdoll()
    {
        animator.enabled = false;
        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = false;
        }
        this.enabled = false;
    }

    public void DisableRagdoll()
    {
        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = true;
        }
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
