using UnityEngine;

public class SpreadNemey : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint; 
    public float shootInterval = 2f;
    public float currentbulletSpeed = 10f;
    public int bulletDamage = 10;
    public int bulletCount = 12;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip clip;

    private float shootTimer;

    void Update()
    {
        shootTimer += Time.deltaTime;

        if (shootTimer >= shootInterval)
        {
            ShootInCircle();
            shootTimer = 0f;
        }
    }

    private void ShootInCircle()
    {
        float angleStep = 360f / bulletCount;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * angleStep;
            Quaternion rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 direction = rotation * Vector3.forward;
            direction = direction.normalized;

            if (bulletPrefab != null && firePoint != null)
            {
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));

                BulletEnemy bulletScript = bullet.GetComponent<BulletEnemy>();
                if (bulletScript != null)
                {
                    audioSource.PlayOneShot(clip);
                    bulletScript.Initialize(direction, currentbulletSpeed, 10, 0);
                    bulletScript.SetDamage(bulletDamage);
                }
            }
        }
    }
}
