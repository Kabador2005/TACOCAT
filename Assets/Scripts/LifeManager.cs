
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    [Header("Lives")]
    [SerializeField] private int startingLives = 3;

    private int currentLives;
    private bool isGameOver = false;

    private void Start()
    {
        currentLives = startingLives;
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
        isGameOver = true;
        Debug.Log("GAME OVER");
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }
}
