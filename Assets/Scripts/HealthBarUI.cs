using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour, IHealthBar
{
    [SerializeField] private Image healthBarFill;
    [SerializeField] private GameObject healthBarCanvas;
    [SerializeField] private float healthBarVisibleTime = 2f;

    private float healthBarTimer;

    public void UpdateHealthBar(float currentHP, float maxHP)
    {
        Debug.LogError("LOG 2");
        healthBarFill.fillAmount = currentHP / maxHP;
        ShowHealthBar();
    }

    public void ShowHealthBar()
    {
        healthBarCanvas.SetActive(true);
        healthBarTimer = healthBarVisibleTime;
    }

    public void HideHealthBar()
    {
        healthBarCanvas.SetActive(false);
    }

    private void Update()
    {
        if (healthBarCanvas.activeSelf)
        {
            healthBarTimer -= Time.deltaTime;
            if (healthBarTimer <= 0)
            {
                HideHealthBar();
            }
        }
    }
}
