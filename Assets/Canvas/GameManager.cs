using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TMP_Text TxtScore;
    public TMP_Text TxtBestScore;

    private int score;
    public int Score
    {
        get { return score; }

        set
        {
            score = value;
            TxtScore.text = "Score " + score;
        }
    }

    private int bestScore;
    public int BestScore
    {
        get { return bestScore; }

        set
        {
            bestScore = value;
            TxtBestScore.text = "Best Score " + bestScore;
        }
    }

    private void Start()
    {
        BestScore = PlayerPrefs.GetInt("BestScore");
    }

    public void SaveBestScore()
    {
        PlayerPrefs.SetInt("BestScore", BestScore);
    }


    public void AddOnePoint()
    {
        Score++;
    }

}