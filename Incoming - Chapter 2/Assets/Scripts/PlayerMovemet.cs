using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovemet : MonoBehaviour
{

    private PlayerInput playerInput;
    private CharacterController characterController;
    public float Speed;
    public float MouseSensitivity;
    private MouseLook mouseLook;
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();  
        playerInput = new PlayerInput();
        playerInput.Player.Enable();
        mouseLook=GetComponentInChildren<MouseLook>();
        mouseLook.GetValues(MouseSensitivity, this.transform);
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

    }


    void Update()
    {
       
    }

    void FixedUpdate()
    {
        float x, z;
        var delta = playerInput.Player.Movement.ReadValue<Vector2>();
        x = delta.x;
        z = delta.y;
        Vector3 move = transform.right * x + transform.forward * z;
        characterController.Move(move * Speed * Time.deltaTime);

    }
}
