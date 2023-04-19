using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StoryEventDependent : MonoBehaviour
{
    protected void CallDependentAction (UnityAction action, int extraFrames = 0)
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine ( StoryEventDependentAction(action, extraFrames) );
        else
            action.Invoke();
    }

    protected IEnumerator StoryEventDependentAction (UnityAction action, int extraFrames = 0)
    {
        if (!StoryEventsManager.Initiated)
        {
            yield return new WaitUntil( () => StoryEventsManager.Initiated );
        
            for (int i = 0; i < extraFrames; i++)
                yield return new WaitForEndOfFrame();
        }

        action.Invoke();
    } 
}
