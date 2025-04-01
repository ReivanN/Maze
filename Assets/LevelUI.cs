using TMPro;
using UnityEngine;

public class LevelUI : MonoBehaviour
{
    public TextMeshProUGUI TextMeshProUGUI;
    void Start()
    {
        LevelManager.Instance.LevelUI(TextMeshProUGUI);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
