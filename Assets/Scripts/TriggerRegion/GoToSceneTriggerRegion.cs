﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GoToSceneTriggerRegion : MonoBehaviour
{
    [SerializeField] BuildIndex targetScene;
    [SerializeField] int nextSceneSpawnPoint = -1;

    [HideInInspector] public UnityAction OnTriggerAction;

    bool activated;

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (!collision.CompareTag("Player"))
            return;    

        if (activated)
            return;

        if (OnTriggerAction != null)
            OnTriggerAction.Invoke();

        if (nextSceneSpawnPoint > -1)
            SpawnManager.Index = nextSceneSpawnPoint;

        GameManager.GoToScene(targetScene);
        activated = true;
    }
}
