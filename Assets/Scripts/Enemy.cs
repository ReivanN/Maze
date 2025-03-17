using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class Enemy : MonoBehaviour
{
    private Rigidbody[] ragdollBodies;
    private Animator animator;
    public InputAction reloadAction;
    [HideInInspector]public UnityAction EnemyDeath;
    public bool isAlive = true;

    void Start()
    {
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        animator = GetComponent<Animator>();
        DisableRagdoll();
    }

    public void EnableRagdoll()
    {
        animator.enabled = false;
        foreach (var rb in ragdollBodies)
        {
            isAlive = false;
            rb.isKinematic = false;
            EnemyDeath?.Invoke();
            this.enabled = false;
        }
    }

    public void DisableRagdoll()
    {
        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = true;

        }
    }
    

}
