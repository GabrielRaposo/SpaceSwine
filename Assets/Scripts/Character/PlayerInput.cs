using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlatformerCharacter))]
[RequireComponent(typeof(GravityInteraction))]
[RequireComponent(typeof(SpaceJumper))]
public class PlayerInput : MonoBehaviour
{
    [SerializeField] GameplayState gameplayState;

    PlatformerCharacter platformerCharacter;
    GravityInteraction gravityInteraction;
    SpaceJumper spaceJumper;

    void Start()
    {
        platformerCharacter = GetComponent<PlatformerCharacter>();
        gravityInteraction = GetComponent<GravityInteraction>();
        spaceJumper = GetComponent<SpaceJumper>();
    }

    void Update()
    {
        platformerCharacter.HorizontalInput(Input.GetAxis("Horizontal"));

        if (Input.GetButtonDown("Jump"))
        {
            if (gameplayState == GameplayState.Exploration)
                platformerCharacter.JumpInput();
            else
                spaceJumper.JumpInput();

        }

        gravityInteraction.SetJumpHeld(Input.GetButton("Jump"));           
    }
}
