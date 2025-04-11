using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Bomb : MonoBehaviour, IDamageable
{
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private LayerMask obstaclesMask;
    [SerializeField] private float HP = 20f;
    [SerializeField] private float currentHP;
    [SerializeField] private AudioClip activationSound;

    private Transform player;
    private bool isActivated = false;
    private NavMeshAgent agent;
    private IHealthBar healthBar;
    private AudioSource audioSource;
    [SerializeField] private float explosionDelay = 3f;
    private Coroutine explosionCoroutine;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        meshRenderer.enabled = false;
        agent.enabled = false;
        healthBar = GetComponentInChildren<IHealthBar>();
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        currentHP = HP;
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
                /*if (Vector3.Distance(transform.position, player.position) <= agent.stoppingDistance)
                {
                    Explode();
                }*/
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

        if (activationSound != null)
        {
            audioSource.PlayOneShot(activationSound);
        }

        if (explosionCoroutine == null)
        {
            explosionCoroutine = StartCoroutine(DelayedExplosion());
        }
    }

    private void DeactivateBomb()
    {
        isActivated = false;
        player = null;
        agent.ResetPath();
        if (explosionCoroutine != null)
        {
            StopCoroutine(explosionCoroutine);
            explosionCoroutine = null;
        }
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
        /*Debug.LogError("Я коснулся его");
        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
        if (other.gameObject.CompareTag("Player") && damageable != null)
        {
            Debug.LogError("Я коснулся его 1");
            TakeDamage(20, TrapType.SaveMaze);
            Explode();
        }*/
    }

    private IEnumerator DelayedExplosion()
    {
        yield return new WaitForSeconds(explosionDelay);
        Explode();
    }

    private void Explode()
    {
        if (!isActivated) return;
        isActivated = false;
        if (explosionCoroutine != null)
        {
            StopCoroutine(explosionCoroutine);
            explosionCoroutine = null;
        }

        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        DealAreaDamage(transform.position);
        Destroy(gameObject);
    }


    private void ExplodeDead()
    {
        if (!isActivated) return;
        isActivated = false;
        if (explosionCoroutine != null)
        {
            StopCoroutine(explosionCoroutine);
            explosionCoroutine = null;
        }

        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }


    public float radius = 2f;
    public LayerMask damageableLayers;
    public void DealAreaDamage(Vector3 center)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius, damageableLayers);

        foreach (Collider collider in hitColliders)
        {
            IDamageable damageable = collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(20, TrapType.SaveMaze);
            }
        }
    }


    public void TakeDamage(float damage, TrapType trapType)
    {
        currentHP -= damage;
        healthBar?.UpdateHealthBar(currentHP, HP);

        if (currentHP <= 0)
        {
            ExplodeDead();
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
