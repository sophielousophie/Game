using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SpaceToReplay : MonoBehaviour
{
    private BirdController bird;

    private void Start()
    {
        GameObject birdObject = GameObject.Find("FlappyBird");

        if (birdObject != null)
        {
            bird = birdObject.GetComponent<BirdController>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (bird != null && bird.BirdIsDead)
            {
                SceneManager.LoadScene(
                    SceneManager.GetActiveScene().buildIndex
                );
            }
        }
    }

}

