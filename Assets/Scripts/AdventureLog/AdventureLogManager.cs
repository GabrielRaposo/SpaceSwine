using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventureLogManager : StoryEventDependent
{
    [SerializeField] List<AdventureLogScriptableObject> adventureLogs;
    
    AdventureLogDisplay display;

    private static bool Initiated;
    public static AdventureLogManager Instance;

    private void Awake() 
    {
        if (Instance != null)
        {
            gameObject.SetActive(false);
            return;
        }

        Instance = this;
    }

    public static void ResetStates()
    {
        Initiated = false;
        
        if (!Instance)
            return;
       
        if (Instance.display)
        {
            Instance.CallForUpdate (Instance.display);
        }
    }

    public void CallForUpdate (AdventureLogDisplay display) 
    {
        this.display = display;

        CallDependentAction ( SetAndSendList, extraFrames: 1 );

        if (!Initiated)
        {
            Initiated = true;
            CallDependentAction ( SetListeners, extraFrames: 2 );
        }
    }

    private void SetListeners()
    {
        if (adventureLogs.Count < 1)
            return;

        foreach (AdventureLogScriptableObject log in adventureLogs)
        {
            if (log.activationEventKey != null)
                StoryEventsManager.AddListener (log.activationEventKey, (b) => AddToList      (b, log) );
            
            if (log.completionEventKey != null)
                StoryEventsManager.AddListener (log.completionEventKey, (b) => RemoveFromList (b, log) );
        }
    }

    private void SetAndSendList()
    {
        List<AdventureLogScriptableObject> logsList = new List<AdventureLogScriptableObject>();

        foreach (var log in adventureLogs)
        {
            if (log.activationEventKey != null && StoryEventsManager.IsComplete(log.activationEventKey))
            {
                if (log.completionEventKey != null && StoryEventsManager.IsComplete(log.completionEventKey))
                    continue;

                logsList.Add (log);
            }
        }

        display.Setup (logsList);
    }

    private void AddToList (bool value, AdventureLogScriptableObject log)
    {
        if (!value)
            return;

        if (display == null)
            return;

        display.AddToList(log);
    }

    private void RemoveFromList (bool value, AdventureLogScriptableObject log)
    {
        if (!value)
            return;

        if (display == null)
            return;

        display.RemoveFromList(log);
    }
}
