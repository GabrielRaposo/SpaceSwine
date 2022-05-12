using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckPlayerOnRadius : MonoBehaviour
{
    [SerializeField] float radius;
    [SerializeField] float baseOffset;

    [Header("References")]
    [SerializeField] Transform visualComponent;

    ContactFilter2D contactFilter2D = new ContactFilter2D();

    public UnityAction <bool> PlayerOnRadiusEvent;

    void Start()
    {
        //contactFilter2D.SetLayerMask(LayerMask.NameToLayer("Default"));

        if (!visualComponent)
            enabled = false;
    }

    void Update()
    {
        Transform rightTarget = RaycastForTarget (direction:  1);
        Transform leftTarget  = RaycastForTarget (direction: -1);

        if (rightTarget)
            PlayerOnRadiusEvent?.Invoke(true);
        if (leftTarget)
            PlayerOnRadiusEvent?.Invoke(false);

        if (rightTarget == leftTarget)
            return;

        float yValue = 0;
        if (leftTarget)
            yValue = 180;
            
        visualComponent.localEulerAngles = new Vector3 ( visualComponent.localEulerAngles.x, yValue, visualComponent.localEulerAngles.z );
    }

    private Transform RaycastForTarget (int direction)
    {
        Transform target = null;

        List<Collider2D> results = new List<Collider2D>();
        if ( Physics2D.OverlapCircle (
                transform.position + (transform.right.normalized * (radius + baseOffset) * direction), 
                radius, 
                contactFilter2D, 
                results
            ) > 0)
        {
            foreach (Collider2D collider2D in results)
            {
                PlayerInput playerInput = collider2D.GetComponent<PlayerInput>();
                if (!playerInput)
                    continue;

                target = playerInput.transform;
                break;
            }
        }

        return target;
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + (transform.right.normalized * (radius + baseOffset) * 1), radius);
        Gizmos.DrawWireSphere(transform.position + (transform.right.normalized * (radius + baseOffset) * -1), radius);
    }
}
