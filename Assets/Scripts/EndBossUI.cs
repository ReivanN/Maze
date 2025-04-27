using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndBossUI : MonoBehaviour
{
    [SerializeField] private Button ICEBIOM;
    [SerializeField] private Button VULKANBIOM;
    [SerializeField] private Button JUNGLEBIOM;
    [SerializeField] private TextMeshProUGUI pointPass;
    [SerializeField] private GameObject UI;
    [SerializeField] private GameObject HUD;

    private GameData data;

    public void StartBiom()
    {
        HUD.SetActive(false);
        UI.SetActive(true);
        data = SaveManager.Instance.Load();
        UpdateUI();

        ICEBIOM.onClick.AddListener(() => SpendPoint(BiomType.Ice));
        VULKANBIOM.onClick.AddListener(() => SpendPoint(BiomType.Vulkan));
        JUNGLEBIOM.onClick.AddListener(() => SpendPoint(BiomType.Jungle));
    }

    private void SpendPoint(BiomType biom)
    {
        if (data.pass > 0)
        {
            data.pass--;
            SaveManager.Instance.Save(data);
            UpdateUI();
            switch (biom)
            {
                case BiomType.Ice:
                    data.IceBiom = true;
                    data.VelkanBiom = false;
                    data.JungleBiom = false;
                    LoadBiomScene();
                    break;
                case BiomType.Vulkan:
                    data.IceBiom = false;
                    data.VelkanBiom = true;
                    data.JungleBiom = false;
                    LoadBiomScene();
                    break;
                case BiomType.Jungle:
                    data.IceBiom = false;
                    data.VelkanBiom = false;
                    data.JungleBiom = true;
                    LoadBiomScene();
                    break;
            }
        }
        else
        {
            Debug.Log("Недостаточно очков для перехода.");
        }
    }

    private void LoadBiomScene()
    {
        LevelManager.Instance.LevelCompleted();
        MazeManager.Instance.NewMaze();
    }

    private void UpdateUI()
    {
        pointPass.text = data.pass.ToString();
    }

    private enum BiomType
    {
        Ice,
        Vulkan,
        Jungle
    }
}
