using UnityEngine;

public class BlueTrap : MonoBehaviour
{
    private IHealthBar healthBar;
    private float HP = 100;
    private float currentHP;
    private void Awake()
    {
        currentHP = HP;
        healthBar = GetComponentInChildren<IHealthBar>();
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 bulletPosition = collision.transform.position;
                Vector3 hitDirection = (transform.position - bulletPosition).normalized;
                Rigidbody bulletRb = collision.gameObject.GetComponent<Rigidbody>();
                float pushForce = 5f;
                rb.AddForce(hitDirection * pushForce, ForceMode.Impulse);
                TakeDamage(10, TrapType.NewMaze);
            }
        }

        if (collision.gameObject.CompareTag("Player")) 
        {
            Destroy(this.gameObject);
        }
    }

    public void TakeDamage(int damage, TrapType trapType)
    {
        currentHP -= damage;
        healthBar?.UpdateHealthBar(currentHP, HP);

        if (currentHP <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
