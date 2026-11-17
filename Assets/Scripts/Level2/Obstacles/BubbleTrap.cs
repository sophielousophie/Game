using UnityEngine;

public sealed class BubbleTrap : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out CheckpointRespawner respawner))
        {
            respawner.Respawn();
        }
    }
}
