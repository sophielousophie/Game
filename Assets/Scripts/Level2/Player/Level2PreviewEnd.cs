using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public sealed class Level2PreviewEnd : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out PonyoMovement2D movement))
        {
            movement.SetMovementEnabled(false);
        }
    }
}
