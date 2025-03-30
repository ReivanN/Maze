using TMPro;
using UnityEngine;

public class KillsUI : MonoBehaviour
{
    public TextMeshProUGUI killscount;
    private FinisherSystem finisherSystem;

    private void OnEnable()
    {
        TryFindFinisherSystem();
        InvokeRepeating(nameof(TryFindFinisherSystem), 0f, 1f); // Проверка каждую секунду
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
                CancelInvoke(nameof(TryFindFinisherSystem)); // Останавливаем поиск после нахождения
            }
        }
    }

    private void CountText(int count)
    {
        killscount.text = "Kills " + count;
    }
}
