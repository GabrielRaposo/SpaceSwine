using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResumePlaylistOnStart : MonoBehaviour
{
    public static bool Trigger;

    void Start()
    {
        if (!Trigger)
            return;

        Trigger = false;
        
        SoundtrackManager soundtrackManager = SoundtrackManager.Instance;
        if (soundtrackManager)
            soundtrackManager.Resume();
    }
}
