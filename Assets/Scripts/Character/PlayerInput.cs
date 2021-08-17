using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LocalGameplayState))]
[RequireComponent(typeof(PlatformerCharacter))]
[RequireComponent(typeof(GravityInteraction))]
[RequireComponent(typeof(SpaceJumper))]
[RequireComponent(typeof(CollectableInteraction))]
[RequireComponent(typeof(PlayerInteractor))]
public class PlayerInput : MonoBehaviour
{
    //[SerializeField] GameplayState gameplayState;

    LocalGameplayState gameplayState;
    PlatformerCharacter platformerCharacter;
    GravityInteraction gravityInteraction;
    SpaceJumper spaceJumper;
    CollectableInteraction collectableInteraction;
    PlayerInteractor playerInteractor;

    void Start()
    {
        gameplayState = GetComponent<LocalGameplayState>();
        platformerCharacter = GetComponent<PlatformerCharacter>();
        gravityInteraction = GetComponent<GravityInteraction>();
        spaceJumper = GetComponent<SpaceJumper>();
        collectableInteraction = GetComponent<CollectableInteraction>();
        playerInteractor = GetComponent<PlayerInteractor>();
    }

    void Update()
    {
        if (DialogSystem.OnDialog)
            return;

        platformerCharacter.HorizontalInput(Input.GetAxis("Horizontal"));
        collectableInteraction.AxisInput(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));

        if (Input.GetButtonDown("Jump"))
        {
            platformerCharacter.JumpInput();
        } 
        else if (Input.GetButtonDown("Launch"))
        {
            if (!playerInteractor.Interact())
            {
                spaceJumper.JumpInput();
            }
        } 
        else if (Input.GetButtonDown("Throw"))
        {
            collectableInteraction.InteractInput();
        }

        gravityInteraction.SetJumpHeld(Input.GetButton("Jump"));           
    }
}
