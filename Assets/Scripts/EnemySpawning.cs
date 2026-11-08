using UnityEngine;

public class EnemySpawing : MonoBehaviour
{
    public GameObject[] EnemyPrefabs;

    public float minSpawnTime = 3f;
    public float maxSpawnTime = 5f;
    public float spawnYpos = 8f;
    public float[] lanes = { -5f, -2f, 2f, 5f };

    [Header("Spawn Clearance")]
    [SerializeField] private Camera spawnCamera;
    [SerializeField, Min(0f)] private float spawnClearance = 0.75f;

    private float Spawntime;

    void Start()
    {
        if (spawnCamera == null)
            spawnCamera = Camera.main;

        SetNextSpawnTime();
    }

    void Update()
    {
        Spawntime -= Time.deltaTime;
        if (Spawntime <= 0f)
        {
            SpawnEnemy();
            SetNextSpawnTime();
        }
    }

    void SetNextSpawnTime()
    {
        Spawntime = Random.Range(minSpawnTime, maxSpawnTime);
    }

    void SpawnEnemy()
    {
        if (EnemyPrefabs.Length == 0) return;

        bool customerAlreadyExists = false;
        EnemyBehavior[] activeEnemies = FindObjectsByType<EnemyBehavior>(FindObjectsSortMode.None);
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null && enemy.enemyType == EnemyBehavior.EnemyType.Customer)
            {
                customerAlreadyExists = true;
                break;
            }
        }


        int randomIndex = Random.Range(0, EnemyPrefabs.Length);
        GameObject selectedPrefab = EnemyPrefabs[randomIndex];

        
        if (selectedPrefab == null)
        {
            Debug.LogWarning("An enemy prefab slot is empty or missing!");
            return;
        }

        EnemyBehavior prefabBehavior = selectedPrefab.GetComponent<EnemyBehavior>();
        if (prefabBehavior != null && prefabBehavior.enemyType == EnemyBehavior.EnemyType.Customer && customerAlreadyExists)
        {
            return;
        }

        float randomX = lanes[Random.Range(0, lanes.Length)];
        Vector2 spawnPos = new Vector2(randomX, spawnYpos);

        bool entersFromBottom = prefabBehavior != null && prefabBehavior.enemyType == EnemyBehavior.EnemyType.SpeedDemon;
        if (entersFromBottom)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                spawnPos = new Vector2(randomX, player.transform.position.y - 5f);
            }
        }

        // Spawn the full vehicle outside the viewport so long sprites don't appear clipped.
        GameObject spawnedEnemy = Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
        MoveOutsideCameraView(spawnedEnemy, entersFromBottom);
    }

    private void MoveOutsideCameraView(GameObject spawnedEnemy, bool entersFromBottom)
    {
        if (spawnCamera == null)
            spawnCamera = Camera.main;
        if (spawnCamera == null || spawnedEnemy == null)
            return;

        float cameraDepth = Mathf.Abs(spawnedEnemy.transform.position.z - spawnCamera.transform.position.z);
        float viewportEdgeY = spawnCamera.ViewportToWorldPoint(
            new Vector3(0.5f, entersFromBottom ? 0f : 1f, cameraDepth)).y;

        bool hasBounds = false;
        Bounds bounds = new Bounds(spawnedEnemy.transform.position, Vector3.zero);

        foreach (Renderer renderer in spawnedEnemy.GetComponentsInChildren<Renderer>(true))
        {
            if (!hasBounds)
            {
                bounds = renderer.bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(renderer.bounds);
            }
        }

        foreach (Collider2D collider in spawnedEnemy.GetComponentsInChildren<Collider2D>(true))
        {
            if (!hasBounds)
            {
                bounds = collider.bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(collider.bounds);
            }
        }

        if (!hasBounds)
            return;

        float currentEdge = entersFromBottom ? bounds.max.y : bounds.min.y;
        float targetEdge = entersFromBottom
            ? viewportEdgeY - spawnClearance
            : viewportEdgeY + spawnClearance;
        spawnedEnemy.transform.position += Vector3.up * (targetEdge - currentEdge);
    }
}
