using UnityEngine;

public class Atribute
{
    public string name;
    public string description;
    public AtributeType type;
    //public string requiredAttributeName;
    public int cost;
    public Sprite icon;

    public Atribute(string name, string description, AtributeType type, int cost, Sprite icon)
    {
        this.name = name;
        this.description = description;
        this.type = type;
        this.cost = cost;
        this.icon = icon;
    }
}

public enum AtributeType
{
    Shield,
    IceBullet,
    FireBullet,
    LiansBullet,
    PoisonBullets,
    piercingBullets,
    GrabBullets,
    Invinñible,
    Bomb,

    IceBomb,
    FireBomb
}