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
    public float fireRate = 1f;
    public int ricochets = 0;
    public int coins = 1000;

    public int damageType = 0;

    public List<string> appliedUpgrades = new List<string>();
    public List<string> appliedAttributes = new List<string>();
}