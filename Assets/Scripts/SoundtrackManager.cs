using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using DG.Tweening;

public class SoundtrackManager : MonoBehaviour
{
    [SerializeField] PlaylistScriptableObject fullPlaylist;

    [Header("Parameters")]
    [SerializeField] AK.Wwise.RTPC masterParameter;
    [SerializeField] AK.Wwise.RTPC musicParameter;
    [SerializeField] AK.Wwise.RTPC sfxParameter;
    [SerializeField] AK.Wwise.RTPC placeholderParameter;
    
    PlaylistScriptableObject playlist;
    List<int> playOrder;

    static AK.Wwise.Event soundtrackEvent;

    public static SoundtrackManager Instance;
    
    public static bool OverrideChecksTrigger;
    public static bool IsPlaying;
    bool applicationIsPaused;

    bool manuallyPaused;

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

        if (playlist == null)
        {
            playlist = fullPlaylist; 
            MakePlaylistPlayOrder();
        }

        if (!Application.isEditor)
        {
            if (placeholderParameter != null) 
            {
                placeholderParameter.SetGlobalValue(0);
            }
        }
    }

    private void MakePlaylistPlayOrder()
    {
        playOrder = new List<int>();

        if (playlist == null)
            return;

        // -- Playlist linear
        if (!playlist.shuffle) 
        {
            for (int i = 0; i < playlist.Count; i++) playOrder.Add(i);
            return;
        }

        // -- Playlist com shuffle
        List<int> aux = new List<int>();
        for (int i = 0; i < playlist.Count; i++) aux.Add(i);

        for (int i = 0; i < playlist.Count; i++)
        {
            int random = 0;
            if (aux.Count > 1) 
                random = Random.Range(0, aux.Count);
            int randomizedIndex = aux[random];
            aux.RemoveAt(random);

            playOrder.Add(randomizedIndex);
            //Debug.Log("randomizedIndex: " + randomizedIndex);
        }

        currentIndex = 0;
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

    public void SetPlaylist (PlaylistScriptableObject playlist)
    {
        DebugDisplay.Log($"SetPlaylist: {playlist.name}");

        if (OverrideChecksTrigger)
            OverrideChecksTrigger = false;
        else if (this.playlist == playlist && this.playlist != fullPlaylist)
            return;

        this.playlist = playlist;
        MakePlaylistPlayOrder();

        if (soundtrackEvent != null && soundtrackEvent.IsPlaying(gameObject))
        {
            float duration = 1.0f;
            FadeOutMusic(duration);
            RaposUtil.WaitSeconds(this, duration, () => PlayTrack());
            
            return;
        }

        PlayTrack();        
    }

    public void ChangePlaylistOnTheBack (PlaylistScriptableObject playlist)
    {
        DebugDisplay.Log($"Change Playlist: {playlist.name}");

        if (this.playlist == playlist)
            return;

        this.playlist = playlist;
        MakePlaylistPlayOrder();
    }

    public void PlayTrack ()
    {
        Stop();

        if (playlist == null)
            return;
        
        if (manuallyPaused)
            return;

        int orderedIndex = currentIndex;
        if (playOrder != null) // -- Interação com shuffle
        {
            orderedIndex = playOrder[currentIndex % playOrder.Count];
        }
        orderedIndex %= playlist.Count;

        MusicDataScriptableObject musicData = playlist[orderedIndex];
        soundtrackEvent = musicData.akEvent;

        if (soundtrackEvent != null)
            soundtrackEvent.Post(gameObject);

        OnTrackPlayedEvent?.Invoke(musicData.fileName, currentIndex);
        IsPlaying = true;

        StopAllCoroutines();
    }

    public (string fileName, int currentIndex) GetTrackData()
    {
        if (playlist == null)
            return (string.Empty, 0);

        int orderedIndex = currentIndex;
        if (playOrder != null) // -- Interação com shuffle
        {
            orderedIndex = playOrder[currentIndex % playOrder.Count];
        }
        orderedIndex %= playlist.Count;

        MusicDataScriptableObject musicData = playlist[orderedIndex];
        return (musicData.fileName, currentIndex);
    }

    public void Resume()
    {
        if (IsPlaying)
            return;

        PlayTrack();
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

        soundtrackEvent = null;

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

    public void StopInput()
    {
        manuallyPaused = true;
        Stop();
    }

    public void PlayInput()
    {
        manuallyPaused = false;
        SkipTrack(1);
    }

    public void FadeOutAndPause(float duration = 2f)
    {
        if (soundtrackEvent == null || !IsPlaying)
            return;

        soundtrackEvent.Pause(gameObject, duration, value: true);
    }

    public void ResumeAndFadeIn(float duration = 2f)
    {
        if (soundtrackEvent == null || !IsPlaying)
            return;

        soundtrackEvent.Pause(gameObject, duration, value: false);
    }
}
