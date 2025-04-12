using UnityEngine;

public class Trap : MonoBehaviour
{

    public TrapData trapData;

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            damageable.TakeDamage(trapData.damage, trapData.trapType);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            damageable.TakeDamage(trapData.damage, trapData.trapType);
        }
    }*/
}

public enum TrapType 
{
    NewMaze,
    SaveMaze
}
