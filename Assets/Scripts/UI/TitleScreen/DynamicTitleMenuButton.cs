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
    TextMeshProUGUI labelDisplay;

    private void Awake()
    {
        menuButton = GetComponent<TitleMenuButton>();
        labelDisplay = GetComponentInChildren<TextMeshProUGUI>();
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

        if (labelDisplay)
            labelDisplay.text = LocalizationManager.GetUiText(data.labelID, "Continue");

        menuButton.OnClickEvent.RemoveAllListeners();
        menuButton.OnClickEvent = data.OnClickEvent;
    }

}
