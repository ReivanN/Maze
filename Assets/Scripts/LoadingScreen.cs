using System.Collections;
using TMPro;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private TextMeshProUGUI loadingText;

    private bool isLoading = true;
    void Start()
    {
        loadingScreen.SetActive(true);
    }

    void Update()
    {
        if(MazeGenerator.IsDone == false) 
        {
            loadingScreen.SetActive(true);
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
