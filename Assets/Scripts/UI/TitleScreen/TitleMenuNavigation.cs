using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using DG.Tweening;

public class TitleMenuNavigation : InputSystemButtonsNavigation<TitleMenuButton>
{
    
    [Header("Dynamic Button")]
    [SerializeField] TitleMenuButton continueButton;
    [SerializeField] StoryEventScriptableObject newSaveCheckEvent;

    [Header("Audio")]
    [SerializeField] AK.Wwise.Event OnEnterMenuAKEvent;

    [Header("Sequence")]
    [SerializeField] float fadeDuration;
    [SerializeField] float slideDuration;
    [SerializeField] AnimationCurve approachCurve;
    [SerializeField] CanvasGroup canvasGroup;

    public UnityAction OnEnterMenuEvent;

    Sequence s;

    protected override void OnEnable() 
    {
        playerInputActions = new PlayerInputActions();    
    
        axisInput = playerInputActions.UI.Navigation;
        axisInput.Enable();

        playerInputActions.UI.Confirm.performed += (ctx) => 
        {               
            if(!OnFocus || SceneTransition.OnTransition)
                return;

            buttons[current].Submit();
        };
        playerInputActions.UI.Confirm.Enable();
    }

    protected override void Start()
    {
        SetContinueButtonState();
        
        if (startOnFocus)
            OnFocus = true;
    }


    private void SetContinueButtonState()
    {
        if(continueButton == null) return;
        
        if (StoryEventsManager.IsComplete(newSaveCheckEvent) )
        {
            continueButton.SetInteraction (true);
            current = 0;
        }
        else
        {
            // remove da lista de scroll
            continueButton.SetInteraction (false);
            current = 1;
        }
        SelectCurrent();
    }
    
    public void FadeInSequence()
    {
        if (s != null) 
            s.Kill();

        {
            VerticalLayoutGroup verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
            if (verticalLayoutGroup)
                verticalLayoutGroup.enabled = false;

            if (canvasGroup)
                canvasGroup.alpha = 0;

            SetContinueButtonState();

            OnEnterMenuEvent?.Invoke();
            if (OnEnterMenuAKEvent != null && !OnEnterMenuAKEvent.IsPlaying(gameObject))
                OnEnterMenuAKEvent.Post(gameObject);

            SelectCurrent(instant: true);
        }

        s = DOTween.Sequence();
        s.Append( canvasGroup.DOFade(1, fadeDuration) );

        float startX = 2200f;
        for (int i = 0; i < buttons.Count; i++)
        {
            RectTransform buttonRT = buttons[i].GetComponent<RectTransform>();
            Vector2 originalPosition = buttonRT.anchoredPosition;
            buttonRT.anchoredPosition = new Vector2(startX, buttonRT.anchoredPosition.y);

            float delay = .04f * i;
            //s.Join( buttonRT.DOAnchorPos(originalPosition, slideDuration).SetEase(Ease.OutCirc).SetDelay(delay) );
            s.Join
            ( 
                DOVirtual.Float 
                (
                    from: 0, to: 1, slideDuration, 
                    (f) => buttonRT.anchoredPosition = new Vector2 
                    ( 
                        GetModifiedPosition(f, startX, originalPosition.x), 
                        buttonRT.anchoredPosition.y
                    )
                ).SetDelay(delay)
            );
        }

        s.OnComplete( () => 
        {
            enabled = true;
            OnFocus = true;
        });
    }

    public void FadeOutSequence()
    {
        if (s != null) 
            s.Kill();

        {
            VerticalLayoutGroup verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
            if (verticalLayoutGroup)
                verticalLayoutGroup.enabled = false;

            if (canvasGroup)
                canvasGroup.alpha = 1;

            enabled = false;
            OnFocus = false;
        }

        s = DOTween.Sequence();
        s.Append( canvasGroup.DOFade(0, fadeDuration) );
    }


    private float GetModifiedPosition(float t, float start, float end)
    {
        float f = Mathf.Lerp (start, end, t);
        return f * approachCurve.Evaluate(t);
    }
    protected override void MoveCursor (int direction)
    {
        holdCount = holdCooldown;
        int buttonCount = ActiveButtonsCount();

        current += direction;
        if (current % buttonCount == 0 && !StoryEventsManager.IsComplete( newSaveCheckEvent ))
            current += direction;

        if (current < 0)
            current = buttonCount - 1;
        
        current %= buttonCount;

        SelectCurrent(instant: false, playSound: true);
    }
}
