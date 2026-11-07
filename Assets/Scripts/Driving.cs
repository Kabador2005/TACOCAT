using UnityEngine;
using UnityEngine.InputSystem;

public class Driving : MonoBehaviour
{
    public float MoveSpeed = 2.5f;
    public float MoveSize = 1.0f;
    public float MaxAngle = 15f;
    public float AngleSpeed = 15f;


    private Vector2 TargetPos;
    private Rigidbody2D rb;
    private float currentAngle = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        TargetPos = rb.position;
    }

    // Update is called once per frame
    void Update()
    {
        MyInput();
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
                if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
                {
                    TargetPos += Vector2.right * MoveSize;
                }
                else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
                {
                    TargetPos += Vector2.left * MoveSize;
                }
            }
        }

        float TargetAngle = 0f;
        if (TargetPos.x > rb.position.x)
        {
            TargetAngle = -MaxAngle;
        }
        else if (TargetPos.x < rb.position.x)
        {
            TargetAngle = MaxAngle;
        }

        currentAngle = Mathf.Lerp(currentAngle, TargetAngle, AngleSpeed * Time.deltaTime);
        rb.rotation = currentAngle;


    }
}
