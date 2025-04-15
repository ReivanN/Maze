using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class AttributeUI : MonoBehaviour
{
    public Button[] buttons;
    public Image[] attributeSlots;
    public Button skipButton;
    public TextMeshProUGUI warningText;

    private List<Atribute> availableAttributes;
    private Atribute selectedNewAttribute;
    private Atribute attributeToReplace;
    private TopDownCharacterController topDownCharacterController;
    private void Awake()
    {
        UpgradeIcons.LoadIcons();
    }
    void Start()
    {
        StartCoroutine(InitializeWithDelay());
        UpdateHUD();
    }

    private IEnumerator InitializeWithDelay()
    {
        while (AttributeManager.activeAttributes == null)
        {
            yield return null;
        }

        while (topDownCharacterController == null)
        {
            topDownCharacterController = FindAnyObjectByType<TopDownCharacterController>();
            yield return null;
        }


        availableAttributes = AttributeManager.GetAvailableAttributes();

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            if (i < availableAttributes.Count)
            {
                Atribute attribute = availableAttributes[index];
                TextMeshProUGUI textComponent = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
                textComponent.text = $"{attribute.name}\n{attribute.description}\nСтоимость: {attribute.cost}";
                Image iconImage = buttons[i].transform.Find("Icon")?.GetComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.sprite = attribute.icon != null ? attribute.icon : UpgradeIcons.GetIcon(attribute.name);
                    iconImage.enabled = iconImage.sprite != null;
                }
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() => OnAttributeSelected(attribute));
            }
            else
            {
                buttons[i].gameObject.SetActive(false);
            }
        }

        if (skipButton != null)
        {
            skipButton.onClick.RemoveAllListeners();
            skipButton.onClick.AddListener(SkipAttributeSelection);
        }

        yield return null;
    }

    void OnAttributeSelected(Atribute newAttribute)
    {
        if (topDownCharacterController.currentCoins < newAttribute.cost)
        {
            if (warningText != null)
            {
                warningText.text = "Недостаточно монет для улучшения!";
            }
            return;
        }

        selectedNewAttribute = newAttribute;

        if (!AttributeManager.IsFull())
        {
            AttributeManager.AddAttribute(selectedNewAttribute);
            topDownCharacterController.ApplyAtributes(selectedNewAttribute);
            SaveManager.Instance.SaveAttribute(selectedNewAttribute);
            CloseUI();
            return;
        }

        ShowReplacePrompt();
    }


    void ShowReplacePrompt()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i < AttributeManager.activeAttributes.Count)
            {
                Atribute activeAttr = AttributeManager.activeAttributes[i];
                int index = i;

                TextMeshProUGUI textComponent = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
                textComponent.text = $"Change: {activeAttr.name}\n\n{activeAttr.description}";

                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() =>
                {
                    attributeToReplace = activeAttr;
                    ReplaceAttribute();
                });
            }
            else
            {
                buttons[i].gameObject.SetActive(false);
            }
        }

        if (warningText != null)
        {
            warningText.text = "";
        }
    }

    void ReplaceAttribute()
    {
        if (attributeToReplace != null && selectedNewAttribute != null)
        {
            AttributeManager.ReplaceAttribute(attributeToReplace, selectedNewAttribute);
            topDownCharacterController.ApplyAtributes(selectedNewAttribute);
            SaveManager.Instance.SaveAttribute(selectedNewAttribute);
        }
        UpdateHUD();
        CloseUI();
    }

    public void UpdateHUD()
    {
        GameData gameData = SaveManager.Instance.Load();

        List<Atribute> saveAttributes = AttributeManager.activeAttributes;

        for (int i = 0; i < attributeSlots.Length; i++)
        {
            if (i < saveAttributes.Count)
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
        }

    }

    void SkipAttributeSelection()
    {
        if (warningText != null)
        {
            warningText.text = "";
        }

        CloseUI();
    }

    void CloseUI()
    {
        gameObject.SetActive(false);
    }
}
