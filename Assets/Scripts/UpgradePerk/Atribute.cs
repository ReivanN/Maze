using UnityEngine;

public class Atribute
{
    public string name;
    public string description;
    public AtributeType type;
    public float value;
    public int cost;

    public Atribute(string name, string description, AtributeType type, float value, int cost)
    {
        this.name = name;
        this.description = description;
        this.type = type;
        this.value = value;
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
    Invinñible,
    Bomb,

    IceBomb,
    FireBomb
}