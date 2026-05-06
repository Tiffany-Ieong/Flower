using UnityEngine;

public class flowerbend1 : MonoBehaviour
{
    private Quaternion startRotation;
    private bool playerInside = false;

    void Start()
    {
        startRotation = transform.localRotation;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }

    void Update()
    {
        if (playerInside)
        {
            transform.localRotation = Quaternion.Lerp(
                transform.localRotation,
                startRotation * Quaternion.Euler(30f, 0f, 0f),
                Time.deltaTime * 8f);
        }
        else
        {
            transform.localRotation = Quaternion.Lerp(
                transform.localRotation,
                startRotation,
                Time.deltaTime * 5f);
        }
    }
}