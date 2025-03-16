using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class FinisherSystem : MonoBehaviour
{
    [Header("Finisher Settings")]
    public float finisherRange = 3f;
    public LayerMask enemyLayer;

    [Header("References")]
    [SerializeField] private GameObject finisherUI;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject sword;
    [SerializeField] private CharacterController playerController;

    [Header("Events")]
    public UnityEvent<Transform> OnFinisherStarted;

    private Transform targetEnemy;
    private bool canFinish = false;
    [SerializeField] private InputAction finisherAction;
    private Enemy[] enemies;
    private bool isFinishing = false;

    private void Start()
    {
        finisherUI = GameObject.FindGameObjectWithTag("finisherUI");

        if (finisherUI != null)
            finisherUI.SetActive(false);

        UpdateEnemies();
    }

    private void OnEnable()
    {
        finisherAction.Enable();
        finisherAction.performed += TryStartFinisher;
    }

    private void OnDisable()
    {
        finisherAction.performed -= TryStartFinisher;
        finisherAction.Disable();
    }

    void Update()
    {
        FindClosestEnemy();
    }

    void FindClosestEnemy()
    {
        if (enemies == null || enemies.Length == 0) return;

        float minDistance = float.MaxValue;
        targetEnemy = null;

        foreach (var enemy in enemies)
        {
            if (enemy == null || !enemy.isAlive) continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance && distance <= finisherRange)
            {
                minDistance = distance;
                targetEnemy = enemy.transform;
            }
        }

        canFinish = targetEnemy != null;
        if (finisherUI != null)
            finisherUI.SetActive(canFinish);
    }

    void TryStartFinisher(InputAction.CallbackContext ctx)
    {
        if (!canFinish || targetEnemy == null || isFinishing) return;

        OnFinisherStarted?.Invoke(targetEnemy);
        StartCoroutine(StartFinisher(targetEnemy));
    }

    IEnumerator StartFinisher(Transform enemy)
    {
        isFinishing = true;
        if (playerController != null)
            playerController.enabled = false;

        finisherUI.SetActive(false);

        float stopDistance = 0.5f;
        Vector3 enemyPos = enemy.position;

        while (Vector3.Distance(transform.position, enemyPos) > stopDistance)
        {
            Vector3 direction = (enemyPos - transform.position).normalized;
            transform.position += direction * 5f * Time.deltaTime;
            transform.LookAt(enemyPos);
            yield return null;
        }

        transform.LookAt(enemyPos);
        yield return new WaitForSeconds(0.1f);
        gun.SetActive(false);
        sword.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        animator.SetTrigger("Finisher");

        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Finisher") &&
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        Enemy ragdoll = enemy.GetComponent<Enemy>();
        if (ragdoll != null)
        {
            ragdoll.EnableRagdoll();
        }

        yield return new WaitForSeconds(1f);

        sword.SetActive(false);
        gun.SetActive(true);

        if (playerController != null)
            playerController.enabled = true;

        isFinishing = false;
        RemoveEnemy(ragdoll);
        UpdateEnemies();
    }
    public void RemoveEnemy(Enemy enemy)
    {
        enemies = enemies.Where(e => e != null && e != enemy).ToArray();
    }

    public void UpdateEnemies()
    {
        enemies = FindObjectsOfType<Enemy>();
        //Debug.Log($"Обновление списка: найдено {enemies.Length} врагов.");
    }
}
