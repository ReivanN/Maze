using UnityEngine;

[CreateAssetMenu(fileName = "MazeSettings", menuName = "Maze/MazeSettings")]
public class MazeSettings : ScriptableObject
{
    [Header("Count Objects")]
    public int enemyCount = 8;
    public int trapCount = 8;

    [Header("Prefabs Objects")]
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject[] trapPrefabs;
    public GameObject enemyPrefab;
    public GameObject playerPrefab;
    public GameObject startPointPrefab;
    public GameObject exitPointPrefab;
    public GameObject exitBossRoomPrefab;
    public GameObject Trader;
    public GameObject bossPrefab;

}
