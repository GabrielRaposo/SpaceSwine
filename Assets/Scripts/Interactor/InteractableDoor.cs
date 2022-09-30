using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class InteractableDoor : Interactable
{
    //[SerializeField] InputAction testInput;

    [Header("Door References")]
    [SerializeField] SpriteRenderer frontVisuals;
    [SerializeField] ClosedSpaceInner backComponent;
    [SerializeField] GameObject inputHelper;

    bool closedSpaceIsActive; 

    private void Start() 
    {
        if (inputHelper)
            inputHelper.SetActive(false);

        //SetupColliderPosition();
    }

    public override void SetInteraction(bool value) 
    {
        base.SetInteraction(value);

        //Debug.Log("elevator interaction");
        //SetActivation(value);
    }

    public override void Interaction (PlayerInteractor interactor) 
    {
        base.Interaction(interactor);

        SetState (interactor, !closedSpaceIsActive);
    }

    private void SetState (PlayerInteractor interactor, bool value)
    {
        if (!interactor || !backComponent)
            return;

        if (value == closedSpaceIsActive)
            return;

        //if (frontVisuals)
        //    frontVisuals.enabled = !value;

        // -- Desativação do backComponent DEVE ocorrer antes do "SetOverrideGravitationalBody"
        // -- Se precisar botar pra depois, então dividir a função em cases de "true" e casos de "false" ocorrendo em tempos diferentes
        backComponent.SetState(value);

        GravityInteraction gravityInteraction = interactor.GetComponent<GravityInteraction>();
        if (gravityInteraction)
        {
            gravityInteraction.SetOverrideGravitationalBody = 
                value ? 
                    backComponent.GetGravitationalBody : 
                    null;
        }

        closedSpaceIsActive = value;
    }

    protected override void HighlightState (bool value) 
    {
        if (inputHelper) 
            inputHelper.SetActive(value && interactable);
    }

    public override void IconState (bool value) 
    {
        //if (ballonSpriteSwapper)
        //    ballonSpriteSwapper.SetVisibility(value);
    }
}
