using UnityEngine;

public class BendWhenNear : MonoBehaviour
{
    [Header("Bend Settings")]
    public Transform playerTransform;   // drag player here
    public float bendDistance = 2f;
    public float maxBend = 15f;
    public float returnSpeed = 3f;

    private Quaternion startRotation;
    private float currentBend = 0f;

    void Start()
    {
        startRotation = transform.localRotation;

        if (playerTransform == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playerTransform = p.transform;
            else Debug.LogError($"{name}: No player found! Assign in Inspector or tag player as 'Player'.");
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance < bendDistance)
        {
            float targetBend = (1f - distance / bendDistance) * maxBend;
            currentBend = Mathf.Lerp(currentBend, targetBend, Time.deltaTime * 8f);
        }
        else
        {
            currentBend = Mathf.Lerp(currentBend, 0f, Time.deltaTime * returnSpeed);
        }

        if (currentBend > 0.01f)
        {
            Vector3 dir = transform.position - playerTransform.position;
            dir.y = 0;
            if (dir != Vector3.zero)
            {
                Vector3 axis = Vector3.Cross(dir.normalized, Vector3.up);
                if (axis == Vector3.zero) axis = Vector3.right;
                Quaternion bendRot = Quaternion.AngleAxis(currentBend, axis);
                transform.localRotation = Quaternion.Lerp(transform.localRotation,
                    startRotation * bendRot, Time.deltaTime * 10f);
            }
        }
        else
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation,
                startRotation, Time.deltaTime * returnSpeed);
        }
    }
}