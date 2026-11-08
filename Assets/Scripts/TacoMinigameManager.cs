using System.Collections.Generic;
using UnityEngine;

public class TacoMinigameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject[] ingredientPrefabs;
    [SerializeField] private GameObject tacoMinigameUIPanel;
    [SerializeField] private Transform tacoShellTransform;

    [Header("Layers")]
    [Tooltip("Layer used by the taco shell collider AND landed ingredients.")]
    [SerializeField] private string stackLayerName = "TacoStack";

    [Header("Gameplay")]
    [SerializeField] private float spawnOffsetY = 4f;
    [SerializeField] private float dropInterval = 0.8f;
    [SerializeField] private float blockStepSize = 0.5f;   
    [SerializeField] private float horizontalStep = 1f;    
    [SerializeField] private float halfPlayWidth = 3f;     
    [SerializeField] private float ingredientScale = 2f;
    [SerializeField] private int maxStackHeight = 5;

    private LayerMask stackMask;
    private int stackLayer;

    private TacoBlock currentBlock;
    private readonly List<GameObject> spawnedBlocks = new List<GameObject>();
    private EnemyBehavior currentCustomer;

    private float dropTimer;
    private int stackedCount;
    private bool isMinigameActive;

    private void Awake()
    {
        stackLayer = LayerMask.NameToLayer(stackLayerName);
        if (stackLayer < 0)
        {
            Debug.LogError($"Layer '{stackLayerName}' doesn't exist. Add it in Project Settings > Tags and Layers.");
        }
        stackMask = LayerMask.GetMask(stackLayerName);
    }

    private void Start()
    {
        if (tacoMinigameUIPanel != null) tacoMinigameUIPanel.SetActive(false);
    }

    private void Update()
    {
        if (!isMinigameActive || currentBlock == null) return;

        dropTimer += Time.deltaTime;
        if (dropTimer >= dropInterval)
        {
            dropTimer = 0f;
            StepBlockDown();
        }
    }

    public void StartTacoMinigame(EnemyBehavior customer)
    {
        currentCustomer = customer;
        stackedCount = 0;
        dropTimer = 0f;

        ClearStack();

        if (tacoMinigameUIPanel != null) tacoMinigameUIPanel.SetActive(true);

        isMinigameActive = true;
        SpawnNewBlock();
    }

    private void SpawnNewBlock()
    {
        if (ingredientPrefabs.Length == 0) return;

        GameObject prefab = ingredientPrefabs[Random.Range(0, ingredientPrefabs.Length)];
        Vector3 spawnPos = tacoShellTransform.position + Vector3.up * spawnOffsetY;

        GameObject go = Instantiate(prefab, spawnPos, Quaternion.identity, transform);
        go.transform.localScale = new Vector3(ingredientScale, ingredientScale, 1f);
        spawnedBlocks.Add(go);

        // Physics is not used for movement, so make the body kinematic + collider a trigger
        if (go.TryGetComponent(out Rigidbody2D rb))
        {
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        if (go.TryGetComponent(out Collider2D col)) col.isTrigger = true;

        currentBlock = go.GetComponent<TacoBlock>();
        if (currentBlock == null) currentBlock = go.AddComponent<TacoBlock>();

        float shellX = tacoShellTransform.position.x;
        currentBlock.Init(horizontalStep, shellX - halfPlayWidth, shellX + halfPlayWidth, stackMask);
    }

    private void StepBlockDown()
    {
        // Ask the collider how far it can really fall: lands flush on the shell / other blocks
        float free = currentBlock.GetFreeDistance(Vector2.down, blockStepSize);
        currentBlock.transform.position += Vector3.down * free;

        bool blocked = free < blockStepSize - 0.002f;
        if (blocked) LandCurrentBlock();
    }

    private void LandCurrentBlock()
    {
        GameObject landed = currentBlock.gameObject;

        // Lock it in place and make it part of the stack for future collision checks
        currentBlock.enabled = false;
        Destroy(currentBlock);
        landed.layer = stackLayer;
        currentBlock = null;
        stackedCount++;

        // Stack reached the spawn point -> no room left
        float spawnY = tacoShellTransform.position.y + spawnOffsetY;
        if (landed.GetComponent<Collider2D>().bounds.max.y >= spawnY)
        {
            EndTacoMinigame(false);
            return;
        }

        if (stackedCount >= maxStackHeight) EndTacoMinigame(true);
        else SpawnNewBlock();
    }

    private void ClearStack()
    {
        currentBlock = null;

        foreach (GameObject block in spawnedBlocks)
        {
            if (block != null) Destroy(block);
        }
        spawnedBlocks.Clear();
    }

    public void EndTacoMinigame(bool success)
    {
        isMinigameActive = false;
        ClearStack();

        if (tacoMinigameUIPanel != null) tacoMinigameUIPanel.SetActive(false);

        if (success)
        {
            Debug.Log("Taco completed successfully! Customer drives away happy.");
            if (currentCustomer != null) currentCustomer.CompleteCustomerOrder();
        }
        else
        {
            Debug.Log("Taco order failed.");
        }

        currentCustomer = null;
    }
}