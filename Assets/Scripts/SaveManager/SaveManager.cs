using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private string savePath => Path.Combine(Application.persistentDataPath, "save.json");

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DebugRef();
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    public void DebugRef() 
    {
        GameData data = Load();
        Debug.LogError("Level " + data.level);
        Debug.LogError("Level " + data.completedlevel);
    }
    public void Save(GameData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }

    public GameData Load()
    {
        if (!File.Exists(savePath))
            return new GameData();

        string json = File.ReadAllText(savePath);
        return JsonUtility.FromJson<GameData>(json);
    }

    public void SaveUpgrade(Upgrade upgrade)
    {
        GameData data = Load();
        data.appliedUpgrades.Add(upgrade.name);
        Save(data);
    }

    public void SaveAttribute(Atribute attribute) 
    {
        GameData data = Load();
        data.appliedAttributes.Add(attribute.name);
        Save(data);
    }

    public void RemoveAttribute(Atribute attribute)
    {
        GameData data = Load();
        if (data.appliedAttributes.Contains(attribute.name))
        {
            data.appliedAttributes.Remove(attribute.name);
            Save(data);
        }
    }

    public void ReplaceAttribute(Atribute attributeToRemove, Atribute newAttribute)
    {
        GameData data = Load();

        if (data.appliedAttributes.Contains(attributeToRemove.name))
        {
            data.appliedAttributes.Remove(attributeToRemove.name);
        }

        if (!data.appliedAttributes.Contains(newAttribute.name))
        {
            data.appliedAttributes.Add(newAttribute.name);
        }

        Save(data);
    }


    public bool SaveExists() => File.Exists(savePath);


    public void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            MazeManager.Instance.savedMaze = null;
            Debug.Log("Сохранение удалено!");
        }
        else
        {
            Debug.Log("Файл сохранения не найден.");
        }
    }

}






