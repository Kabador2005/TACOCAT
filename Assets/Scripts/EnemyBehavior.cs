using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public enum EnemyType
    {
        Grandma,        
        SpeedDemon,     
        SemiTruck,      
        Merger,         
        Motorbike,      
        BigButha,
        Hillbilly,
        Cop
    }

    public EnemyType enemyType;

    [Header("General Settings")]
    public float baseSpeed = 3f;

    [Header("Merger Settings")]
    public float laneSwitchInterval = 2f;
    public float laneStepSize = 1f;
    private float laneTimer;
    private float targetXPos;

    [Header("Motorbike Settings")]
    private float speedTimer;
    private float currentMultiplier = 1f;

    [Header("HillBilly")]
    public GameObject debrisPrefab;
    public float dropInterval = 2f;
    public int spreadCount = 3;
    public float spreadAngle = 45f;
    private float dropTimer;

    [Header("Cop Settings")]
    private Vector2 CopTargetPos;
    private bool CopHasReachedTarget = false;
    private float[] possibleLanes = { -2f, -1f, 1f, 2f };



    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        targetXPos = transform.position.x;

        if (enemyType == EnemyType.Cop)
        {
            float StartX = Random.value > 0.5f ? -3f : 3f;
            transform.position = new Vector2(StartX, transform.position.y);

            float TargetLaneX = possibleLanes[Random.Range(0, possibleLanes.Length)];
            float targetY = Random.Range(1f, 5f);
            CopTargetPos = new Vector2(TargetLaneX, targetY);
        }

    }

    void Update()
    {
        HandleBehaviors();

        // Clean up when off-screen safely
        if (transform.position.y < -12f || transform.position.y > 15f)
        {
            Destroy(gameObject);
        }
    }

    void HandleBehaviors()
    {
        if (rb == null) return;

        switch (enemyType)
        {
            case EnemyType.Grandma:
                rb.linearVelocity = new Vector2(0f, -baseSpeed * 0.5f);
                break;

            case EnemyType.SpeedDemon:
                baseSpeed += 4f * Time.deltaTime;
                rb.linearVelocity = new Vector2(0f, baseSpeed);
                break;

            case EnemyType.SemiTruck:
                rb.linearVelocity = new Vector2(0f, -baseSpeed * 0.8f);
                break;

            case EnemyType.Merger:
                laneTimer += Time.deltaTime;
                if (laneTimer >= laneSwitchInterval)
                {
                    laneTimer = 0f;
                    float direction = UnityEngine.Random.value > 0.5f ? 1f : -1f;
                    targetXPos += direction * laneStepSize;
                    targetXPos = Mathf.Clamp(targetXPos, -2f, 2f);
                }

                float newX = Mathf.MoveTowards(transform.position.x, targetXPos, 3f * Time.deltaTime);
                transform.position = new Vector3(newX, transform.position.y, 0f);
                rb.linearVelocity = new Vector2(0f, -baseSpeed);
                break;

            case EnemyType.Motorbike:
                speedTimer += Time.deltaTime;
                if (speedTimer > 1.5f)
                {
                    speedTimer = 0f;
                    currentMultiplier = UnityEngine.Random.Range(0.4f, 1.8f);
                }
                rb.linearVelocity = new Vector2(0f, -baseSpeed * currentMultiplier);
                break;

            case EnemyType.BigButha:
                rb.linearVelocity = new Vector2(0f, -baseSpeed * 0.7f);
                break;

            case EnemyType.Hillbilly:
                rb.linearVelocity = new Vector2(0f, -baseSpeed * 0.6f);


                dropTimer += Time.deltaTime;
                if(dropTimer >= dropInterval)
                {
                    dropTimer = 0f;
                    DropDebrisSpread();
                }
                break;

            case EnemyType.Cop:
                if (!CopHasReachedTarget)
                {
                    transform.position = Vector2.MoveTowards(transform.position, CopTargetPos, (baseSpeed * 1.5f) * Time.deltaTime);
                    rb.linearVelocity = Vector2.zero;


                    if (Vector2.Distance(transform.position, CopTargetPos) < 0.1f)
                    {
                        CopHasReachedTarget = true;
                    }
                }
                else
                {
                    rb.linearVelocity = new Vector2(0f, -baseSpeed * 1.2f);
                }
                break;
        }
    }

    void DropDebrisSpread()
    {
        if (debrisPrefab == null) return;

        float StartAngle = -spreadAngle / 2f;
        float angleStep = spreadCount > 1 ? spreadAngle / (spreadCount - 1) : 0f;

        for (int i = 0; i < spreadCount; i++)
        {
            float CurrentAngle = StartAngle + (i * angleStep);
            Quaternion rotation = Quaternion.Euler(0, 0f, CurrentAngle);
            Vector2 spawnDir = rotation * Vector2.down;

            GameObject debris = Instantiate(debrisPrefab, transform.position, Quaternion.identity);

            Rigidbody2D debrisRb = debris.GetComponent<Rigidbody2D>();
            if (debrisRb != null)
            {
                debrisRb.linearVelocity = spawnDir * 4f;
            }



        }
    }
}
