
using UnityEngine;

public class FishCollision : MonoBehaviour
{
    private bool isDead = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead)
            return;

        if (other.CompareTag("Pipes"))
        {
            isDead = true;

            Debug.Log("GAME OVER !");

            GetComponent<FishPlayerMovement>().enabled = false;
        }
    }
}