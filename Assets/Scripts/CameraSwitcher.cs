using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera mainCamera;
    public Camera birdEyeCamera;

    [Header("Bird Eye Zoom")]
    public float birdEyeZoomSpeed = 20f;
    public float birdEyeMinHeight = 20f;
    public float birdEyeMaxHeight = 200f;

    private bool isBirdEye = false;
    private float targetHeight;

    void Start()
    {
        mainCamera.gameObject.SetActive(true);
        birdEyeCamera.gameObject.SetActive(false);
        targetHeight = birdEyeCamera.transform.position.y;
    }

    void Update()
    {
        // Switch cameras on right click
        if (Input.GetMouseButtonDown(1))
        {
            isBirdEye = !isBirdEye;
            mainCamera.gameObject.SetActive(!isBirdEye);
            birdEyeCamera.gameObject.SetActive(isBirdEye);
        }

        // Scroll to zoom bird eye camera
        if (isBirdEye)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                targetHeight -= scroll * birdEyeZoomSpeed;
                targetHeight = Mathf.Clamp(targetHeight, birdEyeMinHeight, birdEyeMaxHeight);
            }

            // Smoothly move camera to target height
            Vector3 pos = birdEyeCamera.transform.position;
            pos.y = Mathf.Lerp(pos.y, targetHeight, Time.deltaTime * 5f);
            birdEyeCamera.transform.position = pos;
        }
    }
}