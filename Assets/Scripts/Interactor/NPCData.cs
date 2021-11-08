using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPC Data", menuName = "ScriptableObjects/NPC Data")]
public class NPCData : ScriptableObject
{
    public string npcName;
    public List<DialogueGroup> dialogueGroups;
    // After Talk Event

    public DialogueGroup this[int i]
    {
        get 
        {
            if (dialogueGroups == null || dialogueGroups.Count < 1)
                return new DialogueGroup();

            return dialogueGroups[i % dialogueGroups.Count]; 
        }
    }
}

[System.Serializable]
public class DialogueGroup 
{
    public List<string> tags;

    public string this [int i]
    {
        get 
        {
            if (tags == null || tags.Count < 1)
                return null;

            return tags[i % tags.Count]; 
        }
    }
}
