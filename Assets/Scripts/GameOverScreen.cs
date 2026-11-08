
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    public GameObject gameOverPanel;

    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    public TMP_Text newHighScoreText;

    public void ShowGameOver(int score)
    {
        gameOverPanel.SetActive(true);

        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (score > highScore)
        {
            highScore = score;

            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();

            newHighScoreText.gameObject.SetActive(true);
        }
        else
        {
            newHighScoreText.gameObject.SetActive(false);
        }

        scoreText.text = "SCORE: " + score;
        highScoreText.text = "HIGH SCORE: " + highScore;

        Time.timeScale = 0f;
    }

    public void Replay()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(0);
    }
}
