using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UpgradeManager : MonoBehaviour
{
    public static List<Upgrade> GetRandomUpgrades()
    {
        return UpgradeDatabase.allUpgrades.OrderBy(u => Random.value).Take(3).ToList();
    }
}
