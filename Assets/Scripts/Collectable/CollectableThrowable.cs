using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableThrowable : Collectable
{
    public override void Interact (CollectableInteraction interactor) 
    {
        interactor.LaunchInput();
    }
}
