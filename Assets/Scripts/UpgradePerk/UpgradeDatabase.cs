using System.Collections.Generic;
using UnityEngine;

public class UpgradeDatabase : MonoBehaviour
{
    public static List<Upgrade> allUpgrades = new List<Upgrade>
    {
        new Upgrade("Health +20%", "Increases health by 20%", UpgradeType.HealthBoost, 1.2f),
        new Upgrade("Damage +10%", "Increases damage by 10%", UpgradeType.DamageIncrease, 1.1f),
        new Upgrade("FireRate +15%", "Increases FireRate speed by 15%", UpgradeType.FireRate, 0.15f),
        new Upgrade("HPBonus +30", "Add a 30HP at your currentHP", UpgradeType.HpPlus, 30f)
    };
}
