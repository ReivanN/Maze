using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLabirint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(MazeManager.Instance.savedMaze != null) 
            {
                MazeManager.Instance.savedMaze = null;
            }
            SceneManager.LoadScene("MazeScene");
        }
    }
}
