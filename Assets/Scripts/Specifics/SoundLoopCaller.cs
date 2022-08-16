using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLoopCaller : MonoBehaviour
{
    [SerializeField] AK.Wwise.Event soundscapeAKEvent;
    [SerializeField] float fadeDuration;

    bool playing;

    public void StartLoop()
    {
        if (soundscapeAKEvent == null)
            return;

        soundscapeAKEvent.Post(gameObject);
        playing = true;
    }

    void Update()
    {
        if (!playing)
            return;

        if (!soundscapeAKEvent.IsPlaying(gameObject))
        {
            soundscapeAKEvent.Stop(gameObject);
            StartLoop();
        }
    }

    public void CallFadeOut()
    {
        if (soundscapeAKEvent == null)
            return;

        soundscapeAKEvent.FadeOut(gameObject, fadeDuration);
        playing = false;
    }

    private void OnDisable() 
    {
        if (soundscapeAKEvent == null)
            return;

        soundscapeAKEvent.Stop(gameObject);
    }
}
