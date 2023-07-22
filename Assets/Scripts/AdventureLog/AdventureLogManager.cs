using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventureLogManager : StoryEventDependent
{
    [SerializeField] List<AdventureLogScriptableObject> adventureLogs;
    
    AdventureLogDisplay display;
    bool initiated;

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

    public void CallForUpdate (AdventureLogDisplay display) 
    {
        this.display = display;

        CallDependentAction ( SendList, extraFrames: 1 );

        if (!initiated)
        {
            initiated = true;
            CallDependentAction ( SetListeners, extraFrames: 2 );
        }
    }

    private void SetListeners()
    {
        if (adventureLogs.Count < 1)
            return;

        foreach (AdventureLogScriptableObject log in adventureLogs)
        {
            StoryEventsManager.AddListener (log.activationEventKey, (b) => AddToList      (b, log) );
            StoryEventsManager.AddListener (log.completionEventKey, (b) => RemoveFromList (b, log) );
        }
    }

    private void SendList()
    {
        List<AdventureLogScriptableObject> logsList = new List<AdventureLogScriptableObject>();

        foreach (var log in adventureLogs)
        {
            if (StoryEventsManager.IsComplete(log.activationEventKey) && !StoryEventsManager.IsComplete(log.completionEventKey))
                logsList.Add (log);
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
