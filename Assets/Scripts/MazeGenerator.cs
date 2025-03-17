using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] public int width = 21;
    [SerializeField] public int height = 21;
    private int[,] maze;
    private System.Random rand = new System.Random();

    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject[] trapPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject startPointPrefab;
    [SerializeField] private GameObject exitPointPrefab;

    private ObjectPool wallPool;
    private ObjectPool floorPool;
    private ObjectPool enemyPool;
    private List<ObjectPool> bombPools;
    private ObjectPool playerPool;
    private ObjectPool startPointPool;
    private ObjectPool exitPointPool;


    private Vector2Int startPosition ;
    private Vector2Int exitPosition;

    void Start()
    {
        wallPool = new ObjectPool(wallPrefab, 250, transform);
        floorPool = new ObjectPool(floorPrefab, 200, transform);
        enemyPool = new ObjectPool(enemyPrefab, 5, transform);
        playerPool = new ObjectPool(playerPrefab, 1, transform);
        startPointPool = new ObjectPool(startPointPrefab, 1, transform);
        exitPointPool = new ObjectPool(exitPointPrefab, 1, transform);
        bombPools = new List<ObjectPool>();
        foreach (var trap in trapPrefab)
        {
            bombPools.Add(new ObjectPool(trap, 10, transform));
        }

        if (MazeManager.Instance.savedMaze != null)
        {
            maze = MazeManager.Instance.savedMaze;
            DefineStartAndExit();
            SpawnEntities();
            SpawnPlayer();
        }
        else
        {
            GenerateAndSpawn();
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RegenerateMaze();
        }
    }
    
    public void GenerateAndSpawn()
    {
        GenerateMazeCoroutine();
        PlaceEnemiesAndTraps(5,5);
        DefineStartAndExit();
        SpawnEntities();
        SpawnPlayer();
        MazeManager.Instance.SaveMaze(maze);
    }

    public void GenerateMazeCoroutine()
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
            List<Vector2Int> neighbors = GetUnvisitedNeighbors(current);

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

        }
    }

    List<Vector2Int> GetUnvisitedNeighbors(Vector2Int pos)
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

    System.Random rand = new System.Random();

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

    void DefineStartAndExit()
    {
        startPosition = new Vector2Int(1, 1);
        exitPosition = FindFarthestExit(startPosition);
    }

Vector2Int FindFarthestExit(Vector2Int start)
{
    Queue<Vector2Int> queue = new Queue<Vector2Int>();
    Dictionary<Vector2Int, int> distances = new Dictionary<Vector2Int, int>();
    HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

    queue.Enqueue(start);
    distances[start] = 0;
    visited.Add(start);

    Vector2Int farthestPoint = start;
    int maxDistance = 0;
    float maxEuclideanDist = 0;

    while (queue.Count > 0)
    {
        Vector2Int current = queue.Dequeue();
        int currentDistance = distances[current];

        foreach (Vector2Int dir in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
        {
            Vector2Int next = current + dir;
            if (next.x > 0 && next.y > 0 && next.x < width - 1 && next.y < height - 1 && (maze[next.x, next.y] == 1 || maze[next.x, next.y] == 2)|| maze[next.x, next.y]== 3)
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
                InstantiateFromPool(floorPool, position);
                int randomTrapIndex = rand.Next(bombPools.Count);
                InstantiateFromPool(bombPools[randomTrapIndex], position + Vector3.up * 0.5f);
            }
            else if (maze[x, y] == 3)
            {
                InstantiateFromPool(floorPool, position);
                InstantiateFromPool(enemyPool, position);
            }
        }
    }

    InstantiateFromPool(startPointPool, new Vector3(startPosition.x, 0.01f, startPosition.y));
    InstantiateFromPool(exitPointPool, new Vector3(exitPosition.x, 0.01f, exitPosition.y));
}


    void SpawnPlayer()
    {
        Vector3 spawnPosition = new Vector3(startPosition.x, 0, startPosition.y);
        InstantiateFromPool(playerPool, spawnPosition);
    }

    private void InstantiateFromPool(ObjectPool pool, Vector3 position)
    {
        GameObject obj = pool.Get();
        obj.transform.position = position;
        //obj.gameObject.SetActive(true);
    }
    private void ClearMaze()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                ObjectPool pool = GetPoolForObject(child.gameObject);
                if (pool != null)
                {
                    pool.Return(child.gameObject);
                }
                else
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }

    private ObjectPool GetPoolForObject(GameObject obj)
    {
        if (obj.name.Contains(wallPrefab.name))
            return wallPool;
        if (obj.name.Contains(floorPrefab.name))
            return floorPool;
        if (obj.name.Contains(enemyPrefab.name))
            return enemyPool;

        // Поиск по списку ловушек
        foreach (var pool in bombPools)
        {
            if (obj.name.Contains(pool.prefab.name))
                return pool;
        }

        if (obj.name.Contains(playerPrefab.name))
            return playerPool;
        if (obj.name.Contains(startPointPrefab.name))
            return startPointPool;
        if (obj.name.Contains(exitPointPrefab.name))
            return exitPointPool;

        return null;
    }

    public void RegenerateMaze()
    {
        ClearMaze();
        GenerateAndSpawn();
    }
}
