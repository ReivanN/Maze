using System.Collections.Generic;
using UnityEngine;

public class UpgradeDatabase : MonoBehaviour
{
    public static List<Upgrade> allUpgrades = new List<Upgrade>
    {
        new Upgrade("�������� +20%", "����������� �������� �� 20%", UpgradeType.HealthBoost, 1.2f),
        new Upgrade("���� +10%", "����������� ���� �� 10%", UpgradeType.DamageIncrease, 1.1f),
        new Upgrade("�������� +15%", "����������� �������� ������������ �� 15%", UpgradeType.SpeedBoost, 1.15f)
    };
}
