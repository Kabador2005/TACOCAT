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
    [SerializeField] private float SpawnGap = 1f;
    [SerializeField] private float dropInterval = 0.3f;
    [SerializeField] private float softDropInterval = 0.05f;
    [SerializeField] private float blockStepSize = 0.5f;   
    [SerializeField] private float horizontalStep = 1f;    
    [SerializeField] private float halfPlayWidth = 3f;     
    [SerializeField] private float ingredientScale = 2f;
    

    [Header("Order Settings")]
    [SerializeField] private int minBlocks = 4;
    [SerializeField] private int maxBlocks = 8;
    [SerializeField] private Transform stackLimitLine;
    [SerializeField] private float fitHeight = 4f;

    private int targetBlocks;
    private float limitY;
    public int TargetBlocks => targetBlocks;
    public int BlocksRemaining => targetBlocks - stackedCount;

    private float currentBlockSpawnY;

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

        float interval = currentBlock.IsSoftDropping ? softDropInterval : dropInterval;

        dropTimer += Time.deltaTime;
        if (dropTimer >= interval)
        {
            dropTimer = 0f;
            StepBlockDown();
        }
    }
    private float GetShellTopY()
    {
        float top = tacoShellTransform.position.y;
        foreach (Collider2D c in tacoShellTransform.GetComponentsInChildren<Collider2D>())
            top = Mathf.Max(top, c.bounds.max.y);
        return top;
    }

    public void StartTacoMinigame(EnemyBehavior customer)
    {
        currentCustomer = customer;
        stackedCount = 0;
        dropTimer = 0f;

        ClearStack();
        targetBlocks = Random.Range(minBlocks, maxBlocks + 1);
        limitY = stackLimitLine != null ? stackLimitLine.position.y : GetShellTopY() + fitHeight;
        Debug.Log($"Order: stack {targetBlocks} ingredients under y = {limitY:F2}");

        if (tacoMinigameUIPanel != null) tacoMinigameUIPanel.SetActive(true);

        isMinigameActive = true;
        SpawnNewBlock();
    }

    private void SpawnNewBlock()
    {
        if (ingredientPrefabs.Length == 0) return;

        GameObject prefab = ingredientPrefabs[Random.Range(0, ingredientPrefabs.Length)];
        Vector3 spawnPos = tacoShellTransform.position;

        GameObject go = Instantiate(prefab, spawnPos, Quaternion.identity);
        go.transform.localScale = new Vector3(ingredientScale, ingredientScale, 1f);
        spawnedBlocks.Add(go);

        // Physics is not used for movement, so make the body kinematic + collider a trigger
        if (go.TryGetComponent(out Rigidbody2D rb))
        {
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        foreach (Collider2D c in go.GetComponentsInChildren<Collider2D>())
            c.isTrigger = true;

        currentBlock = go.GetComponent<TacoBlock>();
        if (currentBlock == null) currentBlock = go.AddComponent<TacoBlock>();

        float shellX = tacoShellTransform.position.x;
        currentBlock.Init(horizontalStep, shellX - halfPlayWidth, shellX + halfPlayWidth, stackMask);

        float lift = (limitY + SpawnGap) - currentBlock.Bounds.min.y;
        go.transform.position += Vector3.up * lift;


        Collider2D shellCol = tacoShellTransform.GetComponentInChildren<Collider2D>();
        float shellCenterX = shellCol != null ? shellCol.bounds.center.x : tacoShellTransform.position.x;

        float cx = go.transform.position.x;
        currentBlock.Init(horizontalStep, cx - halfPlayWidth, cx + halfPlayWidth, stackMask);

        Physics2D.SyncTransforms();
        currentBlockSpawnY = currentBlock.transform.position.y;
    }

    private void StepBlockDown()
    {
        float free = currentBlock.GetFreeDistance(Vector2.down, blockStepSize);
        bool blocked = free < blockStepSize - 0.002f;

        // Blocked before it ever moved down = stack has reached the spawn point
        bool neverMoved = Mathf.Abs(currentBlock.transform.position.y - currentBlockSpawnY) < 0.001f;
        if (blocked && neverMoved && free < 0.01f)
        {
            EndTacoMinigame(false);
            return;
        }

        currentBlock.transform.position += Vector3.down * free;

        if (blocked) LandCurrentBlock();
    }

    private void LandCurrentBlock()
    {
        GameObject landed = currentBlock.gameObject;

        Physics2D.SyncTransforms(); // bounds are stale right after we moved the block
        Bounds landedBounds = currentBlock.Bounds;

        currentBlock.enabled = false;
        Destroy(currentBlock);

        foreach (Transform t in landed.GetComponentsInChildren<Transform>())
            t.gameObject.layer = stackLayer;

        currentBlock = null;
        stackedCount++;

        // Doesn't fit: this block sticks out above the limit
        if (landedBounds.max.y > limitY)
        {
            Debug.Log("Ingredients didn't fit!");
            EndTacoMinigame(false);
            return;
        }

        // Fit them all
        if (stackedCount >= targetBlocks) EndTacoMinigame(true);
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