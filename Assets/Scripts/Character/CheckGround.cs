using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGround : MonoBehaviour
{
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Vector2 offset; 
    [SerializeField] float radius; 
    
    bool onGround;

    Color debugCollisionColor = Color.blue;

    public bool OnGround
    {
        get { return onGround; }
    }

    void Update()
    {  
        onGround = Physics2D.OverlapCircle (transform.position + (transform.up * offset.y), radius, groundLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = debugCollisionColor;
        Gizmos.DrawWireSphere(transform.position + (transform.up * offset.y), radius);
    }
}
