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
                bool alreadyOwned = AttributeManager.activeAttributes.Contains(attribute);

                TextMeshProUGUI textComponent = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
                Image iconImage = buttons[i].transform.Find("Icon")?.GetComponent<Image>();

                // ������������� �����
                textComponent.text = alreadyOwned
                    ? $"{attribute.name}\n��� ������"
                    : $"{attribute.name}\n{attribute.description}\n���������: {attribute.cost}";

                // ������������� ������
                if (iconImage != null)
                {
                    iconImage.sprite = attribute.icon != null ? attribute.icon : UpgradeIcons.GetIcon(attribute.name);
                    iconImage.enabled = iconImage.sprite != null;

                    var color = iconImage.color;
                    color.a = alreadyOwned ? 0.4f : 1f;
                    iconImage.color = color;
                }

                // ��������� ������
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
        if (topDownCharacterController.currentCoins < newAttribute.cost)
        {
            if (warningText != null)
            {
                warningText.text = "������������ ����� ��� ���������!";
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

        for (int i = 0; i < buttons.Length; i++)
        {
            if (i < activeAttributes.Count)
            {
                Atribute activeAttr = activeAttributes[i];
                int index = i;
                TextMeshProUGUI textComponent = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
                textComponent.text = $"Replace: {activeAttr.name}";
                Image iconImage = buttons[i].transform.Find("Icon")?.GetComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.sprite = activeAttr.icon != null ? activeAttr.icon : UpgradeIcons.GetIcon(activeAttr.name);
                    iconImage.enabled = iconImage.sprite != null;
                }
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() =>
                {
                    attributeToReplace = activeAttr;
                    ReplaceAttribute();
                });

                buttons[i].gameObject.SetActive(true);
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
            // ������� �� ���������� � �� �����
            AttributeManager.RemoveAttribute(attributeToReplace);
            SaveManager.Instance.RemoveAttribute(attributeToReplace);
            topDownCharacterController.RemoveAtribute(attributeToReplace);

            // ��������� �����
            AttributeManager.AddAttribute(selectedNewAttribute);
            topDownCharacterController.ApplyAtributes(selectedNewAttribute);
            SaveManager.Instance.SaveAttribute(selectedNewAttribute);

            // ��������� ���������
            UpdateHUD();

            // ���������� �����
            selectedNewAttribute = null;
            attributeToReplace = null;

            // ������� UI ��� ������������� �����
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
