using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCameraSize : MonoBehaviour
{
    const float CAMERA_SIZE = 4.0f;
    
    [Header("Valor de Base: 4.0")]
    public float customSize = CAMERA_SIZE;

    bool onCustomCamera;

    private void OnEnable() 
    {
        if ((customSize == CAMERA_SIZE && CameraSizeController.Size == CAMERA_SIZE) || onCustomCamera)
            return;

        //cam.transform.position = new Vector3(0, 0, cam.transform.position.z);
        CameraSizeController.Size = customSize;
        onCustomCamera = true;

        //Debug.Log("Set custom size: " + customSize);
    }

    private void OnDisable() 
    {
        if (!onCustomCamera)
            return;

        CameraSizeController.Size = CAMERA_SIZE;
        onCustomCamera = false;
    }

    public void ForceCustomCamera()
    {
        CameraSizeController.Size = customSize;
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.blue;
        Vector3 v = new Vector3(customSize * 16f / 9f, customSize);
        Gizmos.DrawWireCube( Vector3.zero, v * 2f );
    }
}
