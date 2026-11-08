
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;

    private float score = 0f;

    public int CurrentScore => Mathf.FloorToInt(score);

    void Update()
    {
        score += Time.deltaTime/5;
        UpdateScoreDisplay();
    }

    public void AddBonusPoints(int points)
    {
        score += points;
        UpdateScoreDisplay();
    }

    void UpdateScoreDisplay()
    {
        scoreText.text = "SCORE:" + Mathf.FloorToInt(score).ToString();
    }
}
