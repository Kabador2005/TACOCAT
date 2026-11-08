using System.Collections;
using UnityEngine;

public class TacoPanelPop : MonoBehaviour
{
    [SerializeField, Min(0.05f)] private float duration = 0.28f;
    [SerializeField, Range(0.1f, 1f)] private float startingScale = 0.72f;
    [SerializeField, Range(0f, 1f)] private float overshoot = 0.12f;

    private Coroutine popRoutine;

    private void OnEnable()
    {
        if (popRoutine != null)
            StopCoroutine(popRoutine);

        Vector3 targetScale = transform.localScale;
        transform.localScale = targetScale * startingScale;
        popRoutine = StartCoroutine(PopIn(targetScale));
    }

    private void OnDisable()
    {
        if (popRoutine != null)
            StopCoroutine(popRoutine);

        popRoutine = null;
    }

    private IEnumerator PopIn(Vector3 targetScale)
    {
        float elapsed = 0f;
        float back = 1f + overshoot;
        float backPlusOne = back + 1f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = 1f + backPlusOne * Mathf.Pow(t - 1f, 3f) + back * Mathf.Pow(t - 1f, 2f);
            transform.localScale = targetScale * Mathf.LerpUnclamped(startingScale, 1f, eased);
            yield return null;
        }

        transform.localScale = targetScale;
        popRoutine = null;
    }
}
