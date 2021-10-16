using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class InteractableElevator : Interactable
{
    private void Start() 
    {
        Setup();
    }

    #if UNITY_EDITOR
    private void Update() 
    {
        if (Application.isPlaying)
            return;

        Setup();
    }
    #endif

    private void Setup()
    {
        PlanetBlock planetBlock = GetComponent<PlanetBlock>();
        if (!planetBlock)
            return;

        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (!boxCollider)
            return;

        boxCollider.offset = Vector2.up * (planetBlock.GetSize() - .35f);
    }

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

    private void OnDrawGizmos() 
    {
            
    }
}
