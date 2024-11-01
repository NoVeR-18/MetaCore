using UnityEngine;
using UnityEngine.UI;

public class SettingsPopUp : MonoBehaviour
{
    public Slider VibrationSlider;
    public Slider MusicSlider;
    public Slider SoundsSlider;

    public Button vibrationSet;
    public Button musicSet;
    public Button soundSet;



    private void Start()
    {
        soundSet.onClick.AddListener(() =>
        {
            if (GameManager.Instance.sounds)
            {
                GameManager.Instance.SetSound(false);
                SoundsSlider.value = 0;
            }
            else
            {
                GameManager.Instance.SetSound(true);
                SoundsSlider.value = 1;
            }
        });
        musicSet.onClick.AddListener(() =>
        {
            if (GameManager.Instance.musics)
            {
                GameManager.Instance.SetMusic(false);
                MusicSlider.value = 0;
            }
            else
            {
                GameManager.Instance.SetMusic(true);
                MusicSlider.value = 1;
            }
        });
        vibrationSet.onClick.AddListener(() =>
        {
            if (GameManager.Instance.vibrations)
            {
                GameManager.Instance.SetVibration(false);
                VibrationSlider.value = 0;
            }
            else
            {
                GameManager.Instance.SetVibration(true);
                VibrationSlider.value = 1;
            }
        });
    }
    private void OnEnable()
    {
        if (GameManager.Instance.sounds)
            SoundsSlider.value = 1;
        else
            SoundsSlider.value = 0;

        if (GameManager.Instance.vibrations)
            VibrationSlider.value = 1;
        else
            VibrationSlider.value = 0;

        if (GameManager.Instance.musics)
            MusicSlider.value = 1;
        else
            MusicSlider.value = 0;

    }



}
