using UnityEngine;

public class GhostData : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public Renderer bodyRenderer;

    [HideInInspector] public Color trailColor;  // used by flowers

    void Awake()
    {
        if (bodyRenderer == null)
            bodyRenderer = GetComponent<Renderer>();
    }

    public void Initialize(Color color)
{
    trailColor = color;
    if (bodyRenderer != null)
        bodyRenderer.material.color = color;
    Debug.Log($"Ghost spawned: {gameObject.name}, trail color = {color}, body color = {color}");
}

    public void SetBodyColor(Color color)
    {
        if (bodyRenderer != null)
            bodyRenderer.material.color = color;
    }

    public Color GetTrailColor() => trailColor;
}