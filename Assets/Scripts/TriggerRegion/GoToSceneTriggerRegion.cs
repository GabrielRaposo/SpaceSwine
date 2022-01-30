using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToSceneTriggerRegion : MonoBehaviour
{
    [SerializeField] BuildIndex targetScene;
    bool activated;

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (!collision.CompareTag("Player"))
            return;    

        if (activated)
            return;

        GameManager.GoToScene(targetScene);
        activated = true;   
    }
}
