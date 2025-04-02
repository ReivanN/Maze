using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Maze/PlayerData")]
public class PlayerData : ScriptableObject
{
    public  float health;
    public  float damage;
    public  float moveSpeed;
}
