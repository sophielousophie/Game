using UnityEngine;

public sealed class BounceSurface2D : MonoBehaviour
{
    [SerializeField, Min(0f)] private float bounceSpeed = 13f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out PonyoMovement2D movement)
            && collision.contactCount > 0 && collision.GetContact(0).normal.y < -0.5f)
        {
            movement.Bounce(bounceSpeed);
        }
    }
}
