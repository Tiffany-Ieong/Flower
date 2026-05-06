using System.Collections.Generic;
using UnityEngine;

public class ReplayGhost : MonoBehaviour
{
    private ReplayData data;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private Quaternion rotationOffset;
    private float startTime;
    private float duration;

    private List<Vector3> transformedPositions;
    private List<Quaternion> transformedRotations;

    private bool isPlaying = false;

    [Header("Ghost Visuals")]
    public float fadeOutDuration = 1f;
    private Material ghostMaterial;
    private Color originalColor;

    void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            ghostMaterial = rend.material;
            originalColor = ghostMaterial.color;
        }
    }

    public void Initialize(ReplayData replayData, Vector3 spawnPos, Quaternion lookAtPlayerRotation)
    {
        data = replayData;
        spawnPosition = spawnPos;
        spawnRotation = lookAtPlayerRotation;

        Quaternion originalForwardRot = Quaternion.LookRotation(data.startForward);
        rotationOffset = spawnRotation * Quaternion.Inverse(originalForwardRot);

        int frameCount = data.frames.Count;
        transformedPositions = new List<Vector3>(frameCount);
        transformedRotations = new List<Quaternion>(frameCount);

        Vector3 basePos = data.frames[0].position;
        for (int i = 0; i < frameCount; i++)
        {
            Vector3 originalPos = data.frames[i].position;
            Vector3 relativePos = originalPos - basePos;
            transformedPositions.Add(spawnPosition + rotationOffset * relativePos);

            Quaternion originalRot = Quaternion.Euler(data.frames[i].rotationEuler);
            transformedRotations.Add(rotationOffset * originalRot);
        }

        duration = (frameCount - 1) / data.frameRate;
        startTime = Time.time;
        isPlaying = true;

        transform.position = transformedPositions[0];
        transform.rotation = transformedRotations[0];
    }

    void Update()
    {
        if (!isPlaying) return;

        float elapsed = Time.time - startTime;
        if (elapsed >= duration)
        {
            FinishPlayback();
            return;
        }

        float t = elapsed * data.frameRate;
        int frameA = Mathf.FloorToInt(t);
        int frameB = frameA + 1;
        float frac = t - frameA;

        if (frameB >= transformedPositions.Count)
        {
            transform.position = transformedPositions[transformedPositions.Count - 1];
            transform.rotation = transformedRotations[transformedRotations.Count - 1];
        }
        else
        {
            transform.position = Vector3.Lerp(transformedPositions[frameA], transformedPositions[frameB], frac);
            transform.rotation = Quaternion.Slerp(transformedRotations[frameA], transformedRotations[frameB], frac);
        }
    }

    void FinishPlayback()
    {
        isPlaying = false;
        StartCoroutine(FadeOutAndDestroy());
    }

    private System.Collections.IEnumerator FadeOutAndDestroy()
    {
        if (ghostMaterial != null)
        {
            float elapsed = 0f;
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = 1f - (elapsed / fadeOutDuration);
                Color c = originalColor;
                c.a = alpha;
                ghostMaterial.color = c;
                yield return null;
            }
        }
        Destroy(gameObject);
    }
}