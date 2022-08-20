using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutSoundtrackOnStart : MonoBehaviour
{
    void Start()
    {
        SoundtrackManager soundtrackManager = SoundtrackManager.Instance;
        if (!soundtrackManager)
            return;

        RaposUtil.Wait(this, frames: 15, () => 
        {
            //Debug.Log("Play");
            if (SoundtrackManager.IsPlaying)
                soundtrackManager.FadeOutMusic(duration: 2f);
        }); 
    }
}
