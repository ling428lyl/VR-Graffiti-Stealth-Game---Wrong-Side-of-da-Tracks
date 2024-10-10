using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    void Start()
    {
        // Check if ScoreManager instance is available
        if (ScoreManager.Instance != null)
        {
            // Retrieve the current score from ScoreManager
            int currentScore = ScoreManager.Instance.GetCurrentScore();
            // Update the TMP_Text component with the current score
            scoreText.text = currentScore.ToString() + " PTS";
        }
        else
        {
            Debug.LogError("ScoreManager instance not found!");
            scoreText.text = "0 PTS"; // Fallback or error handling
        }
    }
}
