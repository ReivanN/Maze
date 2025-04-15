using System.Collections.Generic;
using UnityEngine;

public class AtributeDataBase : MonoBehaviour
{
    public static List<Atribute> allAttributes;

    private void Awake()
    {
        InitialiseAttribute();
    }
    public static void InitialiseAttribute()
    {
        allAttributes = new List<Atribute>()
        {
            LoadDataBase("Shield", "Add for player a shield", AtributeType.Shield, 50),
            LoadDataBase("Ice Bullets", "Add an ice bulltes which slown down the enemy", AtributeType.IceBullet, 100),
            LoadDataBase("Fire Bullets", "Add the fire bullets, which intermittent damage (1.2) for 3 seconds. ", AtributeType.FireBullet, 75),
            LoadDataBase("Poison Bullets", "Add the poison bullets, which intermittent damage (5) for 5 seconds. ", AtributeType.PoisonBullets, 135)
        };
    }

    public static Atribute LoadDataBase(string name, string description, AtributeType type, int cost)
    {
        Sprite icon = Resources.Load<Sprite>($"Icons/{name}");
        return new Atribute(name, description, type, cost, icon);
    }
}
