using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAnchor : MonoBehaviour
{
    GravityInteraction gravityInteraction;
    Transform gravityCenter;

    private void Awake() 
    {
        gravityInteraction = GetComponentInParent<GravityInteraction>();

        if (!gravityInteraction)
            return;

        gravityInteraction.OnChangeGravityAnchor += (t) =>
        { 
            gravityCenter = t;
        };
    }

    private void FixedUpdate() 
    {
        
    }
}
