using UnityEngine;

[CreateAssetMenu(fileName = "NewHealthBonus", menuName = "Bonuses/Health Bonus")]
public class HealthBonus : BonusItem
{
    public float healthIncrease;

    public override void ApplyBonus(GameObject target)
    {
        TopDownCharacterController playerHealth = target.GetComponent<TopDownCharacterController>();
        if (playerHealth != null)
        {
            playerHealth.IncreaseHealth(healthIncrease);
            Debug.Log($"{bonusName} применён! HP увеличено на {healthIncrease}");
        }
    }
}
