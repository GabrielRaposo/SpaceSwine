using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventCaller : MonoBehaviour
{
    public UnityEvent animationEvent;
    
    void Start()
    {
        if (animationEvent == null)
            animationEvent = new UnityEvent();
    }

    // -- Chamado por Animation
    public void _CallEvent()
    {
        animationEvent.Invoke();
    }
}
