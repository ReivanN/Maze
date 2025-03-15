using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class Enemy : MonoBehaviour
{
    private Rigidbody[] ragdollBodies;
    private Animator animator;
    public InputAction reloadAction;

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
            rb.isKinematic = false;
            StartCoroutine(Died());
        }
    }

    public void DisableRagdoll()
    {
        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = true;
        }
    }
    IEnumerator Died()
    {
        yield return new WaitForSeconds(5);
        Destroy(this.gameObject);
    }

}
