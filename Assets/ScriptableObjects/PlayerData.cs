using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Maze/PlayerData")]
public class PlayerData : ScriptableObject
{
    public int health;
    public int damage;
    public float moveSpeed;
}
