using UnityEngine;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{
    public UnityEngine.UI.Slider musicSlider;
    public UnityEngine.UI.Slider sfxSlider;

    private void Start()
    {
        musicSlider.value = MusicManager.Instance.musicVolume;
        sfxSlider.value = MusicManager.Instance.sfxVolume;

        musicSlider.onValueChanged.AddListener(v => MusicManager.Instance.SetMusicVolume(v));
        sfxSlider.onValueChanged.AddListener(v => MusicManager.Instance.SetSfxVolume(v));
    }
}
