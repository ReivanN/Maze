using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{

    public int level = 1;
    public float health = 100;
    public float maxHealth = 100;

    public List<string> appliedUpgrades = new List<string>();
}