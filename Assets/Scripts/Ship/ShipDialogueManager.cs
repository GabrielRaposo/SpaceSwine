using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class ShipDialogueManager : StoryEventDependent
{
    static string dialogOptionsRegex = @"^.*(<\d+>(.)+)+[^>]$";
    static string flowRedirectRegex = @"^.*->\d+$";
    public static string shipDialogListRegex = @"^CHAT_W\d_([\w\d\-]+)_\d{3}\.\d{2}$";

    [System.Serializable]
    public class DialogueIndexer
    {
        public StoryEventScriptableObject storyEvent;
        public List<StoryEventScriptableObject> extraEvents;
        public string textCode;
    }

    [Header("Start Scene")]
    [SerializeField] ShipNPCData startDialogueData;
    [SerializeField] float startUpDelay;
    [SerializeField] float lookAtScreensDuration;
    [SerializeField] Transform lookAtTarget;
    [SerializeField] StoryEventScriptableObject talkedWithShipStoryEvent;

    [Header("References")]
    [SerializeField] PlayerCharacter playerCharacter;
    [SerializeField] ShipDialogueBox dialogueBox;
    [SerializeField] ShipDialogueOptions optionsBox;
    [SerializeField] ShipInitializerSystem shipInitializer;
    [SerializeField] ShipExclamationIcon shipExclamationIcon;

    [Header("Intro Dialogue")]
    [SerializeField] int testSpecific = -1;
    [SerializeField] List<DialogueIndexer> dialogueIndexers;
    
    StoryEventScriptableObject afterDialogueStoryEvent;

    Sequence startSequence;
    private PlatformerCharacter platformerCharacter;
    private PlayerInput playerInput;

    void Start()
    {
        if (!playerCharacter || !dialogueBox || !shipInitializer || TrailerSceneCaller.AutoStart)
        {
            enabled = false;
            return;
        }

        CallDependentAction( SetDialogueOnStart );
        
        platformerCharacter = playerCharacter.gameObject.GetComponent<PlatformerCharacter>();
        playerInput = playerCharacter.gameObject.GetComponent<PlayerInput>();
    }

    private void SetDialogueOnStart()
    {
        if (dialogueIndexers == null)
            return;

        int StartDialogueIndex = -1;
        DialogueIndexer dialogueIndexer = null;

        for (int i = 0; i < dialogueIndexers.Count; i++)
        {
            //Debug.Log($"i: {i}, test: {testSpecific}");

            if (testSpecific > -1 && i != testSpecific)
                continue;

            DialogueIndexer data = dialogueIndexers[i];
            if (data.storyEvent == null)
                continue;

            bool isStoryEventComplete = false;
            if (StoryEventsManager.IsComplete(data.storyEvent))
                isStoryEventComplete = true;
            else if (data.extraEvents != null && data.extraEvents.Count > 0)
            {
                foreach (var d in data.extraEvents)
                {
                    if (StoryEventsManager.IsComplete(d))
                    {
                        isStoryEventComplete = true;
                        break;
                    }
                }
            }

            if (isStoryEventComplete && !UINotificationManager.Check( NotificationID(i) ))
            {   
                StartDialogueIndex = i;
                dialogueIndexer = data;
                UINotificationManager.Create (NotificationID(i));
                break;
            }
        }

        if (StartDialogueIndex < 0)
            return;

        DialogueGroup dialogueGroup = new DialogueGroup();
        dialogueGroup.tags = new List<string>();

        int localIndex = 1;

        for (int k = 0; k < 30; k++) 
        {
            if (!CheckTagValidity( FormatedTag(dialogueIndexer.textCode, localIndex) ))
                break;

            dialogueGroup.tags.Add( FormatedTag(dialogueIndexer.textCode, localIndex) );
            localIndex++;
        }

        if (dialogueGroup.tags.Count < 1)
           return;

        GameManager.OnDialogue = true;

        shipInitializer.AfterStartAction = () => 
        {
            CallDialogueOnStart(dialogueGroup);
        };
    }

    string NotificationID (int id) => "StartDialogue_" + id.ToString("00");

    string FormatedTag (string tag, int i) => tag + "." + i.ToString("00");

    private bool CheckTagValidity (string tag)
    {
        (bool isValid, string text) data = LocalizationManager.GetShipText( tag );

        return data.isValid;
    }

    private void CallDialogueOnStart (DialogueGroup dialogueGroup)
    {
        startSequence = DOTween.Sequence();
        startSequence.AppendInterval(startUpDelay);
        startSequence.Append(SetupForScene(TurnOnScreensAndTalk: true));
        startSequence.AppendInterval(lookAtScreensDuration);
        startSequence.AppendCallback
        (
            () => dialogueBox.SetShown(true)
        );
        startSequence.AppendInterval(.5f);
        startSequence.OnComplete
        (
            () => SetDialogueGroup (startDialogueData, dialogueGroup)
        );
    }

    public void CallDialogueFromAvailableTalks()
    {
        //SetExclamationIcon(false);
        //return;

        var shipTalkIds = SaveManager.GetShipTalkIds();
        
        if (shipTalkIds.Count == 0)
            return;
        
        GameManager.OnDialogue = true;
        SetPlayerLocked(true);
        
        var mainDialogId = shipTalkIds[0];
        var dialogId = mainDialogId.Remove(mainDialogId.Length - 3, 3);
        
        var dialogGroup = new DialogueGroup();
        
        dialogGroup.tags = new List<string>();

        bool sucess;
        int n = 1;

        do
        {
            string localizationCode = dialogId + "." + n.ToString("00");
            var tuple = LocalizationManager.GetShipText(localizationCode);
            sucess = tuple.Item1;
            
            if(!sucess)
                break;
            
            dialogGroup.tags.Add(localizationCode);
            n++;

        } while (true);

        startSequence = DOTween.Sequence();
        //startSequence.AppendInterval(startUpDelay);
        startSequence.Append( SetupForScene (startDialogueData) );
        //startSequence.AppendInterval(lookAtScreensDuration);
        startSequence.AppendCallback
        (
            () => dialogueBox.SetShown(true)
        );
        startSequence.AppendInterval(.5f);
        startSequence.OnComplete
        (
            () => SetDialogueGroup(startDialogueData, dialogGroup, 0, mainDialogId)
        );
    }

    private void SetDialogueGroup (ShipNPCData dialogueData, DialogueGroup dialogueGroup, int index = 0, string idToRemove = "")
    {
        (bool isValid, string text) shipTextInfo = LocalizationManager.GetShipText( dialogueGroup[index % dialogueGroup.Count] );
        
        bool isDialogOptions = false;
        UnityAction afterInputAction = null;
        string[] split = new string[] { };

        int nextMessageIndex = index + 1;

        if (Regex.IsMatch(shipTextInfo.text, flowRedirectRegex))
        {
            split = shipTextInfo.text.Split('>','-');
            nextMessageIndex = Int32.Parse(split[2]) -1 ;
            shipTextInfo.text = split[0];
        }
        else
        {
            if (Regex.IsMatch(shipTextInfo.text, dialogOptionsRegex))
            {
                split = shipTextInfo.text.Split('<', '>');
                shipTextInfo.text = split[0];
                isDialogOptions = true;
            }
        }

        if (index < dialogueGroup.Count) // -- Chama mais texto
        {
            if (!isDialogOptions)
            {
                afterInputAction = () => SetDialogueGroup(dialogueData, dialogueGroup, nextMessageIndex, idToRemove);    
            }
            else
            {
                afterInputAction = () => OpenDialogOptionsSelectionMenu(split, dialogueData, dialogueGroup);
            }
        }
        else // -- Termina a sessão de diálogos 
        {
            EndDialogue(dialogueData, idToRemoveFromShipDialog: idToRemove);
            return;
        }

        // --Manda para a DialogueBox
        if (!shipTextInfo.isValid)
        {
            EndDialogue (dialogueData, forceOut: true, idToRemoveFromShipDialog: idToRemove);
            return;
        }

        dialogueBox.Type (shipTextInfo.text, delay: .5f, instantText: false, afterInputAction);
    }

    private void OpenDialogOptionsSelectionMenu(string[] split, ShipNPCData dialogueData, DialogueGroup dialogueGroup)
    {
        int optionsCount = Mathf.RoundToInt((split.Length - 1) / 2f);
        List<(int, string)> options = new List<(int, string)>();

        for (int i = 0; i < optionsCount; i++)
        {
            int j = (i * 2) + 1;
            options.Add((Int32.Parse(split[j]), split[j + 1]));
        }
        optionsBox.InitializeOptions(options, n => SetDialogueGroup(dialogueData, dialogueGroup, n));
        optionsBox.gameObject.SetActive(true);
    }

    private void EndDialogue (ShipNPCData dialogueData, string idToRemoveFromShipDialog , bool forceOut = false)
    {
        SetPlayerLocked(false);

        if (!forceOut)
            dialogueBox.SetShown(false);
        else 
            dialogueBox.SetShown(false, duration: .5f, forceOut: true);

        ResumeOnScene(dialogueData);
        if (afterDialogueStoryEvent != null)
            StoryEventsManager.ChangeProgress(afterDialogueStoryEvent, +999);
        if (talkedWithShipStoryEvent != null)
            StoryEventsManager.ChangeProgress(talkedWithShipStoryEvent, +999);

        GameManager.OnDialogue = false;

        SaveManager.RemoveFromShipTalkIds(idToRemoveFromShipDialog);
        
        dialogueBox.ClearAndCloseDialogBox();

    }

    private Sequence SetupForScene (bool TurnOnScreensAndTalk)
    {
        Sequence s = DOTween.Sequence();

        if (TurnOnScreensAndTalk)
        { 
            PlatformerCharacter platformerCharacter = playerCharacter.GetComponent<PlatformerCharacter>();
            if (platformerCharacter)
                s.AppendCallback( () => platformerCharacter.LookAtTarget(lookAtTarget != null ? lookAtTarget : transform) );
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

    private void SetPlayerLocked(bool value)
    {
        playerCharacter.SetPhysicsBody(!value);
        playerCharacter.enabled = !value;
        platformerCharacter.enabled = !value;
        playerInput.enabled = !value;
        
        if(value)
            platformerCharacter.StandStillState();
    }

    public void SetExclamationIcon(bool value)
    {
        if(value)
            shipExclamationIcon.Show();
        else
            shipExclamationIcon.Hide();
    }
    
}

public enum ShipSceneType
{
    None, TurnOnScreensAndTalk
}
