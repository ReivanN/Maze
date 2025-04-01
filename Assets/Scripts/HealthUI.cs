using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image healthBar;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float animationDuration = 0.5f;

    private void Start()
    {
        UpdateHealth(maxHealth, instant: true);
    }

    public void UpdateHealth(int currentHealth, bool instant = false)
    {
        healthText.text = $"{currentHealth} / {maxHealth}";
        float healthPercentage = Mathf.Clamp01((float)currentHealth / maxHealth);

        if (instant)
        {
            healthBar.fillAmount = healthPercentage;
        }
        else
        {
            healthBar.DOFillAmount(healthPercentage, animationDuration).SetEase(Ease.OutQuad);
        }
    }
}
