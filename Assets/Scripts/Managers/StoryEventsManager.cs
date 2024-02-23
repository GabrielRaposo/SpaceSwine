using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;

public class StoryEventsManager : MonoBehaviour
{
    [SerializeField] bool loadEventsFromSave;

    [SerializeField] StoryEventScriptableObject newSaveStoryEvent;
    [SerializeField] StoryEventScriptableObject shipAccessStoryEvent;
    [SerializeField] List<StoryEventScriptableObject> storyEvents;

    TextMeshProUGUI listDisplay;

    public static bool Initiated;

    static Dictionary<StoryEventScriptableObject, EventProgress> eventsDictionary;
    public static StoryEventsManager Instance;

    private void Awake() 
    {
        if (Instance != null)
            return;

        Instance = this;

        if (!SaveManager.Initiated)
            SaveManager.Load();
    }

    private void Start() 
    {
        if (storyEvents == null || storyEvents.Count < 1)
            return;

        StartCoroutine (WaitForSaveManager());
    }

    private IEnumerator WaitForSaveManager()
    {
        yield return new WaitUntil ( () => SaveManager.Initiated );

        ReloadEventsDictionary();
    }

    public static void ReloadEventsDictionary()
    {
        if (Instance == null)
            return;

        Instance.LoadEventsDictionary();
    }

    public void LoadEventsDictionary()
    {
        eventsDictionary = new Dictionary<StoryEventScriptableObject, EventProgress>();

        foreach (var storyEvent in storyEvents)
        {
            //if (!loadEventsFromSave ) // ou lista do save está vazia
            
            int progress = storyEvent.StartingState ? storyEvent.Goal : 0;
            int goal = storyEvent.Goal;

            if (loadEventsFromSave)
            {
                EventProgressData data = SaveManager.GetStoryEvents().Find ( (d) => d.id == storyEvent.name );
                if (data != null)
                {
                    progress = data.progress;
                    goal = data.goal;
                }
            }

            eventsDictionary.Add 
            (
                key: storyEvent, 
                value: new EventProgress (progress, goal, storyEvent)
            );
        }

        ParseToSaveFormat();

        Initiated = true;
    } 

    public static void ParseToSaveFormat()
    {
        if (!SaveManager.Initiated)
            return;

        List<EventProgressData> dataList = new List<EventProgressData>();

        foreach (var pair in eventsDictionary)
        {
            dataList.Add 
            (
                new EventProgressData
                (
                    id: pair.Key.name,
                    progress: pair.Value.Progress,
                    goal: pair.Value.Goal
                )
            );
        }

        SaveManager.SetStoryEvents (dataList);
    }

    public static EventProgress GetEventProgress (StoryEventScriptableObject key)
    {
        // temp --
        if (eventsDictionary == null)
            return null;
        // --

        if (eventsDictionary.TryGetValue(key, out EventProgress value))
            return value;

        Debug.Log( $"Unable to find event progress for the key { key.name }.");

        return null;
    }

    public static void ChangeProgress (StoryEventScriptableObject key, int value)
    {
        EventProgress eventProgress = GetEventProgress(key);

        if (eventProgress == null)
            return;

        eventProgress.ChangeProgress(value);

        UpdatePrintedEventStates();

        ParseToSaveFormat();
    }

    public static void ClearProgress (StoryEventScriptableObject key)
    {
        EventProgress eventProgress = GetEventProgress(key);

        if (eventProgress == null)
            return;

        eventProgress.Clear();

        UpdatePrintedEventStates();
    }

    public static bool IsComplete (StoryEventScriptableObject key)
    {
        EventProgress eventProgress = GetEventProgress(key);

        if (eventProgress == null)
            return false;
        
        return eventProgress.IsComplete;
    }

    public static void AddListener (StoryEventScriptableObject key, UnityAction<bool> action)
    {
        EventProgress eventProgress = GetEventProgress(key);

        if (eventProgress == null)
            return;

        eventProgress.AddOnStateChangeAction(action);
    }

    public static void RemoveListener (StoryEventScriptableObject key, UnityAction<bool> action)
    {
        EventProgress eventProgress = GetEventProgress(key);

        if (eventProgress == null)
            return;

        eventProgress.RemoveOnStateChangeAction(action);
    }

    public static void CompleteAll()
    {
        if (Instance == null)
            return;

        foreach (var key in Instance.storyEvents)
        {
            ChangeProgress(key, 99);
        }
    }

    public static void UnlockShipAccess()
    {
        if (Instance == null || Instance.newSaveStoryEvent == null || Instance.shipAccessStoryEvent == null)
            return;

        ChangeProgress (Instance.newSaveStoryEvent, 99);
        ChangeProgress (Instance.shipAccessStoryEvent, 99);
    }

    #region Debug Display

    bool showing = false;

    private void OnEnable() 
    {
        listDisplay = GetComponentInChildren<TextMeshProUGUI>();
        listDisplay.text = string.Empty;
    }

    public static void UpdatePrintedEventStates()
    {
        if (Instance == null)
            return;

        Instance.PrintEventStates();
    }

    public static void TogglePrintEventStates()
    {
        if (!Instance)
            return;

        Instance.showing = !Instance.showing;
        Instance.PrintEventStates();
    }

    private void PrintEventStates()
    {
        if (eventsDictionary == null)
            return;

        string s = "Events States::: \n";
        foreach (var key in storyEvents)
        {
            EventProgress progress = GetEventProgress(key);
            if (progress == null)
                continue;

            if (key.UpdatedState != progress.IsComplete)
                key.SetUpdatedState(progress.IsComplete);

            s += $"{ (progress.Completion * 100).ToString("0") }% - { key.name } \n";
        }

        s += "\n\n" + UINotificationManager.PrintList();

        listDisplay.text = s;
        listDisplay.enabled = showing;
    }

    #endregion
    
    public List<StoryEventScriptableObject> GetStoryEventsList()
    {
        return storyEvents;
    }

}

[System.Serializable]
public class EventProgress
{
    public EventProgress (int progress, int goal, StoryEventScriptableObject eventObject)
    {
        this.progress = progress;
        this.goal = goal > 0 ? goal : 1;
        OnStateChangedEvent = new UnityEvent<bool>();
        OnStateChangedEvent.AddListener((isComplete) =>
        {
            if(!isComplete) return;
            SaveManager.AddShipTalkEvent(eventObject);
        });
    }

    int progress;
    int goal;
    UnityEvent<bool> OnStateChangedEvent;

    public int Progress => progress;
    public int Goal => goal;

    public bool IsComplete => progress >= goal;

    public float Completion => (float) progress / goal;

    public void ChangeProgress (int value)
    {
        bool previousState = IsComplete;

        progress += value;

        if (progress > goal)
            progress = goal;

        if (previousState != IsComplete)
            OnStateChangedEvent.Invoke(IsComplete);
    }

    public void Complete()
    {
        ChangeProgress (+goal);
    }

    public void Clear()
    {
        bool previousState = IsComplete;
        progress = 0;

        if (previousState != IsComplete)
            OnStateChangedEvent.Invoke(IsComplete);
    }

    public void AddOnStateChangeAction (UnityAction<bool> action)
    {
        OnStateChangedEvent.AddListener(action);
    }

    public void RemoveOnStateChangeAction (UnityAction<bool> action)
    {
        OnStateChangedEvent.RemoveListener(action);
    }

    public void ClearOnStateChangeListeners ()
    {
        OnStateChangedEvent.RemoveAllListeners();
    }
}