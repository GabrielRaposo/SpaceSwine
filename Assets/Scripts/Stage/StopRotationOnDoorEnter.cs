using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopRotationOnDoorEnter : MonoBehaviour
{
    [SerializeField] DoorAnimation doorAnimation;
    [SerializeField] CustomRotate customRotate;

    void Start()
    {
        if (!doorAnimation || !customRotate)
            return;

        doorAnimation.OnAnimationStartAction += () => 
        {
            customRotate.PauseRotation();
        };
    }
}
