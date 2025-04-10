[System.Serializable]
public class Upgrade
{
    public string name;
    public string description;
    public UpgradeType type;
    public float value;
    public int cost;

    public Upgrade(string name, string description, UpgradeType type, float value, int cost)
    {
        this.name = name;
        this.description = description;
        this.type = type;
        this.value = value;
        this.cost = cost;
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
