
using UnityEngine;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class FishDead : MonoBehaviour
{
    public  bool isDead = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead)
            return;

        if (collision.gameObject.CompareTag("Pipes") ||
        collision.gameObject.name == "ZoneOutOfScreen")
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        GetComponent<AudioSource>().Play();
        GetComponent<BoxCollider2D>().enabled = false;

        GameObject[] GoPipes = GameObject.FindGameObjectsWithTag("Pipes");

        foreach (GameObject g in GoPipes)
        {
            Rigidbody2D rb = g.GetComponent<Rigidbody2D>();

            if (rb != null)
                rb.linearVelocity = Vector2.zero;
        }

        Rigidbody2D fishRb = GetComponent<Rigidbody2D>();

        if (fishRb != null)
            fishRb.linearVelocity = Vector2.zero;

        GetComponent<FishPlayerMovement>().enabled = false;

        GameObject.Find("SpawnerManager")
            .GetComponent<SpawnerManager>().StopSpawn();

        GameManager Gm = GameObject.Find("Canvas")
            .GetComponent<GameManager>();

        if (Gm.Score > Gm.BestScore)
        {
            PlayerPrefs.SetInt("BestScore", Gm.Score);
            Gm.BestScore = Gm.Score;
        }

        if (GameObject.Find("TxtReplay") != null)
        {
            GameObject.Find("TxtReplay")
                .GetComponent<TMP_Text>().enabled = true;
        }
    }
}
