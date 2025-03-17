using UnityEngine;

public class Blue : MonoBehaviour
{
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
            }
        }
    }
}
