using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShipDialogueManager : MonoBehaviour
{
    [SerializeField] ShipNPCData startDialogueData;

    [Header("References")]
    [SerializeField] ShipDialogueBox dialogueBox;
    [SerializeField] ShipShuttleSystem shuttleSystem;

    //public static int StartDialogueIndex = -1; // -- Chama "-1" se não tiver diálogo no início
    public static int StartDialogueIndex = 0; // -- Chama "-1" se não tiver diálogo no início

    void Start()
    {
        if (!dialogueBox || !shuttleSystem)
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

        shuttleSystem.AfterStartAction = () => 
        {
            CallDialogueOnStart();
        };
    }

    private void CallDialogueOnStart()
    {
        List<DialogueGroup> dialogueGroups = startDialogueData.dialogueGroups;
        if (dialogueGroups.Count < 1)
            return;

        SetupForScene (startDialogueData);

        dialogueBox.SetShown(true);

        SetDialogueGroup (startDialogueData, dialogueGroups[StartDialogueIndex % dialogueGroups.Count]);
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
            ResumeOnScene(dialogueData);
            dialogueBox.SetShown(false);
            StartDialogueIndex = -1;
            return;
        }

        // -- Localiza o texto e manda para a DialogueBox
        (bool isValid, string text) data = LocalizationManager.GetShipText( dialogueGroup[index % dialogueGroup.Count] );
        if (!data.isValid)
        {
            dialogueBox.SetShown(false);
            return;
        }

        // -- TO-DO: tratar tags do texto

        dialogueBox.Type (data.text, delay: .5f, instantText: false, afterInputAction);
    }
    private void SetupForScene (ShipNPCData data)
    {
        switch (data.sceneType)
        {
            case ShipSceneType.TurnOnScreensAndTalk:
                break;
        }
    }

    private void ResumeOnScene (ShipNPCData data)
    {
        switch (data.sceneType)
        {
            case ShipSceneType.TurnOnScreensAndTalk:
                break;
        }
    }
}

public enum ShipSceneType
{
    None, TurnOnScreensAndTalk
}
