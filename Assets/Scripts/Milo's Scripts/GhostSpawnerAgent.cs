using UnityEngine;
using System.Collections;

public class GhostSpawnerAgent : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject ghostPrefab;
    public Transform playerSpawnPoint;
    public float minSpawnDistance = 2f;
    public float maxSpawnDistance = 5f;
    public float spawnDelay = 0.5f;

    private bool hasSpawned = false;
    private GhostData ghostData;
    private Collider myCollider;
    private Renderer ghostRenderer;

    void Start()
    {
        ghostData = GetComponent<GhostData>();
        myCollider = GetComponent<Collider>();
        ghostRenderer = GetComponent<Renderer>();

        if (playerSpawnPoint == null)
        {
            GameObject marker = GameObject.FindGameObjectWithTag("PlayerSpawn");
            if (marker != null) playerSpawnPoint = marker.transform;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasSpawned) return;

        bool isPlayer = other.CompareTag("Player");
        bool isOtherGhost = other.CompareTag("Ghost");
        if (!isPlayer && !isOtherGhost) return;
        if (other.gameObject == gameObject) return;

        hasSpawned = true;

        // Turn body white – but DO NOT disable collider (trail colour stays)
        if (ghostData != null)
            ghostData.SetBodyColor(Color.white);

        StartCoroutine(SpawnNewGhost());
    }

    private IEnumerator SpawnNewGhost()
    {
        yield return new WaitForSeconds(spawnDelay);
        if (playerSpawnPoint == null) yield break;

        ReplayData replay = ReplayLibrary.GetRandomReplay();
        if (replay == null) yield break;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) yield break;

        Vector2 randomCircle = Random.insideUnitCircle * Random.Range(minSpawnDistance, maxSpawnDistance);
        Vector3 offset = new Vector3(randomCircle.x, 0f, randomCircle.y);
        Vector3 spawnPos = playerSpawnPoint.position + offset;

        Vector3 dirToPlayer = player.transform.position - spawnPos;
        dirToPlayer.y = 0f;
        Quaternion ghostRotation = Quaternion.LookRotation(dirToPlayer);

        GameObject newGhost = Instantiate(ghostPrefab, spawnPos, ghostRotation);
        GhostData newData = newGhost.GetComponent<GhostData>();
        if (newData != null)
        {
            Color randomColor = Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.8f, 1f);
            newData.Initialize(randomColor);
        }

        // Ensure the new ghost's collider is a trigger (prefab should already have it)
        Collider newCollider = newGhost.GetComponent<Collider>();
        if (newCollider != null && !newCollider.isTrigger)
            newCollider.isTrigger = true;

        ReplayGhost replayGhost = newGhost.GetComponent<ReplayGhost>();
        if (replayGhost != null) replayGhost.Initialize(replay, spawnPos, ghostRotation);
        else Destroy(newGhost);
    }
}