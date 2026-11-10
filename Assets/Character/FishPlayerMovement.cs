using UnityEngine;

public class FishPlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float acceleration = 3f;
    [SerializeField] private float maxSpeed = 3f;
    [SerializeField] private float waterDrag = 1.5f;

    private float verticalSpeed = 0f;

    [SerializeField] private Transform fishModel;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float maxRotation = 25f;
    private Quaternion initialRotation;

    [SerializeField] private float sinkSpeed = 1f;

    void Start()
    {
        initialRotation = fishModel.localRotation;
    }


    void Update()
    {
        // Récupérer les touches haut et bas
        float input = 0f;

        if (Input.GetKey(KeyCode.UpArrow))
            input = 1f;

        if (Input.GetKey(KeyCode.DownArrow))
            input = -1f;

        // Accélération dans l'eau
        if (input != 0)
        {
            verticalSpeed += input * acceleration * Time.deltaTime;
        }
        else
        {
            // Le poisson ralentit progressivement
            /*verticalSpeed = Mathf.MoveTowards(
                verticalSpeed,
                0f,
                waterDrag * Time.deltaTime
            );*/
            verticalSpeed = Mathf.MoveTowards(
                verticalSpeed,
                -sinkSpeed,
                waterDrag * Time.deltaTime
            );
        }

        // Limiter la vitesse maximale
        verticalSpeed = Mathf.Clamp(
            verticalSpeed,
            -maxSpeed,
            maxSpeed
        );

        // Déplacer le poisson verticalement
        transform.position += Vector3.up * verticalSpeed * Time.deltaTime;


        float targetRotation = verticalSpeed / maxSpeed * maxRotation;

        Quaternion target = initialRotation * Quaternion.Euler(
            -targetRotation,
            0f,
            0f
        );

        fishModel.localRotation = Quaternion.Lerp(
            fishModel.localRotation,
            target,
            rotationSpeed * Time.deltaTime
        );

        // Orienter le poisson selon sa vitesse verticale
        /*float targetRotation = -verticalSpeed / maxSpeed * maxRotation;

        Quaternion rotation = Quaternion.Euler(0f, 0f, targetRotation);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            rotation,
            rotationSpeed * Time.deltaTime
        );*/
    }
}
