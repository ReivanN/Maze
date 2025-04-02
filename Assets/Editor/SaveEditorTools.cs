using UnityEditor;

public class SaveEditorTools
{
    [MenuItem("Tools/DeleteSave")]
    public static void DeleteSaveFromMenu()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.DeleteSave();
        }
        else
        {

        }
    }
}
