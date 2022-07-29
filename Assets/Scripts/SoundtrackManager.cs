using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using DG.Tweening;

public class SoundtrackManager : MonoBehaviour
{
    [SerializeField] PlaylistScriptableObject fullPlaylist;
    //[SerializeField] List<AK.Wwise.Event> randomPlaylist;

    [Header("Parameters")]
    [SerializeField] AK.Wwise.RTPC masterParameter;
    [SerializeField] AK.Wwise.RTPC musicParameter;
    [SerializeField] AK.Wwise.RTPC sfxParameter;

    [Header("Test Inputs")]
    [SerializeField] InputAction playMusicTestInput;
    [SerializeField] InputAction stopMusicTestInput;
    
    PlaylistScriptableObject playlist;

    static AK.Wwise.Event soundtrackEvent;

    public static SoundtrackManager Instance;
    //static bool paused;
    public static bool IsPlaying;
    bool applicationIsPaused;

    int currentIndex = 0;

    public UnityAction<string, int> OnTrackPlayedEvent;

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
        if (playlist == null)
            playlist = fullPlaylist;

        playMusicTestInput.performed += (ctx) => PlayTrack();
        playMusicTestInput.Enable();

        stopMusicTestInput.performed += (ctx) => Stop();
        stopMusicTestInput.Enable();
    }

    private void Update() 
    {
        if (!IsPlaying || applicationIsPaused)
            return;

        if (soundtrackEvent == null)
            return;

        if (!soundtrackEvent.IsPlaying(gameObject))
        {
            SkipTrack(1);
        }
    }

    private void OnApplicationPause (bool pause) 
    {
        applicationIsPaused = pause;
    }

    //public void PlayMusic(bool skipPlay = false)
    //{
    //    if (!play && !skipPlay)
    //        return;

    //    Stop();

    //    if (currentIndex < 0)
    //        currentIndex = Random.Range(0, randomPlaylist.Count);
    //    else
    //        currentIndex = (currentIndex + 1) % randomPlaylist.Count;

    //    soundtrackEvent = randomPlaylist[currentIndex];

    //    if (soundtrackEvent != null)
    //        soundtrackEvent.Post(gameObject);

    //    IsPlaying = true;

    //    StopAllCoroutines();
    //}

    public void SetPlaylist (PlaylistScriptableObject playlist)
    {
        Stop();
        this.playlist = playlist;
    }

    public void PlayTrack ()
    {
        Stop();

        MusicDataScriptableObject musicData = playlist[currentIndex % playlist.Count];
        soundtrackEvent = musicData.akEvent;

        if (soundtrackEvent != null)
            soundtrackEvent.Post(gameObject);

        OnTrackPlayedEvent?.Invoke(musicData.fileName, currentIndex);
        IsPlaying = true;

        StopAllCoroutines();
    }

    public void SkipTrack(int direction)
    {
        currentIndex += direction;
        
        if (currentIndex < 0)
            currentIndex = playlist.Count - 1;
        currentIndex %= playlist.Count;

        PlayTrack();
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

        IsPlaying = false;
    }
}
