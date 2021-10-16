using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotation : MonoBehaviour
{
    float startingRotation;    

    void Start()
    {
        startingRotation = transform.eulerAngles.z;
    }

    void Update()
    {
        //transform.eulerAngles = Vector3.forward * startingRotation;
        transform.eulerAngles = Vector3.forward * 0;
    }
}
