using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class EnemyMelee : MonoBehaviour, IDamageable
{
    private Rigidbody[] ragdollBodies;
    private Animator animator;
    private NavMeshAgent agent;
    private Transform player;

    [SerializeField] private float health = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float stoppingDistance = 1.5f; // Уменьшено для ближнего боя
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float meleeAttackRange = 1.5f;
    [SerializeField] private float meleeDamage = 15f;
    [SerializeField] private float meleeAttackCooldown = 1.5f;
    private float meleeAttackTimer = 0f;
    [SerializeField] private LayerMask obstaclesMask;
    [SerializeField] private LayerMask playerLayer;

    [HideInInspector] public UnityAction EnemyDeath;
    [HideInInspector] public bool isAlive = true;
    private bool isActivated = false;
    [SerializeField] private GameObject HPBonus;
    private IHealthBar healthBar;

    public AudioSource audioSource;
    public AudioClip meleeAttackSound;

    [Header("Particles")]
    [SerializeField] private ParticleSystem slowEffectParticles;
    [SerializeField] private ParticleSystem burnEffectParticles;
    [SerializeField] private ParticleSystem poisonEffectParticles;
    [SerializeField] private ParticleSystem meleeAttackEffect;
    private Coroutine slowCoroutine;
    private Coroutine burnCoroutine;
    private Coroutine posionCourutine;

    [Header("Coins")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private float spawnRadius = 0.1f;
    [SerializeField] private Transform spawnCenter;

    [Header("Animation Tracking")]
    [SerializeField] private string attackAnimationName = "Attack";
    [SerializeField][Range(0, 1)] private float damageApplicationPoint = 0.5f;
    private AnimatorStateInfo currentStateInfo;
    private bool isInAttackAnimation = false;
    private bool damageAppliedThisAttack = false;

    void Start()
    {
        currentSpeed = speed;
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
                RotateTowardsPlayer();

                meleeAttackTimer += Time.deltaTime * PauseGameState.LocalTimeScale;

                float distanceToPlayer = Vector3.Distance(transform.position, player.position);

                if (distanceToPlayer <= meleeAttackRange &&
                    meleeAttackTimer >= meleeAttackCooldown &&
                    !isInAttackAnimation)
                {
                    StartAttack();
                }

                // Отслеживание прогресса анимации
                if (isInAttackAnimation)
                {
                    TrackAttackAnimation();
                }
            }
        }
    }

    private void StartAttack()
    {
        isInAttackAnimation = true;
        damageAppliedThisAttack = false;
        meleeAttackTimer = 0f;
        animator.SetBool("Walk", false);
        animator.SetTrigger("Attack");

        if (meleeAttackSound != null)
        {
            audioSource.PlayOneShot(meleeAttackSound);
        }
    }

    private void TrackAttackAnimation()
    {
        currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Проверяем, что играет именно анимация атаки
        if (currentStateInfo.IsName(attackAnimationName))
        {
            float normalizedTime = currentStateInfo.normalizedTime % 1; // Получаем прогресс анимации (0-1)

            // Если достигли нужного момента и еще не наносили урон
            if (normalizedTime >= damageApplicationPoint && !damageAppliedThisAttack)
            {
                ApplyDamage();
                damageAppliedThisAttack = true;

                if (meleeAttackEffect != null)
                {
                    meleeAttackEffect.Play();
                }
            }

            // Если анимация завершилась
            if (normalizedTime >= 0.95f) // 0.95 чтобы не ждать полного завершения
            {
                isInAttackAnimation = false;
            }
        }
        else
        {
            // Если анимация была прервана
            isInAttackAnimation = false;
        }
    }

    private void ApplyDamage()
    {
        Collider[] hitPlayers = Physics.OverlapSphere(
            transform.position + transform.forward * meleeAttackRange / 2,
            meleeAttackRange / 2,
            playerLayer);

        foreach (var hitPlayer in hitPlayers)
        {
            IDamageable damageable = hitPlayer.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(meleeDamage, TrapType.SaveMaze, DamageType.Normal);
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
                ChasePlayer();
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
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= meleeAttackRange)
            {
                agent.isStopped = true;
            }
            else
            {
                animator.SetBool("Walk", true);
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
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

    public void TakeDamage(float damage, TrapType trapType, DamageType damageType)
    {
        currentHealth -= damage;
        healthBar?.UpdateHealthBar(currentHealth, health);
        if ((damageType & DamageType.Ice) != 0)
        {
            ApplySlow(2f, 0.5f);
        }

        if ((damageType & DamageType.Fire) != 0)
        {
            ApplyBurn(8f, 3f, 1f);
        }

        if ((damageType & DamageType.Poison) != 0)
        {
            ApplyPoison(5f, 10f, 1f);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (!isAlive) return;
        isAlive = false;
        healthBar.HideHealthBar();
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

    private void ApplySlow(float duration, float slowMultiplier)
    {
        if (slowCoroutine != null) StopCoroutine(slowCoroutine);
        slowCoroutine = StartCoroutine(SlowCoroutine(duration, slowMultiplier));
    }

    private IEnumerator SlowCoroutine(float duration, float slowMultiplier)
    {
        currentSpeed = speed * slowMultiplier;
        agent.speed = currentSpeed;
        if (slowEffectParticles != null && !slowEffectParticles.isPlaying)
        {
            slowEffectParticles.Play();
        }
        yield return new WaitForSeconds(duration);
        currentSpeed = speed;
        if (slowEffectParticles != null && slowEffectParticles.isPlaying)
        {
            slowEffectParticles.Stop();
        }
    }

    void ApplyBurn(float damagePerTick, float duration, float tickRate)
    {
        if (burnCoroutine != null)
            StopCoroutine(burnCoroutine);
        burnCoroutine = StartCoroutine(BurnCoroutine(damagePerTick, duration, tickRate));
    }

    private IEnumerator BurnCoroutine(float damagePerTick, float duration, float tickRate)
    {
        if (burnEffectParticles != null)
            burnEffectParticles.Play();

        float elapsed = 0f;
        while (elapsed < duration)
        {
            currentHealth -= damagePerTick;
            healthBar?.UpdateHealthBar(currentHealth, health);

            if (currentHealth <= 0)
            {
                Die();
                yield break;
            }

            yield return new WaitForSeconds(tickRate);
            elapsed += tickRate;
        }

        if (burnEffectParticles != null)
            burnEffectParticles.Stop();
    }

    private void ApplyPoison(float damagePerTick, float duration, float tickRate)
    {
        if (posionCourutine != null)
            StopCoroutine(posionCourutine);
        posionCourutine = StartCoroutine(PoisonCourutine(damagePerTick, duration, tickRate));
    }

    private IEnumerator PoisonCourutine(float damagePerTick, float duration, float tickRate)
    {
        if (poisonEffectParticles != null)
            poisonEffectParticles.Play();

        float elapsed = 0f;
        while (elapsed < duration)
        {
            currentHealth -= damagePerTick;
            healthBar?.UpdateHealthBar(currentHealth, health);

            if (currentHealth <= 0)
            {
                Die();
                yield break;
            }

            yield return new WaitForSeconds(tickRate);
            elapsed += tickRate;
        }

        if (poisonEffectParticles != null)
            poisonEffectParticles.Stop();
    }

    private void SpawnCoins()
    {
        int coinCount = Random.Range(3, 6);

        for (int i = 0; i < coinCount; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = spawnCenter.position + new Vector3(randomOffset.x, 0.5f, randomOffset.y);

            Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Радиус обнаружения
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Радиус атаки
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, meleeAttackRange);

        if (player != null && isActivated)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}