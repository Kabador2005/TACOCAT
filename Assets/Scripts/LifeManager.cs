using System.Collections;
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    [Header("Lives")]
    [SerializeField] private int startingLives = 3;

    [Header("Heart UI")]
    [SerializeField] private UnityEngine.UI.Image[] hearts;   // One Image per life, left to right
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;                // Leave empty to hide lost hearts instead

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

        if (hearts != null && hearts.Length < startingLives)
            Debug.LogWarning("LifeManager: fewer heart Images assigned than startingLives.");

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

            bool hasLife = i < currentLives;

            if (emptyHeart != null && fullHeart != null)
            {
                // Swap sprites between full and empty
                hearts[i].enabled = true;
                hearts[i].sprite = hasLife ? fullHeart : emptyHeart;
            }
            else
            {
                // No empty sprite assigned: just hide lost hearts
                hearts[i].enabled = hasLife;
            }
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

        // Display the ending screen
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        // Stop gameplay
        Time.timeScale = 0f;
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }
}