using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    float xRotation = 0f;
    float yRotation = 0f;
    bool isMouseLocked=true;
    private void Update()
    {
        if(!InventorySystem.instance.isOpen &&!CraftingSystem.instance.isOpen)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            yRotation += mouseX;

            transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
            if (Input.GetKeyDown(KeyCode.L))
            {
                isMouseLocked = !isMouseLocked;
                LockedMouse(isMouseLocked);
            }
        }
       
    }
    private void LockedMouse(bool isLocked) 
    { 
        if(isLocked)
        {
            Cursor.lockState= CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = (CursorLockMode.None);
        }    
    }

}
