using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(TitleMenuButton))]
public class DynamicTitleMenuButton : StoryEventDependent
{
    [SerializeField] StoryEventScriptableObject criteriaStoryEvent;

    [System.Serializable]
    public class Data
    {
        public string labelID;
        public UnityEvent OnClickEvent;
    }

    [SerializeField] List<Data> dataList;

    TitleMenuButton menuButton;
    LocalizedText localizedText;

    private void OnEnable()
    {
        menuButton = GetComponent<TitleMenuButton>();
        localizedText = GetComponentInChildren<LocalizedText>();

        if (localizedText)
            LocalizationManager.AddToActiveTextList(localizedText);
    }

    private void OnDisable()
    {
        if (localizedText)
            LocalizationManager.RemoveFromActiveTextList(localizedText);
    }

    void Start()
    {
        if (criteriaStoryEvent == null)
            return;

        CallDependentAction (SetupButton);
    }

    void SetupButton()
    {
        int state = StoryEventsManager.IsComplete(criteriaStoryEvent) ? 1 : 0;

        Data data = dataList[state % dataList.Count];

        if (localizedText)
            localizedText.SetText (data.labelID);

        menuButton.OnClickEvent.RemoveAllListeners();
        menuButton.OnClickEvent = data.OnClickEvent;
    }
}
