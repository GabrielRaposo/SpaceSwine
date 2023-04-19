using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StoryEventDependent : MonoBehaviour
{
    protected void CallDependentAction (UnityAction action, int extraFrames = 0)
    {
        StartCoroutine ( StoryEventDependentAction(action, extraFrames) );
    }

    protected IEnumerator StoryEventDependentAction (UnityAction action, int extraFrames = 0)
    {
        yield return new WaitUntil( () => StoryEventsManager.Initiated );
        
        for (int i = 0; i < extraFrames; i++)
            yield return new WaitForEndOfFrame();

        action.Invoke();
    } 
}
