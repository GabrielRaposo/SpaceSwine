using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StoryEventsManager : MonoBehaviour
{
    [System.Serializable]
    public class EventProgress
    {
        public EventProgress (int goal)
        {
            this.progress = 0;
            this.goal = goal;
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

    [SerializeField] List<StoryEventScriptableObject> storyEvents;

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
        Debug.Log("Make");

        eventsDictionary = new Dictionary<StoryEventScriptableObject, EventProgress>();
        foreach (var storyEvent in storyEvents)
        {
            eventsDictionary.Add (storyEvent, new EventProgress(storyEvent.goal));
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
}
