using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttributesUIImages : MonoBehaviour
{
    public Image[] attributeSlots;
    [SerializeField] private Button[] hudAttributeSlotButtons;

    private void Start()
    {
        UpgradeIcons.LoadIcons();
        foreach (var btn in hudAttributeSlotButtons)
        {
            btn.gameObject.SetActive(false);
        }

        GameData gameData = SaveManager.Instance.Load();
        AttributeManager.LoadAttributesFromSave(gameData.appliedAttributes);
        UpdateHUD();    
    }
    public void UpdateHUD()
    {
        GameData gameData = SaveManager.Instance.Load();

        List<Atribute> saveAttributes = AttributeManager.activeAttributes;

        int count = Mathf.Min(attributeSlots.Length, saveAttributes.Count);
        for (int i = 0; i < attributeSlots.Length; i++)
        {
            if (i < count)
            {
                attributeSlots[i].sprite = saveAttributes[i].icon != null
                    ? saveAttributes[i].icon
                    : UpgradeIcons.GetIcon(saveAttributes[i].name);
                attributeSlots[i].enabled = attributeSlots[i].sprite != null;
            }
            else
            {
                attributeSlots[i].sprite = null;
                attributeSlots[i].enabled = false;
            }
            Debug.Log($"[{i}] {saveAttributes[i].name}");
            Debug.Log($"[{i}] {gameData.appliedAttributes[i]}");
        }
    }
}
