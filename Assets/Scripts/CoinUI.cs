using TMPro;
using UnityEngine;

public class CoinUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;
    private void OnEnable() => TopDownCharacterController.OnCoinsChanged += UpdateUI;  
    private void OnDisable() => TopDownCharacterController.OnCoinsChanged -= UpdateUI;

    private void Start()
    {
        
    }
    private void UpdateUI(int count)
    {
        coinText.text = $"{count}";
        //Debug.LogError("Wasssssssssssssssss");
    }

}
