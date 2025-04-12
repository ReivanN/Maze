using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using DG.Tweening;

public class UpgradeUI : MonoBehaviour
{
    public Button[] buttons;
    public Button skipButton;
    public TextMeshProUGUI warningText;
    public float animationDuration = 0.5f;
    public float startYOffset = -800f;

    private List<Upgrade> currentUpgrades;
    private TopDownCharacterController topDownCharacterController;
    private RectTransform rectTransform;
    private bool wasActivated;

    private void Awake()
    {
        UpgradeIcons.LoadIcons();
    }

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        //StartCoroutine(InitializeWithDelay());
        //Animation();
    }

    public void StartShop() 
    {
        rectTransform = GetComponent<RectTransform>();
        if (wasActivated == false)
            StartCoroutine(InitializeWithDelay());
        PauseGameState.Pause();
        Animation();
        wasActivated = true;
    }

    private IEnumerator InitializeWithDelay()
    {
        PauseGameState.Pause();
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
            textComponent.text = $"{upgrade.name}\n{upgrade.description}\nСтоимость: {upgrade.cost}";
            Image iconImage = buttons[i].transform.Find("Icon")?.GetComponent<Image>();
            if (iconImage != null)
            {
                iconImage.sprite = upgrade.icon != null ? upgrade.icon : UpgradeIcons.GetIcon(upgrade.name);
                iconImage.enabled = iconImage.sprite != null;
            }   

            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => ApplyUpgrade(index));
        }

        if (skipButton != null)
        {
            skipButton.onClick.RemoveAllListeners();
            skipButton.onClick.AddListener(SkipUpgrade);
        }

        //Time.timeScale = 0;
    }

    public void Animation() 
    {
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, startYOffset);
        rectTransform.DOAnchorPosY(0f, animationDuration).SetEase(Ease.OutBack);
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
        PauseGameState.Resume();
    }

    void SkipUpgrade()
    {
        if (warningText != null)
        {
            warningText.text = "";
        }

        gameObject.SetActive(false);
        PauseGameState.Resume();
    }
}
