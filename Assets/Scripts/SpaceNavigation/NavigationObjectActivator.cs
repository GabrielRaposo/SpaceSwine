﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationObjectActivator : MonoBehaviour
{
    [SerializeField] List<StoryEventScriptableObject> criteriaEvents;

    void Start()
    {
        if (criteriaEvents == null)
            return;

        NavigationObject[] navigationObjects = GetComponentsInChildren<NavigationObject>();
        if (navigationObjects == null)
            return;

        foreach (var criteria in criteriaEvents)
        {
            if (!criteria.state)
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
