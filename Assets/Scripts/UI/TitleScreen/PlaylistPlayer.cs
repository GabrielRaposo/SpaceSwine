using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class PlaylistPlayer : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] PlaylistScriptableObject playlist;
    [SerializeField] float offscreenX;
    [SerializeField] float slideDuration;
    [SerializeField] float autoHideTimer;

    [Header("References")]
    [SerializeField] RectTransform mainAnchor;
    [SerializeField] TextMeshProUGUI screenLabel;

    float timerCount;
    bool visible;
    Sequence s;

    SoundtrackManager soundtrackManager;

    private void Awake() 
    {
        SetAbsolutePosition (visible: false);
    }

    private bool Initiation()
    {
        soundtrackManager = SoundtrackManager.Instance;
        if (!soundtrackManager)
            return false;

        if (!playlist)
            return false;

        if (playlist.list.Count < 1)
            return false;

        return true;
    }
    
    void Start()
    {
        if (!Initiation())
        {
            enabled = false;
            return;
        }

        soundtrackManager.OnTrackPlayedEvent += SetupOnTrackEvent;
        soundtrackManager.SetPlaylist(playlist);
        PlayCurrent();
    }

    private void OnDestroy() 
    {
        if (soundtrackManager)
            soundtrackManager.OnTrackPlayedEvent -= SetupOnTrackEvent;
    }

    private void SetupOnTrackEvent(string fileName, int index)
    {
        screenLabel.text = fileName;
        SlideIn();
    }

    public void SetAbsolutePosition (bool visible)
    {
        mainAnchor.MoveX (visible ? 0 : offscreenX);
        this.visible = visible;
    }

    private void Update() 
    {
        if (!visible)
            return;

        timerCount -= Time.deltaTime;
        if (timerCount <= 0)
            SlideOut();
    }

    public void SlideIn()
    {
        timerCount = autoHideTimer;

        if (visible)
            return;
        visible = true;
        
        if (s != null)
            s.Kill();

        s = DOTween.Sequence();
        s.Append
        (
            DOVirtual.Float(offscreenX, 0, slideDuration, f => mainAnchor.MoveX(f) )
                .SetEase(Ease.OutCirc)
        );
        //s.OnComplete( () => timerCount = autoHideTimer );
    }

    public void SlideOut()
    {
        if (!visible)
            return;
        visible = false;

        if (s != null)
            s.Kill();

        s = DOTween.Sequence();
        s.Append
        (
            DOVirtual.Float(0, offscreenX, slideDuration, f => mainAnchor.MoveX(f) )
                .SetEase(Ease.InCirc)
        );
        //s.OnComplete( () => SetAbsolutePosition(false) );
    }

    private void PlayCurrent()
    {
        soundtrackManager.PlayTrack();
    }
    public void SkipMusic (int direction)
    {
        soundtrackManager.SkipTrack(direction);
    }

    //public void SetScreenLabel (MusicDataScriptableObject musicData)
    //{
    //    screenLabel.text = musicData.fileName;
    //}

}
