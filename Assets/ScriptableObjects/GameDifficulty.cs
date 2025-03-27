using UnityEngine;

[CreateAssetMenu(fileName = "GameDifficulty", menuName = "Maze/GameDifficulty")]
public class GameDifficulty : ScriptableObject
{
    public string difficultyName;
    public float enemyHealthMultiplier = 1f;
    public float enemyDamageMultiplier = 1f;
    public float spawnRateMultiplier = 1f;
}
