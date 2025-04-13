using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Enemy : MonoBehaviour, IDamageable
{
    private Rigidbody[] ragdollBodies;
    private Animator animator;
    private NavMeshAgent agent;
    private Transform player;

    [SerializeField] private float health = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float stoppingDistance = 2f;
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private LayerMask obstaclesMask;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 2f;
    [SerializeField] private float currentbulletSpeed;
    [SerializeField] private float fireRate = 1f;
    private float fireCooldownTimer = 0f;
    private float nextFireTime;

    [HideInInspector] public UnityAction EnemyDeath;
    [HideInInspector] public bool isAlive = true;
    private bool isActivated = false;
    [SerializeField] private GameObject HPBonus;
    private IHealthBar healthBar;

    public AudioSource audioSource;
    public AudioClip clip;
    [SerializeField] private ParticleSystem slowEffectParticles;


    [Header("Coins")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private float spawnRadius = 0.1f;
    [SerializeField] private Transform spawnCenter;

    void Start()
    {
        currentSpeed = speed;
        currentbulletSpeed = bulletSpeed;
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        healthBar = GetComponentInChildren<IHealthBar>();
        DisableRagdoll();
        agent.speed = currentSpeed;
        agent.stoppingDistance = stoppingDistance;
        currentHealth = health;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (!isActivated )
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
                RotateTowardsPlayer();
                float DT = Time.deltaTime * PauseGameState.LocalTimeScale;
                fireCooldownTimer += DT;
                if (fireCooldownTimer >= 1f / fireRate && CanSeePlayer(player))
                {
                    Shoot();
                    fireCooldownTimer = 0f;
                }
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
        return Vector3.Distance(transform.position, player.position) <= detectionRadius && CanSeePlayer(player);
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

    private void RotateTowardsPlayer()
    {
        float DT = Time.deltaTime * PauseGameState.LocalTimeScale;
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, DT * 5f);
        }
    }

    private void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            Vector3 direction = (player.position - firePoint.position).normalized;
            direction.y = 0;
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));

            BulletEnemy bulletScript = bullet.GetComponent<BulletEnemy>();
            if (bulletScript != null)
            {
                audioSource.PlayOneShot(clip);
                bulletScript.Initialize(direction, currentbulletSpeed, 10, 0);
                bulletScript.SetDamage(10f);
            }
        }
    }

    public void TakeDamage(float damage, TrapType trapType, DamageType damageType)
    {
        currentHealth -= damage;
        healthBar?.UpdateHealthBar(currentHealth, health);
        if (damageType == DamageType.Ice)
        {
            ApplySlow(2f, 0.5f);
            Debug.LogError("Damage" + currentSpeed + currentbulletSpeed);
        }

        if (currentHealth <= 0)
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
        Vector3 HPPosition = new Vector3(this.transform.position.x, 0.5f, this.transform.position.z);
        GameObject bonus = Instantiate(HPBonus, HPPosition, Quaternion.identity);
        SpawnCoins();
        AnimateBonusDrop(bonus);
    }

    private void AnimateBonusDrop(GameObject bonus)
    {
        float jumpHeight = 2f;
        float jumpDuration = 0.5f;
        float fallDuration = 0.3f;

        Vector3 targetPosition = bonus.transform.position + new Vector3(Random.Range(0f, 0f), 0, Random.Range(0f, 0f));

        bonus.transform.DOMoveY(bonus.transform.position.y + jumpHeight, jumpDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                bonus.transform.DOMove(targetPosition, fallDuration)
                    .SetEase(Ease.InQuad);
            });

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

    private Coroutine slowCoroutine;

    private void ApplySlow(float duration, float slowMultiplier)
    {
        if (slowCoroutine != null) StopCoroutine(slowCoroutine);
        slowCoroutine = StartCoroutine(SlowCoroutine(duration, slowMultiplier));
    }

    private IEnumerator SlowCoroutine(float duration, float slowMultiplier)
    {
        currentSpeed = speed * slowMultiplier;
        currentbulletSpeed = bulletSpeed * slowMultiplier;
        agent.speed = currentSpeed;
        // �������� ��������
        if (slowEffectParticles != null && !slowEffectParticles.isPlaying)
        {
            slowEffectParticles.Play();
        }

        yield return new WaitForSeconds(duration);

        // ���������� ��������
        currentSpeed = speed;
        currentbulletSpeed = bulletSpeed;

        // ��������� ��������
        if (slowEffectParticles != null && slowEffectParticles.isPlaying)
        {
            slowEffectParticles.Stop();
        }
    }


    private void SpawnCoins() 
    {
        int coinCount = Random.Range(1, 6);

        for (int i = 0; i < coinCount; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = spawnCenter.position + new Vector3(randomOffset.x, 0.5f, randomOffset.y);

            Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
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
