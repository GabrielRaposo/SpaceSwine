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
        {
            string[] subString = controlTypes[i].ToString().Split('_');
            if (subString.Length < 2)
                stringValues.Add ( controlTypes[i].ToString() );
            else
            {
                string s = subString[0] + " " + subString[1];
                stringValues.Add (s);
            }
        }

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

        GroundControlType controlType = controlTypes[index % controlTypes.Count];
        PlatformerCharacter.OnGroundControlType = controlType;
    }
}
