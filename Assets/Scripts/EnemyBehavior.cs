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
        Cop,
        Customer
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

    [Header("Customer Settings")]
    public float sideOffsetX = 2.5f;     
    public float targetStopYOffset = 0f;
    private Transform playerTransform;
    private Vector2 customerTargetPos;
    private bool hasTriggeredOrder = false;
    private TacoMinigameManager tacoManager;

    private enum CustomerState 
    { 
        DrivingToPosition, 
        WaitingForTaco, 
        DrivingAway 
    }

    private CustomerState customerState = CustomerState.DrivingToPosition;

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
        else if (enemyType == EnemyType.Customer)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;

                float dx = transform.position.x - playerTransform.position.x;
                float sideDirection = Mathf.Abs(dx) > 0.01f ? Mathf.Sign(dx) : (Random.value > 0.5f ? 1f : -1f);

                float targetX = playerTransform.position.x + (sideOffsetX * sideDirection);
                float targetY = playerTransform.position.y + targetStopYOffset;
                customerTargetPos = new Vector2(targetX, targetY);
            }
            else
            {
                customerTargetPos = new Vector2(2f, 0f);
            }

            tacoManager = FindFirstObjectByType<TacoMinigameManager>();
            customerState = CustomerState.DrivingToPosition;
        }

    }

    void Update()
    {
        HandleBehaviors();

        if (enemyType != EnemyType.Customer && (transform.position.y < -12f || transform.position.y > 15f))
        {
            Destroy(gameObject);
        }
        else if (enemyType == EnemyType.Customer && (transform.position.y < -12f || transform.position.y > 20f))
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
                if (dropTimer >= dropInterval)
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

            case EnemyType.Customer:
                HandleCustomerBehavior();
                break;
        }
    }

    void HandleCustomerBehavior()
    {

        switch (customerState)
        {
            case CustomerState.DrivingToPosition:
                transform.position = Vector2.MoveTowards(transform.position, customerTargetPos, (baseSpeed * 1.2f) * Time.deltaTime);
                rb.linearVelocity = Vector2.zero;

                if(Vector2.Distance(transform.position, customerTargetPos) < 0.1f)
                {
                    customerState = CustomerState.WaitingForTaco;
                    
                    if(!hasTriggeredOrder && tacoManager != null)
                    {
                        hasTriggeredOrder = true;
                        tacoManager.StartTacoMinigame(this);
                    }
                }
                break;

            case CustomerState.WaitingForTaco:
                rb.linearVelocity = Vector2.zero;
                break;

            case CustomerState.DrivingAway:
                rb.linearVelocity = new Vector2(0f, baseSpeed * 2f);
                break;
        }


    }

    public void CompleteCustomerOrder()
    {
        customerState = CustomerState.DrivingAway;
    }

    void DropDebrisSpread()
    {
        if (debrisPrefab == null) return;

        Collider2D[] myColliders = GetComponentsInChildren<Collider2D>();
        Collider2D[] spawnedColliders = new Collider2D[spreadCount];

        float startAngle = -spreadAngle / 2f;
        float angleStep = spreadCount > 1 ? spreadAngle / (spreadCount - 1) : 0f;

        for (int i = 0; i < spreadCount; i++)
        {
            float currentAngle = startAngle + (i * angleStep);
            Vector2 spawnDir = Quaternion.Euler(0f, 0f, currentAngle) * Vector2.down;

            GameObject debris = Instantiate(debrisPrefab, transform.position, Quaternion.identity);

            if (debris.TryGetComponent(out Rigidbody2D debrisRb))
            {
                debrisRb.linearVelocity = spawnDir * 4f;
            }

            Collider2D debrisCol = debris.GetComponent<Collider2D>();
            spawnedColliders[i] = debrisCol;

            if (debrisCol != null)
            {
                // Ignore the hillbilly itself
                foreach (Collider2D mine in myColliders)
                    Physics2D.IgnoreCollision(debrisCol, mine, true);

                // Ignore the other pieces from this same drop so they don't push each other apart
                for (int j = 0; j < i; j++)
                {
                    if (spawnedColliders[j] != null)
                        Physics2D.IgnoreCollision(debrisCol, spawnedColliders[j], true);
                }
            }

            Destroy(debris, 8f); // cleanup so debris doesn't pile up forever
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.enabled) return;

        if (collision.gameObject == GameObject.FindGameObjectWithTag("Player"))
        {
            Debug.Log("YOU FAT FUCK I HATE YOU");
            Destroy(gameObject);
        }
    }

}
