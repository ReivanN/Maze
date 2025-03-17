using System.Collections;
using TMPro;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private GameObject player;
    private bool isLoading = true;
    void Start()
    {
        loadingScreen.SetActive(true);
    }

    void Update()
    {
        if(player == null) 
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        else 
        {
                loadingScreen.SetActive(false);
                this.enabled = false;
        }

    }

    IEnumerator LoadingDots()
    {
        string baseText = "Loading";
        int dotCount = 0;

        while (isLoading)
        {
            dotCount = (dotCount + 1) % 4;
            loadingText.text = baseText + new string('.', dotCount);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
