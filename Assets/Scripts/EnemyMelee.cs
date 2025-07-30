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
    [SerializeField] private int level = 1;
    [SerializeField] private float currentHealth;
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float stoppingDistance = 1.5f;
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

    [Header("Area Damage")]
    [SerializeField] private GameObject areaDamagePrefab;
    [SerializeField] private float areaDamageRadius = 2f;
    [SerializeField] private float areaDamageAmount = 20f;
    [SerializeField] private float areaDamageDuration = 5f;
    [SerializeField] private float areaDamageTickRate = 2f;
    [SerializeField] private LayerMask playerMask;


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
    private static readonly int AttackAnimationHash = Animator.StringToHash("Attack");
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
        SpawnAreaDamage(transform.position + transform.forward * 1.5f);
    }

    private void Update()
    {
        if (!isActivated)
        {
            DetectPlayer();
            return;
        }

        if (player == null || !IsPlayerStillVisible())
        {
            DeactivateEnemy();
            return;
        }

        ChasePlayer();
        meleeAttackTimer += Time.deltaTime * PauseGameState.LocalTimeScale;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= meleeAttackRange &&
            meleeAttackTimer >= meleeAttackCooldown &&
            !isInAttackAnimation &&
            CanSeePlayer(player))
        {
            StartAttack();
        }

        if (isInAttackAnimation)
        {
            TrackAttackAnimation(); // <-- Раскомментировать
        }
    }



    private void StartAttack()
    {
        isInAttackAnimation = true;
        damageAppliedThisAttack = false;
        meleeAttackTimer = 0f;
        agent.isStopped = true;
        animator.SetBool("Walk", false);
        animator.SetTrigger("Attack");

        if (meleeAttackSound != null)
        {
            audioSource.PlayOneShot(meleeAttackSound);
        }
    }

    public void SpawnAreaDamage(Vector3 position)
    {
        GameObject area = Instantiate(areaDamagePrefab, position, Quaternion.identity);
        StartCoroutine(AreaDamageCoroutine(area.transform));
    }

    private IEnumerator AreaDamageCoroutine(Transform areaTransform)
    {
        float elapsed = 0f;

        if (areaDamagePrefab != null)
        {
            ParticleSystem ps = areaTransform.GetComponentInChildren<ParticleSystem>();
            ps?.Play();
        }

        while (elapsed < areaDamageDuration)
        {
            Collider[] hitPlayers = Physics.OverlapSphere(areaTransform.position, areaDamageRadius, playerMask);

            foreach (var hitPlayer in hitPlayers)
            {
                IDamageable damageable = hitPlayer.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(areaDamageAmount, TrapType.SaveMaze, DamageType.Normal);
                }
            }

            yield return new WaitForSeconds(areaDamageTickRate);
            elapsed += areaDamageTickRate;
        }

        Destroy(areaTransform.gameObject);
    }


    private void TrackAttackAnimation()
    {
        if (animator.IsInTransition(0)) return;

        currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (currentStateInfo.shortNameHash == AttackAnimationHash)
        {
            float normalizedTime = currentStateInfo.normalizedTime;

            if (normalizedTime >= damageApplicationPoint && !damageAppliedThisAttack)
            {
                ApplyDamage();
                damageAppliedThisAttack = true;

                if (meleeAttackEffect != null)
                {
                    meleeAttackEffect.Play();
                }
            }

            if (normalizedTime >= 0.95f) // Завершение атаки
            {
                isInAttackAnimation = false;
                agent.isStopped = false;
                animator.SetBool("Walk", true); // Возвращаем анимацию ходьбы
            }
        }
        else
        {
            isInAttackAnimation = false;
            agent.isStopped = false;
            animator.SetBool("Walk", true);
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
                animator.SetBool("Walk", false);
            }
            else if (!isInAttackAnimation)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
                animator.SetBool("Walk", true);

                RotateTowards(agent.desiredVelocity);
            }
        }
    }





    private void RotateTowards(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.01f)
        {
            float DT = Time.deltaTime * PauseGameState.LocalTimeScale;
            Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, DT * 5f);
        }
    }



    public void TakeDamage(float damage, TrapType trapType, DamageType damageType)
    {
        currentHealth -= damage;
        healthBar?.UpdateHealthBar(currentHealth, health, level);
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
            healthBar?.UpdateHealthBar(currentHealth, health, level);

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
            healthBar?.UpdateHealthBar(currentHealth, health, level);

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