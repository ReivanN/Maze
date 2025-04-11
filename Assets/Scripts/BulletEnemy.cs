using UnityEngine;

public class BulletEnemy : MonoBehaviour
{
    [Header("Stats")]
    private Vector3 direction;
    private float speed;
    private float damage;

    [Header("Ricochet")]
    //public int maxRicochets = 3;
    private int currentRicochets;
    public LayerMask ricochetMask;
    public GameObject impactEffect;

    public void Initialize(Vector3 direction, float speed, float damage, int maxRicochets)
    {
        this.direction = direction.normalized;
        this.speed = speed;
        this.damage = damage;
        this.currentRicochets = maxRicochets;
    }

    private void Update()
    {
        float moveDistance = speed * Time.deltaTime;

        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, moveDistance, ricochetMask, QueryTriggerInteraction.Collide))
        {
            HandleHit(hit);
        }
        else
        {
            transform.Translate(direction * moveDistance, Space.World);
        }
    }

    private void HandleHit(RaycastHit hit)
    {
        GameObject hitObject = hit.collider.gameObject;

        if (impactEffect != null)
        {
            Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        }

        IDamageable damageable = hitObject.GetComponent<IDamageable>();
        if (damageable != null && !hitObject.CompareTag("Player"))
        {
            damageable.TakeDamage(damage, TrapType.NewMaze);
            Debug.Log($"Damage applied: {damage} to {hitObject.name}");
            Destroy(gameObject);
            return;
        }

        if (currentRicochets > 0)
        {
            currentRicochets--;
            direction = Vector3.Reflect(direction, hit.normal).normalized;
            transform.position = hit.point + hit.normal * 0.01f;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }
}
