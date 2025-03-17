using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame() 
    {
        SceneManager.LoadScene("MazeScene");
    }

    public void EndGame() 
    {
        Application.Quit();
    }
}
