using UnityEngine;
using UnityEngine.Audio;

/// <summary>Lecture / écriture / application des paramètres.</summary>
public static class GameSettings
{
    public const string MasterParam = "MasterVolume"; // paramètres exposés dans l'AudioMixer
    public const string MusicParam = "MusicVolume";
    public const string SfxParam = "SfxVolume";

    public static float GetVolume(string param) => PlayerPrefs.GetFloat(param, 0.8f);

    public static void SetVolume(AudioMixer mixer, string param, float linear01)
    {
        PlayerPrefs.SetFloat(param, linear01);
        ApplyVolume(mixer, param, linear01);
    }

    private static void ApplyVolume(AudioMixer mixer, string param, float linear01)
    {
        // Slider linéaire (0..1) -> décibels
        mixer.SetFloat(param, Mathf.Log10(Mathf.Max(linear01, 0.0001f)) * 20f);
    }

    public static void ApplyAll(AudioMixer mixer)
    {
        ApplyVolume(mixer, MasterParam, GetVolume(MasterParam));
        ApplyVolume(mixer, MusicParam, GetVolume(MusicParam));
        ApplyVolume(mixer, SfxParam, GetVolume(SfxParam));

        Screen.fullScreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Quality", QualitySettings.GetQualityLevel()));
    }
}
