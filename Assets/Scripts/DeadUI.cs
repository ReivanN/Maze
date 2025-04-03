using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadUI : MonoBehaviour
{
    public GameObject Dead;
    public void Activate() 
    {
        Dead.SetActive(true);
    }
    public void Restart() 
    {
        MazeManager.Instance.NewMaze();
    }

    public void MainMenu() 
    {
        SceneManager.LoadScene(0);
    }
}
