using UnityEngine;

public sealed class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField, Range(0f, 1f)] private float horizontalFactor = 0.2f;

    private Vector3 origin;
    private float cameraOriginX;

    private void Start()
    {
        origin = transform.position;
        cameraOriginX = cameraTransform != null ? cameraTransform.position.x : 0f;
    }

    private void LateUpdate()
    {
        if (cameraTransform == null)
        {
            return;
        }

        float offset = (cameraTransform.position.x - cameraOriginX) * horizontalFactor;
        transform.position = new Vector3(origin.x + offset, origin.y, origin.z);
    }
}
