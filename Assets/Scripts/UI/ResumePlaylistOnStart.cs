using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResumePlaylistOnStart : MonoBehaviour
{
    [SerializeField] private bool DontWorkDuringTransition;

    public static bool Trigger;

    void Start()
    {
        if (DontWorkDuringTransition && TransitionSafetyToDanger.OnTransition)
            return;

        if (!Trigger)
            return;

        Trigger = false;
        
        SoundtrackManager soundtrackManager = SoundtrackManager.Instance;
        if (soundtrackManager)
            soundtrackManager.Resume();
    }
}
