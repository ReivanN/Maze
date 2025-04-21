using TMPro;
using UnityEngine;

public class EndLabirint : MonoBehaviour
{
    private bool playerInZone = false;
    private GameObject end;
    private void Start()
    {
        end = GameObject.FindGameObjectWithTag("End");
        end.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (playerInZone && Input.GetKeyDown(KeyCode.E))
        {
            TriggerNewMaze();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            end.
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            EndLevelUI.Instance.HidePrompt();
        }
    }

    private void TriggerNewMaze()
    {
        EndLevelUI.Instance.HidePrompt();

        if (MazeManager.Instance.savedMaze != null)
        {
            MazeManager.Instance.savedMaze = null;
        }

        LevelManager.Instance.LevelCompleted();
        MazeManager.Instance.NewMaze();
    }
}
