using UnityEngine;
using UnityEngine.Audio;

/// <summary>À mettre dans la scène MainMenu (lancée en premier) pour appliquer les réglages sauvegardés.</summary>
public class SettingsBootstrap : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;

    private void Start() => GameSettings.ApplyAll(mixer);
}
