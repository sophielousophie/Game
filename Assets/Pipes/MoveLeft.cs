using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    [SerializeField]
    int speed = 5;
    [SerializeField]
    float range = 2f;

    private Rigidbody2D rb;

    void Start()
    {
        /*GetComponent<Rigidbody2D>().linearVelocity = Vector2.left * speed;

        transform.position = new Vector2(transform.position.x,transform.position.y *  range * Random.value);*/

        rb = GetComponent<Rigidbody2D>();
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
