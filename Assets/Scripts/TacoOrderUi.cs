using UnityEngine;
using TMPro;

public class TacoOrderUi : MonoBehaviour
{

    [SerializeField] private TacoMinigameManager manager;
    [SerializeField] private TMP_Text label;

    
    void Awake()
    {
        if (label == null) label = GetComponent<TMP_Text>();
        if (manager == null) manager = FindFirstObjectByType<TacoMinigameManager>();
    }

    void Update()
    {
        if (manager == null || label == null) return;

        label.text = $"Order: {manager.TargetBlocks} ingredients\nLeft: {manager.BlocksRemaining}";
    }
}
