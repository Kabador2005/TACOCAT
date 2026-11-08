using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("KM");
    }

    public void doExitGame() {
        Application.Quit();
    }
}
