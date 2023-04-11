using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class ShipDialogueManager : MonoBehaviour
{
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

        if (index < dialogueGroup.Count) // -- Chama mais texto
        {
            afterInputAction = () =>
            {
                SetDialogueGroup(dialogueData, dialogueGroup, index + 1);
            };
        }
        else // -- Termina a sessão de diálogos 
        {
            EndDialogue(dialogueData);
            return;
        }

        // -- Localiza o texto e manda para a DialogueBox
        (bool isValid, string text) data = LocalizationManager.GetShipText( dialogueGroup[index % dialogueGroup.Count] );
        if (!data.isValid)
        {
            EndDialogue (dialogueData, forceOut: true);
            return;
        }

        dialogueBox.Type (data.text, delay: .5f, instantText: false, afterInputAction);
    }

    private void EndDialogue (ShipNPCData dialogueData, bool forceOut = false)
    {
        //Debug.Log("EndDialogue()");

        if (!forceOut)
            dialogueBox.SetShown(false);
        else 
            dialogueBox.SetShown(false, duration: .5f, forceOut: true);

        // -- TEMP PRA BUILD -------
        if (SetShipDialogueOnEvent.AllDialoguesSet && StartDialogueIndex < 3)
        {
            StartDialogueIndex = 3;
            //RaposUtil.WaitSeconds(this, duration: .5f,  );
            dialogueBox.Type(" ", delay: .5f, instantText: true, afterInputAction: null);
            CallDialogueOnStart();
            return;
        }
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
