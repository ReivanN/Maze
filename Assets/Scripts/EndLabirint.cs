using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLabirint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene("MazeScene");
        }
    }
}
