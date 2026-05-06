using UnityEngine;
using System.Collections;

public class GhostSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject ghostPrefab;               // must have ReplayGhost + ColourAgent
    public float minSpawnDistance = 2f;
    public float maxSpawnDistance = 5f;
    public float spawnDelay = 0.5f;

    private Transform playerSpawnPoint;
    private bool hasSpawned = false;

    void Start()
    {
        // Find the player spawn point by tag ("PlayerSpawn")
        GameObject spawnMarker = GameObject.FindGameObjectWithTag("PlayerSpawn");
        if (spawnMarker != null)
            playerSpawnPoint = spawnMarker.transform;
        else
            Debug.LogError("GhostSpawner: No GameObject with tag 'PlayerSpawn' found!");
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasSpawned) return;
        if (!other.CompareTag("Player")) return;

        hasSpawned = true;
        StartCoroutine(SpawnGhostAfterDelay());
    }

    private IEnumerator SpawnGhostAfterDelay()
    {
        yield return new WaitForSeconds(spawnDelay);

        if (playerSpawnPoint == null)
        {
            Debug.LogError("GhostSpawner: Player spawn point missing.");
            yield break;
        }

        // 1. Get a random replay
        ReplayData replay = ReplayLibrary.GetRandomReplay();
        if (replay == null)
        {
            Debug.LogError("GhostSpawner: No replay data available.");
            yield break;
        }

        // 2. Find current player position (for ghost facing)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("GhostSpawner: Player not found.");
            yield break;
        }

        // 3. Random spawn position around the fixed playerSpawnPoint
        Vector2 randomCircle = Random.insideUnitCircle * Random.Range(minSpawnDistance, maxSpawnDistance);
        Vector3 offset = new Vector3(randomCircle.x, 0f, randomCircle.y);
        Vector3 spawnPos = playerSpawnPoint.position + offset;

        // 4. Make ghost face toward the player's current position
        Vector3 dirToPlayer = player.transform.position - spawnPos;
        dirToPlayer.y = 0f;
        Quaternion ghostRotation = Quaternion.LookRotation(dirToPlayer);

        // 5. Instantiate ghost
        GameObject ghostObj = Instantiate(ghostPrefab, spawnPos, ghostRotation);

        // 6. Assign a random colour to the ghost (via ColourAgent)
        ColourAgent colourAgent = ghostObj.GetComponent<ColourAgent>();
        if (colourAgent != null)
        {
            // Bright, saturated random colour
            Color randomColor = Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.8f, 1f);
            colourAgent.SetColor(randomColor);
            Debug.Log($"Ghost spawned with colour {randomColor}");
        }
        else
        {
            Debug.LogWarning("Ghost prefab has no ColourAgent – will use default white.");
        }

        // 7. Initialise replay movement
        ReplayGhost ghost = ghostObj.GetComponent<ReplayGhost>();
        if (ghost != null)
        {
            ghost.Initialize(replay, spawnPos, ghostRotation);
        }
        else
        {
            Debug.LogError("Ghost prefab missing ReplayGhost component.");
            Destroy(ghostObj);
        }
    }
}