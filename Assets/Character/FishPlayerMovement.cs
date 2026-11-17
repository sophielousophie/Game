
using UnityEngine;

public class FishPlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float acceleration = 3f;
    [SerializeField] private float maxSpeed = 3f;
    [SerializeField] private float waterDrag = 1.5f;
    [SerializeField] private float sinkSpeed = 1f;

    [Header("Rotation")]
    [SerializeField] private Transform fishModel;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float maxRotation = 25f;

    private float verticalSpeed = 0f;
    private Rigidbody2D rb;
    private Quaternion initialRotation;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (fishModel != null)
            initialRotation = fishModel.localRotation;
    }

    private void FixedUpdate()
    {
        float input = 0f;

        if (Input.GetKey(KeyCode.UpArrow))
            input = 1f;

        if (Input.GetKey(KeyCode.DownArrow))
            input = -1f;

        if (input != 0)
            verticalSpeed += input * acceleration * Time.fixedDeltaTime;
        else
            verticalSpeed = Mathf.MoveTowards(
                verticalSpeed,
                -sinkSpeed,
                waterDrag * Time.fixedDeltaTime
            );

        verticalSpeed = Mathf.Clamp(
            verticalSpeed,
            -maxSpeed,
            maxSpeed
        );

        rb.linearVelocity = new Vector2(0f, verticalSpeed);

        if (fishModel != null)
        {
            float targetRotation =
                -verticalSpeed / maxSpeed * maxRotation;

            Quaternion target = initialRotation *
                Quaternion.Euler(targetRotation, 0f, 0f);

            fishModel.localRotation = Quaternion.Lerp(
                fishModel.localRotation,
                target,
                rotationSpeed * Time.fixedDeltaTime
            );
        }
    }
}
