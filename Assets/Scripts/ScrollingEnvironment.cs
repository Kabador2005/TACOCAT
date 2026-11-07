using UnityEngine;

public class ScrollingEnvironment : MonoBehaviour
{
    public float scrollSpeed = 15f;
    public float resetPosition = -20f;
    public float startPosition = 20f;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * scrollSpeed * Time.deltaTime);

        if (transform.position.y <= resetPosition)
        {
            transform.position = new Vector3(transform.position.x, startPosition, transform.position.z);
        }
    }
}
