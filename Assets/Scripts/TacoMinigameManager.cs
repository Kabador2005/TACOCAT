using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacoMinigameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject[] ingredientPrefabs;
    [SerializeField] private GameObject tacoMinigameUIPanel;
    [SerializeField] private Transform tacoShellTransform;
    [SerializeField] private Collider2D playArea; // Assigned to PlayArea trigger box
    [SerializeField] private Material minigameUnlitMaterial;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private int successfulOrderBonus = 100;

    [Header("Layers")]
    [Tooltip("Layer used by the taco shell collider AND landed ingredients.")]
    [SerializeField] private string stackLayerName = "TacoStack";

    [Header("Gameplay")]
    [SerializeField] private float SpawnGap = 0.35f;
    [SerializeField] private float dropInterval = 0.14f;
    [SerializeField] private float softDropInterval = 0.025f;
    [SerializeField] private float blockStepSize = 0.35f;
    [SerializeField] private float horizontalStep = 0.35f;
    [SerializeField] private float halfPlayWidth = 3f;     
    [SerializeField] private float ingredientScale = 1f;
    [SerializeField] private float resultHoldTime = 1.2f;

    [Header("Order Settings")]
    [SerializeField] private int minBlocks = 4;
    [SerializeField] private int maxBlocks = 8;
    [SerializeField] private Transform stackLimitLine;
    [SerializeField] private float fitHeight = 4f;

    private int targetBlocks;
    private float limitY;
    public int TargetBlocks => targetBlocks;
    public int BlocksRemaining => targetBlocks - stackedCount;
    public bool IsActive { get; private set; }

    public event Action<int, int> ProgressChanged; // Target blocks, blocks remaining
    public event Action<bool> OrderFinished;       // Success flag

    private float currentBlockSpawnY;
    private LayerMask stackMask;
    private int stackLayer;

    private TacoBlock currentBlock;
    private readonly List<GameObject> spawnedBlocks = new List<GameObject>();
    private EnemyBehavior currentCustomer;

    private float dropTimer;
    private int stackedCount;
    private bool isEnding;

    private void Awake()
    {
        ApplyUnlitMaterialToPanel();

        if (scoreManager == null)
            scoreManager = FindFirstObjectByType<ScoreManager>();

        // TacoGameRoot is hidden until a customer reaches the delivery position.
        if (tacoMinigameUIPanel != null)
            tacoMinigameUIPanel.SetActive(false);

        stackLayer = LayerMask.NameToLayer(stackLayerName);
        if (stackLayer < 0)
        {
            Debug.LogError($"Layer '{stackLayerName}' doesn't exist. Add it in Project Settings > Tags and Layers.");
        }
        stackMask = LayerMask.GetMask(stackLayerName);
    }

    private void ApplyUnlitMaterialToPanel()
    {
        if (tacoMinigameUIPanel == null || minigameUnlitMaterial == null)
            return;

        foreach (SpriteRenderer spriteRenderer in tacoMinigameUIPanel.GetComponentsInChildren<SpriteRenderer>(true))
            spriteRenderer.sharedMaterial = minigameUnlitMaterial;
    }

    private void Update()
    {
        if (!IsActive || currentBlock == null || isEnding) return;

        float interval = currentBlock.IsSoftDropping ? softDropInterval : dropInterval;

        dropTimer += Time.deltaTime;
        if (dropTimer >= interval)
        {
            dropTimer = 0f;
            StepBlockDown();
        }
    }

    private void GetPlayRange(out float minX, out float maxX)
    {
        if (playArea != null)
        {
            minX = playArea.bounds.min.x;
            maxX = playArea.bounds.max.x;
            return;
        }

        Collider2D[] shellCols = tacoShellTransform != null ? tacoShellTransform.GetComponentsInChildren<Collider2D>() : null;
        if (shellCols != null && shellCols.Length > 0)
        {
            Bounds combined = shellCols[0].bounds;
            for (int i = 1; i < shellCols.Length; i++) combined.Encapsulate(shellCols[i].bounds);
            minX = combined.min.x;
            maxX = combined.max.x;
            return;
        }

        float shellX = tacoShellTransform != null ? tacoShellTransform.position.x : 0f;
        minX = shellX - halfPlayWidth;
        maxX = shellX + halfPlayWidth;
    }

    private float GetShellTopY()
    {
        if (tacoShellTransform == null) return 0f;
        float top = tacoShellTransform.position.y;
        foreach (Collider2D c in tacoShellTransform.GetComponentsInChildren<Collider2D>())
            top = Mathf.Max(top, c.bounds.max.y);
        return top;
    }

    public void StartTacoMinigame(EnemyBehavior customer)
    {
        if (IsActive || isEnding) return;

        currentCustomer = customer;
        stackedCount = 0;
        dropTimer = 0f;
        isEnding = false;

        ClearStack();
        targetBlocks = UnityEngine.Random.Range(minBlocks, maxBlocks + 1);
        limitY = stackLimitLine != null ? stackLimitLine.position.y : GetShellTopY() + fitHeight;

        if (tacoMinigameUIPanel != null) tacoMinigameUIPanel.SetActive(true);

        IsActive = true;
        ProgressChanged?.Invoke(targetBlocks, BlocksRemaining);
        SpawnNewBlock();
    }

    private void SpawnNewBlock()
    {
        if (ingredientPrefabs.Length == 0) return;

        GameObject prefab = ingredientPrefabs[UnityEngine.Random.Range(0, ingredientPrefabs.Length)];
        Vector3 spawnPos = tacoShellTransform != null ? tacoShellTransform.position : Vector3.zero;

        GameObject go = Instantiate(prefab, spawnPos, Quaternion.identity, tacoShellTransform != null ? tacoShellTransform.parent : null);
        go.transform.localScale = new Vector3(ingredientScale, ingredientScale, 1f);

        // Keep falling ingredients on the same layer as the shell and draw them in front of it.
        if (tacoShellTransform != null && tacoShellTransform.TryGetComponent(out SpriteRenderer shellRenderer))
        {
            foreach (SpriteRenderer ingredientRenderer in go.GetComponentsInChildren<SpriteRenderer>())
            {
                ingredientRenderer.sortingLayerID = shellRenderer.sortingLayerID;
                ingredientRenderer.sortingOrder = shellRenderer.sortingOrder + 1;
                if (minigameUnlitMaterial != null)
                    ingredientRenderer.sharedMaterial = minigameUnlitMaterial;
            }
        }

        spawnedBlocks.Add(go);

        if (go.TryGetComponent(out Rigidbody2D rb))
        {
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        foreach (Collider2D c in go.GetComponentsInChildren<Collider2D>())
            c.isTrigger = true;

        currentBlock = go.GetComponent<TacoBlock>();
        if (currentBlock == null) currentBlock = go.AddComponent<TacoBlock>();

        GetPlayRange(out float playMinX, out float playMaxX);
        currentBlock.Init(horizontalStep, playMinX, playMaxX, stackMask);

        Physics2D.SyncTransforms();
        float lift = (limitY + SpawnGap) - currentBlock.Bounds.min.y;
        go.transform.position += Vector3.up * lift;
        Physics2D.SyncTransforms();

        float centerOffset = ((playMinX + playMaxX) * 0.5f) - currentBlock.Bounds.center.x;
        go.transform.position += Vector3.right * centerOffset;
        currentBlock.SnapToGrid(playMinX, horizontalStep);

        Physics2D.SyncTransforms();
        currentBlockSpawnY = currentBlock.transform.position.y;
    }

    private void StepBlockDown()
    {
        float free = currentBlock.GetFreeDistance(Vector2.down, blockStepSize);
        bool blocked = free < blockStepSize - 0.002f;

        currentBlock.transform.position += Vector3.down * free;

        if (blocked) 
        {
            LandCurrentBlock();
        }
    }

    private void LandCurrentBlock()
    {
        GameObject landed = currentBlock.gameObject;

        Physics2D.SyncTransforms(); 
        Bounds landedBounds = currentBlock.Bounds;

        currentBlock.enabled = false;
        Destroy(currentBlock);

        foreach (Transform t in landed.GetComponentsInChildren<Transform>())
            t.gameObject.layer = stackLayer;

        currentBlock = null;
        stackedCount++;
        ProgressChanged?.Invoke(targetBlocks, BlocksRemaining);

        // Fail check: Exact rim hits pass (landedBounds.max.y > limitY + 0.001f)
        if (landedBounds.max.y > limitY + 0.001f)
        {
            EndTacoMinigame(false);
            return;
        }

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
        if (!IsActive || isEnding) return;
        isEnding = true;
        IsActive = false;

        if (currentBlock != null)
        {
            currentBlock.enabled = false;
            currentBlock = null;
        }

        OrderFinished?.Invoke(success);
        GameSfx.PlayOrderResult(success);
        StartCoroutine(EndSequenceRoutine(success));
    }

    /// <summary>
    /// Immediately closes the current order when the truck collides with a customer.
    /// This also cancels a pending result sequence so it cannot award a late bonus.
    /// </summary>
    public void CancelActiveOrder()
    {
        if (!IsActive && !isEnding) return;

        StopAllCoroutines();
        IsActive = false;
        isEnding = false;

        if (currentBlock != null)
            currentBlock.enabled = false;

        ClearStack();

        if (tacoMinigameUIPanel != null)
            tacoMinigameUIPanel.SetActive(false);

        // If a different customer was waiting, let it leave instead of freezing
        // in the waiting state. A collided customer is destroyed by its handler.
        if (currentCustomer != null)
            currentCustomer.FailCustomerOrder();

        currentCustomer = null;
    }

    private IEnumerator EndSequenceRoutine(bool success)
    {
        yield return new WaitForSeconds(resultHoldTime);

        ClearStack();
        if (tacoMinigameUIPanel != null) tacoMinigameUIPanel.SetActive(false);

        if (success)
        {
            if (scoreManager != null)
                scoreManager.AddBonusPoints(successfulOrderBonus);
            else
                Debug.LogWarning("Taco order succeeded, but no ScoreManager is assigned.");

            if (currentCustomer != null) currentCustomer.CompleteCustomerOrder();
        }
        else
        {
            if (currentCustomer != null) currentCustomer.FailCustomerOrder();
        }

        currentCustomer = null;
        isEnding = false;
    }
}
