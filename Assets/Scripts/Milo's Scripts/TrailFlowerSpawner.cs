using UnityEngine;

public class TrailFlowerSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject flowerPrefab;
    public float spawnInterval = 0.3f;
    public int flowersPerSpawn = 3;
    public float spawnRadius = 1.5f;
    public float minScale = 0.8f;
    public float maxScale = 1.2f;
    public float yOffset = 0f;

    private float timer = 0f;
    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        // Do NOT spawn if player is inside the exclusion zone
        if (ExclusionZone.IsPlayerInside)
            return;

        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        if (distanceMoved < 0.1f) return;

        lastPosition = transform.position;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            for (int i = 0; i < flowersPerSpawn; i++)
            {
                SpawnFlower();
            }
        }
    }

    void SpawnFlower()
    {
        Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
        randomOffset.y = 0;
        Vector3 spawnPos = transform.position + randomOffset;
        spawnPos.y = transform.position.y + yOffset;

        GameObject flower = Instantiate(flowerPrefab, spawnPos, Quaternion.identity);
        flower.transform.rotation = Quaternion.Euler(-90f, Random.Range(0f, 360f), 0f);
        float scale = Random.Range(minScale, maxScale);
        flower.transform.localScale = Vector3.one * scale;

        BendWhenNear bend = flower.GetComponent<BendWhenNear>();
        if (bend != null) bend.playerTransform = transform;
    }
}