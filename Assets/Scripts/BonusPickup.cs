using UnityEngine;

public class BonusPickup : MonoBehaviour
{
    public BonusItem bonus;

    private void OnTriggerEnter(Collider  other)
    {
        if (other.CompareTag("Player"))
        {
            bonus.ApplyBonus(other.gameObject);
            Destroy(gameObject);
        }
    }
}
