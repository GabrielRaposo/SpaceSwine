using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckWall : MonoBehaviour
{
    [SerializeField] LayerMask wallLayer;
    [Space]
    [SerializeField] Vector2 leftOffset; 
    [SerializeField] Vector2 rightOffset; 
    [Space]
    [SerializeField] float radius;
    
    bool onLeftWall;
    bool onRightWall;

    Color debugCollisionColor = Color.red;

    //public (bool isValid, bool rightSide) OnWall
    //{
    //    get { return (onLeftWall || onRightWall, onRightWall); }
    //}

    void Update()
    {  
        onLeftWall  = Physics2D.OverlapCircle (transform.position + (Vector3) RaposUtil.AlignWithTransform(transform, leftOffset),  radius, wallLayer);
        onRightWall = Physics2D.OverlapCircle (transform.position + (Vector3) RaposUtil.AlignWithTransform(transform, rightOffset), radius, wallLayer);
    }

    public bool MovingTowardsWall (float direction)
    {
        if (direction > 0 && onRightWall)
            return true;

        if (direction < 0 && onLeftWall)
            return true;

        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = debugCollisionColor;
        Gizmos.DrawWireSphere(transform.position + (Vector3) RaposUtil.AlignWithTransform(transform, leftOffset) , radius);
        Gizmos.DrawWireSphere(transform.position + (Vector3) RaposUtil.AlignWithTransform(transform, rightOffset), radius);
    }
}

