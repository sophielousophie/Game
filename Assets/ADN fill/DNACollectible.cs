using UnityEngine;

public class DNACollectible : MonoBehaviour
{
    [SerializeField] private float energyGain = 25f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        EnergyManager energyManager = FindFirstObjectByType<EnergyManager>();
        if (energyManager == null)
        {
            Debug.LogWarning("DNACollectible: no EnergyManager found in the scene.");
            return;
        }

        energyManager.AddEnergy(energyGain);
        Destroy(gameObject);
    }
}
