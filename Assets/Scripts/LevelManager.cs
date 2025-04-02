using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private GameDifficulty[] difficultyPresets;
    private int currentLevel = 1;
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
            currentLevel = 1;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void LevelStarted() 
    {
        Debug.Log("Was Started Level " + completedLevels);
    }
    public void LevelUI(TextMeshProUGUI textMeshProUGUI) 
    {
        textMeshProUGUI.text = "Level " + currentLevel;
    }
    public void LevelCompleted()
    {
        currentLevel++;
        completedLevels++;
        SaveProgress();
        Debug.Log("Was Completed Level " + completedLevels);
    }

    private void UpdateDifficulty()
    {
        int index = Mathf.Min(completedLevels / 5, difficultyPresets.Length - 1);
        currentDifficulty = difficultyPresets[index];
    }

    private void SaveProgress()
    {
        GameData gameData = SaveManager.Instance.Load();
        gameData.level = currentLevel;
        SaveManager.Instance.Save(gameData);
    }


    private void LoadProgress()
    {
        GameData data = SaveManager.Instance.Load();
        if (data != null) 
        {
            currentLevel = data.level;
        }
        UpdateDifficulty();
    }

    public void ResetProgress()
    {
        SaveManager.Instance.DeleteSave();
        completedLevels = 0;
        UpdateDifficulty();
    }
}
