[System.Serializable]
public class Upgrade
{
    public string name;
    public string description;
    public UpgradeType type;
    public float value;

    public Upgrade(string name, string description, UpgradeType type, float value)
    {
        this.name = name;
        this.description = description;
        this.type = type;
        this.value = value;
    }
}

public enum UpgradeType
{
    HealthBoost,
    DamageIncrease,
    SpeedBoost
}
