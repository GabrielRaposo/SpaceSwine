using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using DG.Tweening;

public class PlaylistPlayer : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] PlaylistScriptableObject playlist;
    [SerializeField] float offscreenX;
    [SerializeField] float slideDuration;
    [SerializeField] float autoHideTimer;
    [SerializeField] float textStepSize;
    [SerializeField] float textStepDuration;
    [SerializeField] float textStepEndHold;

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
    Sequence textSequence;
    InputAction navigationInput;
    InputAction centerInput;

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
    
        navigationInput = playerInputActions.UI.Navigation;
        navigationInput.performed += (ctx) => 
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
        navigationInput.Enable();

        centerInput = playerInputActions.UI.Confirm;
        centerInput.performed += (ctx) => 
        {               
            if(!OnFocus || SceneTransition.OnTransition)
                return;

            if (titleStateManager)
                titleStateManager.SetMenuState();

            SetPlayerState(false);
        };
        centerInput.Enable();
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
        //string text = index.ToString() + " - " + fileName;
        string text = fileName;
        screenLabel.text = text;

        int stepsCount = 0; 
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == ' ' || text[i] == '.' || text[i] == '-')
                stepsCount += 3;
            else
                stepsCount += 5;
        }
        stepsCount += 4;
        //stepsCount -= 70;
        if (stepsCount > 0)
        {
            RectTransform textRT = screenLabel.GetComponent<RectTransform>();

            if (textSequence != null)
                textSequence.Kill();

            textSequence = DOTween.Sequence();
            textSequence.AppendCallback( () => {
                textRT.anchoredPosition =  new Vector2(0, 0);
            });
            textSequence.AppendInterval(textStepEndHold);
            for (int i = 0; i < stepsCount + 1; i++)
            {
                int local_i = i;
                textSequence.AppendInterval( textStepDuration );
                textSequence.AppendCallback( () => {
                    textRT.anchoredPosition =  new Vector2(local_i * textStepSize * -1, 0);
                });
            }
            textSequence.SetLoops(-1);
        }

        if (playerMode)
            SlideIn(extraTime: stepsCount * textStepDuration);
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

        ButtonsVisualInteractions();

        timerCount -= Time.deltaTime;
        if (timerCount <= 0)
            SlideOut();
    }

    private void ButtonsVisualInteractions()
    {
        Vector2 axis = navigationInput.ReadValue<Vector2>();
        backButton?.SetActive (axis.x > -.5f);
        forwardButton?.SetActive (axis.x < .5f);

        //float button = centerInput.ReadValue<float>();
        //centerButton?.SetActive(button < .5f);
    }

    public void SlideIn(float extraTime = 0)
    {
        timerCount = autoHideTimer + extraTime;

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

    // (f) => XGridMovement(f, 1/8f, card1.transform)).SetEase(Ease.Linear)
    private void XGridMovement (float x, float step, Transform t)
    {
        int division = Mathf.RoundToInt( x / step );
        t.localPosition = new Vector2(division * step, t.localPosition.y);
    }

    private void OnDisable() 
    {
        navigationInput.Disable();
        centerInput.Disable();
    }

}
