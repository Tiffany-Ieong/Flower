using UnityEngine;

public class LightTrigger : MonoBehaviour
{
    public Light roomLight;
    float originalIntensity;

    void Start()
    {
        originalIntensity = roomLight.intensity;
    }

    void OnTriggerEnter(Collider other)
    {
        roomLight.intensity = 0.5f;
    }

    void OnTriggerExit(Collider other)
    {
        roomLight.intensity = originalIntensity;
    }
}