using UnityEngine;

public class FlowerSpawner : MonoBehaviour
{
    public GameObject flowerPrefab;
    public Transform player;                // assign your player's Transform here
    public int flowerCount = 1000;
    public float areaWidth = 100f;
    public float areaLength = 100f;
    public float minScale = 0.8f;
    public float maxScale = 1.2f;
    public float yOffset = 0f;

    void Start()
    {
        for (int i = 0; i < flowerCount; i++)
        {
            float randomX = Random.Range(-areaWidth / 2, areaWidth / 2);
            float randomZ = Random.Range(-areaLength / 2, areaLength / 2);

            Vector3 spawnPos = new Vector3(
                player.position.x + randomX,
                player.position.y + yOffset,
                player.position.z + randomZ);

            GameObject flower = Instantiate(flowerPrefab, spawnPos, Quaternion.identity);
            flower.transform.rotation = Quaternion.Euler(-90f, Random.Range(0f, 360f), 0f);
            float scale = Random.Range(minScale, maxScale);
            flower.transform.localScale = Vector3.one * scale;

            // Assign player reference to BendWhenNear script
            BendWhenNear bend = flower.GetComponent<BendWhenNear>();
            if (bend != null) bend.playerTransform = player;   // note: playerTransform, not player

            flower.transform.parent = transform;
        }
    }
}