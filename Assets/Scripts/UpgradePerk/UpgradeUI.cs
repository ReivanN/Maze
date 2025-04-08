using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class UpgradeUI : MonoBehaviour
{
    public Button[] buttons;
    public Button skipButton;
    public TextMeshProUGUI warningText;

    private List<Upgrade> currentUpgrades;
    private TopDownCharacterController topDownCharacterController;

    void Start()
    {
        StartCoroutine(InitializeWithDelay());
    }

    private IEnumerator InitializeWithDelay()
    {

        while (topDownCharacterController == null)
        {
            topDownCharacterController = FindAnyObjectByType<TopDownCharacterController>();
            yield return null;
        }

        currentUpgrades = UpgradeManager.GetRandomUpgrades();

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            Upgrade upgrade = currentUpgrades[i];
            TextMeshProUGUI textComponent = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
            textComponent.text = $"{upgrade.name}\n\n{upgrade.description}\nСтоимость: {upgrade.cost}";
            if (topDownCharacterController.currentCoins < upgrade.cost)
            {
                textComponent.text += "\n<color=red>Недостаточно монет</color>";
            }

            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => ApplyUpgrade(index));
        }

        if (skipButton != null)
        {
            skipButton.onClick.RemoveAllListeners();
            skipButton.onClick.AddListener(SkipUpgrade);
        }

        yield return null;
        Time.timeScale = 0;
    }

    void ApplyUpgrade(int index)
    {
        Upgrade selectedUpgrade = currentUpgrades[index];

        if (topDownCharacterController.currentCoins < selectedUpgrade.cost)
        {
            if (warningText != null)
            {
                warningText.text = "Недостаточно монет для улучшения!";
            }
            return;
        }

        if (warningText != null)
        {
            warningText.text = "";
        }

        topDownCharacterController.ApplyUpgrade(selectedUpgrade);
        SaveManager.Instance.SaveUpgrade(selectedUpgrade);

        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    void SkipUpgrade()
    {

        if (warningText != null)
        {
            warningText.text = "";
        }
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}
