using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PonyoMovement2D), typeof(Rigidbody2D))]
public sealed class CheckpointRespawner : MonoBehaviour
{
    [SerializeField, Min(0f)] private float respawnDelay = 0.45f;
    [SerializeField] private Transform checkpoint;

    private PonyoMovement2D movement;
    private Rigidbody2D body;
    private bool respawning;

    public void SetCheckpoint(Transform point) => checkpoint = point;

    private void Awake()
    {
        movement = GetComponent<PonyoMovement2D>();
        body = GetComponent<Rigidbody2D>();
    }

    public void Respawn()
    {
        if (!respawning)
        {
            StartCoroutine(RespawnRoutine());
        }
    }

    private IEnumerator RespawnRoutine()
    {
        respawning = true;
        movement.SetMovementEnabled(false);
        yield return new WaitForSeconds(respawnDelay);
        body.position = checkpoint != null ? checkpoint.position : Vector2.zero;
        body.linearVelocity = Vector2.zero;
        movement.SetMovementEnabled(true);
        respawning = false;
    }
}
