using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Un bouton de la grille de sélection (prefab).</summary>
[RequireComponent(typeof(Button))]
public class LevelButton : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private GameObject lockIcon; // optionnel

    public Button Button { get; private set; }

    public void Setup(int level, bool unlocked)
    {
        Button = GetComponent<Button>();
        label.text = level.ToString();
        if (lockIcon != null) lockIcon.SetActive(!unlocked);

        // Non interactable = grisé, non cliquable ET ignoré par la navigation clavier.
        Button.interactable = unlocked;
        Button.onClick.RemoveAllListeners();
        if (unlocked) Button.onClick.AddListener(() => SceneLoader.LoadLevel(level));
    }
}
