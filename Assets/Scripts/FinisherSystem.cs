using System;
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

        enemies = FindObjectsOfType<Enemy>();
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
        if (isFinishing)
        {

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Finisher") && stateInfo.normalizedTime >= 0.02f)
            {
                isFinishing = false;
                ChangeWeapon();
            }
        }
    }

    void FindClosestEnemy()
    {
        Debug.Log($"Найдено врагов: {enemies.Length}");

        float minDistance = float.MaxValue;
        targetEnemy = null;

        foreach (var enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            Debug.Log($"Проверка {enemy.name} на расстоянии {distance}");

            if (distance < minDistance && distance <= finisherRange)
            {
                minDistance = distance;
                targetEnemy = enemy.transform;
            }
        }

        canFinish = targetEnemy != null;

        if (canFinish)
        {
            Debug.Log($"Ближайший враг: {targetEnemy.name}");
        }
        else
        {
            Debug.Log("Врагов поблизости нет!");
        }

        if (finisherUI != null)
            finisherUI.SetActive(canFinish);
    }



    void TryStartFinisher(InputAction.CallbackContext ctx)
    {
        if (!canFinish || targetEnemy == null) 
        {
            Debug.LogError("1");
            return;
        }

        OnFinisherStarted?.Invoke(targetEnemy);
        StartFinisher(targetEnemy);
    }

    void StartFinisher(Transform enemy)
    {
        Debug.LogError("2");
        gun.SetActive(false);
        sword.SetActive(true);
        Debug.LogError("3");
        transform.position = enemy.position - enemy.forward * 1.5f;
        transform.LookAt(enemy);
        animator.SetTrigger("Finisher");
        isFinishing = true;
        Debug.LogError("4");
        Enemy ragdoll = enemy.GetComponent<Enemy>();
        if (ragdoll != null)
        {
            Debug.LogError("5");
            ragdoll.EnableRagdoll();
        }

    }
    void ChangeWeapon()
    {
        sword.SetActive(false);
        gun.SetActive(true);
    }
}
