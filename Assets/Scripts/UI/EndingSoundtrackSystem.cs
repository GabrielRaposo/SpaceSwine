using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class EndingSoundtrackSystem : MonoBehaviour
{
    const float FADE_OUT = 2f;
    const float FADE_IN = 2f;

    [SerializeField] AK.Wwise.Event soundtrackEvent1;
    [SerializeField] AK.Wwise.Event soundtrackEvent2;
    [SerializeField] AK.Wwise.Event soundtrackEvent3;
    
    SoundtrackManager soundtrackManager;

    int focus;

    private void Start()
    {
        soundtrackManager = SoundtrackManager.Instance;
        soundtrackManager.FadeOutAndPause(FADE_OUT);

        //this.WaitSeconds (FADE_IN, SetupSoundtrack);
        this.WaitSeconds (FADE_IN, () => ChangeFocus(1));
    }
    private void SetupSoundtrack()
    {
        if (soundtrackEvent1 != null)
            soundtrackEvent1.Post(gameObject);

        if (soundtrackEvent2 != null)
            soundtrackEvent2.Post(gameObject);

        if (soundtrackEvent3 != null)
            soundtrackEvent3.Post(gameObject);
    }

    private void Update()
    {
        if (focus == 1)
            RepeatSoundtrack (soundtrackEvent1);

        if (focus == 2)
            RepeatSoundtrack (soundtrackEvent2);

        if (focus == 3)
            RepeatSoundtrack (soundtrackEvent3);
    }

    private void RepeatSoundtrack (AK.Wwise.Event soundtrackEvent)
    {
        if (soundtrackEvent == null || soundtrackEvent.IsPlaying(gameObject))
            return;

        soundtrackEvent.Post(gameObject);
    }


    public void ChangeFocus (int focus)
    {
        this.focus = focus;

        SetSoundtrackFocus(soundtrackEvent1, focus == 1);
        SetSoundtrackFocus(soundtrackEvent2, focus == 2);
        SetSoundtrackFocus(soundtrackEvent3, focus == 3);
    }

    private void SetSoundtrackFocus(AK.Wwise.Event soundtrackEvent, bool state)
    {
        if (soundtrackEvent == null)
            return;
        
        if (state)
            soundtrackEvent.Post(gameObject);
        else
            soundtrackEvent.FadeOut(gameObject, FADE_OUT);
    }

    public void TurnOffAll()
    {
        TurnOff(soundtrackEvent1);
        TurnOff(soundtrackEvent2);
        TurnOff(soundtrackEvent3);
    }

    private void TurnOff (AK.Wwise.Event soundtrackEvent)
    {
        if (soundtrackEvent != null && soundtrackEvent.IsPlaying(gameObject))
            soundtrackEvent.FadeOut(gameObject, 1f);
    }

    private void OnDisable()
    {
        TurnOffAll();
    }
}
