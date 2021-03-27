using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlatformerCharacter))]
[RequireComponent(typeof(GravityInteraction))]
public class PlayerInput : MonoBehaviour
{
    PlatformerCharacter platformerCharacter;
    GravityInteraction gravityInteraction;

    void Start()
    {
        platformerCharacter = GetComponent<PlatformerCharacter>();
        gravityInteraction = GetComponent<GravityInteraction>();
    }

    void Update()
    {
        platformerCharacter.HorizontalInput(Input.GetAxis("Horizontal"));

        if (Input.GetButtonDown("Jump"))
            platformerCharacter.JumpInput();

        gravityInteraction.SetJumpHeld(Input.GetButton("Jump"));
    }
}
