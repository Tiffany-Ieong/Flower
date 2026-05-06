using UnityEngine;

public class ExclusionZone : MonoBehaviour
{
    public static bool IsPlayerInside = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            IsPlayerInside = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            IsPlayerInside = false;
    }
}