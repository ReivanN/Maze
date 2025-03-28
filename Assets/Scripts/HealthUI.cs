using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private GameObject[] healthIcons;
    public void UpdateHealth(int currentHealth)
    {
        for (int i = 0; i < healthIcons.Length; i++)
        {
            healthIcons[i].SetActive(i < currentHealth);
        }
    }
}
