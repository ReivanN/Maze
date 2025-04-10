using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private GameDifficulty[] difficultyPresets;
    private int currentLevel = 1;
    private int completedLevels = 0;
    private GameDifficulty currentDifficulty;
    public GameDifficulty CurrentDifficulty => currentDifficulty;
    public int Level => completedLevels;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            UpdateDifficulty();
            LoadProgress();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    
    public void LevelStarted() 
    {
        UpdateDifficulty();
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
        UpdateDifficulty();
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
        gameData.completedlevel = completedLevels;

        SaveManager.Instance.Save(gameData);
    }


    public void LoadProgress()
    {
        GameData data = SaveManager.Instance.Load();
        if (data != null)
        {
            currentLevel = data.level;
            completedLevels = data.completedlevel;
        }
        else
        {
            currentLevel = 1;
            completedLevels = 0;
        }
        UpdateDifficulty();
    }

    public void ResetProgress()
    {
        SaveManager.Instance.DeleteSave();
        completedLevels = 0;
        currentLevel = 1;
        UpdateDifficulty();
    }
}
