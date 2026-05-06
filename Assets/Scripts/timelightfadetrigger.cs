using UnityEngine;

public class TimeLightFadeTrigger : MonoBehaviour

{

    public Light roomLight;
    public float fadeSpeed = 0.5f; // interpreted as fade duration in seconds
    public float minimumIntensity = 0.2f;
    private float originalIntensity;
    private Coroutine fadeCoroutine;

    void Start()
    {
        if (roomLight == null) roomLight = GetComponent<Light>();
        if (roomLight == null)
        {
            Debug.LogWarning("TimeLightFadeTrigger: roomLight not assigned and no Light component found.");
            enabled = false;
            return;
        }
        originalIntensity = roomLight.intensity;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        StartFadeTo(minimumIntensity);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        StartFadeTo(originalIntensity);
    }

    private void StartFadeTo(float target)
    {
        if (roomLight == null) return;
        // stop any running fade first to ensure we sample the actual current intensity
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
        // snapshot current intensity after stopping previous coroutine
        float current = roomLight.intensity;
        if (fadeSpeed <= 0f)
        {
            roomLight.intensity = target;
            return;
        }
        fadeCoroutine = StartCoroutine(FadeToCoroutine(current, target));
    }

    private System.Collections.IEnumerator FadeToCoroutine(float start, float target)
    {
        float duration = Mathf.Max(0.0001f, fadeSpeed);
        if (Mathf.Approximately(start, target))
        {
            fadeCoroutine = null;
            yield break;
        }
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            roomLight.intensity = Mathf.Lerp(start, target, t);
            yield return null;
        }
        roomLight.intensity = target;
        fadeCoroutine = null;
    }
}
