using System.Collections.Generic;
using UnityEngine;

public class UpgradeDatabase : MonoBehaviour
{
    public static List<Upgrade> allUpgrades = new List<Upgrade>
    {
        new Upgrade("Health +20%", "Increases health by 20%", UpgradeType.HealthBoost, 1.2f, 20),
        new Upgrade("Damage +10%", "Increases damage by 10%", UpgradeType.DamageIncrease, 1.1f, 10),
        new Upgrade("FireRate +15%", "Increases FireRate speed by 15%", UpgradeType.FireRate, 0.15f, 15),
        new Upgrade("HPBonus +30", "Add a 30HP at your currentHP", UpgradeType.HpPlus, 30f, 15),
        new Upgrade("Ricoshet bullets 1", "Add bulltes ricoshet (+1)",UpgradeType.Ricoshet, 1f, 50),
        new Upgrade("Ricoshet bullets 2", "Add bulltes ricoshet (+2)",UpgradeType.Ricoshet, 3f, 100),
        new Upgrade("Ricoshet bullets 3", "Add bulltes ricoshet (+1)",UpgradeType.Ricoshet, 5f, 150),

    };

    public static List<Atribute> allAtributes = new List<Atribute>
    {
        new Atribute("Shield", "Add a shield for player. (+50)", AtributeType.Shield, 50f, 45),
    };
}
