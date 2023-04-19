using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;

public class StoryEventsManager : MonoBehaviour
{
    [System.Serializable]
    public class EventProgress
    {
        public EventProgress (int progress, int goal)
        {
            this.progress = progress;
            this.goal = goal > 0 ? goal : 1;
            OnStateChangedEvent = new UnityEvent<bool>();
        }

        int progress;
        int goal;
        UnityEvent<bool> OnStateChangedEvent;

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

    [SerializeField] InputAction testInput;
    [SerializeField] List<StoryEventScriptableObject> storyEvents;

    TextMeshProUGUI listDisplay;

    static Dictionary<StoryEventScriptableObject, EventProgress> eventsDictionary;
    static StoryEventsManager Instance;

    private void Awake() 
    {
        if (Instance != null)
            return;

        Instance = this;

        if (storyEvents == null || storyEvents.Count < 1)
            return;

        MakeEventsDictionary();
    }

    public void MakeEventsDictionary()
    {
        eventsDictionary = new Dictionary<StoryEventScriptableObject, EventProgress>();

        foreach (var storyEvent in storyEvents)
        {
            eventsDictionary.Add 
            (
                key: storyEvent, 
                value: new EventProgress 
                (
                    progress: storyEvent.StartingState ? storyEvent.Goal : 0,
                    goal:     storyEvent.Goal
                )
            );
        }
    } 

    public static EventProgress GetEventProgress(StoryEventScriptableObject key)
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

    #region Debug Display

    bool showing = false;

    private void OnEnable() 
    {
        listDisplay = GetComponentInChildren<TextMeshProUGUI>();
        listDisplay.text = string.Empty;

        if (!Application.isEditor)
            return;

        testInput.performed += (ctx) => 
        {
            showing = !showing;
            PrintEventStates();
        };
        testInput.Enable();
    }

    private void OnDisable() 
    {
        if (!Application.isEditor)
            return;

        testInput.Disable();
    }

    public static void UpdatePrintedEventStates()
    {
        if (Instance == null)
            return;

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

        #if UNITY_EDITOR
            Debug.Log (s);
        #endif

        listDisplay.text = s;
        listDisplay.enabled = showing;
    }

    #endregion

}
