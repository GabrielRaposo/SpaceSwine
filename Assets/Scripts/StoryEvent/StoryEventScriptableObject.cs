﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu (fileName = "StoryW0", menuName = "ScriptableObjects/StoryEvent") ]
public class StoryEventScriptableObject : ScriptableObject
{
    public bool state;
    public UnityAction<bool> OnStateChange;

    public bool State
    {
        set 
        {
            Debug.Log($"state { name } changed to { value }");

            state = value;
            OnStateChange?.Invoke(value);

            SetDirty();
        }
    }
}
