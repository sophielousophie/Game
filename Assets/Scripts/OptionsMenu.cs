using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;

    private void Start()
    {
        // Volumes
        Init(masterSlider, GameSettings.MasterParam);
        Init(musicSlider, GameSettings.MusicParam);
        Init(sfxSlider, GameSettings.SfxParam);

        // Plein écran
        fullscreenToggle.SetIsOnWithoutNotify(Screen.fullScreen);
        fullscreenToggle.onValueChanged.AddListener(on =>
        {
            Screen.fullScreen = on;
            PlayerPrefs.SetInt("Fullscreen", on ? 1 : 0);
        });

        // Qualité graphique
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
        qualityDropdown.SetValueWithoutNotify(QualitySettings.GetQualityLevel());
        qualityDropdown.onValueChanged.AddListener(i =>
        {
            QualitySettings.SetQualityLevel(i);
            PlayerPrefs.SetInt("Quality", i);
        });

        // Résolution
        resolutions = Screen.resolutions;
        var labels = new List<string>();
        int current = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            labels.Add($"{resolutions[i].width} x {resolutions[i].height}");
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height) current = i;
        }
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(labels);
        resolutionDropdown.SetValueWithoutNotify(current);
        resolutionDropdown.onValueChanged.AddListener(i =>
            Screen.SetResolution(resolutions[i].width, resolutions[i].height, Screen.fullScreen));
    }

    private void Init(Slider slider, string param)
    {
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.SetValueWithoutNotify(GameSettings.GetVolume(param));
        slider.onValueChanged.AddListener(v => GameSettings.SetVolume(mixer, param, v));
    }

    private void OnDisable() => PlayerPrefs.Save();

    // Branché sur le bouton Retour
    public void OnBackClicked() => SceneLoader.LoadMainMenu();
}
