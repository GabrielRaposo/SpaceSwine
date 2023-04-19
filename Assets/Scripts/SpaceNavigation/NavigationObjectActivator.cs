﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationObjectActivator : StoryEventDependent
{
    [SerializeField] List<StoryEventScriptableObject> criteriaEvents;

    void Start()
    {
        CallDependentAction ( Init );
    }

    private void Init()
    {
        if (criteriaEvents == null)
            return;

        NavigationObject[] navigationObjects = GetComponentsInChildren<NavigationObject>();
        if (navigationObjects == null)
            return;

        foreach (var criteria in criteriaEvents)
        {
            if ( !StoryEventsManager.IsComplete(criteria) )
            {
                SetNavObjectsState (false, navigationObjects);
                return;
            }
        }

        SetNavObjectsState (true, navigationObjects);
    }
    
    private void SetNavObjectsState (bool value, NavigationObject[] navigationObjects)
    {
        foreach (NavigationObject navigationObject in navigationObjects)
            navigationObject.gameObject.SetActive(value);
    }
}
