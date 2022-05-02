#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

using UnityEngine;

public class MouseLook : MonoBehaviour
{
    const float k_MouseSensitivityMultiplier = 0.01f;
    private float mouseSensitivity = 100f;
    private Transform playerBody;
    private Transform CameraHolder;
    float xRotation = 0f;
    public void GetValues(float sencitivity,Transform player,Transform holder)
    {
        mouseSensitivity = sencitivity;
        playerBody = player;
        CameraHolder = holder;
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {

        float mouseX = 0, mouseY = 0;

        if (Mouse.current != null)
        {
            var delta = Mouse.current.delta.ReadValue() / 15.0f;
            mouseX += delta.x;
            mouseY += delta.y;
        }
        mouseX *= mouseSensitivity * k_MouseSensitivityMultiplier;
        mouseY *= mouseSensitivity * k_MouseSensitivityMultiplier;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        CameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up * mouseX);
    }
}