using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 direction;
    private float speed;

    private float lifetime = 5f;
    private float damage;
    public void Initialize(Vector3 bulletDirection, float bulletSpeed)
    {
        direction = bulletDirection;
        speed = bulletSpeed;
        Destroy(gameObject, lifetime);
    }

    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }
    private void OnCollisionEnter(Collision collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null && !collision.gameObject.CompareTag("Player"))   
        {
            damageable.TakeDamage(damage, TrapType.NewMaze);
            Destroy(gameObject);
            Debug.LogError(damage);
        }
        else 
        {
            Destroy(gameObject);
        }

        
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable != null && !other.gameObject.CompareTag("Player"))
        {
            damageable.TakeDamage(damage, TrapType.NewMaze);
            Destroy(gameObject);
            Debug.LogError(damage);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
