using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public sealed class Level2Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out CheckpointRespawner respawner))
        {
            respawner.SetCheckpoint(transform);
        }
    }
}
