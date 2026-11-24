using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] public Canvas canvas;

    public Slider sliderMaster;
    public Slider sliderSFX;
    public Slider sliderMusic;


    private void Update()
    {
        if (canvas.enabled)
        {
            AudioManager.instance.SetMainVolume(sliderMaster.value);
            AudioManager.instance.SetSfxVolume(sliderSFX.value);
            AudioManager.instance.SetSoundtrackVolume(sliderMusic.value);
        }
    }
}