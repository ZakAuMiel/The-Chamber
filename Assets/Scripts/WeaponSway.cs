using System;
using UnityEngine;


public class WeaponSway : MonoBehaviour
{
   [Header("Sway Settings")]
    [SerializeField] private float smooth;
    [SerializeField] private float multiplier;

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * multiplier ;
        float mouseY = Input.GetAxis("Mouse Y") * multiplier;

       //calculate target rotation
        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        //rotate 
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * smooth);
    }
}
