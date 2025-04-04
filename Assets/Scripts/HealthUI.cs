using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image healthBar;
    [SerializeField] private float animationDuration = 0.5f;

    private void Start()
    {
        //UpdateHealth(maxHealth, instant: true);
    }

    public void UpdateHealth(float currentHealth, float maxHealth, bool instant = false)
    {
        int current = Mathf.RoundToInt(currentHealth);
        int max = Mathf.RoundToInt(maxHealth);

        healthText.text = $"{current} / {max}";
        float healthPercentage = Mathf.Clamp01((float)current / max);

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
