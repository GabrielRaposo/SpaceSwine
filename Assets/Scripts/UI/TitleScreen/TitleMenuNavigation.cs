﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TitleMenuNavigation : MonoBehaviour
{
    [SerializeField] bool startOnFocus;
    [SerializeField] List<TitleMenuButton> titleButtons;

    [Header("Sequence")]
    [SerializeField] float fadeDuration;
    [SerializeField] float slideDuration;
    [SerializeField] AnimationCurve approachCurve;
    [SerializeField] CanvasGroup canvasGroup;

    [HideInInspector] public bool OnFocus;
    
    int current = -1;

    PlayerInputActions playerInputActions;
    Sequence s;

    private void OnEnable() 
    {
        playerInputActions = new PlayerInputActions();    
    
        playerInputActions.UI.Navigation.performed += (ctx) => 
        { 
            if(!OnFocus || SceneTransition.OnTransition)
                return;

            Vector2 navigationInput = ctx.ReadValue<Vector2>();

            if (navigationInput.y != 0)
            {
                if (navigationInput.y > .5f)
                    MoveCursor(-1);
                else if (navigationInput.y < .5f)
                    MoveCursor(1);
            }
        };
        playerInputActions.UI.Navigation.Enable();

        playerInputActions.UI.Confirm.performed += (ctx) => 
        {               
            if(!OnFocus || SceneTransition.OnTransition)
                return;

            titleButtons[current].Submit();
        };
        playerInputActions.UI.Confirm.Enable();
    }

    private void Start() 
    {
        current = 0;
        SelectCurrent();
        
        if (startOnFocus)
            OnFocus = true;
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

            if (current < 0)
                current = 0;
            SelectCurrent(instant: true);
        }

        s = DOTween.Sequence();
        s.Append( canvasGroup.DOFade(1, fadeDuration) );
        //s.AppendInterval(.1f );

        float startX = 2200f;
        for (int i = 0; i < titleButtons.Count; i++)
        {
            RectTransform buttonRT = titleButtons[i].GetComponent<RectTransform>();
            Vector2 originalPosition = buttonRT.anchoredPosition;
            buttonRT.anchoredPosition = new Vector2(startX, buttonRT.anchoredPosition.y);

            float delay = .05f * i;
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

        /**
        float hideX = 2200f;
        for (int i = 0; i < titleButtons.Count; i++)
        {
            RectTransform buttonRT = titleButtons[i].GetComponent<RectTransform>();
            Vector2 originalPosition = buttonRT.anchoredPosition;
            //buttonRT.anchoredPosition = new Vector2(hideX, buttonRT.anchoredPosition.y);

            float delay = .05f * i;
            //s.Join( buttonRT.DOAnchorPos(originalPosition, slideDuration).SetEase(Ease.OutCirc).SetDelay(delay) );
            s.Join
            ( 
                DOVirtual.Float 
                (
                    from: 0, to: 1, slideDuration, 
                    (f) => buttonRT.anchoredPosition = new Vector2 
                    ( 
                        GetModifiedPosition(f, originalPosition.x, hideX), 
                        buttonRT.anchoredPosition.y
                    )
                ).SetDelay(delay)
            );
        }

        s.OnComplete( () => 
        {
            // ---
        });
        **/
    }


    private float GetModifiedPosition(float t, float start, float end)
    {
        float f = Mathf.Lerp(start, end, t);
        return f * approachCurve.Evaluate(t);
    }

    private void SelectCurrent(bool instant = false)
    {
        for (int i = 0; i < titleButtons.Count; i++)
        {
            if (instant)
                titleButtons[i].InstantSelect(false);
            else
                titleButtons[i].Deselect();
                
        }

        if (instant)
            titleButtons[current].InstantSelect(true);
        else
            titleButtons[current].Select();        
    }

    private void MoveCursor (int direction)
    {
        current += direction;
        if (current < 0)
            current = titleButtons.Count - 1;
        current %= titleButtons.Count;

        SelectCurrent();
    }

    private void OnDisable() 
    {
        playerInputActions.UI.Navigation.Disable();
        playerInputActions.UI.Confirm.Disable();
    }

}