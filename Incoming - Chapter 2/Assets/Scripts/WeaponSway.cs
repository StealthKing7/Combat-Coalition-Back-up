using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class WeaponSway : MonoBehaviour
{
    [SerializeField]
    public float Smothness, SwayMultiplier;
    private Transform ActiveChild;
    private List<Transform> Childs = new List<Transform>();
    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Childs.Add(transform.GetChild(i));
        }
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

        mouseX *= SwayMultiplier;
        mouseY *= SwayMultiplier;
        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion TargetRot = rotationX * rotationY;
        foreach (Transform child in Childs)
        {
            if (child.gameObject.activeSelf)
            {
                ActiveChild = child;
            }
        }
        ActiveChild.localRotation = Quaternion.Slerp(transform.localRotation, TargetRot, Smothness * Time.deltaTime);
    }
}
