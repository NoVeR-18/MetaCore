using UnityEngine;
using UnityEngine.Audio;
public class GameManager : MonoBehaviour
{
    private const string VibrationPrefKey = "VibrationEnabled";
    private const string SoundsPrefKey = "SoundsEnabled";
    public string currentScene;
    public bool vibrations;
    public AudioMixer audioMixer;
    public bool sounds;
    public static GameManager Instance
    {
        get; set;
    }
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        LoadVibrationSettings();
        LoadSoundSettings();
    }
    private void LoadVibrationSettings()
    {

        if (PlayerPrefs.HasKey(VibrationPrefKey))
        {
            vibrations = PlayerPrefs.GetInt(VibrationPrefKey) == 1;
        }
        else
        {
            vibrations = true;
        }
    }
    public void Vibrate()
    {
        if (vibrations)
            Vibrate(20, 15);
    }

    private void Vibrate(long milliseconds, int amplitude)
    {
        Debug.Log("Vibration");
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");

            if (vibrator.Call<bool>("hasVibrator"))
            {
                if (AndroidVersion() >= 26)  // Проверяем версию Android (API 26 и выше поддерживают амплитуду)
                {
                    AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
                    AndroidJavaObject vibrationEffect = vibrationEffectClass.CallStatic<AndroidJavaObject>("createOneShot", milliseconds, amplitude);
                    vibrator.Call("vibrate", vibrationEffect);
                }
                else
                {
                    vibrator.Call("vibrate", milliseconds);  // Без амплитуды на старых устройствах
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Vibration failed: " + e.Message);
        }
#endif
    }
    private int AndroidVersion()
    {
        AndroidJavaClass versionClass = new AndroidJavaClass("android.os.Build$VERSION");
        return versionClass.GetStatic<int>("SDK_INT");
    }


    public void SetSound(bool enabled)
    {
        sounds = enabled;

        if (sounds)
        {
            audioMixer.SetFloat("SoundGroup", 0f);
        }
        else
        {
            audioMixer.SetFloat("SoundGroup", -80f);
        }

        PlayerPrefs.SetInt(SoundsPrefKey, sounds ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadSoundSettings()
    {
        if (PlayerPrefs.HasKey(SoundsPrefKey))
        {
            sounds = PlayerPrefs.GetInt(SoundsPrefKey) == 1;
        }
        else
        {
            sounds = true;
        }

        SetSound(sounds);
    }
}
