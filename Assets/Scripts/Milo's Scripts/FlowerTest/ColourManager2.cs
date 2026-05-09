using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public static ColorManager Instance;

    [SerializeField] private Color[] colors = { Color.red, Color.purple, Color.blue };
    private int currentIndex = -1;     // -1 = not activated yet
    public bool colorActivated = false;

    // Reference to the player's ColourAgent (assign in inspector or find at start)
    public ColourAgent playerColourAgent;

    public Color CurrentColor => colorActivated ? colors[currentIndex] : Color.white;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // If not assigned in inspector, try to find the player by tag
        if (playerColourAgent == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerColourAgent = player.GetComponent<ColourAgent>();
        }
    }

    // Call this method when the player picks up a colour‑changing power‑up / trigger
    public Color AdvanceColor()
    {
        currentIndex = (currentIndex + 1) % colors.Length;
        colorActivated = true;

        // Update the player's ColourAgent so that flowers touched by player use this color
        if (playerColourAgent != null)
            playerColourAgent.SetColor(colors[currentIndex]);

        return colors[currentIndex];
    }

    // Optional: manually set player's color without cycling
    public void SetPlayerColor(int index)
    {
        if (index < 0 || index >= colors.Length) return;
        currentIndex = index;
        colorActivated = true;
        if (playerColourAgent != null)
            playerColourAgent.SetColor(colors[currentIndex]);
    }
}