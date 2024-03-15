using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class EndingSoundtrackSystem : MonoBehaviour
{
    const float FADE_DURATION = .5f;

    //[SerializeField] AK.Wwise.Event soundtrackEvent1;
    //[SerializeField] AK.Wwise.Event soundtrackEvent2;
    //[SerializeField] AK.Wwise.Event soundtrackEvent3;
    [SerializeField] List<AK.Wwise.Event> soundtrackEvents;
    [SerializeField] List<AK.Wwise.RTPC> volumeParameters;

    SoundtrackManager soundtrackManager;

    Sequence sequence;

    private void Start()
    {
        soundtrackManager = SoundtrackManager.Instance;
        soundtrackManager.FadeOutAndPause(FADE_DURATION);

        InitParameters();

        this.WaitSeconds (FADE_DURATION, SetupSoundtrack);
        //this.WaitSeconds (FADE_IN, () => ChangeFocus(1));
    }

    private void InitParameters()
    {
        foreach (var volume in volumeParameters)
        {
            volume.SetGlobalValue(0);
        }
    }

    private void SetupSoundtrack()
    {
        foreach (var soundtrack in soundtrackEvents) 
        {
            soundtrack.Post(gameObject);
        }

        ChangeFocus(0);
    }

    private void Update()
    {
        for (int i = 0; i < soundtrackEvents.Count; i++)
        {
            RepeatSoundtrack (soundtrackEvents[i]);
        }
    }

    private void RepeatSoundtrack (AK.Wwise.Event soundtrackEvent)
    {
        if (soundtrackEvent == null || soundtrackEvent.IsPlaying(gameObject))
            return;

        soundtrackEvent.Post(gameObject);
    }


    public void ChangeFocus (int focus)
    {
        sequence = DOTween.Sequence();
        sequence.AppendInterval (FADE_DURATION);

        for (int i = 0; i < volumeParameters.Count; i++)
        {
            var parameter = volumeParameters[i];
            float targetValue = (i == focus) ? 100 : 0;
            if (targetValue == parameter.GetGlobalValue())
                continue;

            sequence.Join
            ( 
                DOVirtual.Float
                (
                    from: parameter.GetGlobalValue(), 
                    to:   targetValue,
                    duration: FADE_DURATION,
                    (f) => parameter.SetGlobalValue(f)
                )
            );
        }
    }

    public void TurnOffAll()
    {
        foreach (var soundtrack in soundtrackEvents)
            TurnOff(soundtrack);
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
