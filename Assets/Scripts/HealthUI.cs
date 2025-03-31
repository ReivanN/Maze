using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image healthBar;  
    [SerializeField] private int maxHealth = 100; 

    private void Start()
    {
        UpdateHealth(maxHealth);
    }

    public void UpdateHealth(int currentHealth)
    {
        healthText.text = $"{currentHealth} / {maxHealth}";
        float healthPercentage = Mathf.Clamp01((float)currentHealth / maxHealth);
        healthBar.fillAmount = healthPercentage;
    }
}
