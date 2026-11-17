using UnityEngine;

public sealed class PingPongMover2D : MonoBehaviour
{
    [SerializeField] private Vector2 movement = new Vector2(3f, 0.5f);
    [SerializeField, Min(0.1f)] private float period = 2.5f;

    private Vector3 origin;
    private Rigidbody2D body;

    private void Awake()
    {
        origin = transform.position;
        body = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        float phase = Mathf.Sin(Time.time * Mathf.PI * 2f / period);
        Vector3 position = origin + (Vector3)(movement * phase);
        if (body != null && body.bodyType == RigidbodyType2D.Kinematic)
        {
            body.MovePosition(position);
        }
        else
        {
            transform.position = position;
        }
    }
}
