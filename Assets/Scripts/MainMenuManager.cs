using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject Con;

    public void Start()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.SaveExists()) 
        {
            Con.SetActive(true);
        }
    }
    public void ContinueGame() 
    {
        SaveManager.Instance.Load();
        SceneManager.LoadScene("MazeScene");
    }

    public void StartGame() 
    {
        SaveManager.Instance.DeleteSave();
        LevelManager.Instance.LoadProgress();
        SceneManager.LoadScene("MazeScene");
    }

    public void EndGame() 
    {
        Application.Quit();
    }
}
