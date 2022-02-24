using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SoundtrackManager : MonoBehaviour
{
    [SerializeField] bool play = true;
    [SerializeField] AK.Wwise.Event stagePlaylist;
    //[SerializeField] AK.Wwise.Event menuMusicEvent;
    //[SerializeField] List <AK.Wwise.Event> stageSongsList;

    [Header("Parameters")]
    [SerializeField] AK.Wwise.RTPC masterParameter;
    [SerializeField] AK.Wwise.RTPC musicParameter;
    [SerializeField] AK.Wwise.RTPC sfxParameter;
    
    
    //public LocalGameplayState localGameplayState;
    //[SerializeField] AK.Wwise.Switch dangerSwitch;

    static AK.Wwise.Event soundtrackEvent;

    public static SoundtrackManager Instance;
    static bool paused;
    public static bool IsPlaying;

    GameplayState gameplayState;

    private void Awake() 
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start() 
    {
        StartCoroutine( RaposUtil.Wait(1, DelayedStart) );
    }

    private void DelayedStart()
    {
        //masterParameter.SetGlobalValue( PlayerPrefsManager.GetFloat( PlayerPrefsManager.Master, 75f) );
        //musicParameter.SetGlobalValue( PlayerPrefsManager.GetFloat( PlayerPrefsManager.Music, 75f) );
        //sfxParameter.SetGlobalValue( PlayerPrefsManager.GetFloat( PlayerPrefsManager.SFX, 75f) );

        //Debug.Log("masterParameter.GetGlobalValue(): " + masterParameter.GetGlobalValue());
        //Debug.Log("musicParameter.GetGlobalValue(): " + musicParameter.GetGlobalValue());
        //Debug.Log("sfxParameter.GetGlobalValue(): " + sfxParameter.GetGlobalValue());
        
        //gameStateSwitch.SetValue(localGameplayState.gameObject);

        PlayMusic();
    }

    //#if UNITY_EDITOR
    private void Update() 
    {
        //if (Input.GetKeyDown(KeyCode.Q))
        //    Stop();

        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    paused = !paused;
        //    if (soundtrackEvent != null)
        //        soundtrackEvent.Pause(gameObject, paused);
        //}

        //if (Input.GetKeyDown(KeyCode.Z))
        //{
        //    Debug.Log("Player prefs deleted");
        //    PlayerPrefs.DeleteAll();
        //}

        //if (!isPlaying)
        //    return;

        //if (soundtrackEvent == null)
        //    return;

        //Debug.Log("soundtrackEvent.IsPlaying: " + soundtrackEvent.IsPlaying(gameObject));
        //if (!soundtrackEvent.IsPlaying(gameObject))
        //{
        //    soundtrackEvent.Post(gameObject);
        //    Debug.Log("bbb");
        //}

    }
    //#endif

    public void PlayMusic(bool skipPlay = false)
    {
        return;

        if (!play && !skipPlay)
            return;

        Stop();

        soundtrackEvent = stagePlaylist;

        if (soundtrackEvent != null)
            soundtrackEvent.Post(gameObject);

        IsPlaying = true;

        StopAllCoroutines();
        StartCoroutine( HardLoopRoutine() );
    }

    IEnumerator HardLoopRoutine()
    {
        while (true)
        {
            float t = (2.0f) * 60f;
            t += 4.753f + 1f;
            yield return new WaitForSecondsRealtime(t);

            Stop();

            soundtrackEvent = stagePlaylist;

            if (soundtrackEvent != null)
                soundtrackEvent.Post(gameObject);

            IsPlaying = true;
        }
    }

    public void Stop()
    {
        if (soundtrackEvent != null)
            soundtrackEvent.Stop(gameObject);

        IsPlaying = false;
    }

    public void FadeOutMusic (float duration)
    {
        if (soundtrackEvent != null)
        {
            soundtrackEvent.FadeOut(gameObject, duration);
        }
    }
}
