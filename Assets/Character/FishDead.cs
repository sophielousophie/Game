
using UnityEngine;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class FishDead : MonoBehaviour
{
    private bool isDead = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead || !collision.gameObject.CompareTag("Pipes"))
            return;

        isDead = true;

        // Jouer le son de collision
        GetComponent<AudioSource>().Play();

        // Désactiver le collider du poisson
        GetComponent<BoxCollider2D>().enabled = false;

        // Arrêter les tuyaux
        GameObject[] GoPipes = GameObject.FindGameObjectsWithTag("Pipes");

        foreach (GameObject g in GoPipes)
        {
            Rigidbody2D rb = g.GetComponent<Rigidbody2D>();

            if (rb != null)
                rb.linearVelocity = Vector2.zero;
        }

        // Arrêter le poisson
        Rigidbody2D fishRb = GetComponent<Rigidbody2D>();

        if (fishRb != null)
            fishRb.linearVelocity = Vector2.zero;

        // Désactiver le déplacement du poisson
        GetComponent<FishPlayerMovement>().enabled = false;

        // Arrêter le spawner
        GameObject.Find("SpawnerManager")
            .GetComponent<SpawnerManager>().StopSpawn();

        // Récupérer le GameManager
        GameManager Gm = GameObject.Find("Canvas").GetComponent<GameManager>();

        if (Gm.Score > Gm.BestScore)
        {
            PlayerPrefs.SetInt("BestScore", Gm.Score);
            Gm.BestScore = Gm.Score;
        }

        // Afficher Replay
        if (GameObject.Find("TxtReplay") != null)
        {
            GameObject.Find("TxtReplay")
                .GetComponent<TMP_Text>().enabled = true;
        }
    }
}
