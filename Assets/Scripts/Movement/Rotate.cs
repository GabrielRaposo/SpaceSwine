using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] float angularSpeed;
    
    private void FixedUpdate() 
    {
        transform.Rotate (Vector3.forward * angularSpeed * Time.fixedDeltaTime);    
    }
}
