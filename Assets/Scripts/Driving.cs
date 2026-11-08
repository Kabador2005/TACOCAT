using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Driving : MonoBehaviour
{
    public float MoveSpeed = 2.5f;
    public float MoveSize = 1.0f;
    public float MaxAngle = 15f;
    public float AngleSpeed = 15f;

    [Header("Lane Positions")]
    // Fixed: Properly ordered and unique lane coordinates
    public float[] lanes = { -5f, -2f, 2f, 5f };
    private int currentLaneIndex = 2; // Index 2 corresponds to 2f

    private Vector2 TargetPos;
    private Rigidbody2D rb;
    private float currentAngle = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        currentLaneIndex = GetClosestLaneIndex(rb.position.x);
        TargetPos = new Vector2(lanes[currentLaneIndex], rb.position.y);
        rb.position = TargetPos;
    }

    void Update()
    {
        MyInput();
        HandleTilt();
    }

    private void FixedUpdate()
    {
        Vector2 newPos = Vector2.MoveTowards(rb.position, TargetPos, MoveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
    }

    private void MyInput()
    {
        if (Vector2.Distance(rb.position, TargetPos) < 0.001f)
        {
            if (Keyboard.current != null)
            {
                // Move Right
                if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
                {
                    if (currentLaneIndex < lanes.Length - 1)
                    {
                        currentLaneIndex++;
                        TargetPos = new Vector2(lanes[currentLaneIndex], rb.position.y);
                    }
                }
                // Move Left
                else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
                {
                    if (currentLaneIndex > 0)
                    {
                        currentLaneIndex--;
                        TargetPos = new Vector2(lanes[currentLaneIndex], rb.position.y);
                    }
                }
            }
        }
    }

    private void HandleTilt()
    {
        float TargetAngle = 0f;

        // Use a small threshold (0.01f) to prevent tiny floating-point jitter/tweaking when arrived
        if (Mathf.Abs(TargetPos.x - rb.position.x) > 0.01f)
        {
            if (TargetPos.x > rb.position.x)
            {
                TargetAngle = -MaxAngle;
            }
            else if (TargetPos.x < rb.position.x)
            {
                TargetAngle = MaxAngle;
            }
        }
        else
        {
            TargetAngle = 0f; // Force upright when position is reached
        }

        currentAngle = Mathf.Lerp(currentAngle, TargetAngle, AngleSpeed * Time.deltaTime);
        rb.rotation = currentAngle;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        TargetPos = rb.position;
        currentLaneIndex = GetClosestLaneIndex(rb.position.x);
    }

    int GetClosestLaneIndex(float PosX)
    {
        int closestIndex = 0;
        float minDist = Mathf.Abs(PosX - lanes[0]);

        for (int i = 1; i < lanes.Length; i++)
        {
            float Dist = Mathf.Abs(PosX - lanes[i]);
            if (Dist < minDist)
            {
                minDist = Dist;
                closestIndex = i;
            }
        }
        return closestIndex;
    }
}