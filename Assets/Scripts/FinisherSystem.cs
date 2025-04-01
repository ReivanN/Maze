using System;
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
    [HideInInspector]public UnityEvent<Transform> OnFinisherStarted;

    private Transform targetEnemy;
    private bool canFinish = false;
    [SerializeField] private InputAction finisherAction;
    private Enemy[] enemies;
    private bool isFinishing = false;

    public event Action<int> OnKillCountChanged;
    private int KillCount = 0;

    private void Start()
    {
        finisherUI = GameObject.FindGameObjectWithTag("finisherUI");

        if (finisherUI != null)
            finisherUI.SetActive(false);

        UpdateEnemies();
        KillCount = PlayerPrefs.GetInt("Kills");
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
        CheckTargetWithForwardRaycast();
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
        KillCount++;
        OnKillCountChanged?.Invoke(KillCount);
        PlayerPrefs.SetInt("Kills" + KillCount, KillCount);
        UpdateEnemies();
    }
    public void RemoveEnemy(Enemy enemy)
    {
        enemies = enemies.Where(e => e != null && e != enemy).ToArray();
    }

    public void UpdateEnemies()
    {
        enemies = FindObjectsOfType<Enemy>();
    }


    void CheckTargetWithForwardRaycast()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.3f;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(rayOrigin, direction, out RaycastHit hit, finisherRange, ~0))
        {
            Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
            if (enemy != null && enemy.isAlive && ((1 << hit.collider.gameObject.layer) & enemyLayer) != 0)
            {
                targetEnemy = enemy.transform;
                canFinish = true;
                if (finisherUI != null)
                    finisherUI.SetActive(true);
                Debug.DrawRay(rayOrigin, direction * hit.distance, Color.red);
                return;
            }
            else
            {
                targetEnemy = null;
                canFinish = false;
                if (finisherUI != null)
                    finisherUI.SetActive(false);
                Debug.DrawRay(rayOrigin, direction * hit.distance, Color.yellow);
                return;
            }
        }
        targetEnemy = null;
        canFinish = false;
        if (finisherUI != null)
            finisherUI.SetActive(false);
        Debug.DrawRay(rayOrigin, direction * finisherRange, Color.green);
    }

}
