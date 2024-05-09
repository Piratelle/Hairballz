using UnityEngine;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private Dictionary<int, int> playerScores = new Dictionary<int, int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void IncrementScore(int playerNum)
    {
        if (!playerScores.ContainsKey(playerNum))
        {
            playerScores[playerNum] = 0;
        }
        playerScores[playerNum]++;
        Debug.Log($"Player {playerNum} score: {playerScores[playerNum]}");
    }

    public int GetScore(int playerNum)
    {
        if (playerScores.TryGetValue(playerNum, out int score))
        {
            return score;
        }
        return 0; // Default score if not found
    }
}

