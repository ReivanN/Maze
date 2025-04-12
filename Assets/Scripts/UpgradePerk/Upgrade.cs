using UnityEngine;

[System.Serializable]
public class Upgrade
{
    public string name;
    public string description;
    public UpgradeType type;
    public float value;
    public int cost;
    public string requiredUpgradeName;
    public string requiredAttributeName;
    public Sprite icon;

    public Upgrade(string name, string description, UpgradeType type, float value, int cost, Sprite icon, string requiredUpgradeName = null, string requiredAttributeName = null)
    {
        this.name = name;
        this.description = description;
        this.type = type;
        this.value = value;
        this.cost = cost;
        this.requiredUpgradeName = requiredUpgradeName;
        this.requiredAttributeName = requiredAttributeName;
        this.icon = icon;
    }
}

public enum UpgradeType
{
    HpPlus,
    HealthBoost,
    DamageIncrease,
    FireRate,
    SpeedBullets,
    Ricoshet,
    UpgradeShield,
    ShieldRicoshet,
    ShieldIncrease,
    MoneyBoost,
    Regen,
    RaduisBomb
}
