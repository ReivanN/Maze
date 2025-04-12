using System.Collections.Generic;
using UnityEngine;

public static class UpgradeIcons
{
    public static Dictionary<string, Sprite> iconMap = new Dictionary<string, Sprite>();

    public static void LoadIcons()
    {
        iconMap["Health +20%"] = Resources.Load<Sprite>("Icons/Health");
        iconMap["Damage +10%"] = Resources.Load<Sprite>("Icons/Damage");
        iconMap["FireRate +15%"] = Resources.Load<Sprite>("Icons/Lightning");
        iconMap["HPBonus +30"] = Resources.Load<Sprite>("Icons/HPBonus");
        iconMap["Ricoshet bullets 1"] = Resources.Load<Sprite>("Icons/Ricoshet");
    }

    public static Sprite GetIcon(string upgradeName)
    {
        return iconMap.ContainsKey(upgradeName) ? iconMap[upgradeName] : null;
    }
}
