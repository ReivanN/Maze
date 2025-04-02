using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class UpgradeUI : MonoBehaviour
{
    public Button[] buttons;
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
            buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentUpgrades[i].name;
            buttons[i].onClick.AddListener(() => ApplyUpgrade(index));
        }
        yield return null;
        Time.timeScale = 0;
    }

    void ApplyUpgrade(int index)
    {
        Upgrade selectedUpgrade = currentUpgrades[index];
        topDownCharacterController.ApplyUpgrade(selectedUpgrade);
        SaveManager.Instance.SaveUpgrade(selectedUpgrade);
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}
