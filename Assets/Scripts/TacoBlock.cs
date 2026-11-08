using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class TacoBlock : MonoBehaviour
{
    private const float Skin = 0.001f;

    private Collider2D col;
    private ContactFilter2D filter;
    private readonly RaycastHit2D[] hits = new RaycastHit2D[8];

    private float stepX;
    private float minX;
    private float maxX;

    public Bounds Bounds => col.bounds;

    /// <summary>Called by the manager right after spawning.</summary>
    public void Init(float stepX, float minX, float maxX, LayerMask stackMask)
    {
        this.stepX = stepX;
        this.minX = minX;
        this.maxX = maxX;

        col = GetComponent<Collider2D>();

        filter = new ContactFilter2D();
        filter.SetLayerMask(stackMask);
        filter.useTriggers = true; // landed blocks are triggers
    }

    /// <summary>
    /// How far this block can travel in 'dir' (up to maxDistance) before touching the stack/shell.
    /// </summary>
    public float GetFreeDistance(Vector2 dir, float maxDistance)
    {
        int count = col.Cast(dir.normalized, filter, hits, maxDistance);
        float free = maxDistance;

        for (int i = 0; i < count; i++)
        {
            free = Mathf.Min(free, hits[i].distance);
        }

        return Mathf.Max(0f, free - Skin);
    }

    private void Update()
    {
        if (col == null || Keyboard.current == null) return;

        if (Keyboard.current.aKey.wasPressedThisFrame) TryMoveSideways(Vector2.left);
        else if (Keyboard.current.dKey.wasPressedThisFrame) TryMoveSideways(Vector2.right);
    }

    private void TryMoveSideways(Vector2 dir)
    {
        float newX = transform.position.x + dir.x * stepX;
        if (newX < minX || newX > maxX) return;

        // Only move if the whole step is clear (blocks can't slide through the stack)
        if (GetFreeDistance(dir, stepX) >= stepX - 2f * Skin)
        {
            transform.position += (Vector3)(dir * stepX);
        }
    }
}