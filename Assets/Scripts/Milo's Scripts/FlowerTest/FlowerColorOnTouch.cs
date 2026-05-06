using UnityEngine;
using System.Collections.Generic;

public class FlowerColorOnTouch : MonoBehaviour
{
    [Header("Visuals")]
    public ParticleSystem fireflies;      // optional particle effect

    private Renderer[] renderers;
    private FlowerSway flowerSway;

    void Awake()
    {
        SetupRenderers();
        flowerSway = GetComponent<FlowerSway>();
    }

    void SetupRenderers()
    {
        Renderer[] allRenderers = GetComponentsInChildren<Renderer>();
        ParticleSystemRenderer psRenderer = fireflies != null ? fireflies.GetComponent<ParticleSystemRenderer>() : null;

        List<Renderer> filtered = new List<Renderer>();
        foreach (Renderer r in allRenderers)
        {
            if (psRenderer != null && r == psRenderer) continue;
            filtered.Add(r);
        }
        renderers = filtered.ToArray();

        // Initial white, no emission
        foreach (Renderer r in renderers)
        {
            r.material.DisableKeyword("_EMISSION");
            r.material.SetColor("_EmissionColor", Color.black);
            r.material.color = Color.white;
        }

        if (fireflies != null)
        {
            fireflies.Stop();
            fireflies.Clear();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        ColourAgent agent = other.GetComponent<ColourAgent>();
        if (agent == null) return;
        if (!agent.CanColor()) return;   // player still starts unable to colour until first pickup

        Color newColor = agent.GetColor();

        // Apply colour via shader or direct material
        if (flowerSway != null)
            flowerSway.SetColor(newColor);
        else
        {
            foreach (Renderer r in renderers)
            {
                if (r == null) continue;
                r.material.EnableKeyword("_EMISSION");
                r.material.color = newColor;
                r.material.SetColor("_EmissionColor", newColor * 2f);
            }
        }

        // Play particle effect
        if (fireflies != null)
            fireflies.Play();
    }
}