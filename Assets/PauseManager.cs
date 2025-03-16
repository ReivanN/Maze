using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.Timeline.DirectorControlPlayable;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pause;
    [SerializeField]private InputAction action;
    private bool isPaused = false;
    void OnEnable()
    {
        action.Enable();
        action.performed += OnPause;
    }

    void OnDisable()
    {
        action.Disable();
        action.performed -= OnPause;
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    void Pause()
    {
        pause.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    void Resume()
    {
        pause.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void OnMenu() 
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Exit() 
    {
        Application.Quit();
    }
}
