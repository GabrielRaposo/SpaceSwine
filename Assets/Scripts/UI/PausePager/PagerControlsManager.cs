using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PagerControlsManager : MonoBehaviour
{
    [SerializeField] List<GroundControlType> controlTypes;
    [SerializeField] List<string> controlTypesLocalizationCodes;
    [SerializeField] PagerInteractableScrollList controlTypeScrollList; 

    void Start()
    {
        if (controlTypes.Count < 1)
            return;

        if (controlTypeScrollList == null)
            return;
        
        int startingIndex = 0;

        GroundControlType groundControlType = PlatformerCharacter.OnGroundControlType;
        if (PlayerPrefs.HasKey(PlatformerCharacter.CONTROLTYPE_KEY))
        {
            groundControlType = (GroundControlType) PlayerPrefs.GetInt(PlatformerCharacter.CONTROLTYPE_KEY);
        }

        if (controlTypes.Contains( groundControlType ))
            startingIndex = controlTypes.FindIndex( (c) => c == groundControlType );

        controlTypeScrollList.InitList(controlTypesLocalizationCodes, startingIndex );
    }

    public void UpdateOnGroundControl (int index)
    {
        if (controlTypes.Count < 1)
            return;

        GroundControlType controlType = controlTypes[index % controlTypes.Count];
        PlatformerCharacter.OnGroundControlType = controlType;
        PlayerPrefs.SetInt(PlatformerCharacter.CONTROLTYPE_KEY, (int) controlType);
    }
}
