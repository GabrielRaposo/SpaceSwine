using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundtrackOnStart : MonoBehaviour
{
    void Start()
    {
        SoundtrackManager soundtrackManager = SoundtrackManager.Instance;
        if (!soundtrackManager)
            return;

        //Debug.Log("Play");
        if (!SoundtrackManager.IsPlaying)
            soundtrackManager.PlayMusic(skipPlay: true);
    }
}
