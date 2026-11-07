using UnityEngine;

public class TacoMinigameManager : MonoBehaviour
{
    [Header("Ingredient Prefabs / other")]
    public GameObject[] ingredientPrefabs;
    public GameObject tacoMinigameUIPanel;

    [Header("Taco Shell")]
    public Transform TacoShellTransform;
    public float SpawnOffsetY = 3f;
    public float DropInterval = 1f;

    private GameObject CurrentBlock;
    private float dropTimer;
    private bool isMinigameActive = false;

    private int StackedIngredientsCount = 0;
    public int MaxStackHeight = 5;

    private EnemyBehavior currentCustomer;

    void Start()
    {
        // Ensure the minigame UI is hidden when the game starts
        if (tacoMinigameUIPanel != null)
        {
            tacoMinigameUIPanel.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMinigameActive || TacoShellTransform == null)
        { 
            return; 
        }

        dropTimer += Time.deltaTime;
        if (dropTimer >= DropInterval)
        {
            dropTimer = 0f;
            StepBlockDown();
        }

    }

    public void StartTacoMinigame()
    {
        isMinigameActive = true;
        StackedIngredientsCount = 0;

        // Pop up the UI panel on screen!
        if (tacoMinigameUIPanel != null)
        {
            tacoMinigameUIPanel.SetActive(true);
        }

        SpawnNewIngredientBlock();
    }

    void SpawnNewIngredientBlock()
    {
        if (ingredientPrefabs.Length == 0)
        {
            return;
        }

        int RandomIndex = Random.Range(0, ingredientPrefabs.Length);
        GameObject PrefabToSpawn = ingredientPrefabs[RandomIndex];

        Vector2 spawnPos = new Vector2(TacoShellTransform.position.x, TacoShellTransform.position.y + SpawnOffsetY);
        CurrentBlock = Instantiate(PrefabToSpawn, spawnPos, Quaternion.identity, TacoShellTransform);

        if(CurrentBlock.GetComponent<TacoBlock>() == null)
        {
            CurrentBlock.AddComponent<TacoBlock>();
        }

    }
    void StepBlockDown()
    {
        if (CurrentBlock != null)
        {
            CurrentBlock.transform.position += Vector3.down;

            float BottomLimit = TacoShellTransform.position.y - 1.5f + (StackedIngredientsCount * 0.5f);

            if (CurrentBlock.transform.position.y <= BottomLimit)
            {
                CurrentBlock = null;
                StackedIngredientsCount++;

                if (StackedIngredientsCount >= MaxStackHeight)
                {
                    EndTacoMinigame(true);
                }
                else
                {
                    SpawnNewIngredientBlock();
                }
            }
        }
    }
    public void EndTacoMinigame(bool success)
    {
        isMinigameActive = false;
        if (CurrentBlock != null)
        {
            Destroy(CurrentBlock);
        }

        // Hide the minigame UI panel when finished
        if (tacoMinigameUIPanel != null)
        {
            tacoMinigameUIPanel.SetActive(false);
        }

        if (success)
        {
            Debug.Log("Taco completed successfully! Customer drives away happy.");

            if(currentCustomer != null)
            {
                currentCustomer.CompleteCustomerOrder();
                currentCustomer = null;
            }

        }
        else
        {
            Debug.Log("Taco order failed.");
        }
    }
}


