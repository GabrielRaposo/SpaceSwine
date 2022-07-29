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

    [Header("Buttons")]
    [SerializeField] GameObject backButton;
    [SerializeField] GameObject forwardButton;
    [SerializeField] GameObject centerButton;
    
    [Header("References")]
    [SerializeField] RectTransform mainAnchor;
    [SerializeField] TextMeshProUGUI screenLabel;

    [HideInInspector] public bool OnFocus;

    float timerCount;
    bool visible;
    bool playerMode;
    Sequence s;

    SoundtrackManager soundtrackManager;
    PlayerInputActions playerInputActions;
    TitleStateManager titleStateManager;

    private void Awake() 
    {
        titleStateManager = GetComponentInParent<TitleStateManager>();    

        SetAbsolutePosition (visible: false);
    }

    private void OnEnable() 
    {
        playerInputActions = new PlayerInputActions();    
    
        playerInputActions.UI.Navigation.performed += (ctx) => 
        { 
            if (!OnFocus || SceneTransition.OnTransition)
                return;

            if (!playerMode)
                return;

            Vector2 navigationInput = ctx.ReadValue<Vector2>();

            if (navigationInput.x != 0)
            {
                if (navigationInput.x > .5f)
                    SkipMusic(1);
                else if (navigationInput.x < .5f)
                    SkipMusic(-1);
            }
        };
        playerInputActions.UI.Navigation.Enable();

        playerInputActions.UI.Confirm.performed += (ctx) => 
        {               
            if(!OnFocus || SceneTransition.OnTransition)
                return;

            if (titleStateManager)
                titleStateManager.SetMenuState();

            SetPlayerState(false);
        };
        playerInputActions.UI.Confirm.Enable();
    }

    public void SetPlayerMode(bool value)
    {
        playerMode = value;
    }

    public void SetPlayerState(bool value)
    {
        if (value)
            SlideIn();
        else 
            SlideOut();
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
        screenLabel.text = index.ToString() + "- " + fileName;

        if (playerMode)
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

    private void OnDisable() 
    {
        playerInputActions.UI.Navigation.Disable();
        playerInputActions.UI.Confirm.Disable();
    }

}
