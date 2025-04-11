using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static List<string> acquiredUpgradeNames = new List<string>();

    public static List<Upgrade> GetRandomUpgrades()
    {
        var activeAttributeNames = AttributeManager.activeAttributes.Select(a => a.name).ToList();

        var availableUpgrades = UpgradeDatabase.allUpgrades
            .Where(u =>
                (string.IsNullOrEmpty(u.requiredUpgradeName) || acquiredUpgradeNames.Contains(u.requiredUpgradeName)) &&
                (string.IsNullOrEmpty(u.requiredAttributeName) || activeAttributeNames.Contains(u.requiredAttributeName))
            )
            .OrderBy(u => Random.value)
            .Take(3)
            .ToList();

        return availableUpgrades;
    }

    public static void AddAcquiredUpgrade(Upgrade upgrade)
    {
        if (!acquiredUpgradeNames.Contains(upgrade.name))
            acquiredUpgradeNames.Add(upgrade.name);
    }
}
