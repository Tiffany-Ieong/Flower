using UnityEngine;

public class ColourAgent : MonoBehaviour
{
    private bool canColor = true;
    private Color bodyColor = Color.white;

    void Start()
    {
        if (CompareTag("Player"))
        {
            canColor = false;
            bodyColor = Color.white;
        }
        else
        {
            canColor = true;
            bodyColor = Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.8f, 1f);
        }
    }

    public void SetColor(Color newColor)
    {
        bodyColor = newColor;
        canColor = true;
    }

    public Color GetColor() => bodyColor;
    public bool CanColor() => canColor;
}