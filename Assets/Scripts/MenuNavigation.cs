using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// À placer dans chaque scène de menu.
/// - Sélectionne automatiquement le premier élément à l'ouverture.
/// - Reprend la sélection si la souris l'a perdue et que le joueur utilise le clavier.
/// - Échap : retour à la scène indiquée (laisser vide pour le menu principal = désactivé).
/// </summary>
public class MenuNavigation : MonoBehaviour
{
    [SerializeField] private Selectable firstSelected;
    [Tooltip("Scène à charger avec Échap. Vide = Échap ne fait rien.")]
    [SerializeField] private string backSceneName = GameConfig.MainMenuScene;

    private void Start() => SelectFirst();

    private void Update()
    {
        // Après un clic dans le vide, plus rien n'est sélectionné : on le rétablit
        // dès qu'une touche de navigation est utilisée.
        if (EventSystem.current != null &&
            EventSystem.current.currentSelectedGameObject == null &&
            (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) ||
             Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)))
        {
            SelectFirst();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !string.IsNullOrEmpty(backSceneName))
            SceneLoader.Load(backSceneName);
    }

    public void SetFirstSelected(Selectable selectable)
    {
        firstSelected = selectable;
        SelectFirst();
    }

    private void SelectFirst()
    {
        if (firstSelected != null && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);
    }
}
