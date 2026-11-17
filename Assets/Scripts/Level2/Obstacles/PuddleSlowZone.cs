using UnityEngine;

public sealed class PuddleSlowZone : MonoBehaviour
{
    [SerializeField, Range(0.1f, 1f)] private float speedMultiplier = 0.55f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out PonyoMovement2D movement))
        {
            movement.SetSpeedMultiplier(speedMultiplier);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out PonyoMovement2D movement))
        {
            movement.SetSpeedMultiplier(1f);
        }
    }
}
