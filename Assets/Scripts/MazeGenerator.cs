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
    [SerializeField] private GameObject trapPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject startPointPrefab;
    [SerializeField] private GameObject exitPointPrefab;

    private Vector2Int startPosition;
    private Vector2Int exitPosition;

    void Start()
    {
        StartCoroutine(GenerateAndSpawn());
    }

    IEnumerator GenerateAndSpawn()
    {
        yield return StartCoroutine(GenerateMazeCoroutine());

        PlaceTraps(5);
        DefineStartAndExit();
        InstantiateMaze();
        SpawnPlayer();
    }

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

            yield return null;
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

    void PlaceTraps(int count)
    {
        List<Vector2Int> possiblePositions = new List<Vector2Int>();

        for (int x = 1; x < width; x += 2)
            for (int y = 1; y < height; y += 2)
                if (maze[x, y] == 1)
                    possiblePositions.Add(new Vector2Int(x, y));

        for (int i = 0; i < count && possiblePositions.Count > 0; i++)
        {
            int index = rand.Next(possiblePositions.Count);
            Vector2Int trapPos = possiblePositions[index];
            possiblePositions.RemoveAt(index);
            maze[trapPos.x, trapPos.y] = 2;
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

        queue.Enqueue(start);
        distances[start] = 0;

        Vector2Int farthestPoint = start;
        int maxDistance = 0;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            int currentDistance = distances[current];

            foreach (Vector2Int dir in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                Vector2Int next = current + dir;

                if (next.x > 0 && next.y > 0 && next.x < width - 1 && next.y < height - 1 && (maze[next.x, next.y] == 1 || maze[next.x, next.y] == 2))
                {
                    if (!distances.ContainsKey(next))
                    {
                        distances[next] = currentDistance + 1;
                        queue.Enqueue(next);

                        if (distances[next] > maxDistance)
                        {
                            maxDistance = distances[next];
                            farthestPoint = next;
                        }
                    }
                }
            }
        }

        return farthestPoint;
    }


    void InstantiateMaze()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x, 0, y);
                Vector3 Wallposition = new Vector3(x, 0.5f, y);

                if (maze[x, y] == 0)
                {
                    Instantiate(wallPrefab, Wallposition, Quaternion.identity);
                }
                else if (maze[x, y] == 1)
                {
                    Instantiate(floorPrefab, position, Quaternion.identity);
                }
                else if (maze[x, y] == 2)
                {
                    Instantiate(floorPrefab, position, Quaternion.identity);
                    Instantiate(trapPrefab, position + Vector3.up * 0.5f, Quaternion.identity);
                }
            }
        }

        Instantiate(startPointPrefab, new Vector3(startPosition.x, 0.01f, startPosition.y), Quaternion.identity);
        Instantiate(exitPointPrefab, new Vector3(exitPosition.x, 0.01f, exitPosition.y), Quaternion.identity);
    }

    void SpawnPlayer()
    {
        Vector3 spawnPosition = new Vector3(startPosition.x, 0, startPosition.y);
        Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
    }
}
