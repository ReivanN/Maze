using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int unlockedEnemyCount = 1;
    public int level = 20;
    public int completedlevel = 19;
    public int pass = 10;
    public float health = 100;
    public float maxHealth = 100;
    public float damage = 10;
    public float fireRate = 1f;
    public int ricochets = 0;
    public int coins = 1000;
    public float shieldValue = 50f;

    public int damageType = 0;

    public bool Shield = false;

    public bool IceBiom = true;
    public bool VelkanBiom = true;
    public bool JungleBiom = true;


    public List<string> appliedUpgrades = new List<string>();
    public List<string> appliedAttributes = new List<string>();
}