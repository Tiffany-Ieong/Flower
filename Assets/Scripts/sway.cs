using UnityEngine;

public class sway : MonoBehaviour
{
    public float swayAmount = 6f;
    public float swaySpeed = 3f;

    private Quaternion startRotation;
    private float randomOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startRotation = transform.rotation;
        randomOffset = Random.Range(0f, 100f);
    }

    // Update is called once per frame
    void Update()
    {
        float angle = Mathf.Sin(Time.time * swaySpeed + randomOffset) * swayAmount;
        transform.rotation = startRotation * Quaternion.Euler(angle, 0, 0);
    }
}
