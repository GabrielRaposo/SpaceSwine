using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class InteractableDoor : Interactable
{
    //[SerializeField] InputAction testInput;

    [Header("Value")]
    [SerializeField] float doorAnimationDuration;
    [SerializeField] float fadeAnimationDuration;

    [Header("Door")]
    [SerializeField] Animator doorAnimator;
    [SerializeField] SpriteRenderer frontVisuals;

    [Header("Closed Space")]
    [SerializeField] ClosedSpaceInner backComponent;
    [SerializeField] GameObject inputHelper;

    [Header("Camera")]
    [SerializeField] float zoomInSize;
    [SerializeField] float zoomOutSize;

    [Header("Audio")]
    [SerializeField] AK.Wwise.Event openAKEvent;
    [SerializeField] AK.Wwise.Event closeAKEvent;

    bool closedSpaceIsActive;
    Sequence zoomSequence;

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

        StartCoroutine( SetState (interactor, !closedSpaceIsActive) );
        SetCameraZoom( !closedSpaceIsActive );
    }

    private IEnumerator SetState (PlayerInteractor interactor, bool value)
    {
        if (!interactor || !backComponent)
            yield break;

        if (value == closedSpaceIsActive)
            yield break;

        BeforeSequence (interactor);

        // -- Desativação do backComponent DEVE ocorrer antes do "BlinkCollider"
        // -- Se precisar botar pra depois, então dividir a função em cases de "true" e casos de "false" ocorrendo em tempos diferentes
        if (value)
        {
            doorAnimator.SetBool("Open", value);
            yield return new WaitForSeconds (doorAnimationDuration);

            backComponent.SetState(value);
            yield return new WaitForSeconds (fadeAnimationDuration);
        }
        else
        {
            backComponent.SetState(value);
            yield return new WaitForSeconds (fadeAnimationDuration);
            
            doorAnimator.SetBool("Open", value);
            yield return new WaitForSeconds (doorAnimationDuration);
        }

        GravityInteraction gravityInteraction = interactor.GetComponent<GravityInteraction>();
        if (gravityInteraction)
        {
            if (value)
                gravityInteraction.HardSetGravityArea(backComponent.GetGravitationalBody.GetComponentInChildren<GravityArea>());
            else
                gravityInteraction.BlinkCollider();
        }

        closedSpaceIsActive = value;

        AfterSequence (interactor);
    }

    private void SetCameraZoom (bool value)
    {
        if (zoomSequence != null)
            zoomSequence.Kill();

        zoomSequence = DOTween.Sequence();
        zoomSequence.Append
        ( 
            DOVirtual.Float
            (
                from: value ? zoomOutSize : zoomInSize,
                to:   value ? zoomInSize : zoomOutSize,
                duration: fadeAnimationDuration,
                (f) => CameraSizeController.Size = f 
            )
        );
    }

    private void BeforeSequence (PlayerInteractor player)
    {
        //if (interactableChildren != null && interactableChildren.Count > 0)
        //{
        //    foreach (Interactable i in interactableChildren)
        //        i.SetInteraction(false);
        //}

        if (openAKEvent != null)
            openAKEvent.Post(gameObject);

        GameManager.BlockCharacterInput = true;

        if (player)
        {
            //PlayerInput playerInput = player.GetComponent<PlayerInput>();
            //if (playerInput) playerInput.enabled = false;

            PlatformerCharacter platformer = player.GetComponent<PlatformerCharacter>();
            if (platformer) platformer.KillInputs();
        }

    }

    private void AfterSequence (PlayerInteractor player)
    {
        //if (interactableChildren != null && interactableChildren.Count > 0)
        //{
        //    foreach (Interactable i in interactableChildren)
        //        i.SetInteraction(true);
        //}

        if (closeAKEvent != null)
            closeAKEvent.Post(gameObject);

        if (player)
        {
            PlayerInput playerInput = player.GetComponent<PlayerInput>();
            if (playerInput) playerInput.enabled = true;
        }

        GameManager.BlockCharacterInput = false;
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
