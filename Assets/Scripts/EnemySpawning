using UnityEngine;

public class EnemySpawing : MonoBehaviour
{
    public GameObject[] EnemyPrefabs;

    public float minSpawnTime = 1f;
    public float maxSpawnTime = 2.5f;
    public float spawnYpos = 8f;
    public float[] lanes = { -2f, -1f, 1f, 2f };

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

        int randomIndex = Random.Range(0, EnemyPrefabs.Length);
        GameObject selectedPrefab = EnemyPrefabs[randomIndex];

        // Safety check to make sure the slot isn't empty/missing
        if (selectedPrefab == null)
        {
            Debug.LogWarning("An enemy prefab slot is empty or missing!");
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
