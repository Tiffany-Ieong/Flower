using UnityEngine;

public class ThresholdLightTrigger : MonoBehaviour
{
    public Light roomLight;
    public float enteredIntensity = 0.5f;

    private float originalIntensity;

    void Start()
    {
        originalIntensity = roomLight.intensity;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        roomLight.intensity = enteredIntensity;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        roomLight.intensity = originalIntensity;
    }
}