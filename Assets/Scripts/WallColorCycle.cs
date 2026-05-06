using UnityEngine;

public class WallColorCycle : MonoBehaviour
{
    private Renderer wallRenderer;
    private bool playerInside = false;

    void Start()
    {
        wallRenderer = GetComponent<Renderer>();
        // Wall starts neutral/white before first approach
        wallRenderer.material.color = Color.white;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (playerInside) return;
        playerInside = true;

        // Advance to next color
        Color newColor = ColorManager.Instance.AdvanceColor();

        // Update wall appearance
        wallRenderer.material.color = newColor;
        wallRenderer.material.EnableKeyword("_EMISSION");
        wallRenderer.material.SetColor("_EmissionColor", newColor * 3f);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }
}
