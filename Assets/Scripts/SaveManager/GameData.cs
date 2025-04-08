using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{

    public int level = 1;
    public int completedlevel = 0;
    public float health = 100;
    public float maxHealth = 100;
    public float damage = 10;
    public float fireRate = 0.5f;
    public int ricochets = 0;

    public int coins = 0;

    public List<string> appliedUpgrades = new List<string>();
}