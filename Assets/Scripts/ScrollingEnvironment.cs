using UnityEngine;

public class ScrollingEnvironment : MonoBehaviour
{
    public float scrollSpeed = 8.5f;

    [HideInInspector]
    public float tileMapHeight = 71f;

    private Vector3 startPosition;

    public EnvironmentManager envManager;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.Translate(Vector3.down * scrollSpeed * Time.deltaTime);

        if (transform.position.y <= startPosition.y - tileMapHeight)
        {
            transform.position = new Vector3(startPosition.x, startPosition.y, startPosition.z);

            if (envManager != null)
            {
                envManager.OnLoopCompleted();
            }
        }
    }
}
