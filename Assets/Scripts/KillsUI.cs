using TMPro;
using UnityEngine;

public class KillsUI : MonoBehaviour
{
    public TextMeshProUGUI killscount;
    private FinisherSystem finisherSystem;


    private void Start()
    {
        int count = PlayerPrefs.GetInt("Kills");
        if(count > 0) 
        {
            CountText(count);
        }
        else 
        {
            CountText(0);
        }
    }
    private void OnEnable()
    {
        TryFindFinisherSystem();
        InvokeRepeating(nameof(TryFindFinisherSystem), 0f, 1f);
    }

    private void OnDisable()
    {
        if (finisherSystem != null)
        {
            finisherSystem.OnKillCountChanged -= CountText;
        }
        CancelInvoke(nameof(TryFindFinisherSystem));
    }

    private void TryFindFinisherSystem()
    {
        if (finisherSystem == null)
        {
            finisherSystem = FindAnyObjectByType<FinisherSystem>();

            if (finisherSystem != null)
            {
                finisherSystem.OnKillCountChanged += CountText;
                Debug.Log("FinisherSystem найден и подписан!");
                CancelInvoke(nameof(TryFindFinisherSystem));
                CountText(0);
            }
        }
    }

    private void CountText(int count)
    {
        killscount.text = "Kills " + count;
    }
}
