using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] private MazeSettings mazeSettings;

    private int[,] maze;
    private System.Random rand = new System.Random();

    private Vector2Int startPosition;
    private Vector2Int exitPosition;

    private int enemyCount;
    private int trapCount;

    public static bool IsDone = false;

    void Start()
    {
        IsDone = false;
        float spawnRateMultiplier = Mathf.Pow(1.15f, LevelManager.Instance.Level);
        enemyCount = Mathf.RoundToInt(mazeSettings.enemyCount * spawnRateMultiplier);
        trapCount = Mathf.RoundToInt(mazeSettings.trapCount * spawnRateMultiplier);

        if ((LevelManager.Instance.Level + 1) % 5 == 0)
        {
            StartCoroutine(GenerateBossRoom());
        }
        else if (MazeManager.Instance.savedMaze != null)
        {
            StartCoroutine(SpawnSave());
        }
        else
        {
            StartCoroutine(GenerateAndSpawn());
        }
    }



    IEnumerator GenerateAndSpawn()
    {
        yield return StartCoroutine(GenerateMazeCoroutine());
        yield return StartCoroutine(PlaceEnemiesAndTrapsCoroutine(enemyCount, trapCount));
        yield return StartCoroutine(DefineStartAndExitCoroutine());
        yield return StartCoroutine(SpawnEntitiesCoroutine());
        yield return StartCoroutine(NavMeshBaker.Instance.BakeNavMeshCoroutine());
        yield return StartCoroutine(SpawnEnemiesCoroutine());
        StartCoroutine(SpawnPlayerCoroutine());
        MazeManager.Instance.SaveMaze(maze);
        IsDone = true;
    }

    IEnumerator SpawnSave()
    {
        maze = MazeManager.Instance.savedMaze;
        yield return StartCoroutine(DefineStartAndExitCoroutine());
        yield return StartCoroutine(SpawnEntitiesCoroutine());
        yield return StartCoroutine(NavMeshBaker.Instance.BakeNavMeshCoroutine());
        yield return StartCoroutine(SpawnEnemiesCoroutine());
        StartCoroutine(SpawnPlayerCoroutine());
        IsDone = true;
    }

    IEnumerator PlaceEnemiesAndTrapsCoroutine(int enemyCount, int trapCount)
    {
        PlaceEnemiesAndTraps(enemyCount, trapCount);
        yield return null;
    }

    IEnumerator DefineStartAndExitCoroutine()
    {
        DefineStartAndExit();
        yield return null;
    }

    IEnumerator SpawnEntitiesCoroutine()
    {
        SpawnEntities();
        yield return null;
    }

    IEnumerator SpawnEnemiesCoroutine()
    {
        SpawnEnemies();
        yield return null;
    }

    IEnumerator SpawnPlayerCoroutine()
    {
        SpawnPlayer();
        yield return null;
    }

    int width = LevelManager.Instance.CurrentDifficulty.width;
    int height = LevelManager.Instance.CurrentDifficulty.height;

    IEnumerator GenerateMazeCoroutine()
    {
        maze = new int[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                maze[x, y] = 0;

        int startX = 1, startY = 1;
        maze[startX, startY] = 1;

        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        stack.Push(new Vector2Int(startX, startY));

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Peek();
            List<Vector2Int> neighbors = GetUnvisitedNeighbors(current, width, height);

            if (neighbors.Count > 0)
            {
                Vector2Int next = neighbors[rand.Next(neighbors.Count)];
                int wallX = (current.x + next.x) / 2;
                int wallY = (current.y + next.y) / 2;
                maze[wallX, wallY] = 1;
                maze[next.x, next.y] = 1;
                stack.Push(next);
            }
            else
            {
                stack.Pop();
            }

            yield return null;
        }
    }

    List<Vector2Int> GetUnvisitedNeighbors(Vector2Int pos, int width, int height)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        Vector2Int[] directions = {
            new Vector2Int(2, 0), new Vector2Int(-2, 0),
            new Vector2Int(0, 2), new Vector2Int(0, -2)
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int neighbor = pos + dir;
            if (neighbor.x > 0 && neighbor.x < width - 1 && neighbor.y > 0 && neighbor.y < height - 1)
            {
                if (maze[neighbor.x, neighbor.y] == 0)
                {
                    neighbors.Add(neighbor);
                }
            }
        }
        return neighbors;
    }

    void DefineStartAndExit()
    {
        startPosition = new Vector2Int(1, 1);
        Vector2Int? exit = FindFarthestExit(startPosition);

        if (exit.HasValue)
        {
            exitPosition = exit.Value;
        }
        else
        {
            Debug.LogWarning("Не удалось найти подходящую точку для выхода!");
        }
    }

    Vector2Int? FindFarthestExit(Vector2Int start)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, int> distances = new Dictionary<Vector2Int, int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(start);
        distances[start] = 0;
        visited.Add(start);

        Vector2Int? farthestPoint = null;
        int maxDistance = 0;
        float maxEuclideanDist = 0;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            int currentDistance = distances[current];

            foreach (Vector2Int dir in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                Vector2Int next = current + dir;

                if (next.x > 0 && next.y > 0 && next.x < width - 1 && next.y < height - 1 &&
                   (maze[next.x, next.y] == 1 || maze[next.x, next.y] == 2 || maze[next.x, next.y] == 3))
                {
                    if (!visited.Contains(next))
                    {
                        distances[next] = currentDistance + 1;
                        queue.Enqueue(next);
                        visited.Add(next);

                        float euclideanDist = Vector2Int.Distance(next, new Vector2Int(width - 2, height - 2));

                        if (distances[next] > maxDistance ||
                           (distances[next] == maxDistance && euclideanDist > maxEuclideanDist))
                        {
                            maxDistance = distances[next];
                            maxEuclideanDist = euclideanDist;
                            farthestPoint = next;
                        }
                    }
                }
            }
        }

        return farthestPoint;
    }

    bool IsNearStart(Vector2Int position, int minDistance, int maxDistance)
    {
        int dx = Mathf.Abs(position.x - startPosition.x);
        int dy = Mathf.Abs(position.y - startPosition.y);
        float distance = Mathf.Sqrt(dx * dx + dy * dy);
        return distance >= minDistance && distance <= maxDistance;
    }

    void PlaceEnemiesAndTraps(int enemyCount, int trapCount)
    {
        List<Vector2Int> possiblePositions = new List<Vector2Int>();

        for (int x = 1; x < width; x += 2)
        {
            for (int y = 1; y < height; y += 2)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (maze[x, y] == 1 && pos != startPosition && pos != exitPosition && !IsNearStart(pos, 3, 5))
                {
                    possiblePositions.Add(pos);
                }
            }
        }

        for (int i = 0; i < trapCount && possiblePositions.Count > 0; i++)
        {
            int index = rand.Next(possiblePositions.Count);
            Vector2Int trapPos = possiblePositions[index];
            possiblePositions.RemoveAt(index);
            maze[trapPos.x, trapPos.y] = 2;
        }

        for (int i = 0; i < enemyCount && possiblePositions.Count > 0; i++)
        {
            int index = rand.Next(possiblePositions.Count);
            Vector2Int enemyPos = possiblePositions[index];
            possiblePositions.RemoveAt(index);
            maze[enemyPos.x, enemyPos.y] = 3;
        }
    }

    void SpawnEntities()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x, 0, y);
                Vector3 wallPosition = new Vector3(x, 1f, y);

                if (maze[x, y] == 0)
                {
                    Instantiate(mazeSettings.wallPrefab, wallPosition, Quaternion.identity, transform);
                }
                else if (maze[x, y] == 1 || maze[x, y] == 2 || maze[x, y] == 3)
                {
                    Instantiate(mazeSettings.floorPrefab, position, Quaternion.identity, transform);
                }

                if (maze[x, y] == 3)
                {
                    Instantiate(mazeSettings.enemyPrefab, position, Quaternion.identity, transform);
                }
            }
        }

        Instantiate(mazeSettings.startPointPrefab, new Vector3(startPosition.x, 0.01f, startPosition.y), Quaternion.identity);
        if ((LevelManager.Instance.Level + 1) % 5 == 0) 
        {
            Instantiate(mazeSettings.exitBossRoomPrefab, new Vector3(exitPosition.x, 0.01f, exitPosition.y), Quaternion.identity);
        }
        else 
        {
            Instantiate(mazeSettings.exitPointPrefab, new Vector3(exitPosition.x, 0.01f, exitPosition.y), Quaternion.identity);
        }
        SpawnTrader();
    }

    void SpawnEnemies()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (maze[x, y] == 2)
                {
                    Vector3 trapPosition = new Vector3(x, 0.5f, y);

                    if (NavMesh.SamplePosition(trapPosition, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                    {
                        trapPosition = hit.position;
                    }
                    else
                    {
                        continue;
                    }

                    GameObject bomb = Instantiate(
                        mazeSettings.trapPrefabs[rand.Next(mazeSettings.trapPrefabs.Length)],
                        trapPosition,
                        Quaternion.identity
                    );

                    NavMeshAgent agent = bomb.GetComponent<NavMeshAgent>();
                    if (agent != null)
                    {
                        StartCoroutine(WaitForNavMesh(agent));
                    }
                }
            }
        }
    }

    void SpawnTrader()
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();

        // Проходим по всем ячейкам, чтобы найти тупики
        for (int x = 1; x < width - 1; x += 2)
        {
            for (int y = 1; y < height - 1; y += 2)
            {
                if (maze[x, y] != 1) continue;

                int openCount = 0;
                List<Vector2Int> openDirections = new List<Vector2Int>();

                // Проверяем соседей
                foreach (Vector2Int dir in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
                {
                    Vector2Int neighbor = new Vector2Int(x + dir.x, y + dir.y);
                    if (maze[neighbor.x, neighbor.y] == 1)
                    {
                        openCount++;
                        openDirections.Add(dir);
                    }
                }

                // Если есть только один проход, это тупик
                if (openCount == 1 && new Vector2Int(x, y) != startPosition && new Vector2Int(x, y) != exitPosition)
                {
                    deadEnds.Add(new Vector2Int(x, y));
                }
            }
        }

        if (deadEnds.Count > 0)
        {
            Vector2Int traderPos = deadEnds[rand.Next(deadEnds.Count)];
            GameObject trader = Instantiate(mazeSettings.Trader, new Vector3(traderPos.x, 0.01f, traderPos.y), Quaternion.identity);

            // Определяем направление торговца
            List<Vector2Int> possibleDirections = new List<Vector2Int>();

            foreach (Vector2Int dir in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                Vector2Int neighbor = traderPos + dir;
                if (maze[neighbor.x, neighbor.y] == 1)
                    possibleDirections.Add(dir);
            }

            // Если мы нашли проход, поворачиваем торговца
            if (possibleDirections.Count == 1)
            {
                Vector2Int direction = possibleDirections[0];
                trader.transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y));
            }
        }
        else
        {
            Debug.LogWarning("Не найдено подходящих тупиков для спавна торговца.");
        }
    }


    private IEnumerator WaitForNavMesh(NavMeshAgent agent)
    {
        yield return new WaitForSeconds(0.1f);

        if (!agent.isOnNavMesh)
        {
            Debug.LogError($"Бомба {agent.gameObject.name} заспавнилась вне NavMesh!");
        }
        else
        {
            Debug.Log($"Бомба {agent.gameObject.name} успешно на NavMesh.");
        }
    }

    IEnumerator GenerateBossRoom()
    {
        width = 15;
        height = 15;
        maze = new int[width, height];

        // Заполняем стены
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                maze[x, y] = 0;

        // Центр - пол
        for (int x = 1; x < width - 1; x++)
            for (int y = 1; y < height - 1; y++)
                maze[x, y] = 1;

        startPosition = new Vector2Int(1, 1);
        exitPosition = new Vector2Int(width - 2, height - 2);

        yield return StartCoroutine(SpawnEntitiesCoroutine());
        yield return StartCoroutine(NavMeshBaker.Instance.BakeNavMeshCoroutine());
        yield return StartCoroutine(SpawnBossCoroutine());
        StartCoroutine(SpawnPlayerCoroutine());
        IsDone = true;
    }

    IEnumerator SpawnBossCoroutine()
    {
        Vector3 bossPosition = new Vector3(width / 2f, 0, height / 2f);
        GameObject boss = Instantiate(mazeSettings.bossPrefab, bossPosition, Quaternion.identity);

        NavMeshAgent agent = boss.GetComponent<NavMeshAgent>();
        if (agent != null)
            yield return WaitForNavMesh(agent);
        else
            yield return null;
    }



    void SpawnPlayer()
    {
        Vector3 spawnPosition = new Vector3(startPosition.x, 0, startPosition.y);
        Instantiate(mazeSettings.playerPrefab, spawnPosition, Quaternion.identity);
    }
}
