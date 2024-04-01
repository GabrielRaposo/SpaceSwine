using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableNPCSimplified : InteractableNPC
{
    [Header("NPC Data")]
    public string npcNameCode;
    public int startingIndex = 0;
    public int holdIndex = -1;
    public List<string> dialogueGroupTags;

    List<DialogueGroup> dialogueGroups;
    int accessIndex;

    private void Start()
    {
        // gerar dilogue groups a partir do tag principal (sem o .XX do final)
        if (dialogueGroupTags == null || dialogueGroupTags.Count < 1)
        {
            gameObject.SetActive(false);
            return;
        }

        dialogueGroups = new List<DialogueGroup>();

        for (int i = 0; i < dialogueGroupTags.Count; i++) 
        {
            DialogueGroup dialogueGroup = new DialogueGroup();

            string tag = dialogueGroupTags[i];
            int localIndex = 1;

            if (!CheckTagValidity( FormatedTag(tag, localIndex) ))
                continue;

            for (int k = 0; k < 30; k++) 
            {
                dialogueGroup.tags.Add( FormatedTag(tag, localIndex) );
                localIndex++;

                if (!CheckTagValidity( FormatedTag(tag, localIndex) ))
                    break;
            }

            dialogueGroups.Add(dialogueGroup);
        }

        //for (int i = 0; i < dialogueGroups.Count; i++)
        //{
        //    for (int j = 0; j < dialogueGroups[i].tags.Count; j++) 
        //    {
        //        Debug.Log($"{i+1}-{j}: {dialogueGroups[i].tags[j]}");
        //    }
        //}

        accessIndex = startingIndex;
    }

    string FormatedTag (string tag, int i) => tag + "." + i.ToString("00");

    private bool CheckTagValidity (string tag)
    {
        (bool isValid, string text) data = LocalizationManager.GetStoryText( tag );

        return data.isValid;
    }

    public override void Interaction (PlayerInteractor interactor)
    {
        //AchievementsManager.SetAchievementState(AchievementEnum.TheSpirit, true);

        //Debug.Log($"Interaction - name: {name}");
        if (!interactable)
            return;

        //base.Interaction (interactor);
        this.interactor = interactor;

        if (dialogueGroups == null)
            return;

        if (OnInteraction != null)
            OnInteraction.Invoke();

        DialogueSystem dialogSystem = DialogueSystem.Instance;

        DialogueGroup dialogueGroup = GetAtIndex();
        // passar o accessIndex e a lógica dele pra esse componente local mesmo

        //Debug.Log("data.npcNameCode: " + data.npcNameCode);
        string npcName = string.Empty;
        if (!string.IsNullOrEmpty (npcNameCode) )
        {
            (bool isValid, string text) nameData = LocalizationManager.GetStoryText(npcNameCode);
            if (nameData.isValid)
                npcName = nameData.text;
        }

        UnityAction OnDialogueEndExtra = OnDialogueEnd;
        OnDialogueEndExtra += () => 
        {
            if (interactor.GetComponent<PlayerInput>())
                interactor.GetComponent<PlayerInput>().LaunchCooldown = .5f;
        };
            
        dialogSystem?.SetDialogue(this, npcName, dialogueGroup.tags, OnDialogueEndExtra, AfterDialogueEnd, customDialogueStyle, talkSoundAKEvent);

        if (interactor)
        {
            PlatformerCharacter platformerCharacter = interactor.GetComponent<PlatformerCharacter>();
            platformerCharacter?.KillInputs();
            platformerCharacter?.LookAtTarget(transform);
        }

        DialogueIndexLogic();
    }

    protected override void DialogueIndexLogic()
    {
        int previousIndex = Index;
        MoveIndex(1);

        if (previousIndex == holdIndex)
            SetIndex(previousIndex);
    }

    public int Index
    {
        get 
        {
            return accessIndex;
        }
    }

    public DialogueGroup GetAtIndex()
    {
        if (dialogueGroups == null) 
            return null;

        return dialogueGroups[accessIndex];
    }

    public void MoveIndex (int value)
    {
        accessIndex += value;
        if (accessIndex < 0)
            accessIndex = dialogueGroups.Count - 1;
        accessIndex %= dialogueGroups.Count;
    }

    public void SetIndex (int value)
    {
        if (dialogueGroups.Count < 1)
            return;

        accessIndex = value % dialogueGroups.Count;
    }
}
