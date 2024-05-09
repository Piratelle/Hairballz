using UnityEngine;
using UnityEngine.UI; // Use TMPro if using TextMeshPro
using TMPro; // Uncomment this if using TextMeshPro

public class ScoreDisplay : MonoBehaviour
{
    public TMP_Text scoreText; // Use public TMP_Text scoreText if using TextMeshPro
    private int lastScore = -1;

    void Update()
    {
        int currentScore = ScoreManager.Instance.GetScore(1); // Assuming 1 is the player number
        if (currentScore != lastScore)
        {
            scoreText.text = $"Score: {currentScore}";
            lastScore = currentScore;
        }
    }
}

