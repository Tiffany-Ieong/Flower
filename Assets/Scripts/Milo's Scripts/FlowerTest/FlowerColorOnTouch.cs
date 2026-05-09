using UnityEngine;
using System.Collections.Generic;

public class FlowerColorOnTouch : MonoBehaviour
{
    public ParticleSystem fireflies;
    private Renderer[] renderers;
    private FlowerSway flowerSway;

    void Awake()
    {
        SetupRenderers();
        flowerSway = GetComponent<FlowerSway>();
    }

    void SetupRenderers()
    {
        Renderer[] all = GetComponentsInChildren<Renderer>();
        ParticleSystemRenderer psr = fireflies != null ? fireflies.GetComponent<ParticleSystemRenderer>() : null;
        List<Renderer> list = new List<Renderer>();
        foreach (var r in all)
            if (psr == null || r != psr) list.Add(r);
        renderers = list.ToArray();

        foreach (var r in renderers)
        {
            r.material.DisableKeyword("_EMISSION");
            r.material.SetColor("_EmissionColor", Color.black);
            r.material.color = Color.white;
        }
        if (fireflies != null) { fireflies.Stop(); fireflies.Clear(); }
    }

    void OnTriggerEnter(Collider other)
    {
     
        Color color = Color.white;

        // Ghosts use GhostData
        GhostData ghost = other.GetComponent<GhostData>();
        if (ghost != null)
        {
            color = ghost.GetTrailColor();
        }
        else
        {
            // Player uses ColourAgent
            ColourAgent agent = other.GetComponent<ColourAgent>();
            if (agent != null && agent.CanColor())
                color = agent.GetColor();
            else
                return;
        }

        // Apply colour
        if (flowerSway != null)
            flowerSway.SetColor(color);
        else
        {
            foreach (var r in renderers)
            {
                r.material.EnableKeyword("_EMISSION");
                r.material.color = color;
                r.material.SetColor("_EmissionColor", color * 2f);
            }
        }
        if (fireflies != null && !fireflies.isPlaying)
            fireflies.Play();
    }
}