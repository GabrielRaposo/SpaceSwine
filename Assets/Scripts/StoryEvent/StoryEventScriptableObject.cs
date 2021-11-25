using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "StoryW0", menuName = "ScriptableObjects/StoryEvent") ]
public class StoryEventScriptableObject : ScriptableObject
{
    public bool state;

    public void SetState(bool value)
    {
        state = value;
    }
}
