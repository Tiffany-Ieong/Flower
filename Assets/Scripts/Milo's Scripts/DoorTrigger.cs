using UnityEngine;
using System.Collections;

public class InitialGhostSpawner : MonoBehaviour
{
    public GameObject ghostPrefab;
    public Transform playerSpawnPoint;
    public float minSpawnDistance = 2f;
    public float maxSpawnDistance = 5f;
    public float spawnDelay = 0.5f;

    private bool hasSpawned = false;

    void Start()
    {
        if (playerSpawnPoint == null)
        {
            GameObject marker = GameObject.FindGameObjectWithTag("PlayerSpawn");
            if (marker != null) playerSpawnPoint = marker.transform;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasSpawned) return;
        if (!other.CompareTag("Player")) return;

        hasSpawned = true;
        StartCoroutine(SpawnGhost());
    }

    private IEnumerator SpawnGhost()
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
        Quaternion ghostRotation = dirToPlayer == Vector3.zero ? Quaternion.identity : Quaternion.LookRotation(dirToPlayer);

        GameObject newGhost = Instantiate(ghostPrefab, spawnPos, ghostRotation);

        ColourAgent newAgent = newGhost.GetComponent<ColourAgent>();
        Renderer newRenderer = newGhost.GetComponent<Renderer>();
        if (newAgent != null)
        {
            Color randomColor = Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.8f, 1f);
            newAgent.SetColor(randomColor);
            if (newRenderer != null)
                newRenderer.material.color = randomColor;
        }

        ReplayGhost replayGhost = newGhost.GetComponent<ReplayGhost>();
        if (replayGhost != null)
            replayGhost.Initialize(replay, spawnPos, ghostRotation);
        else
            Destroy(newGhost);
    }
}