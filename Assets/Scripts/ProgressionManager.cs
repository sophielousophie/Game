using UnityEngine;

/// <summary>
/// Gère uniquement la progression (niveaux débloqués).
/// Version 1 : PlayerPrefs. Remplaçable plus tard par un vrai système de sauvegarde
/// sans toucher à l'UI, tant que l'API publique reste la même.
/// </summary>
public static class ProgressionManager
{
    private const string HighestUnlockedKey = "HighestUnlockedLevel";

    public static int HighestUnlockedLevel =>
        Mathf.Max(GameConfig.FirstLevel, PlayerPrefs.GetInt(HighestUnlockedKey, GameConfig.FirstLevel));

    public static bool IsUnlocked(int level) => level <= HighestUnlockedLevel;

    /// <summary>À appeler quand le joueur réussit un niveau.</summary>
    public static void CompleteLevel(int level)
    {
        int next = level + 1;
        if (next > GameConfig.TotalLevels || next <= HighestUnlockedLevel) return;

        PlayerPrefs.SetInt(HighestUnlockedKey, next);
        PlayerPrefs.Save();
    }

    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(HighestUnlockedKey);
        PlayerPrefs.Save();
    }
}
