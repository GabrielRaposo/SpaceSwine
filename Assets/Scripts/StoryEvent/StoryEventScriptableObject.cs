using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu (fileName = "StoryW0", menuName = "ScriptableObjects/StoryEvent") ]
public class StoryEventScriptableObject : ScriptableObject
{
    public bool StartingState;
    public int Goal = 1;
}
