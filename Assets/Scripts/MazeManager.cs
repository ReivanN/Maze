using UnityEngine;
using UnityEngine.SceneManagement;

public class MazeManager : MonoBehaviour
{
    public static MazeManager Instance;

    public int[,] savedMaze;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        savedMaze = null;
    }

    public void SaveMaze(int[,] maze)
    {
        savedMaze = maze.Clone() as int[,];
    }

    public void SameMaze()
    {
        MazeGenerator.Instance.RegenerateMaze();
    }

    public void NewMaze()
    {
        savedMaze = null;
        MazeGenerator.Instance.RegenerateMaze();
    }
}
