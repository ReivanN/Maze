using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class AttributeUI : MonoBehaviour
{
    public Button[] buttons;
    public Button skipButton;
    public TextMeshProUGUI warningText;

    private List<Atribute> availableAttributes;
    private Atribute selectedNewAttribute;
    private Atribute attributeToReplace;

    void Start()
    {
        StartCoroutine(InitializeWithDelay());
    }

    private IEnumerator InitializeWithDelay()
    {
        while (AttributeManager.activeAttributes == null)
        {
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
                textComponent.text = $"{attribute.name}\n\n{attribute.description}";

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
        //Time.timeScale = 0;
    }

    void OnAttributeSelected(Atribute newAttribute)
    {
        if (!AttributeManager.IsFull())
        {
            AttributeManager.AddAttribute(newAttribute);
            CloseUI();
            return;
        }

        // Если атрибутов уже 3 — открываем окно замены
        selectedNewAttribute = newAttribute;
        ShowReplacePrompt();
    }

    void ShowReplacePrompt()
    {
        // Простая реализация: перезаписываем UI кнопок текущими активными атрибутами
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i < AttributeManager.activeAttributes.Count)
            {
                Atribute activeAttr = AttributeManager.activeAttributes[i];
                int index = i;

                TextMeshProUGUI textComponent = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
                textComponent.text = $"Заменить: {activeAttr.name}\n\n{activeAttr.description}";

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
            warningText.text = "Выберите атрибут, который хотите заменить.";
        }
    }

    void ReplaceAttribute()
    {
        if (attributeToReplace != null && selectedNewAttribute != null)
        {
            AttributeManager.ReplaceAttribute(attributeToReplace, selectedNewAttribute);
        }

        CloseUI();
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
        Time.timeScale = 1;
    }
}
