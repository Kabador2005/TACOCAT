using UnityEngine;
using UnityEngine.InputSystem;

public class TacoBlock : MonoBehaviour
{
    private const float Skin = 0.001f;

    private Collider2D[] cols;
    private Rigidbody2D body;
    private ContactFilter2D filter;
    private readonly RaycastHit2D[] hits = new RaycastHit2D[8];
    private readonly Collider2D[] overlaps = new Collider2D[8];

    private float stepX;
    private float minX;
    private float maxX;

    public Collider2D LastBlocker { get; private set; }
    public bool IsSoftDropping => Keyboard.current != null && Keyboard.current.sKey.isPressed;

    public Bounds Bounds
    {
        get
        {
            Bounds b = cols[0].bounds;
            for (int i = 1; i < cols.Length; i++) b.Encapsulate(cols[i].bounds);
            return b;
        }
    }
    
    public void Init(float stepX, float minX, float maxX, LayerMask stackMask)
    {
        this.stepX = stepX;
        this.minX = minX;
        this.maxX = maxX;

        cols = GetComponentsInChildren<Collider2D>();

        body = GetComponent<Rigidbody2D>();
        if (body == null) body = gameObject.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;

        filter = new ContactFilter2D();
        filter.SetLayerMask(stackMask);
        filter.useTriggers = true; 
    }

    public float GetFreeDistance(Vector2 dir, float maxDistance)
    {
        Physics2D.SyncTransforms();

        int count = body.Cast(dir.normalized, filter, hits, maxDistance);
        float free = maxDistance;
        LastBlocker = null;

        for (int i = 0; i < count; i++)
        {
            if (hits[i].distance < free)
            {
                free = hits[i].distance;
                LastBlocker = hits[i].collider;
            }
        }

        return Mathf.Max(0f, free - Skin);
    }

    private void Update()
    {
        if (body == null || Keyboard.current == null) return;

        if (Keyboard.current.aKey.wasPressedThisFrame) TryMoveSideways(Vector2.left);
        else if (Keyboard.current.dKey.wasPressedThisFrame) TryMoveSideways(Vector2.right);

        if (Keyboard.current.qKey.wasPressedThisFrame) TryRotate(90f);   
        else if (Keyboard.current.eKey.wasPressedThisFrame) TryRotate(-90f); 
    }

    private void TryMoveSideways(Vector2 dir)
    {
        float dx = dir.x * stepX;

        if (Bounds.min.x + dx < minX - 0.001f || Bounds.max.x + dx > maxX + 0.001f) return;

        if (GetFreeDistance(dir, stepX) >= stepX - 2f * Skin)
        {
            transform.position += (Vector3)(dir * stepX);
        }
    }

    private bool IsOverlappingStack()
    {
        Physics2D.SyncTransforms();
        return body.Overlap(filter, overlaps) > 0;
    }

    private bool FitsInPlayArea()
    {
        Physics2D.SyncTransforms();
        Bounds b = Bounds;
        return b.min.x >= minX - 0.001f && b.max.x <= maxX + 0.001f;
    }

    private void TryRotate(float angle)
    {
        Vector3 oldPos = transform.position;
        Quaternion oldRot = transform.rotation;

        transform.Rotate(0f, 0f, angle);
        
        float[] kicks = { 0f, stepX, -stepX };
        foreach (float kick in kicks)
        {
            transform.position = oldPos + Vector3.right * kick;
            SnapToGrid(minX, stepX);   

            if (FitsInPlayArea() && !IsOverlappingStack())
                return; 
        }
        
        transform.position = oldPos;
        transform.rotation = oldRot;
        Physics2D.SyncTransforms();
    }

    public void SnapToGrid(float originX, float cellSize)
    {
        Physics2D.SyncTransforms();
        float offset = Bounds.min.x - originX;
        float snappedMinX = originX + Mathf.Round(offset / cellSize) * cellSize;
        transform.position += Vector3.right * (snappedMinX - Bounds.min.x);
        Physics2D.SyncTransforms();
    }
}