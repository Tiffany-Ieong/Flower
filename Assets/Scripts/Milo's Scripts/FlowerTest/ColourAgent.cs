using UnityEngine;

public class ColourAgent : MonoBehaviour
{
    [Tooltip("Can this agent colour flowers?")]
    private bool canColor = true;      // ghosts: true; player: false until activated

    public Color agentColor = Color.white;

    void Start()
    {
        if (CompareTag("Player"))
        {
            // Player cannot colour flowers until they pick up a colour
            canColor = false;
            agentColor = Color.white;
        }
        else
        {
            // Ghosts can colour from the start with a random colour
            canColor = true;
            agentColor = Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.8f, 1f);
        }
    }

    // Called when the player (or ghost) receives a new colour
    public void SetColor(Color newColor)
    {
        agentColor = newColor;
        canColor = true;     // now allowed to colour flowers
    }

    public Color GetColor()
    {
        return agentColor;
    }

    public bool CanColor()
    {
        return canColor;
    }
}