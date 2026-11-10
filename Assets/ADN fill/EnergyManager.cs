using UnityEngine;
using UnityEngine.UI;

public class EnergyManager : MonoBehaviour
{
    [SerializeField] private Image energyBarFill;
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float initialEnergy = 0f;

    public float CurrentEnergy { get; private set; }

    private void Awake()
    {
        CurrentEnergy = Mathf.Clamp(initialEnergy, 0f, maxEnergy);
        UpdateBar();
    }

    public void AddEnergy(float amount)
    {
        CurrentEnergy = Mathf.Clamp(CurrentEnergy + amount, 0f, maxEnergy);
        UpdateBar();
    }

    // Ready for Time Flap later.
    public bool TryUseEnergy(float amount)
    {
        if (amount < 0f || CurrentEnergy < amount) return false;
        CurrentEnergy -= amount;
        UpdateBar();
        return true;
    }

    private void UpdateBar()
    {
        if (energyBarFill != null)
            energyBarFill.fillAmount = maxEnergy > 0f ? CurrentEnergy / maxEnergy : 0f;
    }
}
