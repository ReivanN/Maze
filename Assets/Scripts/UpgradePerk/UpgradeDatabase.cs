using System.Collections.Generic;
using UnityEngine;

public class UpgradeDatabase : MonoBehaviour
{
    public static List<Upgrade> allUpgrades;

    private void Awake()
    {
        InitializeUpgrades();
    }

    private static void InitializeUpgrades()
    {
        allUpgrades = new List<Upgrade>
        {
            CreateUpgrade("Health +20%", "Increases health by 20%", UpgradeType.HealthBoost, 1.2f, 20),
            CreateUpgrade("Damage +10%", "Increases damage by 10%", UpgradeType.DamageIncrease, 1.1f, 10),
            CreateUpgrade("FireRate +15%", "Increases fire rate by 15%", UpgradeType.FireRate, 0.15f, 15),
            CreateUpgrade("HPBonus +30", "Add 30 HP to your current HP", UpgradeType.HpPlus, 30f, 15),
            CreateUpgrade("Ricoshet bullets 1", "Adds ricochet bullets (+1)", UpgradeType.Ricoshet, 1f, 50),
            CreateUpgrade("Ricoshet bullets 2", "Adds ricochet bullets (+2)", UpgradeType.Ricoshet, 3f, 100, "Ricoshet bullets 1"),
            CreateUpgrade("Ricoshet bullets 3", "Adds ricochet bullets (+1)", UpgradeType.Ricoshet, 5f, 150, "Ricoshet bullets 2")
        };
    }

    private static Upgrade CreateUpgrade(string name, string description, UpgradeType type, float value, int cost, string requiredUpgradeName = null)
    {
        Sprite icon = Resources.Load<Sprite>($"Icons/{name}");
        return new Upgrade(name, description, type, value, cost, icon, requiredUpgradeName);
    }
}
