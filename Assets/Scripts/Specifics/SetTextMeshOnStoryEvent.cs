using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SetTextMeshOnStoryEvent : MonoBehaviour
{
    [SerializeField] StoryEventScriptableObject storyEvent;

    CanvasGroup canvasGroup;
    Sequence s;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    private void OnEnable() 
    {
        if (storyEvent == null)
            return;
 
        storyEvent.OnStateChange  += OnStateChange;
        PauseSystem.OnPauseAction += OnPauseAction;
    }

    private void OnStateChange (bool state)
    {
        if (!state)
            return;

        s = DOTween.Sequence();
        s.Append( canvasGroup.DOFade(endValue: 1, duration: 1.0f) );
    }

    private void OnPauseAction()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable() 
    {
        //Debug.Log("OnDisable() - " + name);
        if (storyEvent == null)
            return;
 
        storyEvent.OnStateChange -= OnStateChange;
        PauseSystem.OnPauseAction -= OnPauseAction;
    }
}
