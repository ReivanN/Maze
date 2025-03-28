using UnityEngine;

[CreateAssetMenu(fileName = "GameDifficulty", menuName = "Maze/GameDifficulty")]
public class GameDifficulty : ScriptableObject
{
    public string difficultyName;
    public float spawnRateMultiplier = 1f;
}
