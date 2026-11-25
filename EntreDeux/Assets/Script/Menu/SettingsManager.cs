using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] public Canvas canvas;

    public Slider sliderMaster;
    public Slider sliderSFX;
    public Slider sliderMusic;

    private void Start()
    {
        sliderMaster.value = AudioManager.instance.GetMainVolume() ;
        sliderSFX.value = AudioManager.instance.GetSfxVolume();
        sliderMusic.value = AudioManager.instance.GetStVolume() ;
    
    }

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