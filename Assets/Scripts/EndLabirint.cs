using TMPro;
using UnityEngine;

public class EndLabirint : MonoBehaviour
{
    private bool playerInZone = false;
    private TextMeshProUGUI endText;

    private void Start()
    {
        GameObject endObject = GameObject.FindGameObjectWithTag("End");
        if (endObject != null)
        {
            endText = endObject.GetComponent<TextMeshProUGUI>();
            endText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Объект с тегом 'End' не найден.");
        }
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
            if (endText != null)
            {
                endText.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            if (endText != null)
            {
                endText.gameObject.SetActive(false);
            }
        }
    }

    private void TriggerNewMaze()
    {
        if (endText != null)
        {
            endText.gameObject.SetActive(false);
        }

        if (MazeManager.Instance.savedMaze != null)
        {
            MazeManager.Instance.savedMaze = null;
        }

        LevelManager.Instance.LevelCompleted();
        MazeManager.Instance.NewMaze();
    }
}
