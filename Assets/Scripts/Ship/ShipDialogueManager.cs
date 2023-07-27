using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class ShipDialogueManager : MonoBehaviour
{
    string dialogOptionsRegex = @"^.*(<\d+>(.)+)+[^>]$";
    string flowRedirectRegex = @"^.*->\d+$";
    
    [System.Serializable]
    public struct DialogueIndexer
    {
        public string notificationID;
        public int index;
    }

    [SerializeField] List<DialogueIndexer> dialogueIndexers;

    [Header("Start Scene")]
    [SerializeField] ShipNPCData startDialogueData;
    [SerializeField] float startUpDelay;
    [SerializeField] float lookAtScreensDuration;
    [SerializeField] Transform lookAtTarget;

    [Header("References")]
    [SerializeField] PlayerCharacter playerCharacter;
    [SerializeField] ShipDialogueBox dialogueBox;
    [SerializeField] ShipInitializerSystem shipInitializer;

    public static int StartDialogueIndex = -1; // -- Chama "-1" se não tiver diálogo no início

    Sequence startSequence;

    void Start()
    {
        if (!playerCharacter || !dialogueBox || !shipInitializer || TrailerSceneCaller.AutoStart)
        {
            enabled = false;
            return;
        }

        SetDialogueOnStart();
    }

    private void SetDialogueOnStart()
    {
        if (dialogueIndexers == null)
            return;

        // Read notifications
        foreach (DialogueIndexer dialogueIndexer in dialogueIndexers)
        {
            if (UINotificationManager.Check (dialogueIndexer.notificationID))
            {
                UINotificationManager.Use (dialogueIndexer.notificationID);
                StartDialogueIndex = dialogueIndexer.index;
                break;
            }
        }

        if (StartDialogueIndex < 0)
            return;

        if (startDialogueData == null)
            return;

        GameManager.OnDialogue = true;

        shipInitializer.AfterStartAction = () => 
        {
            CallDialogueOnStart();
        };
    }

    private void CallDialogueOnStart()
    {
        List<DialogueGroup> dialogueGroups = startDialogueData.dialogueGroups;
        if (dialogueGroups.Count < 1)
            return;

        startSequence = DOTween.Sequence();
        startSequence.AppendInterval(startUpDelay);
        startSequence.Append( SetupForScene (startDialogueData) );
        startSequence.AppendInterval(lookAtScreensDuration);
        startSequence.AppendCallback
        (
            () => dialogueBox.SetShown(true)
        );
        startSequence.AppendInterval(.5f);
        startSequence.OnComplete
        (
            () => SetDialogueGroup (startDialogueData, dialogueGroups[StartDialogueIndex % dialogueGroups.Count])
        );
    }

    private void SetDialogueGroup (ShipNPCData dialogueData, DialogueGroup dialogueGroup, int index = 0)
    {
        UnityAction afterInputAction = null;
        (bool isValid, string text) shipTextInfo = LocalizationManager.GetShipText( dialogueGroup[index % dialogueGroup.Count] );

        int nextMessageIndex = index + 1;

        if (Regex.IsMatch(shipTextInfo.text, flowRedirectRegex))
        {
            string[] split = shipTextInfo.text.Split('>','-');
            nextMessageIndex = Int32.Parse(split[2]) -1 ;
            shipTextInfo.text = split[0];
        }
            

        if (index < dialogueGroup.Count) // -- Chama mais texto
        {
            afterInputAction = () =>
            {
                SetDialogueGroup(dialogueData, dialogueGroup, nextMessageIndex);
            };
        }
        else // -- Termina a sessão de diálogos 
        {
            EndDialogue(dialogueData);
            return;
        }

        // --Manda para a DialogueBox
        if (!shipTextInfo.isValid)
        {
            EndDialogue (dialogueData, forceOut: true);
            return;
        }

        dialogueBox.Type (shipTextInfo.text, delay: .5f, instantText: false, afterInputAction);
    }

    private void EndDialogue (ShipNPCData dialogueData, bool forceOut = false)
    {
        //Debug.Log("EndDialogue()");

        if (!forceOut)
            dialogueBox.SetShown(false);
        else 
            dialogueBox.SetShown(false, duration: .5f, forceOut: true);

        // -- TEMP PRA BUILD -------
        //if (SetShipDialogueOnNotification.AllDialoguesSet && StartDialogueIndex < 3)
        //{
        //    StartDialogueIndex = 3;
        //    //RaposUtil.WaitSeconds(this, duration: .5f,  );
        //    dialogueBox.Type(" ", delay: .5f, instantText: true, afterInputAction: null);
        //    CallDialogueOnStart();
        //    return;
        //}
        // -- 

        ResumeOnScene(dialogueData);
        StartDialogueIndex = -1;
        GameManager.OnDialogue = false;
    }

    private Sequence SetupForScene (ShipNPCData data)
    {
        Sequence s = DOTween.Sequence();

        switch (data.sceneType)
        {
            case ShipSceneType.TurnOnScreensAndTalk:
                PlatformerCharacter platformerCharacter = playerCharacter.GetComponent<PlatformerCharacter>();
                if (platformerCharacter)
                    s.AppendCallback( () => platformerCharacter.LookAtTarget(lookAtTarget != null ? lookAtTarget : transform) );
                break;
        }

        return s;
    }

    private void ResumeOnScene (ShipNPCData data)
    {
        switch (data.sceneType)
        {
            case ShipSceneType.TurnOnScreensAndTalk:
                shipInitializer.RestorePlayerControls();
                break;
        }
    }
}

public enum ShipSceneType
{
    None, TurnOnScreensAndTalk
}
