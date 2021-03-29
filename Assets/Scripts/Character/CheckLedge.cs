using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckLedge : MonoBehaviour
{
    [SerializeField] LayerMask groundLayer;
    [Space]
    [SerializeField] Vector2 leftOffset; 
    [SerializeField] Vector2 rightOffset; 
    [Space]
    [SerializeField] float radius;
    
    bool leftLedge;
    bool rightLedge;
    //PlanetPlatform planetPlatform;

    Color debugCollisionColor = Color.green;

    public (bool isValid, bool leftLedge, bool rightLedge) OnLedge
    {
        get { return (leftLedge || rightLedge, leftLedge, rightLedge); }
    }

    void Update()
    {  
        leftLedge  = Physics2D.OverlapCircle (transform.position + (Vector3) RaposUtil.AlignWithTransform(transform, leftOffset), radius, groundLayer);
        rightLedge = Physics2D.OverlapCircle (transform.position + (Vector3) RaposUtil.AlignWithTransform(transform, rightOffset), radius, groundLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = debugCollisionColor;
        Gizmos.DrawWireSphere(transform.position + (Vector3) RaposUtil.AlignWithTransform(transform, leftOffset), radius);
        Gizmos.DrawWireSphere(transform.position + (Vector3) RaposUtil.AlignWithTransform(transform, rightOffset), radius);
    }
}

