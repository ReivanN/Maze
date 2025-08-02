using UnityEngine;

public class BonusPickup : MonoBehaviour
{
    public BonusItem bonus;
    public GameObject aply;
    private void OnTriggerEnter(Collider  other)
    {
        if (other.CompareTag("Player"))
        {
            bonus.ApplyBonus(other.gameObject);
            Instantiate(aply, transform.position, Quaternion.identity);
            Destroy(gameObject);
            
        }
    }
}
