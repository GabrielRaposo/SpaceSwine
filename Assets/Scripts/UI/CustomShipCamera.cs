using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CustomShipCamera : MonoBehaviour
{
    float startingSize;
    
    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();

        startingSize = cam.orthographicSize;
    }

    public void SetCustomZoom (float size)
    {
        cam.orthographicSize = size;
    }

    public void ResetZoom()
    {
        cam.orthographicSize = startingSize;
    }

    public void SetCustomRotation (float angle)
    {
        transform.eulerAngles = Vector3.forward * angle;
    }
}
