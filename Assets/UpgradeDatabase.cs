using System.Collections.Generic;
using UnityEngine;

public class UpgradeDatabase : MonoBehaviour
{
    public static List<Upgrade> allUpgrades = new List<Upgrade>
    {
        new Upgrade("Здоровье +20%", "Увеличивает здоровье на 20%", UpgradeType.HealthBoost, 1.2f),
        new Upgrade("Урон +10%", "Увеличивает урон на 10%", UpgradeType.DamageIncrease, 1.1f),
        new Upgrade("Скорость +15%", "Увеличивает скорость передвижения на 15%", UpgradeType.SpeedBoost, 1.15f)
    };
}
