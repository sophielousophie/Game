using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    [SerializeField] int speed = 5;
    [SerializeField] float range = 1.5f;

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.left * speed;

        float randomY = Random.Range(-range, range);

        transform.position = new Vector3(
            transform.position.x,
            transform.position.y + randomY,
            transform.position.z
        );

    }

    public void DestroyPipes()
    {
        Destroy(gameObject);
    }
}
