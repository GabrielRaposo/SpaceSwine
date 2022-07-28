using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class SoundtrackManager : MonoBehaviour
{
    [SerializeField] bool play = true;

    [SerializeField] List<AK.Wwise.Event> randomPlaylist;

    [Header("Parameters")]
    [SerializeField] AK.Wwise.RTPC masterParameter;
    [SerializeField] AK.Wwise.RTPC musicParameter;
    [SerializeField] AK.Wwise.RTPC sfxParameter;

    [Header("Test Inputs")]
    [SerializeField] InputAction playMusicTestInput;
    [SerializeField] InputAction stopMusicTestInput;
    
    //public LocalGameplayState localGameplayState;
    //[SerializeField] AK.Wwise.Switch dangerSwitch;

    static AK.Wwise.Event soundtrackEvent;

    public static SoundtrackManager Instance;
    static bool paused;
    public static bool IsPlaying;
    bool applicationIsPaused;

    GameplayState gameplayState;
    int currentIndex = -1;

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

        playMusicTestInput.performed += (ctx) => PlayMusic(skipPlay: true);
        playMusicTestInput.Enable();

        stopMusicTestInput.performed += (ctx) => Stop();
        stopMusicTestInput.Enable();
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
        //if (Input.GetKeyDown(KeyCode.Z))
        //{
        //    Debug.Log("Player prefs deleted");
        //    PlayerPrefs.DeleteAll();
        //}

        if (!IsPlaying || applicationIsPaused)
            return;

        if (soundtrackEvent == null)
            return;

        if (!soundtrackEvent.IsPlaying(gameObject))
        {
            PlayMusic (skipPlay: true);
        }

    }

    private void OnApplicationPause (bool pause) 
    {
        applicationIsPaused = pause;
    }

    public void PlayMusic(bool skipPlay = false)
    {
        if (!play && !skipPlay)
            return;

        Stop();

        if (currentIndex < 0)
            currentIndex = Random.Range(0, randomPlaylist.Count);
        else
            currentIndex = (currentIndex + 1) % randomPlaylist.Count;

        soundtrackEvent = randomPlaylist[currentIndex];

        if (soundtrackEvent != null)
            soundtrackEvent.Post(gameObject);

        IsPlaying = true;

        StopAllCoroutines();
        //StartCoroutine( HardLoopRoutine() );
    }

    IEnumerator HardLoopRoutine()
    {
        while (true)
        {
            float t = (2.0f) * 60f;
            t += 4.753f + 1f;
            yield return new WaitForSecondsRealtime(t);

            Stop();

            if (currentIndex < 0)
                currentIndex = Random.Range(0, randomPlaylist.Count);
            else
                currentIndex = (currentIndex + 1) % randomPlaylist.Count;

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

    public void JumpTo (float percent)
    {
        
    }

    public void FadeOutMusic (float duration)
    {
        if (soundtrackEvent != null)
        {
            soundtrackEvent.FadeOut(gameObject, duration);
        }

        IsPlaying = false;
    }
}
