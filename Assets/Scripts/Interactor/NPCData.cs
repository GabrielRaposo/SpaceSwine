using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPC Data", menuName = "ScriptableObjects/NPC Data")]
public class NPCData : ScriptableObject
{
    public string npcName;
    [TextArea(1, 3)] public List<string> dialog;
    // After Talk Event
}
