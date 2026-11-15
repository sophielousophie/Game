using UnityEngine;
using UnityEngine.UI;

/// <summary>Génère les boutons de niveaux selon la progression sauvegardée.</summary>
public class LevelSelectionMenu : MonoBehaviour
{
    [SerializeField] private LevelButton levelButtonPrefab;
    [SerializeField] private Transform container; // avec un Grid Layout Group
    [SerializeField] private MenuNavigation navigation;

    private void Start()
    {
        Selectable first = null;

        for (int level = GameConfig.FirstLevel; level <= GameConfig.TotalLevels; level++)
        {
            LevelButton btn = Instantiate(levelButtonPrefab, container);
            btn.Setup(level, ProgressionManager.IsUnlocked(level));
            if (first == null) first = btn.Button; // le niveau 1 est toujours débloqué
        }

        navigation.SetFirstSelected(first);
    }

    // Branché sur le bouton Retour
    public void OnBackClicked() => SceneLoader.LoadMainMenu();
}
