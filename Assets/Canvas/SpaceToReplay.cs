
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpaceToReplay : MonoBehaviour
{
    private BirdController bird;
    private FishDead fish;

    private void Start()
    {
        GameObject birdObject = GameObject.Find("FlappyBird");
        GameObject fishObject = GameObject.Find("FishPlayer");

        if (birdObject != null)
            bird = birdObject.GetComponent<BirdController>();

        if (fishObject != null)
            fish = fishObject.GetComponent<FishDead>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bool birdIsDead = bird != null && bird.BirdIsDead;
            bool fishIsDead = fish != null && fish.isDead;

            if (birdIsDead || fishIsDead)
            {
                SceneManager.LoadScene(
                    SceneManager.GetActiveScene().buildIndex
                );
            }
        }
    }
}
