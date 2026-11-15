using UnityEngine;

/// <summary>
/// Exemple : à placer dans un niveau (ex. sur la zone d'arrivée).
/// Débloque le niveau suivant puis revient à la sélection.
/// </summary>
public class LevelCompleteExample : MonoBehaviour
{
    [SerializeField] private int levelNumber = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        ProgressionManager.CompleteLevel(levelNumber);
        SceneLoader.LoadLevelSelection();
    }
}
