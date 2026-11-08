using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LifeManager : MonoBehaviour
{
    [Header("Lives")]
    [SerializeField] private int startingLives = 3;

    [Header("Heart UI")]
    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite fullHeart;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private float gameOverDelay = 0.1f;

    private int currentLives;
    private bool isGameOver = false;

    private void Start()
    {
        currentLives = startingLives;

        // Make sure the game starts normally
        Time.timeScale = 1f;

        // Hide the ending screen at the start
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        UpdateHearts();

        Debug.Log("Lives: " + currentLives);
    }

    public void LoseLife()
    {
        if (isGameOver) return;

        currentLives--;
        currentLives = Mathf.Max(currentLives, 0);

        Debug.Log("Life lost! Remaining lives: " + currentLives);
        UpdateHearts();

        if (currentLives <= 0)
        {
            GameOver();
        }
    }

    public void InstantDeath()
    {
        if (isGameOver) return;

        currentLives = 0;
        UpdateHearts();
        GameOver();
    }

    private void UpdateHearts()
    {
        if (hearts == null) return;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null) continue;

            // Hearts are ordered left to right; deplete the leftmost slot first.
            bool hasLife = i >= hearts.Length - currentLives;
            hearts[i].enabled = hasLife;
            if (hasLife && fullHeart != null)
                hearts[i].sprite = fullHeart;
        }
    }

    private void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("GAME OVER");

        StartCoroutine(ShowGameOverScreen());
    }

    private IEnumerator ShowGameOverScreen()
    {
        // Give the explosion a moment to play
        yield return new WaitForSecondsRealtime(gameOverDelay);

        // Display the score-aware death screen when it is wired in the scene.
        if (gameOverPanel != null)
        {
            GameOverScreen gameOverScreen = gameOverPanel.GetComponent<GameOverScreen>();
            if (gameOverScreen != null)
            {
                ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
                int finalScore = scoreManager != null ? scoreManager.CurrentScore : 0;
                gameOverScreen.ShowGameOver(finalScore);
            }
            else
            {
                gameOverPanel.SetActive(true);
            }
        }

        // Stop gameplay
        Time.timeScale = 0f;
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }
}
