using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleFloatEffect : MonoBehaviour
{
    [SerializeField] float maxY;
    [SerializeField] float duration;
    [SerializeField] AnimationCurve animationCurve;

    bool goingIn = true;
    float t;

    private void OnEnable() 
    {
        transform.localPosition = Vector2.zero;    
        t = 0;
    }

    private void Update() 
    {
        if (Time.timeScale < 1)
            return;

        t += Time.deltaTime;
        if (t > duration)
            t = duration;

        float start = goingIn ? -maxY : maxY;
        float end   = goingIn ? maxY : -maxY;
    
        float y = Mathf.Lerp(start, end, t / duration);
        float evaluated = animationCurve.Evaluate(t/duration);
        if (!goingIn)
            evaluated = 1 - evaluated;
        transform.localPosition = Vector2.up * (y * evaluated);

        if (t == duration)
        {
            //goingIn = !goingIn;
            t = 0;
        } 
    }
}
