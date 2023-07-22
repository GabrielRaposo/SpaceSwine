using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventureLogManager : StoryEventDependent
{
    [SerializeField] List<AdventureLogScriptableObject> adventureLogs;
    
    AdventureLogDisplay display;

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
        CallDependentAction ( PrepData );
    }

    private void PrepData()
    {
        List <string> logTexts = new List<string>();

        foreach (AdventureLogScriptableObject log in adventureLogs)
        {
            if (StoryEventsManager.IsComplete(log.activationEventKey) && !StoryEventsManager.IsComplete(log.completionEventKey))
            {
                logTexts.Add (log.fallbackText); // <<<<<<<<<<<<<< TO-DO: pegar da planilha aqui
            }
        }

        display.Setup (logTexts);
    }
}
