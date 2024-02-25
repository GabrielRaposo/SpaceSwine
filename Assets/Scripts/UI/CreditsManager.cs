using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DevLocker.Utils;
using UnityEngine.InputSystem;

public class CreditsManager : MonoBehaviour
{
    [SerializeField] float speedUpModifier;
    [SerializeField] float duration;
    [SerializeField] float endDelay;
    [SerializeField] RectTransform mainLayout;
    [SerializeField] float overshotDistance;
    [SerializeField] SceneReference afterCreditsScene;
    [SerializeField] CanvasGroup fastForwardInputCanvasGroup;

    Sequence s;

    bool fastForwardTrigger;

    PlayerInputActions inputActions;
    InputAction fastForwardInputAction;

    private void OnEnable()
    {
        inputActions = new PlayerInputActions();
        inputActions.Enable();
        fastForwardInputAction = inputActions.UI.Confirm;
    }

    void Start()
    {
        this.Wait(2, DelayedStart);
    }

    void DelayedStart()
    {
        float distance = (mainLayout.rect.height + overshotDistance) * mainLayout.localScale.y;
        DebugDisplay.Log($"Distance: {distance}");

        s = DOTween.Sequence();

        s.AppendInterval(2f);
        s.Append ( mainLayout.DOAnchorPosY(distance, duration, snapping: true).SetEase(Ease.Linear) );
        s.AppendInterval(endDelay);

        s.OnComplete ( () => SceneTransition.LoadScene(afterCreditsScene.ScenePath, SceneTransition.TransitionType.BlackFade) );
    }

    private void Update()
    {
        if (s == null)
            return;

        if (fastForwardInputAction.ReadValue<float>() > .1f)
        {
            s.timeScale = speedUpModifier;

            if (!fastForwardTrigger)
            {
                fastForwardTrigger = true;
                fastForwardInputCanvasGroup.DOFade(0f, duration: 2f);
            }
            return;
        }
        
        s.timeScale = 1f;
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}
