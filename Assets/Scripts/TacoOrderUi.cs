using UnityEngine;
using TMPro;

public class TacoOrderUi : MonoBehaviour
{
    [SerializeField] private TacoMinigameManager manager;
    [SerializeField] private TMP_Text label;
    [SerializeField] private TMP_Text resultLabel;
    [SerializeField] private Color successColor = new Color(0.48f, 0.77f, 0.5f);
    [SerializeField] private Color failColor = new Color(0.9f, 0.33f, 0.24f);

    private void Awake()
    {
        if (label == null) label = GetComponent<TMP_Text>();
        if (manager == null) manager = FindFirstObjectByType<TacoMinigameManager>();
    }

    private void OnEnable()
    {
        if (manager != null)
        {
            manager.ProgressChanged += HandleProgressChanged;
            manager.OrderFinished += HandleOrderFinished;
        }

        if (resultLabel != null) resultLabel.gameObject.SetActive(false);
        RefreshText();
    }

    private void OnDisable()
    {
        if (manager != null)
        {
            manager.ProgressChanged -= HandleProgressChanged;
            manager.OrderFinished -= HandleOrderFinished;
        }
    }

    private void HandleProgressChanged(int target, int remaining)
    {
        RefreshText();
    }

    private void HandleOrderFinished(bool success)
    {
        if (resultLabel != null)
        {
            resultLabel.gameObject.SetActive(true);
            resultLabel.text = success ? "Order up" : "Order failed";
            resultLabel.color = success ? successColor : failColor;
        }
    }

    private void RefreshText()
    {
        if (manager == null || label == null) return;
        label.SetText("INGREDIENTS LEFT: {0}", manager.BlocksRemaining);
    }
}
