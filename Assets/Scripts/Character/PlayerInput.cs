using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LocalGameplayState))]
[RequireComponent(typeof(PlatformerCharacter))]
[RequireComponent(typeof(GravityInteraction))]
[RequireComponent(typeof(SpaceJumper))]
[RequireComponent(typeof(CollectableInteraction))]
public class PlayerInput : MonoBehaviour
{
    //[SerializeField] GameplayState gameplayState;

    LocalGameplayState gameplayState;
    PlatformerCharacter platformerCharacter;
    GravityInteraction gravityInteraction;
    CollectableInteraction collectableInteraction;
    SpaceJumper spaceJumper;

    void Start()
    {
        gameplayState = GetComponent<LocalGameplayState>();
        platformerCharacter = GetComponent<PlatformerCharacter>();
        gravityInteraction = GetComponent<GravityInteraction>();
        collectableInteraction = GetComponent<CollectableInteraction>();
        spaceJumper = GetComponent<SpaceJumper>();
    }

    void Update()
    {
        platformerCharacter.HorizontalInput(Input.GetAxis("Horizontal"));
        spaceJumper.AxisInput(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));

        //if (Input.GetButtonDown("Jump"))
        //{
        //    if (gameplayState.state == GameplayState.Exploration)
        //        platformerCharacter.JumpInput();
        //    else
        //        spaceJumper.JumpInput();

        //}

        if (Input.GetButtonDown("Jump"))
        {
            platformerCharacter.JumpInput();
        } else
        if (Input.GetButtonDown("Launch"))
        {
            spaceJumper.JumpInput();
        } else 
        if (Input.GetButtonDown("Jump"))
        {
            collectableInteraction.InteractInput();
        }

        gravityInteraction.SetJumpHeld(Input.GetButton("Jump"));           
    }
}
