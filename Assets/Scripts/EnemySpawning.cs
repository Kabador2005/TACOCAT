using UnityEngine;

public class EnemySpawing : MonoBehaviour
{
    public GameObject[] EnemyPrefabs;

    public float minSpawnTime = 3f;
    public float maxSpawnTime = 5f;
    public float spawnYpos = 8f;
    public float[] lanes = { -5f, -2f, 2f, 5f };

    private float Spawntime;

    void Start()
    {
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

        if (randomIndex == 1)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                spawnPos = new Vector2(randomX, player.transform.position.y - 5f);
            }
        }

        // Instantiates a clone into the scene; leaves the source asset untouched
        Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
    }
}
