
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightController : MonoBehaviour
{
    public Light2D globalLight;
    public Camera cam;

    public Color dayLightColor = Color.white;
    public float dayIntensity = 1f;
    public Color dayBackground = new Color(0.5f, 0.7f, 0.9f);

    public Color nightLightColor = new Color32(28, 32, 46, 255);
    public float nightIntensity = 0.7f;
    public Color nightBackground = new Color32(28, 32, 46, 255);

    public float transitionDuration = 2f;

    private Coroutine running;

    public void StartNight()
    {
        StartTransition(true);
    }

    public void StartDay()
    {
        StartTransition(false);
    }

    void StartTransition(bool night)
    {
        if (running != null)
        {
            StopCoroutine(running);
        }

        running = StartCoroutine(TransitionLighting(night));
    }

    IEnumerator TransitionLighting(bool night)
    {
        Color targetColor = night ? nightLightColor : dayLightColor;
        float targetIntensity = night ? nightIntensity : dayIntensity;
        Color targetBackground = night ? nightBackground : dayBackground;

        Color startColor = globalLight.color;
        float startIntensity = globalLight.intensity;
        Color startBackground = cam.backgroundColor;

        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.SmoothStep(0f, 1f, elapsed / transitionDuration);

            globalLight.color = Color.Lerp(startColor, targetColor, t);

            globalLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);

            cam.backgroundColor = Color.Lerp(startBackground, targetBackground, t);

            yield return null;
        }

        globalLight.color = targetColor;
        globalLight.intensity = targetIntensity;
        cam.backgroundColor = targetBackground;

        running = null;
    }
}
