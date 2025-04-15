using UnityEngine;
using System.Linq;

public class Bullet : MonoBehaviour
{
    [Header("Stats")]
    private Vector3 direction;
    private float speed;
    private float damage;
    public DamageType damageType;

    [Header("Ricochet")]
    private int currentRicochets;
    public LayerMask ricochetMask;
    public GameObject impactEffect;

    private bool isPoison => (damageType & DamageType.Poison) != 0;
    private bool isDestroyed = false;

    public void Initialize(Vector3 direction, float speed, float damage, int maxRicochets, DamageType damageType)
    {
        this.direction = direction.normalized;
        this.speed = speed;
        this.damage = damage;
        this.currentRicochets = maxRicochets;
        this.damageType = damageType;
    }

    private void Update()
    {
        if (isDestroyed) return;

        float deltaTime = Time.deltaTime * PauseGameState.LocalTimeScale;
        float moveDistance = speed * deltaTime;

        HandleCollision(moveDistance);
    }

    private void HandleCollision(float moveDistance)
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, moveDistance, ricochetMask, QueryTriggerInteraction.Collide);

        if (hits.Length == 0)
        {
            Move(moveDistance);
            return;
        }

        hits = hits.OrderBy(h => h.distance).ToArray();

        foreach (var hit in hits)
        {
            GameObject hitObject = hit.collider.gameObject;
            IDamageable damageable = hitObject.GetComponent<IDamageable>();
            bool isEnemy = damageable != null && !hitObject.CompareTag("Player");

            if (isEnemy)
            {
                damageable.TakeDamage(damage, TrapType.NewMaze, damageType);
                Debug.Log($"[PoisonCheck: {isPoison}] Damage {damage} applied to {hitObject.name}");

                if (!isPoison)
                {
                    CreateImpact(hit);
                    DestroySelf();
                    return;
                }

                // ядовита€ пул€ Ч продолжаем движение
                continue;
            }
            else
            {
                // —тена или другой объект
                if (currentRicochets > 0)
                {
                    currentRicochets--;
                    direction = Vector3.Reflect(direction, hit.normal).normalized;
                    transform.position = hit.point + hit.normal * 0.01f;
                    CreateImpact(hit);
                }
                else
                {
                    CreateImpact(hit);
                    DestroySelf();
                }
                return;
            }
        }

        // ≈сли все попадани€ Ч это враги, и пул€ €довита€ Ч продолжаем движение
        Move(moveDistance);
    }

    private void Move(float distance)
    {
        transform.Translate(direction * distance, Space.World);
    }

    private void DestroySelf()
    {
        if (isDestroyed) return;
        isDestroyed = true;
        Destroy(gameObject);
    }

    private void CreateImpact(RaycastHit hit)
    {
        if (impactEffect != null)
        {
            Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }

    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }
}
