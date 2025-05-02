using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject Con;
    [SerializeField] private GameObject main;
    [SerializeField] private GameObject settings;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && settings.activeInHierarchy) 
        {
            main.SetActive(true);
            settings.SetActive(false);
        }
    }

    public void Start()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.SaveExists()) 
        {
            Con.SetActive(true);
        }
    }

    public void OpenSettings() 
    {
        settings.SetActive(true);
        main.SetActive(false);
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
