using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableShowDialogueAndCallAction : Interactable
{
    [SerializeField] List<string> dialogueIDs;
    [SerializeField] DialogueBoxStyle dialogueBoxStyle;
    [SerializeField] AK.Wwise.Event chatSoundAKEvent;

    [Space(10)]
    [SerializeField] UnityEvent onInteractionEvent;

    [Space(10)]
    [SerializeField] GameObject inputIcon;

    UnityAction OnDialogueEnd;
    UnityAction OnDialogueCancel;
    DialogueSystem dialogSystem;


    private void Awake()
    {
        if (onInteractionEvent == null)
            onInteractionEvent = new UnityEvent();
    }

    private void Start() 
    {
        dialogSystem = DialogueSystem.Instance;

        Round round = GetComponentInParent<Round>();
        if (round != null)
        {
            round.OnReset += () => 
            {
                dialogSystem.CancelDialogue();
                gameObject.SetActive(true);
            };
        }

        HighlightState(false);
    }

    public override void Interaction (PlayerInteractor interactor) 
    {
        base.Interaction(interactor);

        if (interactor)
        {
            PlayerInput playerInput = interactor.GetComponent<PlayerInput>();
            if (playerInput) playerInput.enabled = false;

            PlatformerCharacter platformer = interactor.GetComponent<PlatformerCharacter>();
            if (platformer) platformer.KillInputs();
        }
                
        gameObject.SetActive(false);
        
        if (dialogueIDs == null)
        {
            CallEvent (interactor);
            return;
        }

        // -- Chama caixa de diálogo do item sendo pego
        OnDialogueEnd    = () => CallEvent(interactor);
        OnDialogueCancel = () => RestorePlayerInteractions(interactor);
       
        CallDialogueText (interactor);
    }

    private void CallDialogueText(PlayerInteractor interactor)
    {            
        dialogSystem?.SetDialogue(this, speakerName: string.Empty, dialogueIDs, OnDialogueEnd, dialogueBoxStyle, chatSoundAKEvent, OnDialogueCancel);

        if (interactor)
        {
            PlatformerCharacter platformerCharacter = interactor.GetComponent<PlatformerCharacter>();
            platformerCharacter?.KillInputs();
            platformerCharacter?.LookAtTarget(transform);
        }
    }

    public void CallEvent (PlayerInteractor interactor)
    {
        onInteractionEvent.Invoke();
        RestorePlayerInteractions(interactor);
    }

    private void RestorePlayerInteractions(PlayerInteractor interactor)
    {
        if (interactor)
        {
            PlayerInput playerInput = interactor.GetComponent<PlayerInput>();
            if (playerInput) playerInput.enabled = true;
        }
    }

    protected override void HighlightState (bool value) 
    {
        if (inputIcon)
            inputIcon.SetActive(value);
    }
}
