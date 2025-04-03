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

    private ObjectPool wallPool;
    private ObjectPool floorPool;
    private ObjectPool enemyPool;

    private Vector2Int startPosition;
    private Vector2Int exitPosition;

    public static bool IsDone = false;

    void Start()
    {
        IsDone = false;
        wallPool = new ObjectPool(mazeSettings.wallPrefab, 250, transform);
        floorPool = new ObjectPool(mazeSettings.floorPrefab, 200, transform);
        enemyPool = new ObjectPool(mazeSettings.enemyPrefab, 5, transform);

        if (MazeManager.Instance.savedMaze != null)
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

        int enemyCount = Mathf.RoundToInt(mazeSettings.enemyCount * LevelManager.Instance.CurrentDifficulty.spawnRateMultiplier);
        int trapCount = Mathf.RoundToInt(mazeSettings.trapCount * LevelManager.Instance.CurrentDifficulty.spawnRateMultiplier);

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


    void PlaceEnemiesAndTraps(int enemyCount, int trapCount)
    {
        List<Vector2Int> possiblePositions = new List<Vector2Int>();

        for (int x = 1; x < width; x += 2)
        {
            for (int y = 1; y < height; y += 2)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (maze[x, y] == 1 && pos != startPosition && pos != exitPosition)
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
                Vector3 wallPosition = new Vector3(x, 0.5f, y);

                if (maze[x, y] == 0)
                {
                    InstantiateFromPool(wallPool, wallPosition);
                }
                else if (maze[x, y] == 1)
                {
                    InstantiateFromPool(floorPool, position);
                }
                else if (maze[x, y] == 2)
                {
                    // Здесь создаём только пол, ловушки будем инстанцировать отдельно
                    InstantiateFromPool(floorPool, position);
                }
                else if (maze[x, y] == 3)
                {
                    InstantiateFromPool(floorPool, position);
                    InstantiateFromPool(enemyPool, position);
                }
            }
        }

        Instantiate(mazeSettings.startPointPrefab, new Vector3(startPosition.x, 0.01f, startPosition.y), Quaternion.identity);
        Instantiate(mazeSettings.exitPointPrefab, new Vector3(exitPosition.x, 0.01f, exitPosition.y), Quaternion.identity);
    }

    void SpawnEnemies()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (maze[x, y] == 2) // 2 - бомба
                {
                    Vector3 trapPosition = new Vector3(x, 0.5f, y);

                    // Проверяем, есть ли рядом NavMesh
                    if (NavMesh.SamplePosition(trapPosition, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                    {
                        trapPosition = hit.position; // Перемещаем точку на NavMesh
                        //Debug.Log($"Бомба скорректирована и спавнится на NavMesh в {trapPosition}");
                    }
                    else
                    {
                        //Debug.LogError($"Не удалось найти ближайший NavMesh для бомбы в позиции {trapPosition}!");
                        continue; // Пропускаем спавн, если нет доступного NavMesh
                    }

                    // Спавним бомбу
                    GameObject bomb = Instantiate(
                        mazeSettings.trapPrefabs[rand.Next(mazeSettings.trapPrefabs.Length)],
                        trapPosition,
                        Quaternion.identity
                    );

                    // Проверяем, действительно ли объект на NavMesh
                    NavMeshAgent agent = bomb.GetComponent<NavMeshAgent>();
                    if (agent != null)
                    {
                        StartCoroutine(WaitForNavMesh(agent));
                    }
                }
            }
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


    void SpawnPlayer()
    {
        Vector3 spawnPosition = new Vector3(startPosition.x, 0, startPosition.y);
        Instantiate(mazeSettings.playerPrefab, spawnPosition, Quaternion.identity);
    }

    void InstantiateFromPool(ObjectPool pool, Vector3 position)
    {
        GameObject obj = pool.Get();
        obj.transform.position = position;
    }
}
