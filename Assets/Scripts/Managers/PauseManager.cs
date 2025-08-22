using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.Timeline.DirectorControlPlayable;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pause;
    [SerializeField] private GameObject tutorial;
    [SerializeField] private GameObject settings;
    [SerializeField] private GameObject main;
    [SerializeField] private GameObject shop;
    [SerializeField] private InputAction action;
    private bool isPaused = false;
    private void Start()
    {
        
    }
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
        if (shop.activeInHierarchy == true)
            return;
        if (isPaused && main.activeInHierarchy)
            Resume();
        else if (isPaused && settings.activeInHierarchy)
            CloseSettings();
        else
            Pause();
    }

    void Pause()
    {
        pause.SetActive(true);
        PauseGameState.Pause();
        isPaused = true;
    }
    public void OpenTutorial() 
    {
        tutorial.SetActive(true);
        main.SetActive(false);
    }

    void CloseTutorial() 
    {
        tutorial.SetActive(false);
        main.SetActive(true);
    }

    public void OpenSettings()
    {
        settings.SetActive(true);
        main.SetActive(false);
    }

    void CloseSettings()
    {
        settings.SetActive(false);
        main.SetActive(true);
    }

    void Resume()
    {
        pause.SetActive(false);
        PauseGameState.Resume();
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
