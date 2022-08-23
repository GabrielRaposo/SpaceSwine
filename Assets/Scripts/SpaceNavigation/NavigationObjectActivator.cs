using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationObjectActivator : MonoBehaviour
{
    [SerializeField] NavigationObject navigationObject;

    void Start()
    {
        if (!navigationObject)
            return;

        // -- TEMP PRA BUILD
        navigationObject.gameObject.SetActive( SetShipDialogueOnEvent.AllDialoguesSet );
    }
    
}
