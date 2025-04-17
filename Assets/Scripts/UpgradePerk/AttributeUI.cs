using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class AttributeUI : MonoBehaviour
{
    public Button[] buttons;
    public Button[] replaceButtons;
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
                bool alreadyOwned = AttributeManager.activeAttributes.Contains(attribute);

                TextMeshProUGUI textComponent = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
                Image iconImage = buttons[i].transform.Find("Icon")?.GetComponent<Image>();

                // Устанавливаем текст
                textComponent.text = alreadyOwned
                    ? $"{attribute.name}\nУже куплен"
                    : $"{attribute.name}\n{attribute.description}\nСтоимость: {attribute.cost}";

                // Устанавливаем иконку
                if (iconImage != null)
                {
                    iconImage.sprite = attribute.icon != null ? attribute.icon : UpgradeIcons.GetIcon(attribute.name);
                    iconImage.enabled = iconImage.sprite != null;

                    var color = iconImage.color;
                    color.a = alreadyOwned ? 0.4f : 1f;
                    iconImage.color = color;
                }

                // Настройка кнопки
                buttons[i].interactable = !alreadyOwned;
                buttons[i].onClick.RemoveAllListeners();

                if (!alreadyOwned)
                {
                    buttons[i].onClick.AddListener(() => OnAttributeSelected(attribute));
                }

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
        if (AttributeManager.activeAttributes.Contains(newAttribute))
        {
            if (warningText != null)
            {
                warningText.text = "This Attribute was bought.";
            }
            return;
        }

        if (topDownCharacterController.currentCoins < newAttribute.cost)
        {
            if (warningText != null)
            {
                warningText.text = "Not enough money!";
            }
            return;
        }

        selectedNewAttribute = newAttribute;

        if (!AttributeManager.IsFull())
        {
            AttributeManager.AddAttribute(selectedNewAttribute);
            topDownCharacterController.ApplyAtributes(selectedNewAttribute);
            SaveManager.Instance.SaveAttribute(selectedNewAttribute);
            UpdateHUD();
            CloseUI();
            return;
        }

        ShowReplacePrompt();
    }



    void ShowReplacePrompt()
    {
        List<Atribute> activeAttributes = AttributeManager.activeAttributes;

        for (int i = 0; i < replaceButtons.Length; i++)
        {
            if (i < activeAttributes.Count)
            {
                Atribute activeAttr = activeAttributes[i];
                int index = i;
                TextMeshProUGUI textComponent = replaceButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                //textComponent.text = $"Replace: {activeAttr.name}";
                Image iconImage = replaceButtons[i].transform.Find("Icon")?.GetComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.sprite = activeAttr.icon != null ? activeAttr.icon : UpgradeIcons.GetIcon(activeAttr.name);
                    iconImage.enabled = iconImage.sprite != null;
                }
                replaceButtons[i].onClick.RemoveAllListeners();
                replaceButtons[i].onClick.AddListener(() =>
                {
                    attributeToReplace = activeAttr;
                    ReplaceAttribute();
                    replaceButtons[i].gameObject.SetActive(false);
                });

                replaceButtons[i].gameObject.SetActive(true);
            }
            else
            {
                replaceButtons[i].gameObject.SetActive(false);
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
            // Удаляем из менеджеров и из сейва
            AttributeManager.RemoveAttribute(attributeToReplace);
            SaveManager.Instance.RemoveAttribute(attributeToReplace);
            topDownCharacterController.RemoveAtribute(attributeToReplace);

            // Добавляем новый
            AttributeManager.AddAttribute(selectedNewAttribute);
            topDownCharacterController.ApplyAtributes(selectedNewAttribute);
            SaveManager.Instance.SaveAttribute(selectedNewAttribute);

            // Обновляем интерфейс
            UpdateHUD();

            // Сбрасываем флаги
            selectedNewAttribute = null;
            attributeToReplace = null;

            // Закрыть UI или перезапустить выбор
            CloseUI();
        }
    }




    public void UpdateHUD()
    {
        List<Atribute> activeAttributes = AttributeManager.activeAttributes;

        for (int i = 0; i < attributeSlots.Length; i++)
        {
            if (i < activeAttributes.Count)
            {
                Sprite icon = activeAttributes[i].icon ?? UpgradeIcons.GetIcon(activeAttributes[i].name);
                attributeSlots[i].sprite = icon;
                attributeSlots[i].enabled = icon != null;
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
        selectedNewAttribute = null;
        attributeToReplace = null; 
        CloseUI();
        
    }

    void CloseUI()
    {
        gameObject.SetActive(false);
        PauseGameState.Resume();
    }
}
