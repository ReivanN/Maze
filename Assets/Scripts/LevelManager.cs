using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private GameDifficulty[] difficultyPresets;
    private int completedLevels = 0;
    private GameDifficulty currentDifficulty;

    public int CompletedLevels => completedLevels;
    public GameDifficulty CurrentDifficulty => currentDifficulty;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LevelCompleted()
    {
        completedLevels++;
        Debug.Log("Was Completed Level " + completedLevels);
        SaveProgress();
        UpdateDifficulty();
    }

    private void UpdateDifficulty()
    {
        int index = Mathf.Min(completedLevels / 5, difficultyPresets.Length - 1);
        currentDifficulty = difficultyPresets[index];
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt("CompletedLevels", completedLevels);
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        completedLevels = PlayerPrefs.GetInt("CompletedLevels", 0);
        UpdateDifficulty();
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey("CompletedLevels");
        completedLevels = 0;
        UpdateDifficulty();
    }
}
