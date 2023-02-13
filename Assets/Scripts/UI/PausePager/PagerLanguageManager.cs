using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PagerLanguageManager : MonoBehaviour
{
    [SerializeField] List<GameLocalizationCode> languageTypes;
    [SerializeField] List<string> controlTypesLocalizationCodes;
    [SerializeField] PagerInteractableScrollList languageTypeScrollList; 
    
    void Start()
    {
        if (languageTypes.Count < 1)
            return;

        if (languageTypeScrollList == null)
            return;
        
        int startingIndex = 0;

        GameLocalizationCode languageType = LocalizationManager.CurrentLanguage;
        if (languageTypes.Contains( languageType ))
            startingIndex = languageTypes.FindIndex( (c) => c == languageType );

        languageTypeScrollList.InitList(controlTypesLocalizationCodes, startingIndex );
    }

    public void UpdateLanguage (int index)
    {
        if (languageTypes.Count < 1)
            return;

        GameLocalizationCode languageType = languageTypes[index % languageTypes.Count];
        LocalizationManager.CurrentLanguage = languageType;
    }
}
