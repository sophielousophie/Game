using UnityEngine.SceneManagement;

/// <summary>Seul point d'entrée pour changer de scène.</summary>
public static class SceneLoader
{
    public static void Load(string sceneName) => SceneManager.LoadScene(sceneName);

    public static void LoadMainMenu() => Load(GameConfig.MainMenuScene);
    public static void LoadLevelSelection() => Load(GameConfig.LevelSelectionScene);
    public static void LoadOptions() => Load(GameConfig.OptionsScene);

    public static void LoadLevel(int level) => Load(GameConfig.LevelScenePrefix + level);
}
