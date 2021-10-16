using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableElevator : Interactable
{
    public override void Interaction (PlayerInteractor interactor) 
    {
        base.Interaction(interactor);

        if (interactor)
        {
            SpaceJumper spaceJumper = interactor.GetComponent<SpaceJumper>();
            if (!spaceJumper)
                return;

            spaceJumper.LaunchIntoDirection(transform.up);

            //PlatformerCharacter platformerCharacter = interactor.GetComponent<PlatformerCharacter>();
            //platformerCharacter?.KillInputs();
            //platformerCharacter?.LookAtTarget(transform);
        }        
    }

    protected override void HighlightState (bool value) 
    {
    }

    public override void IconState (bool value) 
    {
        //if (ballonSpriteSwapper)
        //    ballonSpriteSwapper.SetVisibility(value);
    }
}
