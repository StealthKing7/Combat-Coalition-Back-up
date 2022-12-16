using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static scr_Models;

public class scr_CharacterController : MonoBehaviour
{
    DefaultInputs DefaultInput;
    public Vector2 Input_Movement;
    public Vector2 Input_View;
    Vector3 NewCameraRotation;
    [Header("References")]
    [SerializeField] Transform CameraHolder;

    [Header("Player Settings")]
    [SerializeField] PlayerSettingModel PlayerSettings;
    [SerializeField] float ViewClampYmin = -70;
    [SerializeField] float ViewClampYmax = 80;
    private void Awake()
    {
        DefaultInput = new DefaultInputs();

        DefaultInput.Character.Movement.performed += e => Input_Movement = e.ReadValue<Vector2>();
        DefaultInput.Character.View.performed += e => Input_View = e.ReadValue<Vector2>();
        DefaultInput.Enable();

        NewCameraRotation = CameraHolder.localRotation.eulerAngles;
    }
    private void Update()
    {
        CalculateMovement();
        CalculateView();
    }
    void CalculateView()
    {
        NewCameraRotation.x += PlayerSettings.ViewYSencitivity * Input_View.y * Time.deltaTime;
        NewCameraRotation.x = Mathf.Clamp(NewCameraRotation.x, ViewClampYmin, ViewClampYmax);
        CameraHolder.localRotation = Quaternion.Euler(NewCameraRotation);
    }
    void CalculateMovement()
    {

    }
}
