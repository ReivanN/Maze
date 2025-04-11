using UnityEngine;

public class Atribute
{
    public string name;
    public string description;
    public AtributeType type;
    //public string requiredAttributeName;
    public int cost;

    public Atribute(string name, string description, AtributeType type, int cost)
    {
        this.name = name;
        this.description = description;
        this.type = type;
        this.cost = cost;
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
    Invin�ible,
    Bomb,

    IceBomb,
    FireBomb
}