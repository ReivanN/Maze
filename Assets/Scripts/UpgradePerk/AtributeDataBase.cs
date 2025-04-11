using System.Collections.Generic;
using UnityEngine;

public class AtributeDataBase : MonoBehaviour
{
    public static List<Atribute> allAttributes = new List<Atribute>
    {
        new Atribute("Shield", "Add for player a shield", AtributeType.Shield, 50),
        new Atribute("Ice Bullets", "Add an ice bulltes which slown down the enemy", AtributeType.IceBullet, 100),
        new Atribute("Спринтер", "Увеличивает скорость после уклонения", AtributeType.Shield,50),
    };
}
