using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Spawns a field of flowers with:
/// - Constant density inside a core radius.
/// - Gradual density falloff to zero at fade radius.
/// - Option to control density (flowers/m²) or absolute flower count.
/// </summary>
public class FlowerFieldSpawner : MonoBehaviour
{
    [Header("Spawn Mode")]
    public bool useDensityMode = true;

    [Tooltip("Flowers per square metre inside the constant density core.")]
    public float density = 2.5f;               // flowers/m²

    [Tooltip("Absolute flower count (only used if useDensityMode = false).")]
    public int flowerCount = 800;

    [Header("Spawn Area (Spotlight Pattern)")]
    public GameObject flowerPrefab;

    [Tooltip("Inner radius: constant flower density (the bright centre).")]
    public float constantDensityRadius = 3f;

    [Tooltip("Outer radius: density reaches zero here. No flowers beyond this.")]
    public float fadeRadius = 15f;

    [Tooltip("Falloff power: 1 = linear fade, 2 = smooth, higher = sharper edge.")]
    [Range(0.5f, 5f)]
    public float falloffPower = 1.5f;

    [Header("Flower Orientation")]
    [Tooltip("Which axis of the prefab points up? (e.g., (0,1,0) for Y, (0,0,1) for Z)")]
    public Vector3 flowerUpAxis = Vector3.up;
    public bool randomRotation = true;
    public bool randomScale = false;
    public float minScale = 0.7f;
    public float maxScale = 1.3f;

    [Header("Ground Alignment")]
    public bool alignToGround = true;
    public LayerMask groundMask = -1;
    public float groundRaycastDistance = 50f;
    public float groundOffset = 0.05f;

    [Header("Hierarchy")]
    public Transform parentTransform;
    public bool destroyOldOnSpawn = true;

    private List<GameObject> spawnedFlowers = new List<GameObject>();

    private void Start()
    {
        SpawnFlowers();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0.2f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, constantDensityRadius);
        Gizmos.color = new Color(1f, 0.8f, 0.2f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, fadeRadius);
    }

    public void SpawnFlowers()
    {
        if (flowerPrefab == null)
        {
            Debug.LogError("FlowerFieldSpawner: No flower prefab assigned!", this);
            return;
        }

        if (destroyOldOnSpawn)
            ClearFlowers();

        int totalToSpawn = useDensityMode ? CalculateFlowerCountFromDensity() : flowerCount;
        if (totalToSpawn <= 0)
        {
            Debug.LogWarning("FlowerFieldSpawner: No flowers to spawn (total count = 0).");
            return;
        }

        int spawned = 0;
        int maxAttempts = totalToSpawn * 10;
        int attempts = 0;

        while (spawned < totalToSpawn && attempts < maxAttempts)
        {
            attempts++;

            // Generate a random point inside the full disc (fadeRadius)
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float r = fadeRadius * Mathf.Sqrt(Random.value); // uniform distribution in area

            // Density factor: 1 inside core, then falloff to 0 at fadeRadius
            float densityFactor = 1f;
            if (r > constantDensityRadius)
            {
                float t = (r - constantDensityRadius) / (fadeRadius - constantDensityRadius);
                t = Mathf.Clamp01(t);
                densityFactor = Mathf.Pow(1f - t, falloffPower);
            }

            // Rejection sampling: keep with probability = densityFactor
            if (Random.value > densityFactor)
                continue;

            // Calculate world position
            float x = transform.position.x + r * Mathf.Cos(angle);
            float z = transform.position.z + r * Mathf.Sin(angle);
            Vector3 spawnPos = new Vector3(x, transform.position.y, z);
            Vector3 surfaceNormal = Vector3.up;
            bool groundHit = false;

            if (alignToGround)
            {
                RaycastHit hit;
                Vector3 rayOrigin = new Vector3(x, transform.position.y + groundRaycastDistance, z);
                if (Physics.Raycast(rayOrigin, Vector3.down, out hit, groundRaycastDistance * 2f, groundMask))
                {
                    spawnPos.y = hit.point.y + groundOffset;
                    surfaceNormal = hit.normal;
                    groundHit = true;
                }
                else
                {
                    spawnPos.y = transform.position.y + groundOffset;
                }
            }

            Quaternion rotation = GetFlowerRotation(surfaceNormal, groundHit);
            GameObject flower = Instantiate(flowerPrefab, spawnPos, rotation);

            if (randomScale)
            {
                float scaleVal = Random.Range(minScale, maxScale);
                flower.transform.localScale = Vector3.one * scaleVal;
            }

            if (parentTransform != null)
                flower.transform.SetParent(parentTransform);
            else
                flower.transform.SetParent(transform);

            spawnedFlowers.Add(flower);
            spawned++;
        }

        if (attempts >= maxAttempts)
            Debug.LogWarning($"FlowerFieldSpawner: Reached max attempts ({maxAttempts}) and spawned only {spawned} / {totalToSpawn} flowers. Try increasing fadeRadius or adjusting falloff.");
    }

    /// <summary>
    /// Computes total flower count from the chosen density (flowers/m² inside the core).
    /// Integrates density factor over the whole area.
    /// </summary>
    public int CalculateFlowerCountFromDensity()  // <-- changed from private to public
    {
        if (fadeRadius <= constantDensityRadius)
            return Mathf.RoundToInt(density * Mathf.PI * fadeRadius * fadeRadius);

        // Area of constant density core
        float coreArea = Mathf.PI * constantDensityRadius * constantDensityRadius;
        float totalIntegratedDensity = density * coreArea;

        // Integrate density over the ring from constantDensityRadius to fadeRadius
        int steps = 100;
        float dr = (fadeRadius - constantDensityRadius) / steps;
        for (int i = 0; i < steps; i++)
        {
            float r = constantDensityRadius + i * dr;
            float t = (r - constantDensityRadius) / (fadeRadius - constantDensityRadius);
            float densityFactor = Mathf.Pow(1f - t, falloffPower);
            float ringArea = 2f * Mathf.PI * r * dr;
            totalIntegratedDensity += density * densityFactor * ringArea;
        }

        return Mathf.RoundToInt(totalIntegratedDensity);
    }

    public void ClearFlowers()
    {
        foreach (GameObject flower in spawnedFlowers)
            if (flower != null) DestroyImmediate(flower);
        spawnedFlowers.Clear();
    }

    private Quaternion GetFlowerRotation(Vector3 surfaceNormal, bool groundHit)
    {
        Vector3 targetUp = (alignToGround && groundHit) ? surfaceNormal : Vector3.up;
        Quaternion baseRot = Quaternion.FromToRotation(flowerUpAxis, targetUp);

        if (randomRotation)
        {
            float yaw = Random.Range(0f, 360f);
            Quaternion yawRot = Quaternion.AngleAxis(yaw, targetUp);
            return yawRot * baseRot;
        }
        return baseRot;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(FlowerFieldSpawner))]
public class FlowerFieldSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        FlowerFieldSpawner spawner = (FlowerFieldSpawner)target;
        GUILayout.Space(10);
        if (GUILayout.Button("Spawn Flowers Now")) spawner.SpawnFlowers();
        if (GUILayout.Button("Clear All Flowers")) spawner.ClearFlowers();

        if (spawner.useDensityMode)
        {
            GUILayout.Space(5);
            EditorGUILayout.HelpBox(
                $"Density mode active.\n" +
                $"Current target flowers ≈ {spawner.CalculateFlowerCountFromDensity()}\n" +
                $"based on density = {spawner.density} flowers/m² inside the core.\n" +
                $"Adjust density or radii to change the number.",
                MessageType.Info);
        }
    }
}
#endif