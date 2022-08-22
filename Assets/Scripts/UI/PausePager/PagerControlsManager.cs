using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PagerControlsManager : MonoBehaviour
{
    [SerializeField] List<GroundControlType> controlTypes;
    [SerializeField] PagerInteractableScrollList controlTypeScrollList; 

    void Start()
    {
        if (controlTypes.Count < 1)
            return;

        List <string> stringValues = new List<string>();
        for (int i = 0; i < controlTypes.Count; i++)
            stringValues.Add ( controlTypes[i].ToString() );

        if (controlTypeScrollList == null)
            return;
        
        int startingIndex = 0;

        GroundControlType groundControlType = PlatformerCharacter.OnGroundControlType;
        if (controlTypes.Contains( groundControlType ))
            startingIndex = controlTypes.FindIndex( (c) => c == groundControlType );

        controlTypeScrollList.InitList( stringValues, startingIndex );
    }

    public void UpdateOnGroundControl (int index)
    {
        if (controlTypes.Count < 1)
            return;

        Debug.Log("Troca control type");

        GroundControlType controlType = controlTypes[index % controlTypes.Count];
        PlatformerCharacter.OnGroundControlType = controlType;
    }
}
