using UnityEngine;

public class BulletEnemy : MonoBehaviour
{
    private Vector3 direction;
    private float speed;

    private float lifetime = 500f;
    private float damage;
    public void Initialize(Vector3 bulletDirection, float bulletSpeed)
    {
        direction = bulletDirection;
        speed = bulletSpeed;
    }

    public void SetDamages(float newDamage)
    {
        damage = newDamage;
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null) 
            {
                damageable.TakeDamage(damage, TrapType.NewMaze);
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }


    }

    /*private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable != null && other.gameObject.CompareTag("Player"))
        {
            damageable.TakeDamage(damage, TrapType.NewMaze);
            Debug.LogError(damage);
            Destroy(gameObject);
            
        }
        else
        {
            Destroy(gameObject);
        }
    }*/
}
