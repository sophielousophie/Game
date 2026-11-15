/// <summary>
/// Constantes partagées : noms des scènes et nombre de niveaux.
/// Les noms doivent correspondre à ceux des scènes dans Build Settings.
/// </summary>
public static class GameConfig
{
    public const string MainMenuScene = "MainMenu";
    public const string LevelSelectionScene = "LevelSelection";
    public const string OptionsScene = "Options";
    public const string LevelScenePrefix = "Level"; // Level1, Level2, ...

    public const int FirstLevel = 1;
    public const int TotalLevels = 3; // à adapter
}
