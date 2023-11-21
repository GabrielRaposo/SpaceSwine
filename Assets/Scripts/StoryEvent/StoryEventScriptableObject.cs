using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum WorldTag
{
    World0,
    World1,
    World2,
    World3
}

[CreateAssetMenu (fileName = "StoryW0", menuName = "ScriptableObjects/StoryEvent") ]
public class StoryEventScriptableObject : ScriptableObject
{
    public WorldTag worldTag;
    public string idTag;
    
    public int Goal = 1;
    public bool StartingState;

    [Header("Runtime Value")]
    public bool UpdatedState;

#if UNITY_EDITOR
    private void OnValidate() 
    {
        if (!Application.isPlaying)
        {
            UpdatedState = StartingState;
            return;
        }
        
        EventProgress eventProgress = StoryEventsManager.GetEventProgress(this);
        if (eventProgress == null)
            return;

        if (UpdatedState == StoryEventsManager.IsComplete(this))
            return;

        if (UpdatedState)
            StoryEventsManager.ChangeProgress(this, 999);
        else
            StoryEventsManager.ClearProgress(this);
    }
#endif

    public void SetUpdatedState (bool value)
    {
        UpdatedState = value;
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
}
