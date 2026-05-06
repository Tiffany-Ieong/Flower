using UnityEngine;
using System.Collections;

public class TrailFlower : MonoBehaviour
{
    private Renderer[] renderers;
    private bool isColored = false;
    private Collider flowerCollider;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        flowerCollider = GetComponent<Collider>();
    }

    public void InitAsWhite()
    {
        if (flowerCollider != null) flowerCollider.enabled = false;
        StartCoroutine(SetWhiteAndEnableCollider());
    }

    IEnumerator SetWhiteAndEnableCollider()
    {
        yield return null;

        foreach (Renderer r in renderers)
        {
            if (r == null) continue;
            r.material.DisableKeyword("_EMISSION");
            r.material.color = Color.white;
            r.material.SetColor("_EmissionColor", Color.black);
        }

        // Wait 2 seconds before enabling collider
        // giving the player time to walk away
        yield return new WaitForSeconds(2f);

        if (flowerCollider != null) flowerCollider.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (isColored) return;
        if (!ColorManager.Instance.colorActivated) return;

        ColourAgent agent = other.GetComponent<ColourAgent>();
        if (agent == null || !agent.CanColor()) return;

        isColored = true;

        Color activeColor = agent.GetColor();

        foreach (Renderer r in renderers)
        {
            if (r == null) continue;
            r.material.EnableKeyword("_EMISSION");
            r.material.color = activeColor;
            r.material.SetColor("_EmissionColor", activeColor * 2f);
        }

        FlowerColorOnTouch colorTouch = GetComponent<FlowerColorOnTouch>();
        if (colorTouch != null) colorTouch.enabled = true;

        Destroy(this);
    }
}