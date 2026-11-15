using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>Branché sur les 4 boutons du menu principal (événements OnClick).</summary>
public class MainMenuController : MonoBehaviour
{
    public void OnPlayClicked() => SceneLoader.LoadLevel(GameConfig.FirstLevel);
    public void OnLevelSelectionClicked() => SceneLoader.LoadLevelSelection();
    public void OnOptionsClicked() => SceneLoader.LoadOptions();

    public void OnQuitClicked()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false; // Application.Quit() ne fait rien dans l'éditeur
#else
        Application.Quit();
#endif
    }
}
