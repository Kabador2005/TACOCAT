using UnityEngine;
using UnityEngine.Tilemaps;

public class EnvironmentManager : MonoBehaviour
{
    private int loopsBeforeBridge;
    private int currentLoopCount = 0;

    public ScrollingEnvironment scrollingEnv;
    public Tilemap roadTilemap;
    
    public GameObject bridgePrefab;

    public DayNightController dayNightController;
    private bool isNight;

    void Start()
    {
        RandomizeLoopsBeforeBridge();

        isNight = false;

        if (bridgePrefab != null)
        {
            bridgePrefab.SetActive(false);
        }
    }

    void RandomizeLoopsBeforeBridge()
    {
        loopsBeforeBridge = Random.Range(3,5);

        Debug.Log($"Next bridge after {loopsBeforeBridge} loops.");
    }

    public void OnLoopCompleted()
    {
        currentLoopCount++;
        Debug.Log($"Completed loop: {currentLoopCount}");

        if (currentLoopCount == loopsBeforeBridge)
        {
            StartBridgeLoop();
        }
        else if (currentLoopCount == loopsBeforeBridge + 1)
        {
            EndBridgeLoop();
        }
    }

    void StartBridgeLoop()
    {
        if (bridgePrefab != null)
        {
            bridgePrefab.SetActive(true);
            scrollingEnv.tileMapHeight += 22;
        }
    }

    void EndBridgeLoop()
    {
        if (bridgePrefab != null)
        {
            bridgePrefab.SetActive(false);
        }

        currentLoopCount = 0;

        RandomizeLoopsBeforeBridge();

        scrollingEnv.tileMapHeight -= 22;

        if (isNight)
        {
            dayNightController.StartDay();
            isNight = false;
        }
        else
        {
            dayNightController.StartNight();
            isNight = true;
        }
    }
}