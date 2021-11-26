using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NPC Data", menuName = "ScriptableObjects/NPC Data")]
public class NPCData : ScriptableObject, ISerializationCallbackReceiver
{
    public string npcName;
    public int startingIndex = 0;
    public List<DialogueGroup> dialogueGroups;
    public UnityAction OnDialogueEnd;

    int accessIndex;

    public DialogueGroup this[int i]
    {
        get 
        {
            if (dialogueGroups == null || dialogueGroups.Count < 1)
                return new DialogueGroup();

            return dialogueGroups[i % dialogueGroups.Count]; 
        }
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
        Debug.Log("get at index: " + accessIndex);
        return this[accessIndex];
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

    public void OnAfterDeserialize()
    {
        accessIndex = startingIndex;
    }

    public void OnBeforeSerialize()
    {

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
