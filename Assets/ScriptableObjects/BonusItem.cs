using UnityEngine;

public abstract class BonusItem : ScriptableObject
{
    public string bonusName;
    public string description;

    public abstract void ApplyBonus(GameObject target);
}