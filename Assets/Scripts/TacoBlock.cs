using UnityEngine;
using UnityEngine.InputSystem;
public class TacoBlock : MonoBehaviour
{
    public float StepSize = 1f;

    public float minXLimit = -1f;
    public float maxXLimit = 1f;

    private Rigidbody2D rb;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if(rb != null)
        {
            rb.gravityScale = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if(Keyboard.current == null)
        {
            return;
        }

        Vector3 Targetmove = Vector3.zero;

        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            Targetmove = Vector3.left * StepSize;
        }
        else if(Keyboard.current.dKey.wasPressedThisFrame)
        {
            Targetmove = Vector3.right * StepSize;
        }

        if (Targetmove != Vector3.zero)
        {
            Vector3 newPos = transform.position + Targetmove;

            if (newPos.x >= minXLimit && newPos.x <= maxXLimit)
            {
                transform.position = newPos;
            }
        }

    }    
}
