using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 direction;
    private float speed;

    private float lifetime = 5f;

    public void Initialize(Vector3 bulletDirection, float bulletSpeed)
    {
        direction = bulletDirection;
        speed = bulletSpeed;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }
    void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
    }
}
