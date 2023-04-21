using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINotification
{    
    public string id;
    
    public enum State
    {
        Active,
        Used
    }

    State state;

    public State GetState()
    {
        return state;
    }

    public void Use()
    {
        state = State.Used;
    }
}
