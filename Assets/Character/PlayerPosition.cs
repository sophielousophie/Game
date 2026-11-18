using UnityEngine;

public class PlayerPosition : MonoBehaviour
{
    [SerializeField] private float horizontalPosition = 0.25f;
    [SerializeField] private float verticalPosition = 0.5f;

    private void Start()
    {
        Camera cam = Camera.main;

        Vector3 position = cam.ViewportToWorldPoint(
            new Vector3(horizontalPosition, verticalPosition, 0)
        );

        position.z = transform.position.z;
        transform.position = position;
    }
}