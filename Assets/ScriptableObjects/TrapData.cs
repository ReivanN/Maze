using UnityEngine;

[CreateAssetMenu(fileName = "TrapData", menuName = "Maze/TrapData")]
public class TrapData : ScriptableObject
{
    public TrapType trapType;
    public int damage;
    public float health;
}
