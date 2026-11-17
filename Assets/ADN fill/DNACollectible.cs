using UnityEngine;

public class DNACollectible : MonoBehaviour
{
    [SerializeField] private float energyGain = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        EnergyManager energyManager = FindAnyObjectByType<EnergyManager>();//FindFirstObjectByType<EnergyManager>();
        if (energyManager == null)
        {
            Debug.LogWarning("DNACollectible: no EnergyManager found in the scene.");
            return;
        }

        energyManager.AddEnergy(energyGain);
        Destroy(gameObject);
    }
}
