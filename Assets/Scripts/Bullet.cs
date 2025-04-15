using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Stats")]
    private Vector3 direction;
    private float speed;
    private float damage;
    public DamageType damageType;
    [Header("Ricochet")]
    //public int maxRicochets = 3;
    private int currentRicochets;
    public LayerMask ricochetMask;
    public GameObject impactEffect;
    

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
        float DT = Time.deltaTime * PauseGameState.LocalTimeScale;
        float moveDistance = speed * DT;

        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, moveDistance, ricochetMask, QueryTriggerInteraction.Collide);

        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                GameObject hitObject = hit.collider.gameObject;
                IDamageable damageable = hitObject.GetComponent<IDamageable>();

                if (damageable != null && !hitObject.CompareTag("Player"))
                {
                    HandleHit(hit); // наносим урон

                    // Если пуля не ядовитая — уничтожаем после первого попадания
                    if ((damageType & DamageType.Poison) != 0)
                        break;
                }
                else
                {
                    // Если объект не враг (например, стена) — уничтожаем даже ядовитую пулю
                    Destroy(gameObject);
                    if (impactEffect != null)
                    {
                        Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    }
                    break;
                }
            }
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
            damageable.TakeDamage(damage, TrapType.NewMaze, damageType);
            Debug.Log($"Damage applied: {damage} to {hitObject.name}");
            Destroy(gameObject);
            return;
        }

        if ((damageType & DamageType.Poison) != 0)
        {
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
