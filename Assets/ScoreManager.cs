using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    public UnityEvent<int> onScoreUpdated;
    private int totalScore = 0;
    public bool updateScore = false;
    private void Awake()
    {
        // Ensure only one instance of ScoreManager exists (Singleton pattern)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Keep the ScoreManager across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int GetCurrentScore()
    {
        return totalScore;
    }

    public void AddScore(int score)
    {
        if (!updateScore)
        {
            return;
        }

        totalScore += score;
        onScoreUpdated?.Invoke(totalScore);
    }

    // Method to reset the player's score
    public void ResetScore()
    {
        totalScore = 0;
        onScoreUpdated?.Invoke(totalScore);
    }
}
