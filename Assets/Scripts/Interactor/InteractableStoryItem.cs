using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableStoryItem : Interactable
{
    [SerializeField] StoryEventScriptableObject storyEventData;
    [SerializeField] GameObject inputIcon;

    [Header("Dialogue")]
    [SerializeField] NPCData npcData;
    [SerializeField] DialogueBoxStyle customDialogueStyle;
    [SerializeField] AK.Wwise.Event talkSoundAKEvent;

    private void Start() 
    {
        if (storyEventData == null)
        {
            gameObject.SetActive(false);
            return;
        }

        HighlightState(false);

        StartCoroutine(WaitForStoryEventManager());
    }

    private IEnumerator WaitForStoryEventManager()
    {
        yield return new WaitUntil( () => StoryEventsManager.Initiated );

        Setup();
    }
    
    private void Setup()
    {
        bool state = StoryEventsManager.IsComplete(storyEventData);

        if (state)
        {
            gameObject.SetActive(false);
            return;
        }
    }

    protected override void HighlightState(bool value) 
    {
        if (inputIcon)
            inputIcon.SetActive(value);
    }

    public override void Interaction (PlayerInteractor interactor) 
    {
        base.Interaction(interactor);

        if (storyEventData == null)
            return;

        StoryEventsManager.ChangeProgress(storyEventData, +99);
        gameObject.SetActive(false);

        if (npcData)
        {
            //Debug.Log("data:  " + data);

            DialogueSystem dialogSystem = DialogueSystem.Instance;
            DialogueGroup dialogueGroup = npcData.GetAtIndex();

            //Debug.Log("data.npcNameCode: " + data.npcNameCode);
            string npcName = string.Empty;
            if (!string.IsNullOrEmpty (npcData.npcNameCode) )
            {
                (bool isValid, string text) nameData = LocalizationManager.GetStoryText(npcData.npcNameCode);
                if (nameData.isValid)
                    npcName = nameData.text;
            }
            
            dialogSystem?.SetDialogue(this, npcName, dialogueGroup.tags, OnDialogueEnd: () => {}, AfterDialogueEnd: () => {}, customDialogueStyle, talkSoundAKEvent);

            if (interactor)
            {
                PlatformerCharacter platformerCharacter = interactor.GetComponent<PlatformerCharacter>();
                platformerCharacter?.KillInputs();
                platformerCharacter?.LookAtTarget(transform);
            }
        }
    }
}