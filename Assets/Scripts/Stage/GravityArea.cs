using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityArea : MonoBehaviour
{
    public float intensity;
    public bool linear;
    public Vector2 direction;

    public Vector3 Center 
    {
        get { return transform.position; }
    }
}
