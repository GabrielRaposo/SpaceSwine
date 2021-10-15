using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WWiseExtensions
{
    #region Query

    private static bool IsEventPlayingOnGameObject(string eventName, GameObject go)
    {
        uint count = 50;
        uint[] playingIds = new uint[count];
        uint testEventId = AkSoundEngine.GetIDFromString(eventName);

        AKRESULT result = AkSoundEngine.GetPlayingIDsFromGameObject(go, ref count, playingIds);

        for (int i = 0; i < count; i++)
        {
            uint playingId = playingIds[i];
            uint eventId = AkSoundEngine.GetEventIDFromPlayingID(playingId);

            if (eventId == testEventId)
                return true;
        }

        return false;
    }

    private static List<uint> GetEventIDs(string eventName, GameObject go)
    {
        uint count = 50;
        uint[] playingIds = new uint[count];
        uint testEventId = AkSoundEngine.GetIDFromString(eventName);

        AKRESULT result = AkSoundEngine.GetPlayingIDsFromGameObject(go, ref count, playingIds);
        List<uint> events = new List<uint>();

        for (int i = 0; i < count; i++)
        {
            uint playingId = playingIds[i];
            uint eventId = AkSoundEngine.GetEventIDFromPlayingID(playingId);

            if (eventId != testEventId)
                continue;

            events.Add(eventId);
        }

        return events;
    }

    private static void Stop (string eventName, GameObject go)
    {
        List<uint> eventsIDs = GetEventIDs(eventName, go);

        if (eventsIDs.Count < 1)
            return;

        for (int i = 0; i < eventsIDs.Count; i++)
        {
            AkSoundEngine.ExecuteActionOnEvent(eventsIDs[i], AkActionOnEventType.AkActionOnEventType_Stop, go);
        }
    }

    private static void Pause (string eventName, GameObject go, bool value)
    {
        List<uint> eventsIDs = GetEventIDs(eventName, go);

        if (eventsIDs.Count < 1)
            return;

        AkActionOnEventType action = value ? 
            AkActionOnEventType.AkActionOnEventType_Pause :
            AkActionOnEventType.AkActionOnEventType_Resume;

        for (int i = 0; i < eventsIDs.Count; i++)
        {
            AkSoundEngine.ExecuteActionOnEvent
            (
                eventsIDs[i], 
                action, 
                go, 
                in_uTransitionDuration: 0, 
                AkCurveInterpolation.AkCurveInterpolation_Linear
            );
        }
    }
    
    private static void FadeOut (string eventName, GameObject go, float duration)
    {
        List<uint> eventsIDs = GetEventIDs(eventName, go);

        if (eventsIDs.Count < 1)
            return;

        AkActionOnEventType action = AkActionOnEventType.AkActionOnEventType_Stop;

        for (int i = 0; i < eventsIDs.Count; i++)
        {
            AkSoundEngine.ExecuteActionOnEvent
            (
                eventsIDs[i], 
                action, 
                go, 
                in_uTransitionDuration: Mathf.RoundToInt(duration * 1000), // em milisegundos aparentemente
                AkCurveInterpolation.AkCurveInterpolation_Linear
            );
        }
    }

    private static void StopEventsPlayingOnGameObject(GameObject go)
    {
        uint count = 50;
        uint[] playingIds = new uint[count];

        AKRESULT result = AkSoundEngine.GetPlayingIDsFromGameObject(go, ref count, playingIds);

        for (int i = 0; i < count; i++)
        {
            uint playingId = playingIds[i];
            uint eventId = AkSoundEngine.GetEventIDFromPlayingID(playingId);

            AkSoundEngine.ExecuteActionOnEvent(eventId, AkActionOnEventType.AkActionOnEventType_Stop);
        }
    }

    #endregion

    #region WWise Extensions

    public static bool IsPlaying (this AK.Wwise.Event AKEvent, GameObject go)
    {
        return IsEventPlayingOnGameObject(AKEvent.Name, go);
    }
    public static void Stop (this GameObject go, AK.Wwise.Event AKEvent)
    {
        Stop (AKEvent.Name, go);
    }

    public static void Pause (this AK.Wwise.Event AKEvent, GameObject go, bool value = true)
    {
        Pause (AKEvent.Name, go, value);
    }

    public static void FadeOut (this AK.Wwise.Event AKEvent, GameObject go, float duration)
    {
        FadeOut (AKEvent.Name, go, duration);
    }

    public static void StopAllEvents(this GameObject go)
    {
        StopEventsPlayingOnGameObject (go);
    }

    #endregion
}
