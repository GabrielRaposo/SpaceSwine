﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using RedBlueGames.Tools.TextTyper;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class DialogueBox : MonoBehaviour
{
    [SerializeField] float transitionDuration;

    [Header("References")]
    [SerializeField] TextMeshProUGUI nameDisplay;
    [SerializeField] TextTyper dialogTyper;
    [SerializeField] Image skipArrow;

    bool showing;
    CanvasGroup canvasGroup;

    int delayFrames;
    int dialogIndex;

    Interactable interactable;
    DialogueAnimation dialogueAnimation;
    string speakerName;
    List <string> dialogs;

    Sequence sequence;

    private void Start() 
    {
        canvasGroup = GetComponent<CanvasGroup>(); 
        canvasGroup.alpha = 0;
    }

    public void SetDialogueData(Interactable interactable, string speakerName, List<string> dialogs)
    {
        this.interactable = interactable;
        this.speakerName = speakerName;
        this.dialogs = dialogs;

        SetDialogueEvents();

        dialogIndex = 0;
        ShowText(speakerName, dialogs[dialogIndex]);
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

        if (!DialogueSystem.OnDialogue)
            return;

        if (delayFrames > 0)
        {
            delayFrames--;
            return;
        }

        if (Input.GetButtonDown("Jump") || Input.GetButtonDown("Interact"))
            ForwardInput();
    }

    private void ForwardInput()
    {
        if (dialogTyper.IsTyping)
        {
            dialogTyper.Skip();
        } 
        else
        {
            if (++dialogIndex < dialogs.Count)
            {
                ShowText( speakerName, dialogs[dialogIndex] );
            }
            else
            {
                EndDialogue();
                DialogueSystem.OnDialogue = false;
            }
        }
    }

    public void ShowText (string name, string dialog)
    {
        if (!showing)
        {
            transform.localScale = new Vector3 (1, 0);
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            
            if (sequence != null)
                sequence.Kill();

            sequence.Append( transform.DOScaleY ( 1f, transitionDuration ) );
            sequence.Join ( canvasGroup.DOFade ( 1f, transitionDuration ) );
        }
        showing = true;

        if (nameDisplay)
            nameDisplay.text = name;

        if (dialogTyper)
            dialogTyper.TypeText (dialog);
    }

    public void EndDialogue()
    {
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
    }
}