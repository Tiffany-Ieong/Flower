using UnityEngine;

public class GhostTrail : MonoBehaviour
{
    private Color trailColor = Color.white;

    public void SetTrailColor(Color color)
    {
        trailColor = color;
        Debug.Log($"[GhostTrail] Set trail for {name}: {color}");
    }

    public Color GetTrailColor()
    {
        Debug.Log($"[GhostTrail] Get trail for {name}: {trailColor}");
        return trailColor;
    }
}