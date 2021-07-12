using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerGameplayState))]
[RequireComponent(typeof(PlatformerCharacter))]
[RequireComponent(typeof(GravityInteraction))]
[RequireComponent(typeof(SpaceJumper))]
public class PlayerInput : MonoBehaviour
{
    //[SerializeField] GameplayState gameplayState;

    PlayerGameplayState gameplayState;
    PlatformerCharacter platformerCharacter;
    GravityInteraction gravityInteraction;
    SpaceJumper spaceJumper;

    void Start()
    {
        gameplayState = GetComponent<PlayerGameplayState>();
        platformerCharacter = GetComponent<PlatformerCharacter>();
        gravityInteraction = GetComponent<GravityInteraction>();
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
        }

        gravityInteraction.SetJumpHeld(Input.GetButton("Jump"));           
    }
}
