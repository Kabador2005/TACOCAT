using System.Collections;
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    [Header("Lives")]
    [SerializeField] private int startingLives = 3;

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

        Debug.Log("Lives: " + currentLives);
    }

    public void LoseLife()
    {
        if (isGameOver) return;

        currentLives--;

        Debug.Log("Life lost! Remaining lives: " + currentLives);

        if (currentLives <= 0)
        {
            GameOver();
        }
    }

    public void InstantDeath()
    {
        if (isGameOver) return;

        currentLives = 0;
        GameOver();
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