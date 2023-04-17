using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

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

    static Dictionary<StoryEventScriptableObject, EventProgress> eventsDictionary;

    static StoryEventsManager Instance;

    #region Debug
    private void OnEnable() 
    {
        if (!Application.isEditor)
            return;

        testInput.performed += (ctx) => PrintEventStates();
        testInput.Enable();
    }

    private void OnDisable() 
    {
        if (!Application.isEditor)
            return;

        testInput.Disable();
    }
    #endregion

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

    private static EventProgress GetEventProgress(StoryEventScriptableObject key)
    {
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
    }

    public static void ClearProgress (StoryEventScriptableObject key)
    {
        EventProgress eventProgress = GetEventProgress(key);

        if (eventProgress == null)
            return;

        eventProgress.Clear();
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

            s += $"{ (progress.Completion * 100).ToString("0") }% \t- { key.name } \n";
        }
        Debug.Log (s);
    }
}
