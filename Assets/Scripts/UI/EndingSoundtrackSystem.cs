using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class EndingSoundtrackSystem : MonoBehaviour
{
    //[SerializeField] AK.Wwise.Event soundtrackEvent1;
    //[SerializeField] AK.Wwise.Event soundtrackEvent2;
    //[SerializeField] AK.Wwise.Event soundtrackEvent3;
    [SerializeField] float playlistStartFadeOut;
    [SerializeField] float startUpDelay;
    [SerializeField] float endFadeOut;
    [SerializeField] List<float> fadeDuration;

    [Header("References")]
    [SerializeField] List<AK.Wwise.Event> soundtrackEvents;
    [SerializeField] List<AK.Wwise.RTPC> volumeParameters;

    SoundtrackManager soundtrackManager;

    Sequence sequence;

    bool setup;

    private void Start()
    {
        soundtrackManager = SoundtrackManager.Instance;
        soundtrackManager.FadeOutAndPause(playlistStartFadeOut);

        InitParameters();

        this.WaitSeconds (playlistStartFadeOut + startUpDelay, SetupSoundtrack);
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

        setup = true;
    }

    private void Update()
    {
        if (!setup)
            return;

        for (int i = 0; i < soundtrackEvents.Count; i++)
        {
            RepeatSoundtrack (soundtrackEvents[i]);
        }
    }

    private void RepeatSoundtrack (AK.Wwise.Event soundtrackEvent)
    {
        if (soundtrackEvent == null || soundtrackEvent.IsPlaying(gameObject))
            return;

        //Debug.Log("Post2");
        soundtrackEvent.Post(gameObject);
    }


    public void ChangeFocus (int focus)
    {
        if (focus < 0)
            return;

        float fade = fadeDuration[focus % fadeDuration.Count];

        sequence = DOTween.Sequence();
        sequence.AppendInterval (fade);

        for (int i = 0; i < volumeParameters.Count; i++)
        {
            var parameter = volumeParameters[i];
            float targetValue = (i == focus) ? 100 : 0;
            if (targetValue == parameter.GetGlobalValue())
                continue;

            int local_i = i;
            sequence.Join
            ( 
                DOVirtual.Float
                (
                    from:     parameter.GetGlobalValue(), 
                    to:       targetValue,
                    duration: fade,
                    (f) => { /*Debug.Log($"i: {local_i}, f: {f}");*/ parameter.SetGlobalValue(f); }
                )/*.SetEase(Ease.Linear)*/
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
            soundtrackEvent.FadeOut(gameObject, endFadeOut);
    }

    private void OnDisable()
    {
        TurnOffAll();
    }
}
