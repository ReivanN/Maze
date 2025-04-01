using UnityEngine;

[CreateAssetMenu(fileName = "GameDifficulty", menuName = "Maze/GameDifficulty")]
public class GameDifficulty : ScriptableObject
{
    public string difficultyName;

    [Header("Height and Width")]
    public int width;
    public int height;

    public float spawnRateMultiplier = 1f;
}
