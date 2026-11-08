using UnityEngine;

/// <summary>
/// Editor helper: describe a block as rows of 'X' (filled) and '.' (empty).
/// Right-click the component header > "Build Colliders" to replace the prefab's
/// colliders with one exact BoxCollider2D per cell, centered on the pivot.
/// Remove this component from the prefab when you're done (it does nothing at runtime).
/// </summary>
public class TacoShapeBuilder : MonoBehaviour
{
    [SerializeField] private float cellSize = 0.35f;

    [TextArea(3, 8)]
    [SerializeField] private string shape = "X";

    [ContextMenu("Build Colliders")]
    public void Build()
    {
#if UNITY_EDITOR
        foreach (Collider2D old in GetComponents<Collider2D>())
            UnityEditor.Undo.DestroyObjectImmediate(old);

        string[] rows = shape.Replace("\r", "").Split('\n');
        int height = rows.Length;
        int width = 0;
        foreach (string row in rows) width = Mathf.Max(width, row.Length);

        for (int r = 0; r < height; r++)
        {
            for (int c = 0; c < rows[r].Length; c++)
            {
                if (rows[r][c] != 'X' && rows[r][c] != 'x') continue;

                BoxCollider2D box = UnityEditor.Undo.AddComponent<BoxCollider2D>(gameObject);
                box.size = new Vector2(cellSize, cellSize);
                box.offset = new Vector2(
                    (c - (width - 1) * 0.5f) * cellSize,
                    ((height - 1) * 0.5f - r) * cellSize);
                box.isTrigger = true;
            }
        }

        UnityEditor.EditorUtility.SetDirty(gameObject);
        Debug.Log($"{name}: built colliders for a {width}x{height} shape.");
#endif
    }
}