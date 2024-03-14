using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowInteractableIconByEvent : StoryEventDependent
{
    [System.Serializable]
    public class CriteriaPair
    {
        public StoryEventScriptableObject showCriteria;
        public StoryEventScriptableObject hideCriteria;
    }
    
    [SerializeField] ShipExclamationIcon exclamationIcon;
    [SerializeField] public List<CriteriaPair> criteriaPairs;

    void Start()
    {
        CallDependentAction( Setup, extraFrames: 1 );
    }

    void Setup()
    {
        if (!exclamationIcon)
            return;

        if (criteriaPairs == null || criteriaPairs.Count < 1)
            return;

        foreach (var pair in criteriaPairs)
        {
            if (StoryEventsManager.IsComplete(pair.showCriteria) && !StoryEventsManager.IsComplete(pair.hideCriteria))
            {
                exclamationIcon.Show();
                return;
            }
        }

        exclamationIcon.Hide();
    }
}
