using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using DG.Tweening;
using RedBlueGames.Tools.TextTyper;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class DialogueBox : MonoBehaviour
{
    [SerializeField] float transitionDuration;
    
    [Header("Audio")]
    [SerializeField] AK.Wwise.Event awardSoundAKEvent;

    [Header("References")]
    [SerializeField] TextMeshProUGUI nameDisplay;
    [SerializeField] TextMeshProUGUI textDisplay;
    [SerializeField] TextTyper dialogTyper;
    [SerializeField] Image skipArrow;

    bool showing;
    bool autoSkip;

    int delayFrames;
    int dialogueIndex;

    List<TextBoxTag> textTags;

    CanvasGroup canvasGroup;
    PlaySoundOnType playSoundOnType;

    Interactable interactable;
    DialogueAnimation dialogueAnimation;
    string speakerName;
    List <string> dialogues;
    UnityAction OnDialogueEnd;

    PlayerInputActions playerInputActions;
    Sequence sequence;

    private void OnEnable() 
    {
        playerInputActions = new PlayerInputActions();    

        playerInputActions.Player.Interact.performed += TestForwardInput;
        playerInputActions.Player.Interact.Enable();

        playerInputActions.Player.Jump.performed += TestForwardInput;
        playerInputActions.Player.Jump.Enable();
    }

    private void TestForwardInput(InputAction.CallbackContext ctx)
    {
        if (!DialogueSystem.OnDialogue || Time.timeScale < 1)
            return;

        if (delayFrames > 0)
            return;
            
        ForwardInput();
    }

    private void Start() 
    {
        canvasGroup = GetComponent<CanvasGroup>(); 
        canvasGroup.alpha = 0;

        playSoundOnType = GetComponent<PlaySoundOnType>();
    }

    public void SetDialogueData 
    (
        Interactable interactable, 
        string speakerName, 
        List<string> dialogues, 
        UnityAction OnDialogueEnd,
        DialogueBoxStyle customDialogueStyle,
        AK.Wwise.Event talkSoundAKEvent
    )
    {
        this.interactable = interactable;
        this.speakerName = speakerName;
        this.dialogues = dialogues;
        this.OnDialogueEnd = OnDialogueEnd;

        autoSkip = false;
        if (customDialogueStyle)
            autoSkip = customDialogueStyle.instantText;
        

        DialogueBoxStyleController dialogueBoxStyleController = GetComponent<DialogueBoxStyleController>();
        if (customDialogueStyle != null)
        {
            dialogueBoxStyleController?.SetStyle(customDialogueStyle);
        }
        else
        {
            dialogueBoxStyleController?.SetMainStyle();
        }

        if (playSoundOnType)
            playSoundOnType.SetVoiceEvent(talkSoundAKEvent);

        SetDialogueEvents();

        dialogueIndex = 0;
        ShowText(speakerName, dialogues[dialogueIndex]);
        interactable?.IconState(false);

        DialogueSystem.OnDialogue = true;
        delayFrames = 15;
    }

    private void SetDialogueEvents()
    {
        dialogueAnimation = interactable.GetComponent<DialogueAnimation>();
        if (!dialogueAnimation)
            return;
        
        dialogTyper.CharacterPrinted.RemoveAllListeners();
        dialogTyper.CharacterPrinted.AddListener (dialogueAnimation.ReceiveString);

        //dialogTyper.PrintCompleted.RemoveAllListeners();
        //dialogTyper.PrintCompleted.AddListener (dialogueAnimation.ResetState);
    }

    private void Update() 
    {
        skipArrow.enabled = DialogueSystem.OnDialogue && !dialogTyper.IsTyping;

        if (delayFrames > 0)
            delayFrames--;
    }

    private void ForwardInput()
    {
        if (dialogTyper.IsTyping)
        {
            dialogTyper.Skip();
        } 
        else
        {
            if (++dialogueIndex < dialogues.Count)
            {
                ShowText( speakerName, dialogues[dialogueIndex] );
            }
            else
            {
                EndDialogue();
                StartCoroutine
                (
                    RaposUtil.Wait(3, () =>  DialogueSystem.OnDialogue = false)
                );
            }
        }
    }

    public void ShowText (string name, string dialogue)
    {

        if (!showing)
        {
            transform.localScale = new Vector3 (1, 0);
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            
            if (sequence != null)
                sequence.Kill();

            sequence.Append( transform.DOScaleY ( 1f, transitionDuration ) );
            sequence.Join  ( canvasGroup.DOFade ( 1f, transitionDuration ) );
        }
        showing = true;

        if (nameDisplay)
            nameDisplay.text = name;

        if (!dialogTyper)
            return;
    
        // -- Trata os tags
        textTags = new List<TextBoxTag>();

        var data = TextFunctions.ParseTags(dialogue);
        if (data.tags != null)
        {
            dialogue = data.output;
            textTags = data.tags;
        }

        if (playSoundOnType) 
            playSoundOnType.SetMute ( textTags.Contains(TextBoxTag.InstantText) );

        dialogTyper.TypeText (dialogue);

        // -- Executa os tags
        if (textTags.Contains(TextBoxTag.InstantText) || autoSkip)
            dialogTyper.Skip();
                
        if (textTags.Contains(TextBoxTag.AwardSound))    
        {
            if (awardSoundAKEvent != null)
                awardSoundAKEvent.Post(gameObject);
        }
    }

    public void EndDialogue()
    {
        OnDialogueEnd?.Invoke();

        if (nameDisplay)
            nameDisplay.text = string.Empty;

        if (dialogTyper)
            dialogTyper.TypeText (string.Empty);
        
        transform.localScale = new Vector3 (1, 1);
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
            
        if (sequence != null)
            sequence.Kill();

        sequence.Append( transform.DOScaleY ( 0f, transitionDuration ) );
        sequence.Join ( canvasGroup.DOFade ( 0f, transitionDuration ) );

        interactable?.IconState(true);

        showing = false;
    }

    private void OnDisable() 
    {
        showing = false;    

        playerInputActions.Player.Interact.Disable();
        playerInputActions.Player.Jump.Disable();
    }
}
